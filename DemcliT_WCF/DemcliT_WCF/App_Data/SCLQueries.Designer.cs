﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DemcliT_WCF.App_Data {
    using System;
    
    
    /// <summary>
    ///   Clase de recurso fuertemente tipado, para buscar cadenas traducidas, etc.
    /// </summary>
    // StronglyTypedResourceBuilder generó automáticamente esta clase
    // a través de una herramienta como ResGen o Visual Studio.
    // Para agregar o quitar un miembro, edite el archivo .ResX y, a continuación, vuelva a ejecutar ResGen
    // con la opción /str o recompile su proyecto de VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class SCLQueries {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal SCLQueries() {
        }
        
        /// <summary>
        ///   Devuelve la instancia de ResourceManager almacenada en caché utilizada por esta clase.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DemcliT_WCF.App_Data.SCLQueries", typeof(SCLQueries).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Reemplaza la propiedad CurrentUICulture del subproceso actual para todas las
        ///   búsquedas de recursos mediante esta clase de recurso fuertemente tipado.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a SELECT VE.COD_VENDEDOR
        ///        , VE.NOM_VENDEDOR
        ///        , VE.NUM_IDENT
        ///        , VE.COD_OFICINA
        ///        , O.DES_OFICINA
        ///        , VE.E_MAIL, VE.NUM_MOVIL
        ///        , EST.DES_ESTADO
        ///FROM VE_VENDEDORES VE
        ///    INNER JOIN GE_OFICINAS O ON O.COD_OFICINA = VE.COD_OFICINA
        ///    INNER JOIN VE_ESTADOS_VENDEDOR EST ON EST.COD_ESTADO = VE.COD_ESTADO
        ///WHERE COD_VENDEDOR = &apos;:codVendedor:&apos;.
        /// </summary>
        internal static string InfoVendedor {
            get {
                return ResourceManager.GetString("InfoVendedor", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a SELECT OFI.COD_OFICINA, OFI.DES_OFICINA, DIR.NOM_CALLE, DIR.COD_TIPOCALLE
        ///FROM GE_OFICINAS OFI
        ///    INNER JOIN GE_DIRECCIONES DIR ON DIR.COD_DIRECCION = OFI.COD_DIRECCION.
        /// </summary>
        internal static string Oficinas {
            get {
                return ResourceManager.GetString("Oficinas", resourceCulture);
            }
        }
    }
}