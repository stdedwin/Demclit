using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemcliT_WCF.Models
{
    public class Vertical
    {
        public int idVertical { get; set; }
        public Int16 idTipoVertical { get; set; }
        public String tipoVertical { get; set; }
        public int idProyecto { get; set; }
        public decimal totalPonderacionVertical { get; set; }
    }
}