using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSCitasBambuDC.Modelos
{
    /// <summary>
    /// Clase usada para enlistar a las personas y devolverlas en un metodo al cliente
    /// </summary>
    public class SerializablePersona
    {
        public int PersonaID { get; set; }
        public int Cedula { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public bool EsAdmin { get; set; }

    }
}