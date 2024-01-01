using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.DAL.BDContext;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.DAL.Implementacion
{
    public class VentaRepository : GenericRepository<Venta>, IVentaRepository
    {
        private readonly DbventaContext _dbventaContext;

        public VentaRepository(DbventaContext dbventaContext) : base(dbventaContext)
        {

            _dbventaContext = dbventaContext;
        }

        public async Task<Venta> Registrar(Venta entidad)
        {
            Venta ventaGenerada = new Venta();

            //Usamos una transaccion para evitar problemas con la base de datos a la hora de realiar un Insert. Similar al transaction atomic. Si ocurre un problema con la base de datos, se restablecen los datos como estaban antes.
            using (var transaccion = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    foreach (DetalleVenta dv in entidad.DetalleVenta)
                    {
                        Producto productoEntontrado = _dbContext.Productos
                            .Where(p => p.IdProducto == dv.IdProducto)
                            .FirstOrDefault();

                        productoEntontrado.Stock -= dv.Cantidad;
                        _dbContext.Productos.Update(productoEntontrado);
                    }

                    await _dbContext.SaveChangesAsync();
                    NumeroCorrelativo correlativo = _dbContext.NumeroCorrelativos
                        .Where(p => p.Gestion == "venta")
                        .First();

                    correlativo.FechaActualizacion = DateTime.Now;

                    _dbContext.NumeroCorrelativos.Update(correlativo);
                    await _dbContext.SaveChangesAsync();

                    string ceros = string.Concat(Enumerable.Repeat("0", correlativo.CantidadDigitos.Value));
                    string numeroVenta = ceros + correlativo.UltimoNumero.ToString();

                    numeroVenta = numeroVenta.Substring(numeroVenta.Length - correlativo.CantidadDigitos.Value, correlativo.CantidadDigitos.Value);
                    entidad.NumeroVenta = numeroVenta;
                    await _dbContext.Venta.AddAsync(entidad);

                    await _dbContext.SaveChangesAsync();

                    ventaGenerada = entidad;
                    // Para confirmar la transacción usamos el Commi. Llega hasta este punto cuando no hay errores en la transacción.
                    transaccion.Commit();
                }
                catch (Exception ex)
                {
                    // Para descartar toda la transacción usamos el Rollback
                    transaccion.Rollback();
                    throw; // Todo: Manejar la excepción adecuadamente
                }

                return ventaGenerada; // Retornamos la venta generada. El Id de la venta se generó automáticamente. El Id del correlativo se actualizó automáticamente. El Id de los productos se actualizó automáticamente.
            }
        }

        public async Task<List<DetalleVenta>> Reporte(DateTime FechaInicio, DateTime FechaFin)
        {
            List<DetalleVenta> listaResumen = await _dbContext.DetalleVenta
                .Include(v => v.IdVentaNavigation)
                .ThenInclude(u => u.IdUsuarioNavigation)
                .Include(v => v.IdVentaNavigation)
                .ThenInclude(tdv => tdv.IdTipoDocumentoVentaNavigation)
                .Where(dv => dv.IdVentaNavigation.FechaRegistro.Value.Date >= FechaInicio.Date && dv.IdVentaNavigation.FechaRegistro.Value.Date <= FechaFin.Date)
                .ToListAsync();

            return listaResumen;
        }
    }
}
