using System;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Contexts;

namespace RemoteProxy
{
    [AttributeUsage(AttributeTargets.Class)]
    public class INPCAttribute : ContextAttribute
    {
        public INPCAttribute()
            : base("INPCAttribute")
        {
        }

        public override void GetPropertiesForNewContext(IConstructionCallMessage ctor)
        {
            IContextProperty notifyProperty = new AutoNotifyContextProperty();
            ctor.ContextProperties.Add(notifyProperty);
        }
    }
}