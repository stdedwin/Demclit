using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemcliT_WCF.Models
{
    public class BodegaActivos
    {
        public long idActivo { get; set; }
        public string ambito { get; set; }
        public Int16 idFamilia { get; set; }
        public string familia { get; set; }
        public int idClase { get; set; }
        public string clase { get; set; }
        public string descripcion { get; set; }
        public string referencia { get; set; }
        public string serial { get; set; }
        public decimal costoCompra { get; set; }
        public decimal costoFinanciero { get; set; }
        public DateTime fechaCompra { get; set; }
        public DateTime fechaInicioOperacion { get; set; }
        public Int16 mesesOperacion { get; set; }
        public DateTime fechaFinOperacion { get; set; }
        public Int16 vidaUtilExcendente { get; set; }
        public string idUserRegistro { get; set; }
        public Int16 idEstado { get; set; }
    }
}