using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemcliT_WCF.Models
{
    public class VerticalEquipo
    {
        public int idEquipo { get; set; }
        public int idTipoVertical { get; set; }
        public string tipoVertical { get; set; }
        public Int16 idProyecto { get; set; }
        public int idVertical { get; set; }
        public string ambito { get; set; }
        public string familia { get; set; }
        public Int16 idFamilia { get; set; }
        public int idClase { get; set; }
        public string clase { get; set; }
        public Int16 cantidad { get; set; }
        public string descripcion { get; set; }
        public string referencia { get; set; }
        public string serial { get; set; }
        public string nombreTipoPago { get; set; }
        public decimal valorMes { get; set; }
        public decimal valorCop { get; set; }
        public decimal valorUsd { get; set; }
        public int mesCausacion { get; set; }
        public int mesInicio { get; set; }
        public Int16 duracion { get; set; }
        public decimal totalValorCop { get; set; }
        public decimal totalPonderacionUnidad { get; set; }
        public Int16 opcionPago { get; set; }
        public Boolean esActivo { get; set; }
        //public Int16 IdEstado { get; set; }
    }

    public class VerticalActivo
    {
        public int idProyecto { get; set; }
        public int idTipoVertical { get; set; }
        public string [] activoVertical {get;set;}
    }
}