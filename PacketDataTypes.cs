using System.Collections.Generic;
using System;
using UnityEngine;
using System.Reflection;
using System.Linq;
using MysticNetworking;

public class PacketDataTypes : MonoBehaviour
{
	// any packet added to dictonary will be supported
	public static Dictionary<Type, Type> PacketDataTypesDictionary;
	static PacketDataTypes()
	{
		PacketDataTypesDictionary = BuildPacketDataTypesDictionary();
	}

	private static Dictionary<Type, Type> BuildPacketDataTypesDictionary()
	{
		Dictionary<Type, Type> temp = new Dictionary<Type, Type>();
		Assembly assembly = Assembly.GetExecutingAssembly();
		Type[] types = assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(PacketDataType))).ToArray();
		foreach (Type networkDataType in types)
		{
			FieldInfo dataType = networkDataType.GetField("dataType");
			//MysticLogger.Log(dataType.GetValue(null).ToString());
			Type test = dataType.GetValue(null) as Type;
			temp.Add(test, networkDataType);
		}

		return temp;
	}
}

public class PacketDataType
{
	public static Type dataType;
	public static void Read(Packet packet) { throw new Exception("The read function for this data type has not been implemented"); }
	public static void Write(Packet packet) { throw new Exception("The write function for this data type has not been implemented"); }
	public static void Write(Packet packet, bool _data) { throw new Exception("The write function for this data type has not been implemented"); }
	public static void Write(Packet packet, byte _data) { throw new Exception("The write function for this data type has not been implemented"); }
	public static void Write(Packet packet, char _data) { throw new Exception("The write function for this data type has not been implemented"); }
	public static void Write(Packet packet, short _data) { throw new Exception("The write function for this data type has not been implemented"); }
	public static void Write(Packet packet, ushort _data) { throw new Exception("The write function for this data type has not been implemented"); }
	public static void Write(Packet packet, int _data) { throw new Exception("The write function for this data type has not been implemented"); }
	public static void Write(Packet packet, uint _data) { throw new Exception("The write function for this data type has not been implemented"); }
	public static void Write(Packet packet, nint _data) { throw new Exception("The write function for this data type has not been implemented"); }
	public static void Write(Packet packet, nuint _data) { throw new Exception("The write function for this data type has not been implemented"); }
	public static void Write(Packet packet, long _data) { throw new Exception("The write function for this data type has not been implemented"); }
	public static void Write(Packet packet, ulong _data) { throw new Exception("The write function for this data type has not been implemented"); }
	public static void Write(Packet packet, float _data) { throw new Exception("The write function for this data type has not been implemented"); }
	public static void Write(Packet packet, double _data) { throw new Exception("The write function for this data type has not been implemented"); }
	public static void Write(Packet packet, decimal _data) { throw new Exception("The write function for this data type has not been implemented"); }
	public static void Write(Packet packet, string _data) { throw new Exception("The write function for this data type has not been implemented"); }
	public static void Write(Packet packet, Vector2 _data) { throw new Exception("The write function for this data type has not been implemented"); }
	public static void Write(Packet packet, Vector3 _data) { throw new Exception("The write function for this data type has not been implemented"); }
	public static void Write(Packet packet, Quaternion _data) { throw new Exception("The write function for this data type has not been implemented"); }
}

public class Network_ushort : PacketDataType
{
	public new static Type dataType = typeof(ushort);
	public new static ushort Read(Packet packet)
	{
		return packet.ReadUShort();
	}
	public new static void Write(Packet packet, ushort _data)
	{
		packet.Write(_data);
	}
}

public class Network_string : PacketDataType
{
	public new static Type dataType = typeof(string);
	public new static string Read(Packet packet)
	{
		return packet.ReadString();
	}
	public new static void Write(Packet packet, string _data)
	{
		packet.Write(_data);
	}
}

public class Network_int32 : PacketDataType
{
	public new static Type dataType = typeof(Int32);
	public static new void Read(Packet packet)
	{
		packet.ReadInt();
	}
	public static new void Write(Packet packet, Int32 _data)
	{
		packet.Write(_data);
	}
}

//public class Network_Vector3 : PacketDataType
//{
//	public new static void Read(Packet packet)
//	{
//		//packet.ReadUShort();
//	}
//	public static void Write(Packet packet, Vector3 _data)
//	{
//		//packet.WriteUShort(_data);
//	}
//	private void AddToDataTypeDictionary()
//	{
//		PacketDataTypes.PacketDataTypesDictionary.Add(typeof(Vector3), typeof(Network_Vector3));
//	}
//}
