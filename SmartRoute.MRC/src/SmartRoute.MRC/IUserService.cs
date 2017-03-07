using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartRoute.MRC
{
	public interface IUserService
	{
		void Register(Protocol.UserInfo userinfo, Protocol.OperationStatus processStatus);

		void Remove(string name, Protocol.OperationStatus processStatus);

		Protocol.UserInfo GetUserInfo(string name, Protocol.OperationStatus processStatus);

		void List(Action<Protocol.OperationStatus, List<Protocol.UserInfo>> list);
	}
}
