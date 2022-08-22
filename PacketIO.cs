using System;
using System.Reflection;
using System.Collections.Generic;

namespace MysticNetworking
{
	public class PacketIO
	{
        // Store user defined methods
		private Dictionary<Type, Action<object>> OnReceivedMethods = new Dictionary<Type, Action<object>>();

		// actual black magic
		public void AddReceiveMethod<T>(Action<T> callback)
		{
			OnReceivedMethods.Add(typeof(T), (data) => callback((T)data));
		}
		
		public void ClearReceiveMethods()
		{
			OnReceivedMethods.Clear();
		}

		public PacketIO()
		{
			MysticLogger.Log("Initialized CustomPacketInitializer. List count: " + CustomPacketInitializer.UserStructs.Count);
			MysticLogger.Log("Initialized PacketDataTypes. List count: " + PacketDataTypes.PacketDataTypesDictionary.Count);
		}

        // invoke user method based on packet type
		public void InvokeMethodForPacketType(ICustomPacket packetData)
		{
			// validate incoming data
			//TODO: check if byte count is greater than one

			if (OnReceivedMethods.TryGetValue(packetData.GetType(), out Action<object> onReceivedMethod))
			{
				onReceivedMethod(packetData);
			}
			else
				MysticLogger.Log("No onReceivedMethod defined for packet type: " + packetData.GetType());
		}

		public static object ReadPacket(Packet packet, int startPosition = 0)
		{
			packet.setPosition(startPosition);
			ushort id;

			try
			{
				id = packet.ReadUShort();
			}
			catch (Exception e)
			{
				MysticLogger.Log("Unable to read ushort id of packet. " + e);
				return null;
			}

			if (CustomPacketInitializer.UserStructs.TryGetValue(id, out Type packetType))
			{

				object packetTypeObject = Activator.CreateInstance(packetType);

				foreach (FieldInfo dataType in packetType.GetFields())
				{
					if (PacketDataTypes.PacketDataTypesDictionary.TryGetValue(dataType.FieldType, out Type networkDataType))
					{
						if (networkDataType.BaseType == typeof(PacketDataType))
						{
							MethodBase readPacketMethod = networkDataType.GetMethod("Read");

							// return null if packet invoke method fails
							object value = null;

							try
							{
								value = readPacketMethod.Invoke(null, new object[] { packet });
							}
							catch (Exception e)
							{
								MysticLogger.Log("Packet read method failed.");
								MysticLogger.LogException(e);
							}

							dataType.SetValue(packetTypeObject, value);
						}
						else
							MysticLogger.LogException(new Exception("Data type does not inherit ICustomPacket. Data type: " + networkDataType));
					}
					else
						MysticLogger.LogException(new Exception("Unable to read a data type in the packet. Matching network data type not found."));
				}
				return Convert.ChangeType(packetTypeObject, packetType);
			}
			else
			{
				MysticLogger.LogException(new Exception("Packet struct does not match a supported type."));
				return null;
			}
		}

		public static Packet WritePacket(ICustomPacket packetType, MethodBase _method = null)
		{
			Packet packet = new Packet();

			// write the packetid
			if (CustomPacketInitializer.UserStructsInverse.TryGetValue(packetType.GetType(), out ushort packetId))
			{
				packet.Write(packetId);

				foreach (FieldInfo dataType in packetType.GetType().GetFields())
				{
					PacketDataTypes.PacketDataTypesDictionary.TryGetValue(dataType.FieldType, out Type networkDataType);
					if (networkDataType.BaseType == typeof(PacketDataType))
					{
						MethodBase writePacketMethod = networkDataType.GetMethod("Write");

						try
						{
							writePacketMethod.Invoke(null, new object[] { packet, dataType.GetValue(packetType) });
						}
						catch (Exception e)
						{
							MysticLogger.LogException(e);
						}
					}
					else
						MysticLogger.LogException(new Exception("Data type does not inherit ICustomPacket. Data type: " + networkDataType));
				}
				return packet;
			}
			else
			{
				MysticLogger.LogException(new Exception("Packet type not found: " + packetType));
				// return null packet if unsuccessful
				packet = null;
				return packet;
			}
		}
	}
}
