using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartRoute.MRC.Implement
{
	public class Center : ICenter
	{
		public const string CENTER_TAG = "MRC_CENTER";

		public const string CENTER_OTHER_TAG = @"MRC_CENTER\.";

		public const string USER_SERVICE_TAG = "USER_SERVICE";

		public const string GROUP_SERVICE_TAG = "GROUP_SERVICE";

		private EventSubscriber mCenterSubscriber;

		private EventSubscriber mUserServiceSubscriber;

		private System.Threading.Timer mStartServiceTimer;

		private bool mHasOtherCenter = false;

		private bool mReadyToStart = true;

		public Center(IUserService userService,INode node = null)
		{
			if (node == null)
				Node = SmartRoute.NodeFactory.Default;
			UserService = userService;
			
			ID = CENTER_TAG + "_" + Guid.NewGuid().ToString("N");
		}

		private void OnOpen(object state)
		{
			if (!mReadyToStart)
			{
				mReadyToStart = true;
				Node.Loger.Process(LogType.INFO, "sync user info ....");
			}
			else
			{
				mStartServiceTimer.Dispose();
				mUserServiceSubscriber = Node.DefaultSwitchSubscriber.GetService(USER_SERVICE_TAG);
				Node.Loger.Process(LogType.INFO, "{0}-{1} center started!", CENTER_TAG, Node.DefaultEventSubscriber.Name);
				mUserServiceSubscriber.Register<Protocol.Register>(OnUserRegister);
				mUserServiceSubscriber.Register<Protocol.GetUsersInfo>(OnReceiveUsersInfo);
				mUserServiceSubscriber.Register<Protocol.UnRegister>(OnUserUnregister);
			}
		}

		public void Open()
		{
			mCenterSubscriber = Node.Register<EventSubscriber>(ID);
			mCenterSubscriber.Register<Protocol.SyncUsers>(OnSyncUsers);
			mCenterSubscriber.Register<Protocol.CenterStarted>(OnOtherCenterStarted);
			mCenterSubscriber.Register<Protocol.Register>(OnSyncUser);
			mCenterSubscriber.Register<Protocol.UnRegister>(OnSyncUnRegister);
			Node.SubscriberRegisted += OnSubscriberRegisted;
			mStartServiceTimer = new System.Threading.Timer(OnOpen, null, 5000, 5000);
			Node.Loger.Process(LogType.INFO, "search other center...");
		}
		
		//处理用户上线所在网关信息
		private void OnReceiveUsersInfo(Message msg, Protocol.GetUsersInfo e)
		{
			string[] users = e.Receiver.Split(';');
			Protocol.GetUserInfoResponse response = new Protocol.GetUserInfoResponse();
			response.RequestID = e.RequestID;
			Protocol.OperationStatus status = new Protocol.OperationStatus();
			foreach (string user in users)
			{
				Protocol.UserInfo info = UserService.GetUserInfo(user, status);
				if (info != null)
					response.Items.Add(info);
			}
			msg.Reply(response);
		}
		 //网关用户下线
		private void OnUserUnregister(Message msg, Protocol.UnRegister e)
		{
			Protocol.OperationStatus status = new Protocol.OperationStatus();
			UserService.Remove(e.Name, status);
			msg.Reply(status);
			Node.Loger.Process(LogType.INFO, "{0} user unregister", e.Name);
			//同步到其他中心节点
			if (mHasOtherCenter)
				mCenterSubscriber.Publish(CENTER_OTHER_TAG, e, ReceiveMode.Regex);
		}
		 //网关用户上线
		private void OnUserRegister(Message msg, Protocol.Register e)
		{
			Protocol.OperationStatus status = new Protocol.OperationStatus();
			UserService.Register(new Protocol.UserInfo() { Name = e.Name, Gateway = e.GatewayID }, status);
			msg.Reply(status);
			Node.Loger.Process(LogType.INFO, "{0} user register from {1}", e.Name, e.GatewayID);
			//同步到其他中心节点
			if (mHasOtherCenter)
				mCenterSubscriber.Publish(CENTER_OTHER_TAG, e, ReceiveMode.Regex);
		}

		//同步下线
		private void OnSyncUnRegister(Message msg, Protocol.UnRegister e)
		{
			Protocol.OperationStatus status = new Protocol.OperationStatus();
			UserService.Remove(e.Name, status);
			Node.Loger.Process(LogType.INFO, "{0} user unregister", e.Name);
		}
		//同步上线
		private void OnSyncUser(Message msg, Protocol.Register e)
		{
			Protocol.OperationStatus status = new Protocol.OperationStatus();
			UserService.Register(new Protocol.UserInfo() { Name = e.Name, Gateway = e.GatewayID }, status);
			Node.Loger.Process(LogType.INFO, "{0} user register from {1}", e.Name, e.GatewayID);
		}

		//同步其他中心上线信息
		private void OnSyncUsers(Message msg, Protocol.SyncUsers e)
		{
			Node.Loger.Process(LogType.INFO, "sync user info to local!");
			Protocol.OperationStatus status = new Protocol.OperationStatus();
			foreach (Protocol.UserInfo item in e.Items)
			{
				UserService.Register(item, status);
			}
		}

		private void OnSubscriberRegisted(INode node, ISubscriber subscriber)
		{
			//发现其他中心服务，向服务发起同步用户请求
			if (subscriber.Name.IndexOf(CENTER_TAG) == 0 && subscriber.Name != ID)
			{
				mHasOtherCenter = true;
				mReadyToStart = false;
				Node.Loger.Process(LogType.INFO, "find {0} center", subscriber.Name);
				Protocol.CenterStarted started = new Protocol.CenterStarted();
				started.Name = ID;
				mCenterSubscriber.Publish(subscriber.Name, started);
				Node.Loger.Process(LogType.INFO, "request sync user info ....");
			}
		}


		public INode Node
		{
			get; set;
		}

		public IUserService UserService
		{
			get; set;
		}

		public string ID
		{
			get; private set;
		}



		private void OnOtherCenterStarted(Message msg, Protocol.CenterStarted e)
		{
			if (e.Name != ID)
			{
				UserService.List((s, i) =>
				{
					if (s.Success)
					{
						Protocol.SyncUsers sync = new Protocol.SyncUsers();
						sync.Items = i;
						msg.Reply(sync);
						if ((Node.Loger.Type & LogType.DEBUG) > 0)
							Node.Loger.Process(LogType.DEBUG, "sync users to {0}", e.Name);
					}
					else
					{
						if ((Node.Loger.Type & LogType.DEBUG) > 0)
							Node.Loger.Process(LogType.DEBUG, "sync users to {0} error {1}", e.Name, s.Message);
					}

				});

			}
		}

		public void Dispose()
		{
			Node.SubscriberRegisted -= OnSubscriberRegisted;
		}


	}
}
