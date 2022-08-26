using System;
using UnityEngine;
using MysticNetworking;

public class Mystic_Vector2 : PacketDataType
{
	public new static Type dataType = typeof(Vector2);

	public new static Vector2 Read(Packet packet)
	{
		Vector2 vector = new Vector2();
		vector.x = packet.ReadFloat();
		vector.y = packet.ReadFloat();
        return vector;
		
	}
	public static void Write(Packet packet, Vector2 _data)
	{
		packet.Write(_data.x);
		packet.Write(_data.y);
	}
}

public class Mystic_Vector3 : PacketDataType
{
	public new static Type dataType = typeof(Vector3);

	public new static vector3 Read(Packet packet)
	{
		Vector3 vector = new Vector3();
		vector.x = packet.ReadFloat();
		vector.y = packet.ReadFloat();
		vector.z = packet.ReadFloat();
        return vector
		
	}
	public static void Write(Packet packet, Vector3 _data)
	{
		packet.Write(_data.x);
		packet.Write(_data.y);
		packet.Write(_data.z);
	}
}

public class Mystic_Quaternion : PacketDataType
{
	public new static Type dataType = typeof(Quaternion);

	public new static Quaternion Read(Packet packet)
	{
		Quaternion rotation = Quaternion.identity;
		rotation.w = packet.ReadFloat();
		rotation.x = packet.ReadFloat();
		rotation.y = packet.ReadFloat();
		rotation.z = packet.ReadFloat();
        return rotation;
		
	}
	public static void Write(Packet packet, Quaternion _data)
	{
		packet.Write(_data.w);
		packet.Write(_data.x);
		packet.Write(_data.y);
		packet.Write(_data.z);
	}
}
