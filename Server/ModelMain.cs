using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Server
{
    class ModelMain : ModelBase
    {
        public Command SendCommand { get; set; }
        TcpListener listener;
        Thread thr;
        TcpClient client;

        public ModelMain()
        {
            SendCommand = new Command(Send);
            thr = new Thread(new ThreadStart(ListenRequests));
            thr.IsBackground = true;
            thr.Start();
        }

        private void Send(object obj)
        {
            if (client == null)
            {
                MessageBox.Show("Error");
                return;
            }
            try
            {
                NetworkStream stream = client.GetStream();
                if (stream.CanWrite)
                {
                    if (Str != null && Str != "")
                    {
                        string serverMsg = Str;
                        byte[] serverMsgAsByteArray = Encoding.UTF8.GetBytes(serverMsg);
                        stream.Write(serverMsgAsByteArray, 0, serverMsgAsByteArray.Length);
                        MsgBox += "Server] " + serverMsg + Environment.NewLine;
                    }
                }
            }
            catch (SocketException se)
            {
                MessageBox.Show("Socket exception " + se.ToString());
            }
            catch (ArgumentNullException)
            {

            }
            Str = "";
        }

        private void ListenRequests()
        {
            try
            {
                listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8080);
                listener.Start();
                MsgBox += "Server is listening............." + Environment.NewLine;
                Byte[] bytes = new Byte[1024];
                while (true)
                {
                    using (client = listener.AcceptTcpClient())
                    {
                        MsgBox += "client connected!" + Environment.NewLine;
                        using (NetworkStream stream = client.GetStream())
                        {
                            int length;
                            try
                            {
                                while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                                {
                                    var incomingData = new Byte[length];
                                    Array.Copy(bytes, 0, incomingData, 0, length);
                                    string clientMsg = Encoding.UTF8.GetString(incomingData);
                                    MsgBox += "Client] " + clientMsg + Environment.NewLine;
                                }
                            }
                            catch (Exception)
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
                MessageBox.Show("Socket exception " + se.ToString());
            }
        }
    }
}

