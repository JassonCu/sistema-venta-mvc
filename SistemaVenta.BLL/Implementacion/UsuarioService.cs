using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Net;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.BLL.Implementacion
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IGenericRepository<Usuario> _repositorio;
        private readonly IFireBaseServices _firebaseServices;
        private readonly ICorreoService _CorreoServices;
        private readonly IUtilidades _utilidadesServices;

        public UsuarioService(IGenericRepository<Usuario> repositorio, IFireBaseServices fireBaseService, IUtilidades utilidades, ICorreoService correoService)
        {
            _repositorio = repositorio;
            _firebaseServices = fireBaseService;
            _utilidadesServices = utilidades;
            _CorreoServices = correoService;
        }

        public async Task<bool> CambiarClave(int IdUsuario, string claveActual, string claveNueva)
        {
            try
            {
                Usuario usuarioEncontrado = await _repositorio.Obtener(u => u.IdUsuario == IdUsuario);

                if (usuarioEncontrado == null)
                    throw new TaskCanceledException("No existe el usuario.");

                if (usuarioEncontrado.Clave != _utilidadesServices.ConvertirSha256(claveActual))
                    throw new TaskCanceledException("La clave actual no es correcta.");

                usuarioEncontrado.Clave = _utilidadesServices.ConvertirSha256(claveNueva);

                bool respuesta = await _repositorio.Editar(usuarioEncontrado);

                return respuesta;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Usuario> Crear(Usuario entidad, Stream Foto = null, string NombreFoto = "", string UrlPlantillaCorreo = "")
        {
            // Consultamos si existe un usuario con el mismo correo.
            Usuario usuarioExiste = await _repositorio.Obtener(u => u.Correo == entidad.Correo);

            // Si existe un usuario con el mismo correo, lanzamos una excepción.
            if (usuarioExiste != null)
                throw new TaskCanceledException("El correo ya existe.");

            try
            {
                // Generamos una clave y la encriptamos en SHA-256
                string claveGenerada = _utilidadesServices.GenerarClave();
                entidad.Clave = _utilidadesServices.ConvertirSha256(claveGenerada);

                entidad.NombreFoto = NombreFoto;

                // Si el usuario tiene una foto, la subimos a Firebase y la guardamos en la entidad.
                if(Foto != null)
                {
                    // Guardamos la foto en Firebase y obtenemos la url de la foto.
                    string UrlFoto = await _firebaseServices.SubirStorage(Foto, "carpeta_usuario", NombreFoto);
                    entidad.UrlFoto = UrlFoto;
                }

                Usuario usuarioCreado = await _repositorio.Crear(entidad);

                if (usuarioCreado.IdUsuario == 0)
                    throw new TaskCanceledException("No se pudo crear el usuario.");

                if (UrlPlantillaCorreo != "")
                {
                    // Enviamos el correo de bienvenida al usuario.
                    UrlPlantillaCorreo = UrlPlantillaCorreo.Replace("[correo]", usuarioCreado.Correo).Replace("[clave]", claveGenerada);

                    string htmlCorreo = "";

                    // Obtenemos el contenido del archivo html de la plantilla de correo.
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UrlPlantillaCorreo);

                    // Obtenemos la respuesta del servidor.
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    
                    // Si la respuesta es OK, obtenemos el contenido del archivo html de la plantilla de correo.
                    if(response.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream dataStream = response.GetResponseStream())
                        {
                            StreamReader readerStream = null;

                            if (response.CharacterSet == null)
                                readerStream = new StreamReader(dataStream);
                            
                            else 
                                readerStream = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));
                            
                            htmlCorreo = readerStream.ReadToEnd();
                            response.Close();
                            readerStream.Close();
                        }

                    }
                    if(htmlCorreo != "")
                        // Enviamos el correo de bienvenida al usuario.
                        await _CorreoServices.EnviarCorreo(usuarioCreado.Correo, "Cuenta creada", htmlCorreo);
                }

                // Consultamos al usuario
                IQueryable<Usuario> query = await _repositorio.Consultar(u => u.IdUsuario == usuarioCreado.IdUsuario);
                usuarioCreado = query.Include(rol => rol.IdRolNavigation).FirstOrDefault();

                return usuarioCreado; // Retornamos el usuario creado. 
            }
            catch (Exception ex)
            {   
                throw;
            }
        }

        public async Task<Usuario> Editar(Usuario entidad, Stream Foto = null, string NombreFoto = "")
        {
            // Consultamos si existe un usuario con el mismo correo.
            Usuario usuarioExiste = await _repositorio.Obtener(u => u.Correo == entidad.Correo && u.IdUsuario != entidad.IdUsuario);

            // Si existe un usuario con el mismo correo, lanzamos una excepción.
            if(usuarioExiste != null)
                throw new TaskCanceledException("El correo ya existe.");

            try
            {
                // Realizamos una consulta para obtener el usuario a editar.
                IQueryable<Usuario> queryUsuario = await _repositorio.Consultar(u => u.IdUsuario == entidad.IdUsuario);

                // Obtenemos el usuario a editar.
                Usuario usuarioEditar = queryUsuario.First();

                // Asignamos los valores
                usuarioEditar.Nombre = entidad.Nombre;
                usuarioEditar.Correo = entidad.Correo;
                usuarioEditar.Telefono = entidad.Telefono;
                usuarioEditar.IdRol = entidad.IdRol;
                usuarioEditar.EsActivo = entidad.EsActivo;
                
                // Si el usuario no tiene una foto, la asignamos al usuario a editar.
                if (usuarioEditar.NombreFoto == "")
                    usuarioEditar.NombreFoto = NombreFoto;

                
                if (Foto != null)
                {
                    // Guardamos la foto en Firebase y obtenemos la url de la foto.
                    string UrlFoto = await _firebaseServices.SubirStorage(Foto, "carpeta_usuario", NombreFoto);
                    usuarioEditar.UrlFoto = UrlFoto;
                
                }

                // Guardamos en una variable la respuesta.
                bool respuesta = await _repositorio.Editar(usuarioEditar);

                // Si la respuesta es false, lanzamos una excepción.
                if (!respuesta)
                    throw new TaskCanceledException("No se pudo modificar el usuario");
                
                // Realizamos una consulta para obtener el usuario editado.
                Usuario usuarioEditado = queryUsuario.Include(rol => rol.IdRolNavigation).First();

                return usuarioEditado; // Retornamos el usuario editado.
            }
            catch
            {
                throw; // Lanzamos una excepción si ocurre un error.

            }
        }

        public async Task<bool> Eliminar(int IdUsuario)
        {
            try
            {
                Usuario usuarioEncontrado = await _repositorio.Obtener(u => u.IdUsuario == IdUsuario);

                if(usuarioEncontrado == null)
                    throw new TaskCanceledException("El usuario no existe");

                string nombreFoto = usuarioEncontrado.NombreFoto;

                bool respuesta = await _repositorio.Eliminar(usuarioEncontrado);

                if (respuesta)
                    await _firebaseServices.EliminarStorage("carpeta_usuario", nombreFoto);

                return respuesta; // Retornamos la respuesta. 
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> GuardarPerfil(Usuario entidad)
        {
            try
            {
                // Consultamos si existe un usuario con el mismo Id.
                Usuario usuarioEncontrado = await _repositorio.Obtener(u => u.IdUsuario == entidad.IdUsuario);

                // Si no existe un usuario con el mismo Id, lanzamos una excepción.
                if (usuarioEncontrado == null)
                    throw new TaskCanceledException("El usuario no existe");
                
                usuarioEncontrado.Correo = entidad.Correo;
                usuarioEncontrado.Telefono = entidad.Telefono;

                bool respuesta =  await _repositorio.Editar(usuarioEncontrado); // Retornamos la respuesta. 

                return respuesta; // Retornamos la respuesta. 
            }
            catch
            {
                throw; // Lanzamos una excepción si ocurre un error.
            }
        }

        public async Task<List<Usuario>> Lista()
        {
            // Consultamos los usurios
            IQueryable<Usuario> query = await _repositorio.Consultar();

            // Icluimos y retornamos dentro de la query el rol de usurios ya que tienen relación dentro del modelo Usuario.
            return query.Include(rol => rol.IdRolNavigation).ToList();
        }

        public async Task<Usuario> ObtenerPorCredenciales(string correo, string clave)
        {
            // Encriptamos las credenciales.
            string claveEncriptada = _utilidadesServices.ConvertirSha256(clave);

            // Consultamos si existe un usuario con el mismo correo y clave encriptadas.
            Usuario usuarioEncontrado = await _repositorio.Obtener(u => u.Correo.Equals(correo) && u.Clave.Equals(clave));

            return usuarioEncontrado; // Retornamos el usuario encontrado.
        }

        public async Task<Usuario> ObtenerPorId(int IdUsuario)
        {
            // Hacemos una consulta para ver si el Id que ingresan coincide con el Id que está en la base de datos.
            IQueryable<Usuario> query = await _repositorio.Consultar(u => u.IdUsuario == IdUsuario);

            // Usuario encontrado.
            Usuario resultado = query.Include(rol => rol.IdRolNavigation).FirstOrDefault();

            // Retornamos el usuario encontrado.
            return resultado;
        }

        public async Task<bool> RestablecerClave(string Correo, string UrlPlantillaCorreo)
        {
            try
            {
                Usuario usuarioEncontrado = await _repositorio.Obtener(u => u.Correo == Correo);

                if (usuarioEncontrado == null)
                    throw new TaskCanceledException("No encontramos algún usuario asociado al correo.");
                
                string claveGenerada = _utilidadesServices.GenerarClave();

                usuarioEncontrado.Clave = _utilidadesServices.ConvertirSha256(claveGenerada);

                // Enviamos el correo de bienvenida al usuario.
                    UrlPlantillaCorreo = UrlPlantillaCorreo.Replace("[clave]", claveGenerada);

                    string htmlCorreo = "";

                    // Obtenemos el contenido del archivo html de la plantilla de correo.
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UrlPlantillaCorreo);

                    // Obtenemos la respuesta del servidor.
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    
                    // Si la respuesta es OK, obtenemos el contenido del archivo html de la plantilla de correo.
                    if(response.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream dataStream = response.GetResponseStream())
                        {
                            StreamReader readerStream = null;

                            if (response.CharacterSet == null)
                                readerStream = new StreamReader(dataStream);
                            
                            else 
                                readerStream = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));

                            htmlCorreo = readerStream.ReadToEnd();
                            response.Close();
                            readerStream.Close();

                        }
                    }    
                    bool correoEnviado = false;

                    if(htmlCorreo != "")
                        // Enviamos el correo de restablecimiento de la contraseña.
                        await _CorreoServices.EnviarCorreo(Correo, "Contraseña Restablecida", htmlCorreo);
                    
                    // Si no fué enviado el correo mandamos una excepcion.
                    if (!correoEnviado)
                        throw new TaskCanceledException("No se pudo enviar el correo. Por favor intentalo de nuevo mas tarde");

                    bool respuesta = await _repositorio.Editar(usuarioEncontrado);

                    return respuesta; // Retornamos la respuesta.
            }
            catch
            {
                throw; // Lanzamos una excepción si ocurre un error.
            }
        }
    }
}