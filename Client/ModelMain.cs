using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Client
{
    class ModelMain : ModelBase
    {
        TcpClient client;
        Thread receiveThr;
        public Command ConnectCommand { get; set; }
        public Command SendCommand { get; set; }
        public ModelMain()
        {
            ConnectCommand = new Command(Connect);
            SendCommand = new Command(Send);
            State = "Disconnected";
        }

        private void Send(object obj)
        {
            if (client == null)
            {
                return;
            }
            try
            {
                NetworkStream stream = client.GetStream();
                if (stream.CanWrite)
                {
                    if (Str != null && Str != "")
                    {
                        string clientMsg = Str;
                        byte[] clientMsgAsByteArray = Encoding.UTF8.GetBytes(clientMsg);
                        stream.Write(clientMsgAsByteArray, 0, clientMsgAsByteArray.Length);
                        MsgBox += "Client] " + clientMsg + Environment.NewLine;
                    }
                }
            }
            catch (SocketException se)
            {
                MessageBox.Show("Socket exception: " + se);
            }
            catch (ArgumentNullException )
            {

            }
            Str = "";
        }

        private void Connect(object obj)
        {
            try
            {
                receiveThr = new Thread(new ThreadStart(ListenData));
                receiveThr.IsBackground = true;
                receiveThr.Start();
            }
            catch (Exception e)
            {
                MessageBox.Show("Client connection exception " + e);
            }
        }

        private void ListenData()
        {
            try
            {
                client = new TcpClient();
                IPAddress ip = IPAddress.Parse(IPAddr);
                client.Connect(ip, Port);
                if (client.Connected == true)
                {
                    MsgBox += "client connected!" + Environment.NewLine;
                    State = "Connected";
                    ForegroundColor = System.Windows.Media.Brushes.Blue;
                    Byte[] bytes = new Byte[1024];
                    while (true)
                    {
                        using (NetworkStream stream = client.GetStream())
                        {
                            int length;
                            try
                            {
                                while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                                {
                                    var incommingData = new byte[length];
                                    Array.Copy(bytes, 0, incommingData, 0, length);
                                    string serverMsg = Encoding.UTF8.GetString(incommingData);
                                    MsgBox += "Server] " + serverMsg + Environment.NewLine;
                                }
                            }
                            catch (Exception )
                            {
                                client.Close();
                                MsgBox += "connection close" + Environment.NewLine;
                            }
                        }
                    }
                }
            }
            catch (SocketException se)
            {
                MessageBox.Show("Socket exception: " + se);
            }
        }

    }
}
