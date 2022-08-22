using System;
using System.Linq;
using System.Collections.Generic;

namespace MysticNetworking
{
	public class Packet
	{
		int nativeSize = IntPtr.Size;
		public Dictionary<int, Type> DataTypes = new Dictionary<int, Type>();

		List<byte> buffer = new List<byte>();
		byte[] data;
		int position;

		public Packet()
		{
			position = 0;
		}

		public void setPosition(int _position)
		{
			this.position = _position;
		}

		public Packet(byte[] _data)
		{
			buffer = _data.ToList();
			data = _data;
			position = 0;
		}

		public byte[] ToArray()
		{
			data = buffer.ToArray();
			return data;
		}

		public void Write(byte[] _value)
		{
			buffer.AddRange(_value);
			position += _value.Length;
		}

		public Boolean ReadBool()
		{
			bool readData = BitConverter.ToBoolean(data, position);
			position += 1;
			return readData;
		}

		public Byte ReadByte()
		{
			byte readData = data[position];
			position += 1;
			return readData;
		}
		
		public Char ReadChar()
		{
			char readData = BitConverter.ToChar(data, position);
			position += 2;
			return readData;
		}

		public Int16 ReadShort()
		{
			short readData = BitConverter.ToInt16(data, position);
			position += 2;
			return readData;
		}
		
		public UInt16 ReadUShort()
		{
			ushort readData = BitConverter.ToUInt16(data, position);
			position += 2;
			return readData;
		}

		public Int32 ReadInt()
		{
			int readData = BitConverter.ToInt32(data, position);
			position += 4;
			return readData;
		}
		
		public UInt32 ReadUInt()
		{
			uint readData = BitConverter.ToUInt32(data, position);
			position += 4;
			return readData;
		}
		
		public Int64 ReadLong()
		{
			long readData = BitConverter.ToInt64(data, position);
			position += 8;
			return readData;
		}
		
		public UInt64 ReadULong()
		{
			ulong readData = BitConverter.ToUInt64(data, position);
			position += 8;
			return readData;
		}

		public Single ReadFloat()
		{
			float readData = BitConverter.ToSingle(data, position);
			position += 4;
			return readData;
		}
		
		public Double ReadDouble()
		{
			double readData = BitConverter.ToDouble(data, position);
			position += 8;
			return readData;
		}

		public String ReadString()
		{
			int length = ReadInt();
			string readData = System.Text.Encoding.ASCII.GetString(data, position, length);
			position += length;
			return readData;
		}

		public void Write(Boolean _data)
		{
			Write(BitConverter.GetBytes(_data));
		}
		
		public void Write(Byte _data)
		{
			Write(BitConverter.GetBytes(_data));
		}
		
		public void Write(short _data)
		{
			Write(BitConverter.GetBytes(_data));
		}
		
		public void Write(ushort _data)
		{
			Write(BitConverter.GetBytes(_data));
		}

		public void Write(Int32 _data)
		{
			Write(BitConverter.GetBytes(_data));
		}
		
		public void Write(UInt32 _data)
		{
			Write(BitConverter.GetBytes(_data));
		}
		
		public void Write(Int64 _data)
		{
			Write(BitConverter.GetBytes(_data));
		}
		
		public void Write(UInt64 _data)
		{
			Write(BitConverter.GetBytes(_data));
		}
		
		public void Write(Single _data)
		{
			Write(BitConverter.GetBytes(_data));
		}

		public void Write(Double _data)
		{
			Write(BitConverter.GetBytes(_data));
		}
		
		public void Write(string _data)
		{
			Write(_data.Length);
			Write(System.Text.Encoding.ASCII.GetBytes(_data));
		}
	}

	//TODO: get rid of this of find out if it's faster than dictionary tryget
	public enum NetDataTypes
	{
		Network_ushort = 1,
		Network_int,
		Network_Vector3,
	}

}
