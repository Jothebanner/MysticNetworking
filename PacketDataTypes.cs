using System;
using System.Linq;
using MysticNetworking;
using System.Reflection;
using System.Collections.Generic;

public class PacketDataTypes
{
	// any packet added to dictonary will be supported
	public static Dictionary<Type, Type> PacketDataTypesDictionary;
	static PacketDataTypes()
	{
		PacketDataTypesDictionary = BuildPacketDataTypesDictionary();
	}

    // build the dictionary
	private static Dictionary<Type, Type> BuildPacketDataTypesDictionary()
	{
		Dictionary<Type, Type> temp = new Dictionary<Type, Type>();
		Assembly assembly = Assembly.GetExecutingAssembly(); //TODO: optimize the targeted assemblies
        // use reflection to gather each of the data types of PacketDataType.
        // This is awesome because we can support any user added datatype assuming it's built on the C# built in types
        // such as vector3 and quaternions
		Type[] types = assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(PacketDataType))).ToArray();
		foreach (Type networkDataType in types)
		{
			FieldInfo dataType = networkDataType.GetField("dataType");
			Type test = dataType.GetValue(null) as Type;
			temp.Add(test, networkDataType);
		}

		return temp;
	}
}

// unfortunately for the desired intellisense response I need to hardcode the desired datatypes :(
public class PacketDataType
{
	public static Type dataType;
	public static void Read(Packet packet) { MysticLogger.LogException(new Exception("The read function for this data type has not been implemented")); }
	//public static void Write(Packet packet) { MysticLogger.Log(new Exception("The write function for this data type has not been implemented")); }
	public static void Write(Packet packet, bool _data) { MysticLogger.LogException(new Exception("The write function for this data type has not been implemented")); }
	public static void Write(Packet packet, byte _data) { MysticLogger.LogException(new Exception("The write function for this data type has not been implemented")); }
	public static void Write(Packet packet, char _data) { MysticLogger.LogException(new Exception("The write function for this data type has not been implemented")); }
	public static void Write(Packet packet, short _data) { MysticLogger.LogException(new Exception("The write function for this data type has not been implemented")); }
	public static void Write(Packet packet, ushort _data) { MysticLogger.LogException(new Exception("The write function for this data type has not been implemented")); }
	public static void Write(Packet packet, int _data) { MysticLogger.LogException(new Exception("The write function for this data type has not been implemented")); }
	public static void Write(Packet packet, uint _data) { MysticLogger.LogException(new Exception("The write function for this data type has not been implemented")); }
	//public static void Write(Packet packet, nint _data) { MysticLogger.Log(new Exception("The write function for this data type has not been implemented")); }
	//public static void Write(Packet packet, nuint _data) { MysticLogger.Log(new Exception("The write function for this data type has not been implemented")); }
	public static void Write(Packet packet, long _data) { MysticLogger.LogException(new Exception("The write function for this data type has not been implemented")); }
	public static void Write(Packet packet, ulong _data) { MysticLogger.LogException(new Exception("The write function for this data type has not been implemented")); }
	public static void Write(Packet packet, float _data) { MysticLogger.LogException(new Exception("The write function for this data type has not been implemented")); }
	public static void Write(Packet packet, double _data) { MysticLogger.LogException(new Exception("The write function for this data type has not been implemented")); }
	//public static void Write(Packet packet, decimal _data) { MysticLogger.Log(new Exception("The write function for this data type has not been implemented")); }
	public static void Write(Packet packet, string _data) { MysticLogger.LogException(new Exception("The write function for this data type has not been implemented")); }
}

public class MysticBoolean : PacketDataType
{
	public new static Type dataType = typeof(Boolean);
	public new static bool Read(Packet packet)
	{
		return packet.ReadBool();
	}
	public new static void Write(Packet packet, bool _data)
	{
		packet.Write(_data);
	}
}

public class MysticByte : PacketDataType
{
	public new static Type dataType = typeof(Byte);
	public new static byte Read(Packet packet)
	{
		return packet.ReadByte();
	}
	public new static void Write(Packet packet, byte _data)
	{
		packet.Write(_data);
	}
}

public class MysticChar : PacketDataType
{
	public new static Type dataType = typeof(Char);
	public new static char Read(Packet packet)
	{
		return packet.ReadChar();
	}
	public new static void Write(Packet packet, char _data)
	{
		packet.Write(_data);
	}
}

public class MysticInt16 : PacketDataType
{
	public new static Type dataType = typeof(Int16);
	public new static short Read(Packet packet)
	{
		return packet.ReadShort();
	}
	public new static void Write(Packet packet, short _data)
	{
		packet.Write(_data);
	}
}

public class MysticUInt16 : PacketDataType
{
	public new static Type dataType = typeof(UInt16);
	public new static ushort Read(Packet packet)
	{
		return packet.ReadUShort();
	}
	public new static void Write(Packet packet, ushort _data)
	{
		packet.Write(_data);
	}
}

public class MysticInt32 : PacketDataType
{
	public new static Type dataType = typeof(Int32);
	public new static int Read(Packet packet)
	{
		return packet.ReadInt();
	}
	public new static void Write(Packet packet, Int32 _data)
	{
		packet.Write(_data);
	}
}

public class MysticUInt32 : PacketDataType
{
	public new static Type dataType = typeof(UInt32);
	public new static uint Read(Packet packet)
	{
		return packet.ReadUInt();
	}
	public new static void Write(Packet packet, UInt32 _data)
	{
		packet.Write(_data);
	}
}

public class MysticInt64 : PacketDataType
{
	public new static Type dataType = typeof(Int64);
	public new static long Read(Packet packet)
	{
		return packet.ReadLong();
	}
	public new static void Write(Packet packet, long _data)
	{
		packet.Write(_data);
	}
}

public class MysticUInt64 : PacketDataType
{
	public new static Type dataType = typeof(UInt64);
	public new static ulong Read(Packet packet)
	{
		return packet.ReadULong();
	}
	public new static void Write(Packet packet, ulong _data)
	{
		packet.Write(_data);
	}
}

public class MysticFloat : PacketDataType
{
	public new static Type dataType = typeof(Single);
	public new static float Read(Packet packet)
	{
		return packet.ReadFloat();
	}
	public new static void Write(Packet packet, float _data)
	{
		packet.Write(_data);
	}
}

public class MysticDouble : PacketDataType
{
	public new static Type dataType = typeof(Double);
	public new static double Read(Packet packet)
	{
		return packet.ReadDouble();
	}
	public new static void Write(Packet packet, double _data)
	{
		packet.Write(_data);
	}
}

public class MysticString : PacketDataType
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

    // it was worth it lol
}
