using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;

namespace MysticNetworking
{
	public class Server
	{
		public static int disconnectTimeout = 100;
		public static int dataBufferSize = 256;
		public List<Packet> pendingPackets = new List<Packet>();

		public event EventHandler<Connection> ClientConnected;
		public event EventHandler<Connection> ClientDisconnected;
		public event EventHandler ServerStarted;
		public event EventHandler ServerStopped;

		public Dictionary<string, Connection> Connections = new Dictionary<string, Connection>();

		public int port { get; private set; }

		public int maxClientCount { get; private set; }

		public Server(int _port, int _maxClientCount)
		{
			port = _port;
			maxClientCount = _maxClientCount;
		}

		private bool networkRunning = false;

		private void OnClientConnected(Connection connection)
		{
			//TODO: make this happen if user doesn't have autoid turned off
			//Connections.Add(Connections.Count + 1, connection);

			/// stop coming back to impliment threading. You already did that.
			// start with threads // hodl up, beginread/write is theaded and managed by the os
			StartTCPListener(connection);

			ClientConnected?.Invoke(this, connection);
		}
		
		private void OnClientDisconnected(Connection connection)
		{
			connection.stream.Close(disconnectTimeout);
			connection.tcpSocket.Close(disconnectTimeout);

			ClientDisconnected?.Invoke(this, connection);
		}

		private void OnServerStarted()
		{
			ServerStarted?.Invoke(null, EventArgs.Empty);
		}
		
		private void OnServerStopped()
		{
			ServerStopped?.Invoke(this, new EventArgs());
		}

		Socket TCPNewConnectionListener;

		public void Start()
		{
			try
			{
				networkRunning = true;
				// one udp listener should be able to handle all of the connections
				StartUDPListener();

				// server listens for any address on the specified port
				//IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
				IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);

				TCPNewConnectionListener = new Socket(SocketType.Stream, ProtocolType.Tcp);
				TCPNewConnectionListener.Bind(localEndPoint);
				TCPNewConnectionListener.Listen(10);

				// os managed threading
				TCPNewConnectionListener.BeginAccept(new AsyncCallback(AcceptConnectionCallback), TCPNewConnectionListener);

				OnServerStarted();
			}
			catch (Exception e)
			{
				MysticLogger.Log(e);
			}
		}

		public void Stop()
		{
			networkRunning = false;
			TCPNewConnectionListener?.Close();
			foreach (KeyValuePair<string, Connection> connectionItem in Connections)
			{
				connectionItem.Value.CloseConnection();
			}
			Connections.Clear();

			OnServerStopped();
		}

		private void AcceptConnectionCallback(IAsyncResult result)
		{
			// if server closes don't have an aneurysm
			if (!networkRunning) // this pretty much closes the "listener thread" because it prevents socket.beginaccept to be called again
				return;

			Socket socket = result.AsyncState as Socket;

			try
			{
				Socket newTCPSocket = socket.EndAccept(out byte[] buffer, result);

				// restart listen thread
				socket.BeginAccept(new AsyncCallback(AcceptConnectionCallback), socket); // TODO: make this always restart even if there is an error with a connection attempt

				Connection newConnection = new Connection(newTCPSocket);

				OnClientConnected(newConnection);
			}
			catch (ObjectDisposedException e)
			{
				Debug.Log("Aborting accept socket/thread thingy: " + e);
			}
		}

		// start tcp and udp listeners for each connection
		private void StartTCPListener(Connection connection)
		{
			try
			{
				NetworkStream stream = connection.stream;
				// os managed threading
				stream.BeginRead(connection.tcpBuffer, 0, connection.tcpBuffer.Length, new AsyncCallback(OnTCPReceived), connection);
			}
			catch (Exception e)
			{
				MysticLogger.Log(e);
			}
		}

		private void OnTCPReceived(IAsyncResult result)
		{
			// if server is stopped then stop trying to do stuff
			if (!networkRunning)
				return;

			try
			{
				int bytesReceived = ((Connection)result.AsyncState).stream.EndRead(result);

				if (bytesReceived == 0)
				{
					OnClientDisconnected((Connection)result.AsyncState);
					return;
				}

				byte[] receiveBytes = ((Connection)result.AsyncState).tcpBuffer;

				//TODO: not this // good heavens why did I do this
				if (((Connection)result.AsyncState).udpEndPoint == null)
				{
					if (System.Text.Encoding.ASCII.GetString(receiveBytes, 0, 7) == "UDPPort")
					{
						string portString = System.Text.Encoding.ASCII.GetString(receiveBytes, 7, receiveBytes.Length - 7);
						
						int port = int.Parse(portString);

						if (IPAddress.TryParse((((Connection)result.AsyncState).endPoint as IPEndPoint).Address.ToString(), out IPAddress address))
						{
							((Connection)result.AsyncState).udpEndPoint = new IPEndPoint(address, port);
						}
						else
						{
							MysticLogger.Log(new Exception("Error parsing UDP ipaddress"));
						}
						// restart listener
						((Connection)result.AsyncState).stream.BeginRead(((Connection)result.AsyncState).tcpBuffer, 0, ((Connection)result.AsyncState).tcpBuffer.Length, new AsyncCallback(OnTCPReceived), ((Connection)result.AsyncState));
						return;
					}
				}

				// os managed threading
				((Connection)result.AsyncState).stream.BeginRead(((Connection)result.AsyncState).tcpBuffer, 0, ((Connection)result.AsyncState).tcpBuffer.Length, new AsyncCallback(OnTCPReceived), ((Connection)result.AsyncState));

				Packet packet = new Packet(receiveBytes);

				lock (pendingPackets)
						pendingPackets.Add(packet);

			}
			catch (ObjectDisposedException e)
			{
				MysticLogger.Log("Aborting begin read of server stream: " + e);
			}

		}

