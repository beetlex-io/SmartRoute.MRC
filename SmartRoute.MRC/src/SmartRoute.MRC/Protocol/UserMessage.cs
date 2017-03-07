using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeetleX.Buffers;
using ProtoBuf;
using SmartRoute;
namespace SmartRoute.MRC.Protocol
{
	public class UserMessage : ISerializer
	{

		private static SmartRoute.Protocols.IMessageTypeHandler mTypeHandler = new SmartRoute.Protocols.MessageTypeHandler();

		public string Receiver { get; set; }

		public string CC { get; set; }

		public string Sender { get; set; }

		public object Data { get; set; }

		public void Deserialize(IBinaryReader reader)
		{
			Receiver = reader.ReadShortUTF();
			CC = reader.ReadShortUTF();
			Sender = reader.ReadShortUTF();
			Type type = mTypeHandler.ReadType(reader);
			int length = reader.ReadInt32();
			Data = ProtoBuf.Meta.RuntimeTypeModel.Default.Deserialize(reader.Stream, null, type, length);
		}

		public void Serialize(IBinaryWriter writer)
		{
			writer.WriteShortUTF(Receiver);
			writer.WriteShortUTF(CC);
			writer.WriteShortUTF(Sender);
			mTypeHandler.WriteType(Data, writer);
			using (BeetleX.Buffers.IWriteBlock block = writer.Allocate4Bytes())
			{
				int start = (int)writer.Length;
				ProtoBuf.Meta.RuntimeTypeModel.Default.Serialize(writer.Stream, Data);
				block.SetData((int)writer.Length - start);
			}

		}
	}
}
