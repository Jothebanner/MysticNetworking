using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

public class CustomPacketInitializer
{
	public static Dictionary<ushort, Type> UserStructs;
	public static Dictionary<Type, ushort> UserStructsInverse;

	static CustomPacketInitializer()
	{
		// idk, I'm probably going to hell for this
		var temp = AddPacketTypes();
		UserStructs = temp.Item1;
		UserStructsInverse = temp.Item2;
	}

	// good heavens.... At least it's static lol
	public static Tuple<Dictionary<ushort, Type>, Dictionary<Type, ushort>> AddPacketTypes()
	{
		Assembly assembly = Assembly.GetExecutingAssembly();
		Dictionary<ushort, Type> UserStructs = new Dictionary<ushort, Type>();
		Dictionary<Type, ushort> UserStructsInverse = new Dictionary<Type, ushort>();

		// our "dynamic enum"
		ushort id = 0;
		
		Type[] customPacketClasses = Assembly.GetExecutingAssembly().GetTypes().Where(customType => customType.GetInterfaces().Contains(typeof(ICustomPacket))).ToArray();
		foreach (Type customPacketClass in customPacketClasses)
		{
			UserStructs.Add(id, customPacketClass);
			UserStructsInverse.Add(customPacketClass, id);
			id++;
		}
		return Tuple.Create(UserStructs, UserStructsInverse);
	}
}
