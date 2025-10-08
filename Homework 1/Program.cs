/*Profe tenga en cuenta que puse las clases en español 
en vez de inglés porque directamente usted, puso ese
mapa en español en la plataforma, y por si acaso las dejé en español. 
(aunque las carpetas si las puse en inglés)*/

namespace Mapa_de_clases__Marcos_
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Mapa de clases de Marcos Ariel 2024-1785");
            Console.WriteLine();

            Maestro maestro = new Maestro
            {
                ID = "MR001",
                Name = "Marcus Rafael",
                Age = 45,
                Department = "Educación",
                Area = "Humanidades",
                Subject = "Historia"
            };

            maestro.ShowInformation();
            Console.WriteLine();

            Administrador administrador = new Administrador
            {
                ID = "CP002",
                Name = "Carla Peña",
                Age = 50,
                Department = "Gestión Académica",
                Area = "Administración",
                Level = "Directora General"
            };

            administrador.ShowInformation();
            Console.WriteLine();

            Administrativo administrativo = new Administrativo
            {
                ID = "MQ003",
                Name = "Miguel Queliz",
                Age = 38,
                Department = "Recursos Humanos",
                Position = "Jefe de Personal"
            };

            administrativo.ShowInformation();
            Console.WriteLine();

            Estudiante estudiante = new Estudiante
            {
                ID = "ME004",
                Name = "Marcos Encarnación",
                Age = 21,
                Career = "Ingeniería"
            };

            estudiante.ShowInformation();
            Console.WriteLine();

            ExAlumno exAlumno = new ExAlumno
            {
                ID = "HF005",
                Name = "Heischly Figuereo",
                Age = 26,
                GraduationYear = 2019
            };

            exAlumno.ShowInformation();
            Console.WriteLine();
        }
    }
}
