# MythicNetworking
Dynamic, code-validation focused C# TCP/UDP networking solution.

The main idea for this solution is to enforce coding standards and provide data for intellisense while giving you low level control over the packets that you're working with. It's important to note that this solution can be much slower per data type per packet than a less validated solution.

There are two main parts to this framework. The Server, Client, and Packet classes and the PacketIO suite.
The Server, Client and Packet classes are the bread and butter (and cheese) of the project; they can be used as any generic networking solution they connect, disconnect, send packets and listen for any tcp or udp incoming messages and stack them in a buffer. You can then use the Packet class to read and write the bits of the packets, tho this process can be tedious and error-prone without the grace of intellsense to save you from the fallacies of the human mind...

Which is where the PacketIO suite comes in! PacketIO is a basic attempt to add a layer of abstraction and automate the reading and writing of packets. The PacketIO class is the brain; it reads, writes and manages the basic packet structures you create and complains if anything goes wrong. PacketIO has a few helper classes that keep things organized, intellisense informed, and gathers runtime data.

The helper interface, ICustomPacket, really doesn't do much besides provide a Build function to keep packets consistent. Your custom packets should look something like this, but ya know, whatever works.

public struct MessagePacket : ICustomPacket
{
	public string id;
	public string message;

	public static Packet Build(string _id, string _message)
	{
		MessagePacket temp = new MessagePacket();
		temp.id = _id;
		temp.message = _message;
		return PacketIO.WritePacket(temp);
	}
}

Out of the box only the C# build-in types, such as float, string, and byte are supported. Fortunately you can add a layer of abstraction to those basic types with the PacketDataTypes class. The PacketDataTypes class is the spicy mayo in this oddly devised sandwich. When the suite runs, PacketDataTypes uses reflection to gather everything that extends PacketDataType. I've already added reading and writing for the C# build-in types, but you can create a packet of any type if you tell PacketIO how the data type is to be written and read. Heres and example of a vector3:

public class Mystic_Vector3 : PacketDataType
{
	public new static Type dataType = typeof(Vector3);

	public new static Vector3 Read(Packet packet)
	{
		Vector3 vector = new Vector3();
		vector.x = packet.ReadFloat();
		vector.y = packet.ReadFloat();
		vector.z = packet.ReadFloat();
    return vector;
		
	}
	public static void Write(Packet packet, Vector3 _data)
	{
		packet.Write(_data.x);
		packet.Write(_data.y);
		packet.Write(_data.z);
	}
}

Through abstraction you can piece together any data type from the built-in types or any custom types you've already created:

public class Mystic_DoubleVector3 : PacketDataType
{
	public new static Type dataType = typeof(DoubleVector3);

	public new static void Read(Packet packet)
	{
		DoubleVector3 doubleVector3 = new DoubleVector3();
		firstVector3 = Mystic_Vector3.Read(Packet packet);
		secondVector3 = Mystic_Vector3.Read(Packet packet);
    return doubleVector3;
		
	}
	public static void Write(Packet packet, doubleVector3 _data)
	{
		packet.Write(Mystic_Vector3.Write(_data.firstVector3);
    packet.Write(Mystic_Vector3.Write(_data.secondVector3);
	}
}

Plus, they'll run through PacketIO just fine.





