using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemcliT_WCF.Models
{
    public class AppResponse
    {
        public bool State { get; set; }
        public string Exception { get; set; }
        public string Msg { get; set; }
        public object Data { get; set; }
    }
}