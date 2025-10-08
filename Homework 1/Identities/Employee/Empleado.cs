using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapa_de_clases__Marcos_
{
    public class Empleado: MiembroDeLaComunidad
    {
        public string Department { get; set; }

        public override void ShowInformation()
        {
            base.ShowInformation();
            Console.WriteLine($"Departamento: {Department}");
        }


    }
}
