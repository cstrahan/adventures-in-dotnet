using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using INPCAttribute;

namespace RemoteProxy
{
    public class AutoNotifySink : IMessageSink
    {
        private readonly MarshalByRefObject _target;

        public AutoNotifySink(MarshalByRefObject target, IMessageSink nextSink)
        {
            _target = target;
            NextSink = nextSink;
        }

        public IMessageSink NextSink { get; private set; }

        public IMessage SyncProcessMessage(IMessage msg)
        {
            var returnedMessage = (IMethodReturnMessage)NextSink.SyncProcessMessage(msg);
            TryRaisePropertyChanged((IMethodMessage)msg);
            return returnedMessage;
        }

        public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
        {

            throw new InvalidOperationException();
        }

        private void TryRaisePropertyChanged(IMethodMessage methodMessage)
        {
            if (methodMessage.MethodName.StartsWith("set_"))
            {
                var propertyName = methodMessage.MethodName.Substring(4);
                var type = Type.GetType(methodMessage.TypeName);
                var pi = type.GetProperty(propertyName);

                // check that we have the attribute defined
                if (Attribute.GetCustomAttribute(pi, typeof(NotifyAttribute)) != null)
                {
                    // get the field storing the delegate list that are stored by the event.
                    FieldInfo info = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                        .Where(f => f.FieldType == typeof(PropertyChangedEventHandler))
                        .FirstOrDefault();

                    if (info != null)
                    {
                        // get the value of the field
                        var handler = info.GetValue(_target) as PropertyChangedEventHandler;

                        // invoke the delegate if it's not null (aka empty)
                        if (handler != null)
                            handler.Invoke(_target, new PropertyChangedEventArgs(propertyName));
                    }
                }
            }
        }
    }
}
