using System;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;

namespace RemoteProxy
{
    public class AutoNotifyContextProperty : IContextProperty, IContributeObjectSink
    {
        public string Name
        {
            get
            {
                return "AutoNotify";
            }
        }

        public bool IsNewContextOK(Context ctx)
        {
            return true;
        }

        public void Freeze(Context ctx)
        { }

        public IMessageSink GetObjectSink(MarshalByRefObject obj, IMessageSink nextSink)
        {
            var notifySink = new AutoNotifySink(obj, nextSink);
            return notifySink;
        }
    }
}


