using Demclit_Pagos_Reporte.App_Code;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demclit_Pagos_Reporte
{
    class Program
    {
        static void Main(string[] args)
        {

            int intHoraInicialCE = int.Parse(ConfigurationManager.AppSettings["horaInicialCE"]);
            int intHoraFinalCE = int.Parse(ConfigurationManager.AppSettings["horaFinalCE"]);

            int intHoraInicialRE = int.Parse(ConfigurationManager.AppSettings["horaInicialRE"]);
            int intHoraFinalRE = int.Parse(ConfigurationManager.AppSettings["horaFinalRE"]);

            int horaActual = int.Parse(DateTime.Now.ToString("HHmm"));

            bool rutaCE = (horaActual >= intHoraInicialCE && horaActual < intHoraFinalCE);
            bool rutaRE = (horaActual >= intHoraInicialRE && horaActual < intHoraFinalRE);

            Console.WriteLine(horaActual);
            Console.WriteLine("Inicio creacion reporte CE " + intHoraInicialCE.ToString() + " - " + intHoraFinalCE.ToString() + ",RE " + intHoraInicialRE.ToString() + " - " + intHoraFinalRE.ToString() );
            
            try
            {
                if (rutaCE || rutaRE)
                {
                    Conect conect = new Conect();
                    conect.CommandQuery = "Demclit_SP_Pagos_Reporte_Diario";
                    conect.AddParameters("nombre_bd", "NABISD");
                    conect.AddParameters("ruta", rutaCE ? 1 : 0);
                    conect.ExecTransac();
                    //Console.WriteLine(conect.numRows > 0);
                    Console.WriteLine("Archivo creado con exito");
                }
                else
                {
                    Console.WriteLine("no es tiempo para crear el archivo");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Final creacion reporte");

        }
    }
}
