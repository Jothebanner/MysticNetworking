using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

namespace MysticNetworking
{
	public class Connection
	{
		public EndPoint endPoint;
		public EndPoint udpEndPoint;
		public Socket tcpSocket;
		public byte[] tcpBuffer = new byte[Server.dataBufferSize];
		public byte[] udpBuffer = new byte[Server.dataBufferSize];
		public NetworkStream stream { get; private set; }

		public Connection(Socket _socket)
		{
			stream = new NetworkStream(_socket);
			tcpSocket = _socket;
			endPoint = _socket.RemoteEndPoint;
		}

		public void CloseConnection()
		{
			stream.Close();
			tcpSocket.Close(Server.disconnectTimeout);
		}
	}
}
