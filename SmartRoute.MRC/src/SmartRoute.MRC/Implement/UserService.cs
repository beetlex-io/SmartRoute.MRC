using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRoute.MRC.Protocol;

namespace SmartRoute.MRC.Implement
{
	public class UserService : IUserService
	{

		private System.Collections.Concurrent.ConcurrentDictionary<string, UserInfo> mUsers = new System.Collections.Concurrent.ConcurrentDictionary<string, UserInfo>();

		public UserInfo GetUserInfo(string name, OperationStatus processStatus)
		{
			UserInfo result = null;
			try
			{
				mUsers.TryGetValue(name, out result);
			}
			catch (Exception e_)
			{
				processStatus.Success = false;
				processStatus.Message = e_.Message;
			}
			return result;
		}

		public void List(Action<OperationStatus, List<UserInfo>> list)
		{
			OperationStatus status = new OperationStatus();
			List<UserInfo> result = new List<UserInfo>();
			foreach (string key in mUsers.Keys)
			{
				UserInfo item = GetUserInfo(key, status);
				if (item != null)
					result.Add(item);
				if (result.Count >= 1000)
				{
					list(status, result);
					result.Clear();
				}
			}
			if (result.Count > 0)
			{
				list(status, result);
				result.Clear();
			}

		}

		public void Register(UserInfo userinfo, OperationStatus processStatus)
		{
			try
			{
				mUsers[userinfo.Name] = userinfo;
			}
			catch (Exception e_)
			{
				processStatus.Success = false;
				processStatus.Message = e_.Message;
			}
		}

		public void Remove(string name, OperationStatus processStatus)
		{
			try
			{
				UserInfo result;

				mUsers.TryRemove(name, out result);
			}
			catch (Exception e_)
			{
				processStatus.Success = false;
				processStatus.Message = e_.Message;
			}
		}
	}
}
