using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECSControl.Common
{
    #region ENUM
    public enum SECS_STATUS
    {
        UNKNOWN = 0,
        DISCONNECT,
        CONNECT,
        SELECT
    }

    public enum SECS_ERROR
    {
        NONE = 0,
        UNKNOWN
    }

    public enum TIME_OUT
    {
        T1 = 0, T2, T3, T4, T5, T6, T7, T8
    }
    #endregion

    #region STRUCT
    public struct HSMS_MSG  
    {
        public byte[] SessionID;
        public byte StreamNo;
        public byte FunctionNo;
        public byte Ptype;
        public byte Stype;
        public byte[] SystemByte;

        public void SetHeaderMsg(byte[] item)
        {
            SessionID = new byte[2];
            Array.Copy(item, SessionID, 2);
            StreamNo = item[2];
            FunctionNo = item[3];
            Ptype = item[4];
            Stype = item[5];
            SystemByte = new byte[4];
            Array.Copy(item, 6, SystemByte, 0, 4);
        }

        public void SetDataMsg()
        {

        }
    }
    #endregion

    internal class Config
    {
        internal string EquipmentID = "";
        internal string Version = "";
        internal string RemoteIPAddress = "127.0.0.1";
        internal string LocalIPAddress = "";
        internal int RemotePort = 5000;
        internal int LocalPort = 5000;
        internal int T1 = 500;  //  0.5s
        internal int T2 = 1000; //  1s
        internal int T3 = 45000;    //45s
        internal int T4 = 45000;    //45s
        internal int T5 = 10000;    //10s
        internal int T6 = 5000; //5s
        internal int T7 = 10000;    //10s
        internal int T8 = 10000;    //10s

        internal static byte[] Int2Bytes(int value, int size, bool reverse = false)
        {
            byte[] convertBytes = BitConverter.GetBytes(value);
            if ((!reverse && BitConverter.IsLittleEndian) || (reverse && !BitConverter.IsLittleEndian))
                Array.Reverse(convertBytes, 0, size);
            Array.Resize(ref convertBytes, size);
            return convertBytes;
        }

        internal static byte[] Int2Bytes(long value, int size, bool reverse = false)
        {
            byte[] convertBytes = BitConverter.GetBytes(value);
            if ((!reverse && BitConverter.IsLittleEndian) || (reverse && !BitConverter.IsLittleEndian))
                Array.Reverse(convertBytes);    
            
            Array.Resize(ref convertBytes, size);
            return convertBytes;
        }

        internal static long Bytes2Long(byte[] value, bool reverse = false)
        {
            if ((!reverse && BitConverter.IsLittleEndian) || (reverse && !BitConverter.IsLittleEndian))
                Array.Reverse(value);

            long convertValue = BitConverter.ToInt32(value,0);
            return convertValue;
        }
    }
}
