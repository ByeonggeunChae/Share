using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using SECSControl;
using SECSControl.Common;
using SECSControl.HSMS;

namespace SECSControl
{
    public class SECSManager : IDisposable
    {
        public delegate void SECSConnected(string EquipmentID, XmlDocument XML);
        public delegate void SECSDisConnected(string EquipmentID, XmlDocument XML);
        public delegate void SECSReceived(string EquipmentID, string SECSMsgName, XmlDocument XML);
        public delegate void SECSReceivedWithBodyByte(string EquipmentID, string SECSMsgName, XmlDocument XML, byte[] SECS1Body);
        public delegate void SECSInvalidReceived(string EquipmentID, XmlDocument XML);
        public delegate void SECSTimeOut(string EquipmentID, XmlDocument XML, string TimeOut);
        public delegate void SECSUnknownMessage(string EquipmentID, XmlDocument XML);
        public delegate void SECSAbortMessage(string EquipmentID, XmlDocument XML);
        public delegate void SECS1Log(string EquipmentID, string Direction, string SECS1, string MessageData);
        public delegate void SECS2Log(string EquipmentID, string Direction, string SECS2);

        public event SECSConnected OnSECSConnected;
        public event SECSDisConnected OnSECSDisConnected;
        public event SECSReceived OnSECSReceived;
        public event SECSReceivedWithBodyByte OnSECSReceivedWithBodyByte;
        public event SECSInvalidReceived OnSECSInvalidReceived;
        public event SECSTimeOut OnSECSTimeOut;
        public event SECSUnknownMessage OnSECSUnknownMessage;
        public event SECSAbortMessage OnSECSAbortMessage;
        public event SECS1Log OnSECS1Log;
        public event SECS2Log OnSECS2Log;

        HSMSHandler mHSMSHandler;
        ConcurrentQueue<int> mHandlerQueue = new ConcurrentQueue<int>();
        bool IsThreadRun = false;

        public SECSManager()
        {

        }

        ~SECSManager() => Dispose();

        public void Dispose()
        {
            ReleaseEventHandler();
        }

        public SECS_ERROR Initialize(string EquipmentID)
        {
            SECS_ERROR errorCode = SECS_ERROR.NONE;
            mHSMSHandler = new HSMSHandler();
            LinkEventHandler();
            IsThreadRun = true;
            errorCode = mHSMSHandler.Initialize(EquipmentID);
            return errorCode;
        }
        private void LinkEventHandler()
        {
            mHSMSHandler.OnSECSConnected += MHSMSHandler_OnSECSConnected;
            mHSMSHandler.OnSECSDisConnected += MHSMSHandler_OnSECSDisConnected;
            mHSMSHandler.OnSECSTimeOut += MHSMSHandler_OnSECSTimeOut;
            mHSMSHandler.OnSECSReceived += MHSMSHandler_OnSECSReceived;
            mHSMSHandler.OnSECSInvalidMessage += MHSMSHandler_OnSECSInvalidMessage;
            mHSMSHandler.OnSECSAbortMessage += MHSMSHandler_OnSECSAbortMessage;
            mHSMSHandler.OnSECSUnknownMessage += MHSMSHandler_OnSECSUnknownMessage;
            mHSMSHandler.OnSECS1Log += MHSMSHandler_OnSECS1Log;
            mHSMSHandler.OnSECS2Log += MHSMSHandler_OnSECS2Log;
        }

        private void ReleaseEventHandler()
        {
            mHSMSHandler.OnSECSConnected -= MHSMSHandler_OnSECSConnected;
            mHSMSHandler.OnSECSDisConnected -= MHSMSHandler_OnSECSDisConnected;
            mHSMSHandler.OnSECSTimeOut -= MHSMSHandler_OnSECSTimeOut;
            mHSMSHandler.OnSECSReceived -= MHSMSHandler_OnSECSReceived;
            mHSMSHandler.OnSECSInvalidMessage -= MHSMSHandler_OnSECSInvalidMessage;
            mHSMSHandler.OnSECSAbortMessage -= MHSMSHandler_OnSECSAbortMessage;
            mHSMSHandler.OnSECSUnknownMessage -= MHSMSHandler_OnSECSUnknownMessage;
            mHSMSHandler.OnSECS1Log -= MHSMSHandler_OnSECS1Log;
            mHSMSHandler.OnSECS2Log -= MHSMSHandler_OnSECS2Log;
        }

        private void MHSMSHandler_OnSECSConnected(string aDriverName, XmlDocument aXML)
        {
            if (OnSECSConnected != null)
                OnSECSConnected(aDriverName, aXML);
        }

        private void MHSMSHandler_OnSECSDisConnected(string aDriverName, XmlDocument aXML)
        {
            if (OnSECSDisConnected != null)
                OnSECSDisConnected(aDriverName, aXML);
        }

        private void MHSMSHandler_OnSECSTimeOut(string aDriverName, XmlDocument aXML)
        {
            throw new NotImplementedException();
        }

        private void MHSMSHandler_OnSECSReceived(string aDriverName, XmlDocument aXML)
        {
            throw new NotImplementedException();
        }

        private void MHSMSHandler_OnSECSInvalidMessage(string aDriverName, XmlDocument aXML)
        {
            throw new NotImplementedException();
        }

        private void MHSMSHandler_OnSECSAbortMessage(string aDriverName, XmlDocument aXML)
        {
            throw new NotImplementedException();
        }

        private void MHSMSHandler_OnSECSUnknownMessage(string aDriverName, XmlDocument aXML)
        {
            throw new NotImplementedException();
        }

        private void MHSMSHandler_OnSECS1Log(string aDriverName, string aDirection, string aLog, string aMessageData)
        {
            throw new NotImplementedException();
        }

        private void MHSMSHandler_OnSECS2Log(string aDriverName, string aDirection, string aLog)
        {
            throw new NotImplementedException();
        }

        private void Run()
        {
            while (IsThreadRun)
            {
                try
                {
                    if (!mHandlerQueue.IsEmpty)
                    {
                        int eventType;
                        mHandlerQueue.TryDequeue(out eventType);

                        switch (eventType)
                        {
                            case 0:
                                OnSECSConnected("", null);
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    Thread.Sleep(10);
                }
            }
        }
    }
}
