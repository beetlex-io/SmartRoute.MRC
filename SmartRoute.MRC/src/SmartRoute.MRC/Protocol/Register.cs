using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProtoBuf;
namespace SmartRoute.MRC.Protocol
{
	[ProtoContract]
	public class Register
	{
		[ProtoMember(1)]
		public string Name { get; set; }
		[ProtoMember(2)]
		public string GatewayID { get; set; }

	}

	[ProtoContract]
	public class UnRegister
	{
		[ProtoMember(1)]
		public string Name { get; set; }
	}


}
