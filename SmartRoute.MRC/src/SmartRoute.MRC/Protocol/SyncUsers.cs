using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProtoBuf;
namespace SmartRoute.MRC.Protocol
{
	[ProtoContract]
	public class SyncUsers
	{
		public SyncUsers()
		{
			Items = new List<UserInfo>();
		}

		[ProtoMember(1)]
		public List<UserInfo> Items { get; set; }

		[ProtoMember(2)]
		public bool Eof { get; set; }
	}
}
