using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;

namespace DynamicProxy
{
    class Program
    {
        static void Main(string[] args)
        {
            var proxyGenerator = new ProxyGenerator();
            
            var person = proxyGenerator.CreateClassProxy<PersonViewModel>(new NotifyPropertyChangedInterceptor());
            person.PropertyChanged += (s, e) => Console.WriteLine("Property Changed: " + e.PropertyName);

            person.FirstName = "Charles";
            person.LastName = "Strahan";

            Console.WriteLine("Press any key to exit . . .");
            Console.ReadKey(true);
        }
    }
}
