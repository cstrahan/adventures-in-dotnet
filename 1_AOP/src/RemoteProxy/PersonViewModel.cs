using System;
using System.ComponentModel;
using INPCAttribute;

namespace RemoteProxy
{
    [INPC]
    public class PersonViewModel : ContextBoundObject, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        [Notify]
        public string FirstName { get; set; }
        [Notify]
        public string LastName { get; set; }
    }
}