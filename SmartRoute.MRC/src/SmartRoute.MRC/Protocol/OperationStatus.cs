using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProtoBuf;
namespace SmartRoute.MRC.Protocol
{
	[ProtoContract]
	public class OperationStatus
	{
		[ProtoMember(1)]
		public bool Success { get; set; }
		[ProtoMember(2)]
		public int Status { get; set; }
		[ProtoMember(3)]
		public string Message { get; set; }
	}
}
