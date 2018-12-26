using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemcliT_WCF.Models
{
    public class Proyecto
    {
        public int idProyecto { get; set; }
        public string idOportunidad { get; set; }
        public string nombreProyecto { get; set; }
        public Int16 duracion { get; set; }
        public DateTime fechaRegistro { get; set; }
        public string userLogin { get; set; }
        public string numIdentidadCliente { get; set; }
        public string nombreCliente { get; set; }
        public int idGrupo { get; set; }
        //public Int16 IdEstado { get; set; }
    }
}