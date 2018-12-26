using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using DemcliT_WCF.Models;
using DemcliT_WCF.App_Code;
using System.Data;
using System.Web;
using System.Text.RegularExpressions;
using System.Globalization;
using System.IO;
using DemcliT_WCF.App_Data;
using DemcliT_WCF.WsPagosOnline;
using System.Net;
using System.Web.Script.Serialization;
using System.Configuration;

namespace DemcliT_WCF
{
    [ServiceContract(Namespace = "DemcliT_WCF")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class API
    {
        private static bool modeDeveloper = ConfigurationManager.AppSettings["ModeDeveloper"] == "true";
        private static bool modeTest = ConfigurationManager.AppSettings["ModeTest"] == "true";

        #region constantesPagos
        private string WSPAGO_COD_CONVENIO = (modeDeveloper || modeTest) ? "ASES_SHOWROOM" : "ASES_SHOWROOM";
        private string WSPAGO_COD_ENT_AUTORIZADORA = (modeDeveloper || modeTest) ? "000253" : "000244";
        private string WSPAGO_COD_ENT_ADQUIRIENTE= (modeDeveloper || modeTest) ? "000253" : "000244";
        private long WSPAGO_NUM_CONTROL_CONSULTA = 0;//es un consecutivo de la aplicación
        private long WSPAGO_NUM_CONTROL_PAGO = 0; //se obtiene del  método aplicar pago
        private string WSPAGO_COD_OFICINA = "";
        private string WSPAGO_NUM_TERMINAL = "0";
        private string WSPAGO_DISPOSITIVO_ORIGEN = "O"; //O -> corresponde a oficina
        #endregion
        #region constantesProyectos
        private enum EstadosProyecto
        {
            NUEVO_OFERTADO = 1,
            NUEVO_VENDIDO = 2
            
        }

        private enum EstadosEquipo
        {
            DISPONIBLE = 3,
            OPERTATIVO = 4,
            PRE_RESERVADO = 5,
            RESERVADO = 6
        }


        #endregion



        /// <summary>
        /// Método que retorna una lista generica de clave/valor
        /// </summary>
        /// <param name="Pago">Es el objeto json que representa el pago aplicado</param>
        /// <returns></returns>
        [WebGet(UriTemplate = "/ListaGenerica/{TipoLista}/{Filtros=null}", ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        public List<ListaGenerica> ListaGenerica(string TipoLista, string Filtros = null)
        {
            List<ListaGenerica> lista = new List<ListaGenerica>();
            Conect conect = new Conect();
            conect.CommandQuery = "Demclit_SP_Lista_Generica";
            conect.AddParameters("tipo_lista", TipoLista);
            conect.AddParameters("filtros", Filtros);
            DataTable data = conect.GetDataTable();

            foreach (DataRow row in data.Rows)
            {
                var cantidadColumnas = row.Table.Columns.Count;
                lista.Add(new ListaGenerica
                {
                    Id = row[0].ToString(),
                    Valor = row[cantidadColumnas > 1 ? 1 : 0].ToString(),
                    Grupo = cantidadColumnas > 2 ? row[2].ToString() : null,
                });
            }

            return lista;
        }

        /*
        #region Pagos
       
        /// <summary>
        /// Método que permite registrar un pago
        /// </summary>
        /// <param name="Pago">Es el objeto json que representa el pago aplicado</param>
        /// <returns></returns>
        [WebGet(UriTemplate = "/VendedorInfo/{userLogin}", ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        public VendedorInfo VendedorInfo(string userLogin)
        {
            VendedorInfo vendedor = null;
            Conect conect = new Conect();
            conect.CommandQuery = "Demclit_SP_Pagos_AutorizarVendedor";
            conect.AddParameters("user_login", userLogin);
            DataTable data = conect.GetDataTable();

            //Registra en BD
            if (data.Rows.Count > 0)
            {
                vendedor = new VendedorInfo();
                vendedor.IdUser = data.Rows[0]["ID_USER"].ToString();
                vendedor.IdGrupo = (int)data.Rows[0]["ID_GRUPO"];
                vendedor.IdCanal = (int)data.Rows[0]["ID_CANAL"];
                vendedor.CodVendedor = (int)data.Rows[0]["COD_VENDEDOR"];
                vendedor.CodOficina = data.Rows[0]["COD_PUNTO"].ToString();
            }

            return vendedor;
        }

        /// <summary>
        /// Método que permite consultar el saldo de una cuenta móvil
        /// </summary>
        /// <param name="CodCliente">Es el identificador de la cuenta móvil en SCL</param>
        /// <returns></returns>
        [WebGet(UriTemplate = "/ConsultaSaldo/{CodCliente}", ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        public ResponseConsSaldoNombre ConsultaSaldo(string CodCliente) {
            var user = ConnectionSources.WsPagosUsuario;
            var pass = ConnectionSources.WsPagosClave;
            WSPagosOnlineServiceSoapClient wsPago = new WSPagosOnlineServiceSoapClient();
            RequestConsulta reqConsulta = new RequestConsulta();
            reqConsulta.CodConvenio = WSPAGO_COD_CONVENIO;
            reqConsulta.CodEntAdquiriente = WSPAGO_COD_ENT_ADQUIRIENTE;
            reqConsulta.NumControl = WSPAGO_NUM_CONTROL_CONSULTA;
            reqConsulta.Fecha = DateTime.Now.ToUniversalTime();
            reqConsulta.NumDocumento = long.Parse(CodCliente);
            reqConsulta.CodOficina = WSPAGO_COD_OFICINA;
            reqConsulta.NumTerminal = WSPAGO_NUM_TERMINAL;
            reqConsulta.DispositivoOrigen = WSPAGO_DISPOSITIVO_ORIGEN;
            ResponseConsSaldoNombre resp = wsPago.ConsultarSaldoNombre(user, pass, reqConsulta);
            return resp;
        }

        /// <summary>
        /// Método que permite registrar un pago
        /// </summary>
        /// <param name="Pago">Es el objeto json con la estructura de la clase Pago</param>
        /// <returns></returns>
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        [OperationContract]
        public AppResponse PagoRegistro(Pago PagoRegistro)
        {
            bool esVendedor = false;
            bool esExitoso = false;
            bool pagoExitoso = false;
            string codRespuesta = "-1";
            long idPago = 0;
            AppResponse resp = new AppResponse();
            VendedorInfo vendedorInfo = this.VendedorInfo(PagoRegistro.UserLogin);
            DateTime fechaPago = DateTime.Now;
            Conect conect;
            esVendedor = (vendedorInfo != null);
            resp.State = esVendedor;
            resp.Msg = !esVendedor ? "Error: Usuario no autorizado. por favor renueve su pase de autenticación y/o solicite la activación de su usuario" : resp.Msg;

            if (esVendedor) {
                conect = new Conect();
                conect.CommandQuery = "Demclit_SP_Pago_Registro";
                conect.AddParameters("userLogin", PagoRegistro.UserLogin);
                conect.AddParameters("cod_respuesta", codRespuesta);
                conect.AddParameters("id_canal", vendedorInfo.IdCanal);
                conect.AddParameters("cod_vendedor", vendedorInfo.CodVendedor);
                conect.AddParameters("cod_oficina", vendedorInfo.CodOficina);
                conect.AddParameters("cod_cliente", PagoRegistro.CodCliente);
                conect.AddParameters("monto", PagoRegistro.Monto);
                conect.AddParameters("id_franquicia", PagoRegistro.IdFranquicia);
                conect.AddParameters("num_autorizacion", PagoRegistro.NumAutorizacion);
                conect.AddParameters("num_tarjeta", PagoRegistro.NumTarjeta);
                conect.AddParameters("e_mail", PagoRegistro.EMail);
                conect.AddParametersOutPut("id_pago", 0, SqlDbType.BigInt);
                conect.AddParametersOutPut("pago_existente", 0, SqlDbType.Bit);
                conect.ExecTransac();
                esExitoso = conect.numRows > 0;
                var pago_existente = (bool)conect.GetValueParameterOut("pago_existente");
                resp.Msg = pago_existente ? "Error: ya fue registrado un pago con el número de autorización y número de tarjeta suministrados. Por favor verifique la información e intente nuevamente." : resp.Msg;
                if (esExitoso && !pago_existente)
                {
                    //se obtiene el id del pago registrado anteriormemnte en Demclit
                    idPago = (long)conect.GetValueParameterOut("id_pago");
                    resp.Data = idPago.ToString();
                    try {
                        //Consumo WS de TI
                        WSPagosOnlineServiceSoapClient wsPagos = new WSPagosOnlineServiceSoapClient();
                        wsPagos.InnerChannel.OperationTimeout = new TimeSpan(0,2,0);
                        var user = (modeDeveloper || modeTest) ? ConnectionSources.WsPagosUsuarioTest : ConnectionSources.WsPagosUsuario;
                        var pass = (modeDeveloper || modeTest) ? ConnectionSources.WsPagosClaveTest : ConnectionSources.WsPagosClave;
                        RequestPagoReintento pagoRequest = new RequestPagoReintento();
                        pagoRequest.CodConvenio = WSPAGO_COD_CONVENIO;
                        pagoRequest.CodEntAutorizadora = WSPAGO_COD_ENT_AUTORIZADORA;
                        pagoRequest.CodEntAdquiriente = WSPAGO_COD_ENT_ADQUIRIENTE;
                        pagoRequest.NumControl = WSPAGO_NUM_CONTROL_CONSULTA;
                        pagoRequest.Fecha = fechaPago;
                        pagoRequest.NumDocumento = PagoRegistro.CodCliente;
                        pagoRequest.ValorTransaccion = double.Parse(PagoRegistro.Monto);
                        pagoRequest.ValorEfectivo = double.Parse(PagoRegistro.Monto);
                        pagoRequest.ValorCheque = 0;
                        pagoRequest.Jornada = "N";
                        pagoRequest.NumControl = idPago;
                        pagoRequest.NumAutorizacion = 0;
                        pagoRequest.CodOficina = vendedorInfo.CodOficina;
                        pagoRequest.NumTerminal = WSPAGO_NUM_TERMINAL;
                        pagoRequest.DispositivoOrigen = WSPAGO_DISPOSITIVO_ORIGEN;
                        ResponseConsPagoReint wsPagosResp = wsPagos.AplicarPago(user, pass, pagoRequest);
                        codRespuesta = wsPagosResp.CodRespuesta;
                        //Se actualiza respuesta
                        pagoExitoso = codRespuesta.Equals("00");
                        resp.Msg = pagoExitoso ? "Pago aplicado con Éxito" : codRespuesta.Equals("123") ? "El código de cliente no existe. Por favor verifique la información e intenténtelo de nuevo.": "Error: Se ha presentado una intermitencia en el servicio y no pudo ser aplicado el pago. Por favor intente nuevamente.";

                        conect = new Conect();
                        conect.CommandQuery = "Demclit_SP_Pago_Registro_CodRespuesta";
                        conect.AddParameters("id_pago", idPago);
                        conect.AddParameters("cod_respuesta", codRespuesta);
                        conect.ExecTransac();
                    }
                    catch (Exception ex)
                    {
                        pagoExitoso = false;
                    }                  
                }
            }

            resp.State = esVendedor && esExitoso && pagoExitoso;
            return resp;
        }

        /// <summary>
        /// Método que permite registrar un pago
        /// </summary>
        /// <param name="IdPago">Identifiador interno del pago a consultar</param>
        /// <returns></returns>
        [WebGet(UriTemplate = "/Pago/{IdPago}", ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        public Pago PagoInfo(string IdPago)
        {
            Pago pagoInfo = null;
            Conect conect = new Conect();
            conect.CommandQuery = "Demclit_SP_Pago_Info";
            conect.AddParameters("id_pago", IdPago);
            DataTable data = conect.GetDataTable();

            if (data.Rows.Count > 0) {
                pagoInfo = new Pago();
                pagoInfo.IdPago = (long)data.Rows[0]["ID_PAGO"];
                pagoInfo.CodRespuesta = data.Rows[0]["COD_RESPUESTA"].ToString();
                pagoInfo.FechaRegistro = (DateTime)data.Rows[0]["FECHA_REGISTRO"];
                pagoInfo.UserLogin = data.Rows[0]["USER_LOGIN"].ToString();
                pagoInfo.IdUserRegistro = data.Rows[0]["ID_USER_REGISTRO"].ToString();
                pagoInfo.IdCanal = (int)data.Rows[0]["ID_CANAL"];
                pagoInfo.Canal = data.Rows[0]["GRUPO"].ToString();
                pagoInfo.CodVendedor = (int)data.Rows[0]["COD_VENDEDOR"];
                pagoInfo.CodOficina = data.Rows[0]["COD_OFICINA"].ToString();
                pagoInfo.Oficina = data.Rows[0]["OFICINA"].ToString();
                pagoInfo.CodCliente = (int)data.Rows[0]["COD_CLIENTE"];
                pagoInfo.Monto = data.Rows[0]["MONTO"].ToString();
                pagoInfo.IdFranquicia = (int)data.Rows[0]["ID_FRANQUICIA"];
                pagoInfo.Franquicia = data.Rows[0]["FRANQUICIA"].ToString();
                pagoInfo.NumAutorizacion = data.Rows[0]["NUM_AUTORIZACION"].ToString();
                pagoInfo.NumTarjeta = data.Rows[0]["NUM_TARJETA"].ToString();
                pagoInfo.EMail = data.Rows[0]["E_MAIL"].ToString();
            }

            return pagoInfo;
        }

        /// <summary>
        /// Método que genera el comporbante electrónico del pago asoicado al IdPago especificado
        /// </summary>
        /// <param name="IdPago">Id interno del pago</param>
        /// <returns></returns>
        [WebGet(UriTemplate = "/GenerarEComprobante/{IdPago}", ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        public AppResponse GenerarEComprobante(string IdPago)
        {
            AppResponse resp = new AppResponse();
            Pago pagoInfo = this.PagoInfo(IdPago);
            string pattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
            Regex mailRegex = new Regex(pattern);
            resp.State = true;
            resp.Msg= "No se encontraron pagos asociados al Id:" + IdPago;
            if (pagoInfo != null)
            {
                resp.State = true;
                string path = AppDomain.CurrentDomain.BaseDirectory;
                string mailBody = String.Empty;
                using (StreamReader reader = new StreamReader(path + "Resources/Mail_EComprobante.html"))
                {
                    mailBody = reader.ReadToEnd();
                }
                CultureInfo culture = new CultureInfo("es-CO");
                DateTimeFormatInfo formatoFecha = culture.DateTimeFormat;
                string dia = DateTime.Now.Day.ToString();
                string mes = DateTime.Now.Month.ToString();
                string año = DateTime.Now.Year.ToString();
                string nombreMes = formatoFecha.GetMonthName(Convert.ToInt32(mes)).ToString();
                double montoValue = double.Parse(pagoInfo.Monto);
                mailBody = mailBody.Replace("[Fecha]", pagoInfo.FechaRegistro.ToString("dd/MM/yyyy HH:mm:ss"));
                mailBody = mailBody.Replace("[NumComprobante]", pagoInfo.IdPago.ToString());
                mailBody = mailBody.Replace("[Oficina]", pagoInfo.Oficina);
                mailBody = mailBody.Replace("[Cajero]", pagoInfo.IdUserRegistro);
                mailBody = mailBody.Replace("[Identidad]", pagoInfo.CodCliente.ToString());
                mailBody = mailBody.Replace("[Franquicia]", pagoInfo.Franquicia);
                mailBody = mailBody.Replace("[NumAutorizacion]", pagoInfo.NumAutorizacion);
                mailBody = mailBody.Replace("[Monto]", montoValue.ToString("c", culture));
                mailBody = mailBody.Replace("[copyMovistarYear]", año);
                resp.Data = mailBody;
                resp.Msg = pagoInfo.EMail;
            }
            return resp;
        }

        /// <summary>
        /// Método que realiza el envío del comprobante electrónico (EComprobante) de pago
        /// </summary>
        /// <param name="IdPago">Id interno del pago a enviar</param>
        /// <returns></returns>
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        [OperationContract]
        public AppResponse EnviarEComprobante(string IdPago)
        {
            AppResponse resp = new AppResponse();
            AppResponse eComprobante = eComprobante = GenerarEComprobante(IdPago);
            resp.State = eComprobante.State;
            resp.Msg = eComprobante.Msg;
            if (eComprobante.State) {
                string mailBody = eComprobante.Data.ToString();
                this.Email_Send(resp.Msg, "Comprobante de Pago", eComprobante.Data.ToString(), null, null, null, 1, "soporte.pagos@telefonica.com.co");
                resp.State = true;
                resp.Msg = string.Format("Mensaje enviado con éxito al correo: {0}", resp.Msg);
            }
            return resp;
        }

    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Destinatarios"></param>
        /// <param name="Asunto"></param>
        /// <param name="Body"></param>
        /// <param name="CC"></param>
        /// <param name="CCO"></param>
        /// <param name="PathArchivo"></param>
        /// <param name="Importancia"></param>
        /// <param name="NombreCuentaCorreoOrigen"></param>
        /// <returns></returns>
        public bool Email_Send(string Destinatarios, string Asunto, string Body, string CC = null, string CCO = null, string PathArchivo = null, int Importancia = 1, string NombreCuentaCorreoOrigen = null)
        {
            string pattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
            Regex mailRegex = new Regex(pattern);
            NombreCuentaCorreoOrigen = NombreCuentaCorreoOrigen ?? "Nabis.VentasDigitales@telefonica.com";

            try
            {
                List<string> CorreosDestino = Destinatarios.Split(new char[] { ';' }).ToList();
                List<string> CorreosCC = new List<string>();
                List<string> CorreosCCO = new List<string>();
                CorreosDestino = CorreosDestino.Where(x => mailRegex.IsMatch(x)).ToList();

                if (CC != null)
                {
                    CorreosCC = CC.Split(new char[] { ';' }).ToList();
                    CorreosCC = CorreosCC.Where(x => mailRegex.IsMatch(x)).ToList();
                }

                if (CCO != null)
                {
                    CorreosCCO = CCO.Split(new char[] { ';' }).ToList();
                    CorreosCCO = CorreosCCO.Where(x => mailRegex.IsMatch(x)).ToList();
                }

                if (CorreosDestino.Count > 0)
                {
                    //Conect mailSend = new Conect(SourcesConection.Nabis_2012, true);
                    Conect mailSend = new Conect(ConnectionSources.Nabis, true);
                    mailSend.CommandQuery = "Nab_Mail";
                    mailSend.AddParameters("From", NombreCuentaCorreoOrigen);
                    mailSend.AddParameters("To", string.Join(";", CorreosDestino));
                    mailSend.AddParameters("Subject", Asunto);
                    mailSend.AddParameters("Body", Body);
                    mailSend.AddParameters("CC", (CorreosCC.Count > 0) ? string.Join(";", CorreosCC) : null);
                    mailSend.AddParameters("CCO", (CorreosCCO.Count > 0) ? string.Join(";", CorreosCCO) : null);
                    mailSend.AddParameters("Attachment", PathArchivo);
                    mailSend.AddParameters("Importance", Importancia);
                    mailSend.ExecTransac();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool GenerarReporteContable()
        {
            int i = 0;
            Conect conect = new Conect();
            conect.CommandQuery = "Demclit_SP_Pago_Info";
            //conect.AddParameters("id_pago", IdPago);
            var data = conect.GetDataTable();
            //Ciclo 1 con código 40
            StringBuilder csv = new StringBuilder();
            foreach (DataRow row in data.Rows)
            {
                row[9] = new String(' ', (16 - row[9].ToString().Length)) + row[9].ToString();

                csv.AppendLine(row[0].ToString());
                i++;
            }
            File.AppendAllText(String.Format("{0}.txt", @"\\koral01\et\Documentacion_Procesos\JCC\Docs\FI_NHS_SAP_INGL04_3000_"), csv.ToString(), UTF8Encoding.UTF8);
            return true;
        }
#endregion Pagos

        */

        #region Proyectos
        /// <summary>
        /// Método que permite registrar un pago
        /// </summary>
        /// <param name="Proyecto">Es el objeto json con la estructura de la clase Proyecto</param>
        /// <returns></returns>
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        [OperationContract]
        public AppResponse ProyectoRegistro(Proyecto ProyectoRegistro)
        {
            AppResponse resp = new AppResponse();
            Conect conect;
            int idProyecto = 0;

            try
            {
                idProyecto = ProyectoRegistro.idProyecto;
                conect = new Conect();
                conect.CommandQuery = "Demclit_SP_Proyecto_Registro";
                conect.AddParameters("id_oportunidad", ProyectoRegistro.idOportunidad);
                conect.AddParameters("nombre_proyecto", ProyectoRegistro.nombreProyecto);
                conect.AddParameters("duracion", ProyectoRegistro.duracion);
                conect.AddParameters("num_identidad_cliente", ProyectoRegistro.numIdentidadCliente);
                conect.AddParameters("nombre_cliente", ProyectoRegistro.nombreCliente);
                conect.AddParameters("user_login", ProyectoRegistro.userLogin);
                conect.AddParameters("id_grupo", ProyectoRegistro.idGrupo);
                conect.AddParameters("id_estado", EstadosProyecto.NUEVO_OFERTADO);
                conect.AddParametersInputOutPut("id_proyecto", ProyectoRegistro.idProyecto, SqlDbType.Int);
                conect.ExecTransac();


                resp.State = conect.GetValueParameterOut("id_proyecto").ToString() != "0"; //true;
                resp.Data = conect.GetValueParameterOut("id_proyecto").ToString();
                resp.Msg = idProyecto > 0  ? "Proyecto actualizado con exito" : "Proyecto creado con exito";
            }
            catch (Exception ex)
            {

                resp.State = false;
                resp.Msg = "No se pudo crear el proyecto";
            }

            return resp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="VerticalRegistro"></param>
        /// <returns></returns>
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        [OperationContract]
        public AppResponse VerticalRegistro(Vertical VerticalRegistro)
        {
            AppResponse resp = new AppResponse();
            Conect conect;
            try
            {
                conect = new Conect();
                conect.CommandQuery = "Demclit_SP_Proyecto_Registro";
                conect.AddParameters("id_tipo_vertical", VerticalRegistro.idTipoVertical);
                conect.AddParameters("id_proyecto", VerticalRegistro.idProyecto);
                conect.AddParametersOutPut("id_vertical", 0, SqlDbType.Int);
                conect.ExecTransac();

                resp.State = true;
                resp.Data = conect.GetValueParameterOut("id_vertical").ToString();
                resp.Msg = "Vertical creada con exito";
            }
            catch (Exception ex)
            {
                resp.State = false;
                resp.Msg = "No se pudo crear la vertical";
            }

            return resp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="verticalEquipo"></param>
        /// <returns></returns>
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        [OperationContract]
        public AppResponse VerticalEquipoRegistro(VerticalEquipo verticalEquipo)
        {
            AppResponse resp = new AppResponse();
            Conect conect;
            

            try
            {
                conect = new Conect();
                conect.CommandQuery = "Demclit_SP_Proyecto_Verticales_Equipos_Registro";

                conect.AddParameters("id_proyecto", verticalEquipo.idProyecto);
                conect.AddParameters("id_tipo_vertical", verticalEquipo.idTipoVertical);
                conect.AddParameters("id_equipo", verticalEquipo.idEquipo);
                conect.AddParameters("ambito", verticalEquipo.ambito);
                conect.AddParameters("id_familia", verticalEquipo.idFamilia);
                conect.AddParameters("id_clase", verticalEquipo.idClase);
                conect.AddParameters("cantidad", verticalEquipo.cantidad);
                conect.AddParameters("descripcion", verticalEquipo.descripcion);
                conect.AddParameters("valor_mes", verticalEquipo.valorMes);
                conect.AddParameters("opcion_pago", verticalEquipo.opcionPago);
                conect.AddParameters("valor_cop", verticalEquipo.valorCop);
                conect.AddParameters("valor_usd", verticalEquipo.valorUsd);
                conect.AddParameters("mes_causacion", verticalEquipo.mesCausacion);
                conect.AddParameters("mes_inicio", verticalEquipo.mesInicio);
                conect.AddParameters("duracion", verticalEquipo.duracion);
                conect.AddParameters("id_estado",  EstadosEquipo.PRE_RESERVADO);
                conect.ExecTransac();

                resp.State = true;
                resp.Data = ""; //conect.GetValueParameterOut("id_vertical").ToString();

                resp.Msg = verticalEquipo.idEquipo > 0 ? "Vertical actualizada con exito" : "Vertical creada con exito";

            }
            catch (Exception ex)
            {
                resp.State = false;
                resp.Msg = "No se pudo crear la vertical";
            }

            return resp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IdProyecto"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "/VerticalesConsulta/{IdProyecto}", ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        public List<Vertical> VerticalesConsulta(string IdProyecto)
        {
            List<Vertical> lista = new List<Vertical>();
            Conect conect = new Conect();
            conect.CommandQuery = "Demclit_SP_Proyecto_Verticales_Consulta";
            conect.AddParameters("id_Proyecto", IdProyecto);

            DataTable data = conect.GetDataTable();

            foreach (DataRow row in data.Rows)
            {
                var cantidadColumnas = row.Table.Columns.Count;
                lista.Add(new Vertical
                {
                    idVertical = int.Parse(row["ID_VERTICAL"].ToString()),
                    idTipoVertical = short.Parse(row["ID_TIPO_VERTICAL"].ToString()),
                    tipoVertical = row["TIPO_VERTICAL"].ToString(),
                    idProyecto = int.Parse(row["ID_PROYECTO"].ToString()),
                    totalPonderacionVertical = decimal.Parse(row["TOTAL_PONDERACION_VERTICAL"].ToString())
                });
            }

            return lista;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IdProyecto"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "/ProyectoConsulta/{IdProyecto}", ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        public List<Proyecto> ProyectoConsulta(string IdProyecto)
        {
            List<Proyecto> lista = new List<Proyecto>();
            Conect conect = new Conect();
            conect.CommandQuery = "Demclit_SP_Proyecto_Consulta";
            conect.AddParameters("id_Proyecto", IdProyecto);

            DataTable data = conect.GetDataTable();

            foreach (DataRow row in data.Rows)
            {
                var cantidadColumnas = row.Table.Columns.Count;
                lista.Add(new Proyecto
                {
                    idProyecto = int.Parse(row["ID_PROYECTO"].ToString()),
                    idOportunidad = row["ID_OPORTUNIDAD"].ToString(),
                    nombreProyecto = row["NOMBRE_PROYECTO"].ToString(),
                    duracion = short.Parse(row["DURACION"].ToString()),
                    numIdentidadCliente = row["NUM_IDENTIDAD_CLIENTE"].ToString(),
                    nombreCliente = row["NOMBRE_CLIENTE"].ToString(),
                    idGrupo = short.Parse(row["ID_GRUPO"].ToString())
                });
            }

            return lista;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserLogin"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "/ProyectoConsultaRoles/{UserLogin}", ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        public List<Proyecto> ProyectoConsultaRoles(string UserLogin)
        {
            List<Proyecto> lista = new List<Proyecto>();
            Conect conect = new Conect();
            conect.CommandQuery = "Demclit_SP_Proyecto_Roles";
            conect.AddParameters("user_login", UserLogin);

            DataTable data = conect.GetDataTable();

            foreach (DataRow row in data.Rows)
            {
                var cantidadColumnas = row.Table.Columns.Count;
                lista.Add(new Proyecto
                {
                    idProyecto = int.Parse(row["ID_PROYECTO"].ToString()),
                    idOportunidad = row["ID_OPORTUNIDAD"].ToString(),
                    nombreProyecto = row["NOMBRE_PROYECTO"].ToString(),
                    duracion = short.Parse(row["DURACION"].ToString()),
                    fechaRegistro = DateTime.Parse(row["FECHA_REGISTRO"].ToString()),
                    numIdentidadCliente = row["NUM_IDENTIDAD_CLIENTE"].ToString(),
                    nombreCliente = row["NOMBRE_CLIENTE"].ToString(),
                    idGrupo = short.Parse(row["ID_GRUPO"].ToString())
                });
            }

            return lista;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IdProyecto"></param>
        /// <param name="IdVertical"></param>
        /// <param name="IdEquipo"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "/VerticalesEquiposConsulta/{IdProyecto}/{IdVertical}/{IdEquipo=null}", ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        public List<VerticalEquipo> VerticalesEquiposConsulta(string IdProyecto, string IdVertical, string IdEquipo = null)
        {
            List<VerticalEquipo> lista = new List<VerticalEquipo>();
            Conect conect = new Conect();
            conect.CommandQuery = "Demclit_SP_Verticales_Equipos_Consulta";
            conect.AddParameters("id_proyecto", IdProyecto);
            conect.AddParameters("id_vertical", IdVertical);
            conect.AddParameters("id_equipo", IdEquipo);

            DataTable data = conect.GetDataTable();

            foreach (DataRow row in data.Rows)
            {
                var cantidadColumnas = row.Table.Columns.Count;
                lista.Add(new VerticalEquipo
                {
                    idEquipo = int.Parse(row["ID_EQUIPO"].ToString()),
                    idVertical = int.Parse(row["ID_VERTICAL"].ToString()),
                    idTipoVertical = int.Parse(row["ID_TIPO_VERTICAL"].ToString()),
                    tipoVertical = row["TIPO_VERTICAL"].ToString(),
                    ambito = row["AMBITO"].ToString(),
                    idFamilia = short.Parse(row["ID_FAMILIA"].ToString()),
                    familia = row["FAMILIA"].ToString(),
                    idClase = int.Parse(row["ID_CLASE"].ToString()),
                    clase = row["CLASE"].ToString(),
                    cantidad = short.Parse(row["CANTIDAD"].ToString()),
                    descripcion = row["DESCRIPCION"].ToString(),
                    nombreTipoPago = row["NOMBRE_TIPO_CAPEX"].ToString(),
                    opcionPago = short.Parse(row["TIPO_CAPEX"].ToString()),
                    valorMes = decimal.Parse(row["VALOR_MES"].ToString()),
                    valorCop = decimal.Parse(row["VALOR_COP"].ToString()),
                    valorUsd = decimal.Parse(row["VALOR_USD"].ToString()),
                    mesCausacion = int.Parse(row["MES_CAUSACION"].ToString()),
                    mesInicio = int.Parse(row["MES_INICIO"].ToString()),
                    duracion = short.Parse(row["DURACION"].ToString()),
                    referencia = row["REFERENCIA"].ToString(),
                    serial = row["SERIAL"].ToString(),
                    totalValorCop = decimal.Parse(row["TOTAL_VALOR_COP"].ToString()),
                    totalPonderacionUnidad = decimal.Parse(row["TOTAL_PONDERACION_UNIDAD"].ToString()),
                    esActivo = Boolean.Parse(row["ES_ACTIVO"].ToString())
                });
            }

            return lista;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="verticalEquipo"></param>
        /// <returns></returns>
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        [OperationContract]
        public Boolean VerticalesEquipoEliminar(VerticalEquipo verticalEquipo)
        {
            Boolean booRespuesta = false;
            try
            {
                Conect conect = new Conect();
                conect.CommandQuery = "Demclit_SP_Verticales_Equipos_Eliminar";
                conect.AddParameters("id_proyecto", verticalEquipo.idProyecto);
                conect.AddParameters("id_vertical", string.IsNullOrEmpty(verticalEquipo.idVertical.ToString()) ? "0" : verticalEquipo.idVertical.ToString());
                conect.AddParameters("id_equipo", string.IsNullOrEmpty(verticalEquipo.idEquipo.ToString()) ? "0" : verticalEquipo.idEquipo.ToString());
                conect.AddParameters("es_activo", verticalEquipo.esActivo ? 1 : 0);
                conect.ExecTransac();
                booRespuesta = conect.numRows > 0;
                //booRespuesta = true;
            }
            catch (Exception ex)
            {
                booRespuesta = false;
            }
            return booRespuesta;
        }

        //Activos
        /// <summary>
        /// 
        /// </summary>
        /// <param name="IdTipoVertical"></param>
        /// <param name="IdFamilia"></param>
        /// <param name="IdClase"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "/BodegaActivosConsulta/{IdTipoVertical}/{IdFamilia}/{IdClase}", ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        public List<BodegaActivos> BodegaActivosConsulta(string IdTipoVertical, string IdFamilia, string IdClase)
        {
            List<BodegaActivos> lista = new List<BodegaActivos>();
            Conect conect = new Conect();
            conect.CommandQuery = "Demclit_SP_Bodega_Activos_Consulta";
            conect.AddParameters("id_familia", IdFamilia);
            conect.AddParameters("id_clase", IdClase);
            conect.AddParameters("id_tipo_vertical", IdTipoVertical);

            DataTable data = conect.GetDataTable();

            foreach (DataRow row in data.Rows)
            {
                var cantidadColumnas = row.Table.Columns.Count;
                lista.Add(new BodegaActivos
                {
                    idActivo = long.Parse(row["ID_ACTIVO"].ToString()),
                    ambito = row["AMBITO"].ToString(),
                    idFamilia = short.Parse(row["ID_FAMILIA"].ToString()),
                    familia = row["FAMILIA"].ToString(),
                    idClase = int.Parse(row["ID_CLASE"].ToString()),
                    clase = row["CLASE"].ToString(),
                    descripcion = row["DESCRIPCION"].ToString(),
                    referencia = row["REFERENCIA"].ToString(),
                    serial = row["SERIAL"].ToString(),
                    costoCompra = decimal.Parse(row["COSTO_COMPRA"].ToString()),
                    costoFinanciero = decimal.Parse(row["COSTO_FINANCIERO"].ToString()),
                    fechaCompra = DateTime.Parse(row["FECHA_COMPRA"].ToString()),
                    fechaInicioOperacion = DateTime.Parse(string.IsNullOrEmpty(row["FECHA_INICIO_OPERACION"].ToString()) ? DateTime.MinValue.ToString() : row["FECHA_INICIO_OPERACION"].ToString()),
                    mesesOperacion = short.Parse(row["MESES_OPERACION"].ToString() + 0),
                    fechaFinOperacion = DateTime.Parse(string.IsNullOrEmpty(row["FECHA_FIN_OPERACION"].ToString()) ? DateTime.MinValue.ToString() : row["FECHA_FIN_OPERACION"].ToString()),
                    vidaUtilExcendente = short.Parse(row["VIDA_UTIL_EXCENDENTE"].ToString()),

                });
            }

            return lista;
        }

        [WebInvoke(ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        [OperationContract]
        public AppResponse VerticalActivoRegistro(VerticalActivo verticalActivo)
        {
            AppResponse resp = new AppResponse();
            Conect conect;
            try
            {
                conect = new Conect();
                conect.CommandQuery = "Demclit_SP_Verticales_Activos_Registro";
                conect.AddParameters("id_proyecto", verticalActivo.idProyecto);
                conect.AddParameters("id_tipo_vertical", verticalActivo.idTipoVertical);
                conect.AddParameters("activos_vertical", string.Join(",", verticalActivo.activoVertical));

                conect.ExecTransac();


                resp.State = true;
                resp.Data = "";
                resp.Msg = "Activo asociado con exito";
            }
            catch (Exception ex)
            {

                resp.State = false;
                resp.Msg = "No se pudo asociar el activo";
            }

            return resp;
        }

        [WebGet(UriTemplate = "/GenerarReporteFinanciero/{IdProyecto}", ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        public Stream GenerarReporteFinanciero(string IdProyecto)
        {
            MemoryStream memoryStream = new MemoryStream();
            try
            {
                Conect conect = new Conect();
                conect.CommandQuery = "Demclit_SP_Proyecto_Modelo_Financiero";
                conect.AddParameters("id_proyecto", IdProyecto);
                DataTable data = conect.GetDataTable();

                ClosedXML.Excel.XLWorkbook wbook = new ClosedXML.Excel.XLWorkbook();
                var worksheet = wbook.Worksheets.Add("ReporteFinanciero");

                worksheet.Cell(2, 1).InsertTable(data);

                worksheet.Range("A1", "K1").Merge();
                worksheet.Cell("A1").Value = "CAPEX";
                worksheet.Cell("A1").Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                worksheet.Cell("A1").Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.GrayAsparagus;
                worksheet.Cell("A1").Style.Border.LeftBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                worksheet.Cell("A1").Style.Border.LeftBorderColor = ClosedXML.Excel.XLColor.White;
                worksheet.Cell("A1").Style.Font.FontColor = ClosedXML.Excel.XLColor.White;
                worksheet.Cell("A1").Style.Font.Bold = true;

                worksheet.Range("L1", "N1").Merge();
                worksheet.Cell("L1").Value = "OPEX INSTALACIÓN";
                worksheet.Cell("L1").Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                worksheet.Cell("L1").Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.GrayAsparagus;
                worksheet.Cell("L1").Style.Border.LeftBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                worksheet.Cell("L1").Style.Border.LeftBorderColor = ClosedXML.Excel.XLColor.White;
                worksheet.Cell("L1").Style.Font.FontColor = ClosedXML.Excel.XLColor.White;
                worksheet.Cell("L1").Style.Font.Bold = true;

                worksheet.Cell("O1").Value = "OPEX RECURRENTE";
                worksheet.Range("O1", "R1").Merge();
                worksheet.Cell("O1").Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                worksheet.Cell("O1").Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.GrayAsparagus;
                worksheet.Cell("O1").Style.Border.LeftBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                worksheet.Cell("O1").Style.Border.LeftBorderColor = ClosedXML.Excel.XLColor.White;
                worksheet.Cell("O1").Style.Font.FontColor = ClosedXML.Excel.XLColor.White;
                worksheet.Cell("O1").Style.Font.Bold = true;

                worksheet.Range("S1", "T1").Merge();
                worksheet.Cell("S1").Value = "TOTALES";
                worksheet.Cell("S1").Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                worksheet.Cell("S1").Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.GrayAsparagus;
                worksheet.Cell("S1").Style.Border.LeftBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                worksheet.Cell("S1").Style.Border.LeftBorderColor = ClosedXML.Excel.XLColor.White;
                worksheet.Cell("S1").Style.Font.FontColor = ClosedXML.Excel.XLColor.White;
                worksheet.Cell("S1").Style.Font.Bold = true;

                worksheet.Cells("A2:T2").Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.GrayAsparagus;
                worksheet.Cells("A2:T2").Style.Border.TopBorderColor = ClosedXML.Excel.XLColor.GrayAsparagus;

                worksheet.Columns().AdjustToContents();
                //worksheet.Cell("A1").Value = "Title";
                //var range = worksheet.Range("A1:F1");
                //range.Merge().Style.Font.SetBold().Font.FontSize = 16;
                wbook.SaveAs(memoryStream);
                memoryStream.Position = 0;

                //memoryStream.Close();

                String headerInfo = "attachment; filename=" + "ReporteFinanciero" + "." + "xlsx";
                WebOperationContext.Current.OutgoingResponse.Headers["Content-Disposition"] = headerInfo;
                WebOperationContext.Current.OutgoingResponse.ContentType = "application/octet-stream";
                //return File.OpenRead(downloadFilePath);

            }
            catch (Exception ex)
            {

            }
            return memoryStream;
        }


        [WebInvoke(ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        [OperationContract]
        public AppResponse ProyectoEstadoCambio(Proyecto ProyectoEstado)
        {
            AppResponse resp = new AppResponse();
            Conect conect;
            try
            {
                conect = new Conect();
                conect.CommandQuery = "Demclit_SP_Proyecto_Estado_Cambio";
                conect.AddParameters("id_proyecto", ProyectoEstado.idProyecto);
                conect.ExecTransac();
                resp.State = true;
                resp.Data = "";
                resp.Msg = "El proyecto ha pasado al siguiente proceso.";
            }
            catch (Exception ex)
            {

                resp.State = false;
                resp.Msg = "No se pudo crear el proyecto";
            }

            return resp;
        }
        #endregion Proyectos
    }
}
