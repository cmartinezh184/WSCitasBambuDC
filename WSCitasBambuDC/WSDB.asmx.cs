using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace WSCitasBambuDC
{
    /// <summary>
    /// Summary description for WSDB
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WSDB : System.Web.Services.WebService
    {

        [WebMethod]
        public bool LogIn(string correo, string pass)
        {
            return true;
        }

        [WebMethod]
        public bool LogOut(string correo)
        {
            return true;
        }

        [WebMethod]
        public bool CrearCita(DateTime fecha)
        {
            return true;
        }

        [WebMethod]
        public bool BorrarCita(int idFecha)
        {
            return true;
        }

        [WebMethod]
        public bool ReservarCita(string correoCliente, DateTime fecha)
        {
            return true;
        }

    }
}
