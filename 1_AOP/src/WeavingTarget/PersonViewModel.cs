using System.ComponentModel;
using INPCAttribute;

namespace WeavingTarget
{
    public class PersonViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        [Notify]
        public string FirstName { get; set; }
        [Notify]
        public string LastName { get; set; }

        
        public string Foo { get; set; }

        private string _bar;
        public string Bar
        {
            get { return _bar; }
            set
            {
                _bar = value;

                var propertyChanged = PropertyChanged;
                if (propertyChanged != null)
                {
                    propertyChanged(this, new PropertyChangedEventArgs("Bar"));
                }
            }
        }
    }
}