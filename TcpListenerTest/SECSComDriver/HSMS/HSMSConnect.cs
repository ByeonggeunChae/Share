using SECSControl.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SECSControl.HSMS
{
    internal class HSMSConnect : CustomThread
    {
        private bool IsThreadRun = false;
        private bool mIsActive = false;
        private int mSleepInterval = 1000;
        private int mDivide = 0;
        private TcpListener mListener;
        private HSMSHandler mHandler;

        internal void Initialize(HSMSHandler handler)
        {
            mHandler = handler;
            IsThreadRun = true;
            Start();
        }

        internal void Terminate()
        {
            IsThreadRun = false;
            if(mListener != null)
                mListener.Stop();
            mListener = null;
        }

        internal override void Run()
        {
            //MessageBox.Show($"{DateTime.Now.ToString("yyyyMMdd_HH:mm:ss:fff")}_HSMSConnect Start!!");
            Debug.WriteLine("Initialize HSMSConnectThread!!");

            mDivide = 0;

            while(IsThreadRun)
            {
                try
                {
                    TcpClient client;
                    if(mIsActive)
                    {
                        if(mDivide != 0 && mDivide % mHandler.mConfig.T5 == 0)
                        {
                            //mHandler.TimeOut(TIME_OUT.T5);
                        }
                        client = Active();
                    }
                    else
                    {
                        mListener = string.IsNullOrEmpty(mHandler.mConfig.LocalIPAddress)? new TcpListener(new IPEndPoint(IPAddress.Any, mHandler.mConfig.LocalPort)) :  new TcpListener(new IPEndPoint(IPAddress.Parse(mHandler.mConfig.RemoteIPAddress), mHandler.mConfig.RemotePort));
                        mListener.Start();
                        client = mListener.AcceptTcpClient();
                        mListener.Stop();
                    }
                    mHandler.HSMSConnect(client);
                    IsThreadRun = false;
                }
                catch(SocketException e)
                {

                }
                catch(Exception e)
                {

                }
                finally 
                {
                    mDivide += mSleepInterval;
                    Thread.Sleep(mSleepInterval);
                }
            }
            Debug.WriteLine("Terminate HSMSConnectThread!!");
        }

        private TcpClient Active()
        {
            try
            {
                TcpClient client = string.IsNullOrEmpty(mHandler.mConfig.LocalIPAddress) ?  new TcpClient() : new TcpClient(new IPEndPoint(IPAddress.Parse(mHandler.mConfig.LocalIPAddress), 0));
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(mHandler.mConfig.RemoteIPAddress), mHandler.mConfig.RemotePort);
                client.Connect(remoteEP);
                return client;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
