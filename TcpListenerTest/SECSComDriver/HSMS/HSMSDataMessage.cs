using SECSControl.Common;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SECSControl.HSMS
{
    internal class HSMSDataMessage : CustomThread
    {
        private bool IsThreadRun = false;
        private ConcurrentQueue<HSMSItem> mMessageQueue = new ConcurrentQueue<HSMSItem>();
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
        }

        internal void Enqueue(HSMSItem item)
        {
            mMessageQueue.Enqueue(item);
        }

        internal override void Run()
        {
            HSMSItem item = null;
            while (IsThreadRun)
            {
                try
                {
                    if (!mMessageQueue.IsEmpty)
                    {
                        mMessageQueue.TryDequeue(out item);
                        ProcessMessage(item);
                    }
                }
                catch (Exception e)
                {

                }
            }
        }

        private void ProcessMessage(HSMSItem item)
        {

        }
    }
}
