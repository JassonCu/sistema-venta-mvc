using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;
using SistemaVenta.DAL.Interfaces;
using Firebase.Auth;
using Firebase.Storage;

namespace SistemaVenta.BLL.Implementacion
{
    public class FireBaseService : IFireBaseServices
    {
        private readonly IGenericRepository<Configuracion> _repositorio;

        public FireBaseService(IGenericRepository<Configuracion> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<string> SubirStorage(Stream StreamArchivo, string CarpetaDestino, string NombreArchivo)
        {
            string urlImagen = "";

            try
            {
                // Consulta a la base de datos para obtener las credenciales de autenticación de Firebase
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("Firebase_Storage"));

                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                // Instancia del proveedor de autenticación de Firebase
                var auth = new FirebaseAuthProvider(new FirebaseConfig(Config["api_key"]));

                // Autenticación en Firebase con las credenciales de correo electrónico y contraseña
                var inicioSesionFirebase = await auth.SignInWithEmailAndPasswordAsync(Config["email"], Config["clave"]);

                // Creación de un token de cancelación para la tarea de subida de archivo a Firebase Storage
                var tokenCancelacion = new CancellationTokenSource();

                // Instancia del proveedor de almacenamiento de Firebase Storage con las credenciales de autenticación
                var task = new FirebaseStorage(
                    Config["ruta"],
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(inicioSesionFirebase.FirebaseToken),
                        ThrowOnCancel = true
                    }
                )
                // Subida de un archivo a Firebase Storage con las credenciales de autenticación y el token de cancelación
                .Child(Config[CarpetaDestino])
                .Child(NombreArchivo)
                .PutAsync(StreamArchivo, tokenCancelacion.Token);

                // Espera a que la tarea de subida de archivo a Firebase Storage se complete
                urlImagen = await task;
            }
            catch
            {
                urlImagen = "";
            }
            return urlImagen;
        }
        
        public async Task<bool> EliminarStorage(string CarpetaDestino, string NombreArchivo)
        {
            try
            {
                // Consulta a la base de datos para obtener las credenciales de autenticación de Firebase
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("Firebase_Storage"));

                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                // Instancia del proveedor de autenticación de Firebase
                var auth = new FirebaseAuthProvider(new FirebaseConfig(Config["api_key"]));

                // Autenticación en Firebase con las credenciales de correo electrónico y contraseña
                var inicioSesionFirebase = await auth.SignInWithEmailAndPasswordAsync(Config["email"], Config["clave"]);

                // Creación de un token de cancelación para la tarea de subida de archivo a Firebase Storage
                var tokenCancelacion = new CancellationTokenSource();

                // Instancia del proveedor de almacenamiento de Firebase Storage con las credenciales de autenticación
                var task = new FirebaseStorage(
                    Config["ruta"],
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(inicioSesionFirebase.FirebaseToken),
                        ThrowOnCancel = true
                    }
                )
                // Eliminación de un archivo de Firebase Storage con las credenciales de autenticación y el token de cancelación
                .Child(Config[CarpetaDestino])
                .Child(NombreArchivo)
                .DeleteAsync();

                await task;

                // Si la tarea se completó sin errores, devuelve true, de lo contrario, devuelve false.
                return true;
            }
            catch 
            {
                return false;
            }
        }

    }
}
