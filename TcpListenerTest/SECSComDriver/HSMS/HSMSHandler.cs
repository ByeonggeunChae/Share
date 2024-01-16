using SECSControl.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SECSControl.HSMS
{
    internal class HSMSHandler
    {
        public delegate void SECSConnectDelegate(string aDriverName, XmlDocument aXML);
        public delegate void SECSDisConnectDelegate(string aDriverName, XmlDocument aXML);
        public delegate void SECSTimeOutDelegate(string aDriverName, XmlDocument aXML);
        public delegate void SECSReceivedDelegate(string aDriverName, XmlDocument aXML);
        public delegate void SECSInvalidReceivedDelegate(string aDriverName, XmlDocument aXML);
        public delegate void SECSAbortReceivedDelegate(string aDriverName, XmlDocument aXML);
        public delegate void SECSUnkownMsgReceivedDelegate(string aDriverName, XmlDocument aXML);
        public delegate void SECS1LogDelegate(string aDriverName, string aDirection, string aLog, string aMessageData);
        public delegate void SECS2LogDelegate(string aDriverName, string aDirection, string aLog);

        public event SECSConnectDelegate OnSECSConnected;
        public event SECSDisConnectDelegate OnSECSDisConnected;
        public event SECSTimeOutDelegate OnSECSTimeOut;
        public event SECSReceivedDelegate OnSECSReceived;
        public event SECSInvalidReceivedDelegate OnSECSInvalidMessage;
        public event SECSAbortReceivedDelegate OnSECSAbortMessage;
        public event SECSUnkownMsgReceivedDelegate OnSECSUnknownMessage;
        public event SECS1LogDelegate OnSECS1Log;
        public event SECS2LogDelegate OnSECS2Log;

        public TcpClient mClient;
        public BinaryReader mReader;
        public BinaryWriter mWriter;
        public Config mConfig;

        HSMSConnect mHSMSConnect = null;
        HSMSReceive mHSMSReceive = null;
        HSMSDataMessage mHSMSConvert = null;
        HSMSSend mHSMSSend = null;

        SECS_STATUS mStatus = SECS_STATUS.UNKNOWN;

        internal HSMSHandler()
        {
            mConfig = new Config();
        }

        internal SECS_ERROR Initialize(string EquipmentID)
        {
            try
            {
                mConfig.EquipmentID = EquipmentID;
                HSMSConnectThread();
                return SECS_ERROR.NONE;
            }
            catch (Exception ex)
            {
                return SECS_ERROR.UNKNOWN;
            }
        }

        internal void Terminate()
        {
            TerminateThread();
        }

        private void HSMSConnectThread()
        {
            mHSMSConnect = new HSMSConnect();
            mHSMSConnect.Initialize(this);
        }

        public void HSMSConnect(TcpClient client)
        {
            SetStatus(SECS_STATUS.CONNECT);
            mClient = client;
            mReader = new BinaryReader(client.GetStream());
            mWriter = new BinaryWriter(client.GetStream());

            InitializeThread();
        }

        private void InitializeThread()
        {
            mHSMSReceive = new HSMSReceive();
            mHSMSReceive.Initialize(this);

            mHSMSConvert = new HSMSDataMessage();
            mHSMSConvert.Initialize(this);

            mHSMSSend = new HSMSSend();
            mHSMSSend.Initialize(this);
        }

        private void TerminateThread()
        {
            if (mHSMSSend != null)
            {
                mHSMSSend.Terminate();
                mHSMSSend.Join(100);
                mHSMSSend = null;
            }

            if (mHSMSReceive != null)
            {
                mHSMSReceive.Terminate();
                mHSMSReceive.Join(100);
                mHSMSReceive = null;
            }

            if (mHSMSConnect != null)
            {
                mHSMSConnect.Terminate();
                mHSMSConnect.Join(100);
                mHSMSConnect = null;
            }
        }

        private SECS_STATUS GetStatus()
        {
            return mStatus;
        }

        private void SetStatus(SECS_STATUS status)
        {
            mStatus = status;
        }

        public void HSMSReceive(HSMSItem item)
        {
            byte[] numArray = new byte[4];

            Array.Copy((Array)item.Header, 6, (Array)numArray, 0, 4);
            long systemBytes = Config.Bytes2Long(numArray, true);
            byte num = item.Header[5];
            if (num == (byte)0)
            {
                item.IsControlMsg = false;
                mHSMSConvert.Enqueue(item);
            }
            else
            {
                item.IsControlMsg = true;
                switch (num)
                {
                    case 1: //  Select
                        if (GetStatus() == SECS_STATUS.SELECT)
                            mHSMSSend.SendControlMessage(1, 2, systemBytes);
                        else
                        {
                            mHSMSSend.SendControlMessage(0, 2, systemBytes);
                            SetStatus(SECS_STATUS.SELECT);
                        }
                        break;
                    case 2:
                        break;
                    case 3: //  Deselect
                        if (GetStatus() == SECS_STATUS.SELECT)
                        {
                            SetStatus(SECS_STATUS.CONNECT);
                            mHSMSSend.SendControlMessage(0, 4, systemBytes);
                        }
                        break;
                    case 5: //  LinkTest
                        mHSMSSend.SendControlMessage(0, 6, systemBytes);
                        break;
                    case 7: //  Reject
                        break;
                    case 9: //  Separate
                        DisConnect();
                        break;
                }
            }
        }

        private void DisConnect()
        {
            if (GetStatus() < SECS_STATUS.CONNECT)
                Debug.WriteLine($"CurrentStatus={GetStatus().ToString()}");
            SetStatus(SECS_STATUS.DISCONNECT);

            if (OnSECSDisConnected != null)
                OnSECSDisConnected(mConfig.EquipmentID, null);
            
            TerminateThread();
        }
    }
}
