using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapa_de_clases__Marcos_
{
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public virtual void ShowInformation()
        {
            Console.WriteLine($"Nombre: {Name}, Edad: {Age}");
        }
    }
}
