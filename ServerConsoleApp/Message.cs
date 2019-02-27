using System;
using System.Collections.Generic;
using System.Text;

namespace ServerConsoleApp {

	public enum MessageType {
		LogIn,
		LogOut,
		Movement,
		None
	}

	public class Message {
		public MessageType type;
		public string name;
		public string message;
		public float x;
		public float y;
		public float z;

		public Message() {
			type = MessageType.None;
			name = null;
			message = null;
		}

		public Message(byte[] bytes) {
			type = (MessageType)BitConverter.ToInt32(bytes, 0);
			x = BitConverter.ToSingle(bytes, 4);
			y = BitConverter.ToSingle(bytes, 8);
			z = BitConverter.ToSingle(bytes, 12);

			var nameLength = BitConverter.ToInt32(bytes, 16);
			var msgLength = BitConverter.ToInt32(bytes, 20);

			if (nameLength > 0)
				this.name = Encoding.UTF8.GetString(bytes, 24, nameLength);
			else
				this.name = null;

			if (msgLength > 0)
				this.message = Encoding.UTF8.GetString(bytes, 24 + nameLength, msgLength);
			else
				this.message = null;
		}

		public byte[] GetBytes() {
			List<byte> dataStream = new List<byte>();

			dataStream.AddRange(BitConverter.GetBytes((int)type));
			dataStream.AddRange(BitConverter.GetBytes(x));
			dataStream.AddRange(BitConverter.GetBytes(y));
			dataStream.AddRange(BitConverter.GetBytes(z));

			if (this.name != null)
				dataStream.AddRange(BitConverter.GetBytes(this.name.Length));
			else
				dataStream.AddRange(BitConverter.GetBytes(0));

			if (this.message != null)
				dataStream.AddRange(BitConverter.GetBytes(this.message.Length));
			else
				dataStream.AddRange(BitConverter.GetBytes(0));

			if (this.name != null)
				dataStream.AddRange(Encoding.UTF8.GetBytes(this.name));

			if (this.message != null)
				dataStream.AddRange(Encoding.UTF8.GetBytes(this.message));

			return dataStream.ToArray();
		}
	}
}