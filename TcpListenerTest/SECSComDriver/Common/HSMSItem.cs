using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECSControl.Common
{
    internal class HSMSItem
    {
        internal int Length;
        internal byte[] Header;
        internal byte[] DataItem;
        internal bool IsControlMsg;
        internal byte[] CheckSum;
        internal string MessageData = "";
    }
}
