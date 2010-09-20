using System.ComponentModel;
using INPCAttribute;

namespace DynamicProxy
{
    public class PersonViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        [Notify]
        public virtual string FirstName { get; set; }
        [Notify]
        public virtual string LastName { get; set; }
    }
}