using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapa_de_clases__Marcos_
{
    public class Maestro: Docente
    {
        public string Subject { get; set; }

        public override void ShowInformation()
        {
            base.ShowInformation();
            Console.WriteLine($"Materia: {Subject}");
        }
    }
}
