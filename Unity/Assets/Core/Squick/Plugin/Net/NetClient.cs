using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Drawing;
using UnityEditor.Experimental.GraphView;

namespace Squick
{
    public enum NetState
    {
        Connecting,
        Connected,
        Disconnected
    }
	
	public enum NetEventType
    {
        None,
        Connected,
        Disconnected,
        ConnectionRefused,
        DataReceived
    }

    public class SocketPacket
    {
        public SocketPacket()
        {
            sb = new StringRingBuffer(ConstDefine.NF_PACKET_BUFF_SIZE);
        }

        private StringRingBuffer sb;
        internal StringRingBuffer Sb { get => sb; set => sb = value; }

        public void Reset()
        {
            sb.Clear();
        }

        public void FromBytes(byte[] by, int bytesCount)
        {
            sb.Clear();
            sb.Push(by, bytesCount);
        }
    }

    public class NetEventParams
    {
        public void Reset()
        {
             client = null;
             clientID = 0;
             socket = null;
             eventType = NetEventType.None;
             message = "";
             packet = null;
        }

        public NetClient client = null;
        public int clientID = 0;
        public TcpClient socket = null;
        public NetEventType eventType = NetEventType.None;
        public string message = "";
        public SocketPacket packet = null;

    }
	
    public class NetClient
    {
        private string m_hostname;
        private int m_port;
        void PrintBytes(string info, byte[] bytesData, int len)
        {
            string data = "";
            int id = bytesData[0] * 15 + bytesData[1];
            for (int i = 0; i < len; i++)
            {
                data += bytesData[0 + i].ToString("X02") + ","; // .toString("x2"); 
            }
            string prev = "NetDebug: server " + m_hostname + ":" + m_port + " " + " msgid: " + id + " [" + ((SquickStruct.EGameMsgID)id).ToString() + "]  ";
            Debug.Log(prev + info + data + "  [" + data.ToString() + " ]");
        }
		public NetClient(NetListener xNetListener)
        {
			mxNetListener = xNetListener;
            Init();
        }

        void Init()
        {

            mxState = NetState.Disconnected;
            mxEvents = new Queue<NetEventType>();
            mxMessages = new Queue<string>();
            mxPackets = new Queue<SocketPacket>();
            mxPacketPool = new Queue<SocketPacket>();
        }

        private NetState mxState;
        private NetworkStream mxStream;
        private StreamWriter mxWriter;
        private StreamReader mxReader;
        private Thread mxReadThread;
        private TcpClient mxClient;
        private Queue<NetEventType> mxEvents;
        private Queue<string> mxMessages;
        private Queue<SocketPacket> mxPackets;
        private Queue<SocketPacket> mxPacketPool;

        private NetListener mxNetListener;

        private byte[] tempReadBytes = new byte[ConstDefine.NF_PACKET_BUFF_SIZE];

        private NetEventParams eventParams = new NetEventParams();

        public bool IsConnected()
        {
            return mxState == NetState.Connected;
        }

        public NetState GetState()
        {
            return mxState;
        }

        public NetListener GetNetListener()
        {
            return mxNetListener;
        }

