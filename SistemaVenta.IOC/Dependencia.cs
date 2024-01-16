using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaVenta.DAL.BDContext;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.DAL.Implementacion;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.BLL.Implementacion;

namespace SistemaVenta.IOC
{
    public static class Dependencia
    {
        public static void InyectarDependencias(this IServiceCollection services, IConfiguration configuration)
        {
            // Método encargado de buscar la cadena de conexión de sql server
            // Luego este método es llamado en el Program.cs
            services.AddDbContext<DbventaContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("CadenaSQL"));
            });

            // Con transient podemos hacer que varíen los tipos de datos
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IVentaRepository, VentaRepository>();

            // Agregamos la dependencia de la interfaz IProductoService y la clase ProductoService para el envío de correos
            services.AddScoped<ICorreoService, CorreoService>();

            // Agregamos la dependecia de la interfaz IFireBaseServices y la clase FireBaseService para subir archivos y eliminar archivos a Firebase.
            services.AddScoped<IFireBaseServices, FireBaseService>();

            // Agregamos la dependencia de la interfaz IUtilidades y la clase UtilidadesServices para el manejo y encriptación de contraseñas.
            services.AddScoped<IUtilidades, UtilidadesServices>();

            // Agregamos la dependecia para el manejo de Roles.
            services.AddScoped<IRolService, RolService>();

            // Agregamos la dependecia para el manejo de Usuarios.
            services.AddScoped<IUsuarioService, UsuarioService>();

            // Agregamos la dependecia para el manejo de los negocios.
            services.AddScoped<INegocioService, NegocioService>();
        }
    }
}
