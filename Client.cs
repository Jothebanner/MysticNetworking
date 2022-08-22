using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace MysticNetworking
{
	public class Client
	{
		int disconnectTimeout = 100;
		EndPoint remoteHost;
		Socket udpSocket;
		Socket tcpSocket;
		NetworkStream stream;
		byte[] udpBuffer = new byte[Server.dataBufferSize];
		byte[] tcpBuffer = new byte[Server.dataBufferSize];

		bool networkRunning = false;
		private bool disableUDP = false;

		public List<Packet> pendingPackets = new List<Packet>();

		public event EventHandler Connected;
		public event EventHandler Disconnected;

		public void ConnectToServer(string hostAddress, int port)
		{
			if (networkRunning)
				Stop();

			IPAddress hostIPAddress;

			if (!IPAddress.TryParse(hostAddress, out hostIPAddress))
			{
				hostIPAddress = Dns.GetHostEntry(hostAddress).AddressList[0];
			}
			hostIPAddress = hostIPAddress.MapToIPv6();
			remoteHost = new IPEndPoint(hostIPAddress, port);

			try
			{
				ConnectTCP();

				networkRunning = true;
			} 
			catch (Exception e)
			{
				MysticLogger.LogException(e);
			}

		}

		public void Stop()
		{ 
			CloseSockets();
		}

		private void CloseSockets()
		{
			if (!networkRunning)
				return;

			networkRunning = false;

			if (tcpSocket != null && tcpSocket.Connected)
			{
				stream.Close(disconnectTimeout);
				tcpSocket?.Disconnect(false);
				tcpSocket?.Close(disconnectTimeout);
			}

			// reset buffers
			tcpBuffer = new byte[Server.dataBufferSize];
			udpBuffer = new byte[Server.dataBufferSize];

			Disconnected?.Invoke(null, EventArgs.Empty);
		}

		private void ConnectTCP()
		{
			// start tcp listener
			tcpSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
			tcpSocket.BeginConnect(remoteHost, new AsyncCallback(OnTCPConnected), tcpSocket);
		}

		private void OnTCPConnected(IAsyncResult result)
		{
			if (!networkRunning)
				return;

			try
			{
				tcpSocket.EndConnect(result);
			}
			catch (Exception e)
			{
				MysticLogger.LogException(e);
				if (e.GetType() == typeof(SocketException))
					MysticLogger.LogException(new Exception(((SocketException)e).SocketErrorCode.ToString()));
				Stop();
				return;
			}


			// this probably won't ever run but I think I'll leave it for a sec in-case the socket still isn't connected
			if (!tcpSocket.Connected)
			{
				MysticLogger.Log("TCP socket did not connect.");
				return;
			}

			stream = new NetworkStream(tcpSocket);
			StartTCPListener();

			// now that we know the endpoint is legit
			// TODO: maybe udp should start independantly of tcp and gracefully close if tcp doesn't work
			StartUDPListener();

			if (!disableUDP)
				SendUDPPort();

			Connected?.Invoke(null, EventArgs.Empty);
		}

		private void StartTCPListener()
		{
			try
			{
				stream.BeginRead(tcpBuffer, 0, tcpBuffer.Length, new AsyncCallback(OnTCPReceived), stream);
			}
			catch (Exception e)
			{
				MysticLogger.LogException(e);
			}
		}

		private void OnTCPReceived(IAsyncResult result)
		{
			if (!networkRunning)
				return;

			try
			{
				stream.EndRead(result);
				byte[] receiveBytes = tcpBuffer;

				stream.BeginRead(tcpBuffer, 0, tcpBuffer.Length, new AsyncCallback(OnTCPReceived), stream);

				lock (pendingPackets)
					pendingPackets.Add(new Packet(receiveBytes));

			}
			catch (ObjectDisposedException e)
			{
				MysticLogger.Log("NetworkStream is closed. Aborting BeginRead/TcpListener on client: " + e);
			}
		}

		private void StartUDPListener()
		{
			// start udp listener
			udpSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
			udpSocket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);
			//udpSocket.Bind(new IPEndPoint(IPAddress.Any, 0));
			udpSocket.Connect(remoteHost);

			udpSocket.BeginReceiveFrom(udpBuffer, 0, udpBuffer.Length, SocketFlags.None, ref remoteHost, new AsyncCallback(OnUDPReceived), udpSocket);
		}

		private void OnUDPReceived(IAsyncResult result)
		{
			try
			{
				Socket udpSocket = (Socket)result.AsyncState;
				int numberOfBytes = udpSocket.EndReceiveFrom(result, ref remoteHost);
				byte[] receiveBytes = udpBuffer;

				udpSocket.BeginReceiveFrom(udpBuffer, 0, udpBuffer.Length, SocketFlags.None, ref remoteHost, new AsyncCallback(OnUDPReceived), udpSocket);


				// lock the list to write to it thread safe
				lock (pendingPackets)
					pendingPackets.Add(new Packet(receiveBytes));
			}
			catch (Exception e)
			{
				MysticLogger.LogException(e);
			}
		}

		private void SendUDPPort()
		{
			int port = ((IPEndPoint)udpSocket.LocalEndPoint).Port;
			SendTCP(new Packet(System.Text.Encoding.ASCII.GetBytes("UDPPort" + port)));
		}

		public void SendUDP(Packet packet)
		{
			try
			{
				byte[] buffer = packet.ToArray();
				if (buffer.Length < Server.dataBufferSize)
					udpSocket.BeginSendTo(buffer, 0, buffer.Length, SocketFlags.None, remoteHost, new AsyncCallback(UDPSendCallback), udpSocket);
			}
			catch(Exception e)
			{
				MysticLogger.LogException(e);
			}
		}

		// clean up resources
		public void UDPSendCallback(IAsyncResult result)
		{
			// idk if I actually need this
			if (!networkRunning)
				return;

			try
			{
				udpSocket.EndSendTo(result);
			}
			catch(Exception e)
			{
				MysticLogger.LogException(e);
			}
		}

		public void SendTCP(Packet packet)
		{
			byte[] buffer = packet.ToArray();
			if (buffer.Length < Server.dataBufferSize)
				stream.BeginWrite(buffer, 0, buffer.Length, new AsyncCallback(TCPSendCallback), stream);
		}
		// clean up resources
		public void TCPSendCallback(IAsyncResult result)
		{
			// if connection is closed during write then don't finish
			if (!networkRunning)
				return;

			try
			{
				stream.EndWrite(result);
			}
			catch (Exception e)
			{
				MysticLogger.LogException(e);
			}
		}
	}
}

