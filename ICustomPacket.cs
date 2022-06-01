using MysticNetworking;
using System;

public interface ICustomPacket
{
	public static Packet Build() { throw new Exception("The send function for this data type has not been implemented"); }
}