		Socket udpSocket;
		byte[] udpBuffer = new byte[Server.dataBufferSize];

		private void StartUDPListener()
		{
			udpSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
			udpSocket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);

			udpSocket.Bind(new IPEndPoint(IPAddress.Any, port));

			udpSocket.BeginReceive(udpBuffer, 0, udpBuffer.Length, SocketFlags.None, new AsyncCallback(OnUDPReceived), udpSocket);
		}

		private void OnUDPReceived(IAsyncResult result)
		{
			if (!networkRunning)
				return;

			try
			{
				Socket udpSocket = result.AsyncState as Socket;
				int numberOfBytes = udpSocket.EndReceive(result);
				byte[] receiveBytes = udpBuffer;

				// restart os managed threading
				udpSocket.BeginReceive(udpBuffer, 0, udpBuffer.Length, SocketFlags.None, new AsyncCallback(OnUDPReceived), udpSocket);

				// thread safety
				lock (pendingPackets)
					pendingPackets.Add(new Packet(receiveBytes));
			}
			catch (Exception e)
			{
				MysticLogger.Log(e);
			}
		}

		// UDP
		public void SendUDP(Packet packet, Connection connection)
		{
			byte[] buffer = packet.ToArray();
			if (connection.udpEndPoint != null)
				if (buffer.Length < Server.dataBufferSize)
					udpSocket.BeginSendTo(buffer, 0, buffer.Length, SocketFlags.None, connection.udpEndPoint, new AsyncCallback(UDPSendCallback), udpSocket);
				else
					MysticLogger.Log(new Exception("Packet length is longer than the set buffer. Buffer length: " + Server.dataBufferSize));
			else
				MysticLogger.Log(new Exception("Unable to send packet because the UDP endpoint for the connection not set."));
		}

		public void SendUDPToAll(Packet packet)
		{
			byte[] packetBuffer = packet.ToArray();
			if (Connections.Count > 0)
			{
				foreach (KeyValuePair<string, Connection> connectionItem in Connections)
				{
					try
					{
						if (connectionItem.Value.udpEndPoint != null)
						{
							//TODO: if too long then report
							if (packetBuffer.Length < Server.dataBufferSize)
								udpSocket.BeginSendTo(packetBuffer, 0, packetBuffer.Length, SocketFlags.None, connectionItem.Value.udpEndPoint, new AsyncCallback(UDPSendCallback), udpSocket);
						}
					}
					catch (Exception e)
					{
						MysticLogger.Log(e);
					}
				}
			}
			else
			{
				MysticLogger.Log(new Exception("UDP message to all not sent because the connections dictionary is empty"));
			}
		}

		// clean up resources
		private void UDPSendCallback(IAsyncResult result)
		{
			((Socket)result.AsyncState).EndSendTo(result);
		}


		// TCP
		public void SendTCP(Packet packet, Connection connection)
		{
			byte[] buffer = packet.ToArray();
			if (buffer.Length < Server.dataBufferSize)
				connection.stream.BeginWrite(buffer, 0, buffer.Length, new AsyncCallback(TCPSendCallback), connection);
		}

		public void SendTCPToAll(Packet packet)
		{
			byte[] packetBuffer = packet.ToArray();
			if (Connections.Count > 0)
			{
				foreach (KeyValuePair<string, Connection> connectionItem in Connections)
				{
					try
					{
						if (connectionItem.Value.stream != null)
						{
							if (packetBuffer.Length < Server.dataBufferSize)
								connectionItem.Value.stream.BeginWrite(packetBuffer, 0, packetBuffer.Length, new AsyncCallback(TCPSendCallback), connectionItem.Value);
						}
					}
					catch (Exception e)
					{
						MysticLogger.Log(e);
					}
				}
			}
			else
			{
				MysticLogger.Log("TCP message to all not sent because the connections dictionary is empty");
			}
		}

		// clean up resources
		private void TCPSendCallback(IAsyncResult result)
		{
			((Connection)result.AsyncState).stream.EndWrite(result);
		}
	}
}