        public void Execute()
        {

            while (mxEvents.Count > 0)
            {
                lock (mxEvents)
                {
                    if (mxEvents.Count > 0)
                    {
                        NetEventType eventType = mxEvents.Dequeue();

                        eventParams.Reset();
                        eventParams.eventType = eventType;
                        eventParams.client = this;
                        eventParams.socket = mxClient;

                        if (eventType == NetEventType.Connected)
                        {
                            mxNetListener.OnClientConnect(eventParams);
                        }
                        else if (eventType == NetEventType.Disconnected)
                        {
                            mxNetListener.OnClientDisconnect(eventParams);

                            mxReader.Close();
                            mxWriter.Close();
                            mxClient.Close();

                        }
                        else if (eventType == NetEventType.DataReceived)
                        {
                            lock (mxPackets)
                            {
                                if (mxPackets.Count > 0)
                                {
                                    eventParams.packet = mxPackets.Dequeue();

                                    mxNetListener.OnDataReceived(eventParams);

                                    mxPacketPool.Enqueue(eventParams.packet);
                                }
                            }
                        }
                        else if (eventType == NetEventType.ConnectionRefused)
                        {

                        }
                    }
                }
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {

                TcpClient tcpClient = (TcpClient)ar.AsyncState;
                tcpClient.EndConnect(ar);

                SetTcpClient(tcpClient);

            }
            catch (Exception e)
            {
                lock (mxEvents)
                {
                    mxEvents.Enqueue(NetEventType.ConnectionRefused);
                }
            }
        }
        private SocketPacket GetPacketFromPool()
        {
            if (mxPacketPool.Count <= 0)
            {
                mxPacketPool.Enqueue(new SocketPacket());
            }

            SocketPacket packet = mxPacketPool.Dequeue();
            packet.Reset();

            return packet;
        }
        private void ReadData()
        {
            bool endOfStream = false;

            while (!endOfStream)
            {
               int bytesRead = 0;
               try
               {
                    Array.Clear(tempReadBytes, 0, ConstDefine.NF_PACKET_BUFF_SIZE);
                    
                    bytesRead = mxStream.Read(tempReadBytes, 0, ConstDefine.NF_PACKET_BUFF_SIZE);


                    PrintBytes("NetClient.cs:231, Recv Bytes: " ,tempReadBytes, bytesRead);


                }
               catch (Exception e)
               {
                   e.ToString();
               }

               if (bytesRead == 0)
               {

                   endOfStream = true;

               }
               else
               {
                   lock (mxEvents)
                   {

                       mxEvents.Enqueue(NetEventType.DataReceived);
                   }
                   lock (mxPackets)
                   {
                        SocketPacket packet = GetPacketFromPool();
                        packet.FromBytes(tempReadBytes, bytesRead);

                        mxPackets.Enqueue(packet);
                   }

               }
            }

            mxState = NetState.Disconnected;

            mxClient.Close();
            lock (mxEvents)
            {
                mxEvents.Enqueue(NetEventType.Disconnected);
            }

        }

        // Public
        public void Connect(string hostname, int port)
        {
            if (mxState == NetState.Connected)
            {
                return;
            }

            mxState = NetState.Connecting;

            mxMessages.Clear();
            mxEvents.Clear();
            m_hostname = hostname;
            m_port = port;
            mxClient = new TcpClient();
            mxClient.NoDelay = true;
            mxClient.BeginConnect(hostname,
                                 port,
                                 new AsyncCallback(ConnectCallback),
                                 mxClient);

        }

        public void Disconnect()
        {
            mxState = NetState.Disconnected;

            try { if (mxReader != null) mxReader.Close(); }
            catch (Exception e) { e.ToString(); }
            try { if (mxWriter != null) mxWriter.Close(); }
            catch (Exception e) { e.ToString(); }
            try { if (mxClient != null) mxClient.Close(); }
            catch (Exception e) { e.ToString(); }
        }

        public void SendBytes(byte[] bytes, int length)
        {
            SendBytes(bytes, 0, length);
        }

        private void SendBytes(byte[] bytes, int offset, int size)
        {
            
            PrintBytes( "NetClient.cs:316, Send Bytes: ", bytes, size);
            if (!IsConnected())
                return;
            try
            {
                mxStream.Write(bytes, offset, size);
                mxStream.Flush();
            }
            catch (Exception e)
            {
                lock (mxEvents)
                {
                    mxEvents.Enqueue(NetEventType.Disconnected);
                    Disconnect();
                }
            }

        }

        private void SetTcpClient(TcpClient tcpClient)
        {

            mxClient = tcpClient;

            if (mxClient.Connected)
            {

                mxStream = mxClient.GetStream();
                mxReader = new StreamReader(mxStream);
                mxWriter = new StreamWriter(mxStream);

                mxState = NetState.Connected;

                mxEvents.Enqueue(NetEventType.Connected);

                mxReadThread = new Thread(ReadData);
                mxReadThread.IsBackground = true;
                mxReadThread.Start();
            }
            else
            {
                mxState = NetState.Disconnected;
            }
        }
    }
}