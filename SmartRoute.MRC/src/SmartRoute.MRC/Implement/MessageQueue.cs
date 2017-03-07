using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartRoute.MRC.Implement
{
    public class MessageQueue : IDisposable
    {

        public MessageQueue(IGateway gateway, int threads = 1)
        {
            mThreads = threads;
            Gateway = gateway;
        }

        private int mThreads = 1;

        private bool mIsStart = true;

        private LinkedList<MessageItem> mMessagesLink = new LinkedList<MessageItem>();

        private ConcurrentDictionary<long, LinkedListNode<MessageItem>> mMessagesMaps = new ConcurrentDictionary<long, LinkedListNode<MessageItem>>();

        private ConcurrentQueue<Protocol.GetUserInfoResponse> mSenders = new ConcurrentQueue<Protocol.GetUserInfoResponse>();


        public IGateway Gateway { get; set; }


        private MessageItem GetMessageItem(long id)
        {
            lock (mMessagesLink)
            {
                LinkedListNode<MessageItem> result;
                if (mMessagesMaps.TryGetValue(id, out result))
                {
                    mMessagesLink.Remove(result);
                    return result.Value;
                }
                return null;
            }
        }

        private void OnProcessMessage(object state)
        {
            while (mIsStart)
            {
                Protocol.GetUserInfoResponse response;
                if (mSenders.TryDequeue(out response))
                {
                    try
                    {
                        MessageItem item = GetMessageItem(response.RequestID);
                        if (item != null)
                        {
                            if (response.Items.Count == 0)
                            {
                                Gateway.Node.Loger.Process(LogType.INFO, "{0} user notfound!", item.Receives);

                            }
                            else
                            {
                                for (int i = 0; i < response.Items.Count; i++)
                                {
                                    Protocol.UserInfo info = response.Items[i];
                                    Protocol.UserMessage message = new Protocol.UserMessage();
                                    message.Receiver = info.Name;
                                    message.Data = item.Data;
                                    message.Sender = item.Sender;
                                    message.CC = item.Receives;
                                    Gateway.Node.DefaultEventSubscriber.Publish(info.Gateway, message);
                                }
                            }
                        }
                    }
                    catch (Exception e_)
                    {
                        Gateway.Node.Loger.Process(LogType.ERROR, "gateway process message error {0}", e_.Message);
                    }
                }
                else
                {
                    System.Threading.Thread.Sleep(10);
                }
            }
        }

        public void Push(MessageItem item)
        {
            lock (mMessagesLink)
            {
                LinkedListNode<MessageItem> litem = new LinkedListNode<MessageItem>(item);
                mMessagesLink.AddLast(litem);
                mMessagesMaps[item.ID] = litem;
            }
        }
        public void Push(Protocol.GetUserInfoResponse response)
        {
            mSenders.Enqueue(response);
        }

        public void Open()
        {
            for (int i = 0; i < mThreads; i++)
                System.Threading.ThreadPool.QueueUserWorkItem(OnProcessMessage);

        }

        public void Dispose()
        {
            mIsStart = false;
            mMessagesLink.Clear();
            mMessagesMaps.Clear();

        }

        public class MessageItem
        {
            public long ID { get; set; }

            public string Sender { get; set; }

            public string Receives { get; set; }

            public long Time { get; set; }

            public object Data { get; set; }

        }

    }
}
