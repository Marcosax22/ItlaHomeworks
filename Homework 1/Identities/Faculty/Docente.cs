using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapa_de_clases__Marcos_
{
    public class Docente : Empleado
    {
        public string Area { get; set; }

        public override void ShowInformation()
        {
            base.ShowInformation();
            Console.WriteLine($"Área: {Area}");
        }

    }
}
