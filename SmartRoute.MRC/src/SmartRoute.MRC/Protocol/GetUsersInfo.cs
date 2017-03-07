using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProtoBuf;
namespace SmartRoute.MRC.Protocol
{
	[ProtoContract]
	public class GetUsersInfo
	{
		[ProtoMember(2)]
		public string Receiver { get; set; }
		[ProtoMember(1)]
		public long RequestID { get; set; }
	}

	[ProtoContract]
	public class GetUserInfoResponse
	{

		public GetUserInfoResponse()
		{
			Items = new List<Protocol.UserInfo>();
		}

		[ProtoMember(1)]
		public long RequestID { get; set; }
		[ProtoMember(2)]
		public List<UserInfo> Items { get; set; }
	}

}
