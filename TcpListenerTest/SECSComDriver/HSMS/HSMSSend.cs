using SECSControl.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECSControl.HSMS
{
    internal class HSMSSend : CustomThread
    {
        HSMSHandler mHandler;
        bool IsThreadRun = false;

        internal void Initialize(HSMSHandler handler)
        {
            mHandler = handler;
            IsThreadRun = true;
            Start();
        }

        internal void Terminate()
        {
            IsThreadRun = false;
        }

        internal override void Run()
        {
            Debug.WriteLine("Initialize HSMSSendThread!!");
            Debug.WriteLine("Terminate HSMSSendThread!!");
        }

        internal void SendControlMessage(int rspCode, int type, long systemBytes)
        {
            byte[] sendData = new byte[14];
            Array.Copy(Config.Int2Bytes(10, 4), sendData, 4);   //  Length
            sendData[4] = byte.MaxValue;
            sendData[5] = byte.MaxValue;
            sendData[7] = (byte)rspCode;
            sendData[9] = (byte)type;
            Array.Copy(Config.Int2Bytes(systemBytes, 4, true), 0, sendData, 10, 4);   //  SystemBytes
            
            mHandler.mWriter.Write(sendData);
            mHandler.mWriter.Flush();
        }
    }
}
