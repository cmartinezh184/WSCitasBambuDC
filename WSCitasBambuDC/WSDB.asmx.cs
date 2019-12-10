using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Services;
using WSCitasBambuDC.Modelos;

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

        /// <summary>
        /// Metodo para registrar un usuario en la base de datos
        /// </summary>
        /// <param name="cedula">Cedula del cliente</param>
        /// <param name="primerNombre">Primer nombre del usuario</param>
        /// <param name="segundoNombre">Segundo nombre del usuario, puede ir vacio</param>
        /// <param name="primerApellido">Primer apellido del usuario</param>
        /// <param name="segundoApellido">Segundo apellido del usuario</param>
        /// <param name="telefono">Telefono del usuario</param>
        /// <param name="correo">Correo del usuario</param>
        /// <param name="password">Password del usuario</param>
        /// <returns>Booleano del estado de la operacion</returns>
        [WebMethod]
        public bool CrearUsuario(int cedula, string primerNombre, string segundoNombre, string primerApellido, string segundoApellido, string telefono, string correo, string password)
        {
            // Se verifica que los campos no vengan vacios
            if (cedula == 0 || primerNombre.Trim().Equals("") || segundoNombre.Trim().Equals("") || primerApellido.Trim().Equals("") || segundoApellido.Trim().Equals("") || telefono.Trim().Equals("") || correo.Trim().Equals("") || password.Trim().Equals(""))
            {
                return false;
            }

            // Se crea un objeto de tipo persona con los datos ingresados
            Persona persona = new Persona()
            {
                PrimerNombre = primerNombre,
                SegundoNombre = segundoNombre,
                PrimerApellido = primerApellido,
                SegundoApellido = segundoApellido,
                Correo = correo,
                Telefono = telefono,
                PersonaID = cedula,
                Pass = password,
                EsAdmin = false,
            };

            // Se ingresa el usuario a la base de datos con los datos ingresados
            using (var db = new BambuDBEntities())
            {
                db.Personas.Add(persona);
                if(db.SaveChanges() == 0)
                {
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Metodo para iniciar sesion en el servicio
        /// </summary>
        /// <param name="correo">Correo ingresado</param>
        /// <param name="pass">Password ingresada</param>
        /// <returns>Booleano del estado de la operacion</returns>
        [WebMethod]
        public bool LogIn(string correo, string pass)
        {
            if(correo.Trim().Equals("") || pass.Trim().Equals("") || !ValidarCorreo(correo))
            {
                return false;
            }

            // Se usan las entidades obtenidas de la base de datos
            using (var db = new BambuDBEntities())
            {
                // Se consulta con Linq la tabla en busca de los credenciales ingresados
                var query = from p in db.Personas
                            where p.Correo == correo
                            && p.Pass == pass
                            select p;

                // Si se encontro al usuario se continua
                if (query.Any())
                {
                    return true;
                }
                
            }
            return false;
        }

        /// <summary>
        /// Metodo para validar el formato del correo
        /// </summary>
        /// <param name="correo">correo ingresado por el usuario</param>
        /// <returns>True si el correo tiene un formato valido</returns>
        private bool ValidarCorreo(string correo)
        {
            // Se usa un objeto EmailAddressAttribute para verificar el formato del string de correo
            if(new EmailAddressAttribute().IsValid(correo))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Metodo del servicio para agregar citas a la base de datos
        /// </summary>
        /// <param name="fecha">Fecha de la cita seleccionada</param>
        /// <param name="cedulaCliente">Cedula del cliente que la reserva</param>
        /// <param name="descripcion">Descripcion de la cita</param>
        /// <returns>true si se logra terminar la operacion</returns>
        [WebMethod]
        public bool CrearCita(DateTime? fecha, string descripcion)
        {
            // Se verifica que los datos no vengan vacios
            if(fecha == null || descripcion.Trim().Equals(""))
            {
                return false;
            }
            // Se crea un objeto de tipo cita con la descripcion y la fecha ingresada 
            Cita cita = new Cita()
            {
                Descripcion = descripcion,
                Fecha = fecha
            };

            using (var db = new BambuDBEntities())
            {
                db.Citas.Add(cita);
                if(db.SaveChanges() == 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Metodo para quitar la reservacion de las citas
        /// </summary>
        /// <param name="idCita">Identificador de la cita</param>
        /// <returns>Booleano del estado de la operacion</returns>
        [WebMethod]
        public bool BorrarCita(int idCita)
        {
            if (idCita == 0)
            {
                return false;
            }

            using (var db = new BambuDBEntities())
            {
                var cita = db.Citas.Find(idCita);
                if (cita == null)
                {
                    return false;
                }

                cita.ClienteAsignado = null;

                if (db.SaveChanges() == 0)
                {
                    return false;
                }
            }
                return true;
        }

        /// <summary>
        /// Metodo para asignar un cliente a una cita
        /// </summary>
        /// <param name="cedulaCliente">Cedula del cliente</param>
        /// <param name="idCita">Identificador de la cita</param>
        /// <returns>Booleano del estado de la operacion</returns>
        [WebMethod]
        public bool ReservarCita(int cedulaCliente, int idCita)
        {
            // Se verifica si los campos estan vacios
            if(cedulaCliente == 0 || idCita == 0)
            {
                return false;
            }

            using (var db = new BambuDBEntities())
            {
                // Se busca la cita con una expresion Lambda en la base de datos
                Cita cita = db.Citas.Single(c => c.CitasID == idCita);

                // Se verifica que la cita no tenga clientes asignados
                if(cita.ClienteAsignado != null)
                {
                    return false;
                }

                // Se verifica que el cliente exista en el sistema
                if(db.Personas.Find(cedulaCliente) == null)
                {
                    return false;
                }

                // Se le asigna el cliente a la cita
                cita.ClienteAsignado = cedulaCliente;

                if(db.SaveChanges() == 0)
                {
                    return false;
                }
            }
            
            return true;
        }

        /// <summary>
        /// Metodo que devuelve las citas asociadas a un cliente
        /// </summary>
        /// <param name="cedula">Cedula del cliente</param>
        /// <returns>La lista si se encuentran o nulo si no hay citas asignadas</returns>
        [WebMethod]
        public List<SerializableCita> CitasDeCliente(int cedula)
        {
            // Se verifica que no vengan datos vacios
            if(cedula == 0)
            {
                return null;
            }

            // Se crea una lista local
            List<SerializableCita> listaCitas = new List<SerializableCita>();

            // Se consultan las listas en base a la cedula ingresada
            using (var db = new BambuDBEntities())
            {
                var citas = from c in db.Citas
                            where c.ClienteAsignado == cedula
                            select c;

                // Si la lista viene vacia se termina
                if(citas.Count() == 0)
                {
                    return null;
                }

                foreach(Cita cita in citas)
                {
                    SerializableCita temp = new SerializableCita
                    {
                        CitasID = cita.CitasID,
                        ClienteAsignado = cita.ClienteAsignado,
                        Descripcion = cita.Descripcion,
                        Fecha = cita.Fecha
                    };
                    listaCitas.Add(temp);
                }

                // Se devuelve la lista de citas
                return listaCitas;
            }

        }

        /// <summary>
        /// Metodo que devuelve la informacion del cliente
        /// </summary>
        /// <param name="cedula">Cedula del cliente</param>
        /// <returns>Objeto con la informacion del cliente</returns>
        [WebMethod]
        public SerializablePersona InfoPersona(int cedula)
        {
            // Se verifica si los datos vienen vacios
            if(cedula == 0)
            {
                return null;
            }

            // Se busca en la base de datos al cliente
            using (var db = new BambuDBEntities())
            {
                var q = db.Personas.Find(cedula);

                // Si no hay cliente con esa cedula se termina la operacion
                if(q == null)
                {
                    return null;
                }

                // Se pasa la informacion a un objeto que se puede devolver a traves del metodo
                SerializablePersona persona = new SerializablePersona()
                {
                    PersonaID = q.PersonaID,
                    Correo = q.Correo,
                    EsAdmin = q.EsAdmin,
                    PrimerNombre = q.PrimerNombre,
                    SegundoNombre = q.SegundoNombre,
                    PrimerApellido = q.PrimerApellido,
                    SegundoApellido = q.SegundoApellido,
                    Telefono = q.Telefono
                };

                return persona;
            }
        }

    }
}
