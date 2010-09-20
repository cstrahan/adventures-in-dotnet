using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteProxy
{
    class Program
    {
        static void Main(string[] args)
        {
            var person = new PersonViewModel();

            person.PropertyChanged += (s, e) => Console.WriteLine("Property Changed: " + e.PropertyName);

            person.FirstName = "Charles";
            person.LastName = "Strahan";

            Console.WriteLine("Press any key to exit . . .");
            Console.ReadKey(true);
        }
    }
}
