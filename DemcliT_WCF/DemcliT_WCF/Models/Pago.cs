using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemcliT_WCF.Models
{
    public class Pago
    {
        public long IdPago { get; set; }
        public string CodRespuesta { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UserLogin { get; set; }
        public string IdUserRegistro { get; set; }
        public int IdCanal { get; set; }
        public string Canal { get; set; }
        public int CodVendedor { get; set; }
        public string CodOficina { get; set; }
        public string Oficina { get; set; }
        public int CodCliente { get; set; }
        public string Monto { get; set; }
        public int IdFranquicia { get; set; }
        public string Franquicia { get; set; }
        public string NumAutorizacion { get; set; }
        public string NumTarjeta { get; set; }
        public string EMail { get; set; }
    }
}