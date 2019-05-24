using System;
using System.Text;
namespace PxBook
{
	public class xPacket
	{
		byte[] _bytes;
		public int pointer;
		public xPacket(byte[] bytes)
		{
			this._bytes = bytes;
		}
		public byte ReadByte()
		{
			pointer += 1;
			return _bytes[pointer - 1];
		}
		public byte ReadUInt8()
		{
			pointer += 1;
			return _bytes[pointer - 1];
		}
		public ushort ReadUInt16()
		{
			pointer += 2;
			return BitConverter.ToUInt16(_bytes, pointer - 2);
		}
		public uint ReadUInt32()
		{
			pointer += 4;
			return BitConverter.ToUInt32(_bytes, pointer - 4);
		}
		public int ReadInt32()
		{
			pointer += 4;
			return BitConverter.ToInt32(_bytes, pointer - 4);
		}
		public ulong ReadUInt64()
		{
			pointer += 8;
			return BitConverter.ToUInt64(_bytes, pointer - 8);
		}
		public float ReadSingle()
		{
			pointer += 4;
			return BitConverter.ToSingle(_bytes, pointer - 4);
		}
		public string ReadAscii()
		{
			ushort length = ReadUInt16();
			pointer += length;
			if (length == 0)
				return "";
			return ASCIIEncoding.ASCII.GetString(_bytes, pointer - length, length);
		}
		public string ReadAscii32()
		{
			int length = ReadInt32();
			pointer += length;
			if (length == 0)
				return "";
			return ASCIIEncoding.ASCII.GetString(_bytes, pointer - length, length);
		}
		public string ReadAscii8()
		{
			byte length = ReadUInt8();
			pointer += length;
			if (length == 0)
				return "";
			return ASCIIEncoding.ASCII.GetString(_bytes, pointer - length, length);
		}
		public uint[] ReadUInt32Array(ushort length)
		{
			uint[] result = new uint[length];
			for (int i = 0; i < length; i++)
			{
				result[i] = ReadUInt32();
			}
			return result;
		}
	}
}
