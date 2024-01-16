using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SECSControl.Common
{
    internal class CustomThread
    {
        private Thread mThreadField;

        internal CustomThread()
        {
            this.mThreadField = new Thread(new ThreadStart(this.Run));
            this.mThreadField.IsBackground = true;
        }

        internal CustomThread(ThreadStart p1, object[] obj)
        {
            this.mThreadField = new Thread(p1);
            this.mThreadField.IsBackground = true;
        }

        internal virtual void Run()
        {
        }

        internal virtual void Start() => this.mThreadField.Start();

        internal string Name
        {
            get => this.mThreadField.Name;
            set
            {
                if (this.mThreadField.Name != null)
                    return;
                this.mThreadField.Name = value;
            }
        }

        internal ThreadPriority Priority
        {
            get => this.mThreadField.Priority;
            set => this.mThreadField.Priority = value;
        }

        internal bool IsAlive => this.mThreadField.IsAlive;

        internal bool IsBackground
        {
            get => this.mThreadField.IsBackground;
            set => this.mThreadField.IsBackground = value;
        }

        internal void Join() => this.mThreadField.Join();

        internal void Join(long p1)
        {
            lock (this)
                this.mThreadField.Join(new TimeSpan(p1 * 1000L));
        }

        internal void Join(long p1, int p2)
        {
            lock (this)
                this.mThreadField.Join(new TimeSpan(p1 * 1000L + (long)(p2 * 100)));
        }

        internal void Abort() => this.mThreadField.Abort();

        internal void Abort(object stateInfo)
        {
            lock (this)
                this.mThreadField.Abort(stateInfo);
        }
    }
}
