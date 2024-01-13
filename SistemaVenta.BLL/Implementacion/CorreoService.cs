using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.BLL.Implementacion
{
    public class CorreoService : ICorreoService
    {
        private readonly IGenericRepository<Configuracion> _repositorio;

        public CorreoService(IGenericRepository<Configuracion> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<bool> EnviarCorreo(string CorreoDestino, string Asunto, string Mensaje)
        {
            try
            {
                // Consulta a la base de datos para obtener la configuración del correo electrónico.
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("Servicio_Correo"));

                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                //Obtención de las credenciales para el envío del correo electrónico.
                var credenciales = new NetworkCredential(Config["correo"], Config["clave"]);

                // Creación del correo electrónico.
                var correo = new MailMessage()
                {
                    From = new MailAddress(Config["correo"]),
                    Subject = Asunto,
                    Body = Mensaje,
                    IsBodyHtml = true
                };

                // Agregar destinatario al correo electrónico.
                correo.To.Add(new MailAddress(CorreoDestino));

                // Creación del cliente SMTP para el envío del correo electrónico.
                var cliente = new SmtpClient()
                {
                    Host = Config["host"],
                    Port = int.Parse(Config["puerto"]),
                    Credentials = credenciales,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    EnableSsl = true,
                };

                // Autenticación del cliente SMTP para el envío del correo electrónico.
                cliente.Send(correo);

                return true;

            }
            catch (System.Exception)
            {

                return false;
            }
        }
    }
}
