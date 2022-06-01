using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;
using System.Text;

namespace MysticNetworking
{
	public class Packet
	{
		public Dictionary<int, Type> DataTypes = new Dictionary<int, Type>();

		//foreach (int key in Enum.GetValues(typeof(NetDataTypes)))

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

		public ushort PeekUShort(int readPosition = 0)
		{
			return BitConverter.ToUInt16(data, readPosition);
		}

		public ushort ReadUShort()
		{
			ushort readData = BitConverter.ToUInt16(data, position);
			position += 2;
			return readData;
		}

		public int ReadInt()
		{
			int readData = BitConverter.ToInt32(data, position);
			position += 4;
			return readData;
		}

		public string ReadString()
		{
			int length = ReadInt();
			string readData = Encoding.ASCII.GetString(data, position, length);
			position += length;
			return readData;
		}

		public void Write(ushort _data)
		{
			Write(BitConverter.GetBytes(_data));
		}

		public void Write(Int32 _data)
		{
			Write(BitConverter.GetBytes(_data));
		}
		
		public void Write(string _data)
		{
			Write(_data.Length);
			Write(System.Text.Encoding.ASCII.GetBytes(_data));
		}
	}

	//TODO: get rid of this or keep it, idk maybe it'll be faster than dictionary tryget
	public enum NetDataTypes
	{
		Network_ushort = 1,
		Network_int,
		Network_Vector3,
	}

}
