using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRoute.MRC.Protocol;

namespace SmartRoute.MRC.Implement
{
	public class Gateway : IGateway
	{

		public Gateway(INode node = null)
		{
			if (node == null)
				node = NodeFactory.Default;
			Node = node;
		}

		private long mRequestID = 0;

		private System.Collections.Concurrent.ConcurrentDictionary<string, UserToken> mUserActions =
			new System.Collections.Concurrent.ConcurrentDictionary<string, UserToken>();

		private MessageQueue mMsgQueue;

		public INode Node
		{
			get;
			set;
		}

		protected long GetRequestID()
		{
			return System.Threading.Interlocked.Increment(ref mRequestID);
		}

		private void OnUserMessage(Message msg, UserMessage e)
		{
			UserToken token;
			if (mUserActions.TryGetValue(e.Receiver, out token))
			{
				token.Receive(token, e);
			}
		}

		private void OnGetUserInfoRequest(Message msg, GetUserInfoResponse e)
		{
			mMsgQueue.Push(e);
		}

		public OperationStatus Register(UserToken userToken)
		{
			OperationStatus result;
			Register register = new Register();
			register.Name = userToken.Name;
			register.GatewayID = Node.DefaultEventSubscriber.Name;
			result = Node.DefaultSwitchSubscriber.SyncToService<Protocol.OperationStatus>(Center.USER_SERVICE_TAG, register);
			mUserActions[userToken.Name] = userToken;
			return result;
		}

		public void SendMessage(string receivers, string sender, object message)
		{
			MessageQueue.MessageItem item = new MessageQueue.MessageItem();
			item.ID = GetRequestID();
			item.Receives = receivers;
			item.Sender = sender;
			item.Data = message;
			mMsgQueue.Push(item);
			GetUsersInfo getinfo = new GetUsersInfo();
			getinfo.RequestID = item.ID;
			getinfo.Receiver = receivers;
			Node.DefaultSwitchSubscriber.ToService(Center.USER_SERVICE_TAG, getinfo);
		}

		public void Dispose()
		{
			if (mMsgQueue != null)
				mMsgQueue.Dispose();
		}

		public void Open()
		{
			mMsgQueue = new MessageQueue(this, 2);
			mMsgQueue.Open();
			Node.DefaultSwitchSubscriber.DefaultEventSubscriber.Register<GetUserInfoResponse>(OnGetUserInfoRequest);
			Node.DefaultEventSubscriber.Register<UserMessage>(OnUserMessage);
		}

		public OperationStatus UnRegister(string username)
		{
			UnRegister unregister = new UnRegister();
			unregister.Name = username;
			UserToken token = null;
			mUserActions.TryRemove(username, out token);
			return Node.DefaultSwitchSubscriber.SyncToService<OperationStatus>(Center.USER_SERVICE_TAG, unregister);
		}
	}
}
