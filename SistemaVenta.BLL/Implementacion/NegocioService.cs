using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.BLL.Interfaces;

namespace SistemaVenta.BLL.Implementacion
{
    public class NegocioService : INegocioService
    {
        private readonly IGenericRepository<Negocio> _repositorio;
        private readonly IFireBaseServices _fireBaseServices;

        public NegocioService(IGenericRepository<Negocio> repositorio, IFireBaseServices fireBaseServices)
        {
            _repositorio = repositorio;
            _fireBaseServices = fireBaseServices;
        }

        public async Task<Negocio> GuardarCambios(Negocio entidad, Stream Logo = null, string NombreLogo = "")
        {
            try
            {
                Negocio negocioEncontrado = await _repositorio.Obtener(n => n.IdNegocio == 1);
                negocioEncontrado.NumeroDocumento = entidad.NumeroDocumento;
                negocioEncontrado.Nombre = entidad.Nombre;
                negocioEncontrado.Correo = entidad.Correo;
                negocioEncontrado.Direccion = entidad.Direccion;
                negocioEncontrado.Telefono = entidad.Telefono;
                negocioEncontrado.PorcentajeImpuesto = entidad.PorcentajeImpuesto;
                negocioEncontrado.SimboloMoneda = entidad.SimboloMoneda;
                negocioEncontrado.NombreLogo = negocioEncontrado.NombreLogo == "" ? NombreLogo : negocioEncontrado.NombreLogo;

                if (Logo != null)
                {
                    string urlFireBase = await _fireBaseServices.SubirStorage(Logo, "carpeta_logo", negocioEncontrado.NombreLogo);
                    negocioEncontrado.UrlLogo = urlFireBase;
                }

                await _repositorio.Editar(negocioEncontrado);

                return negocioEncontrado;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Negocio> Obtener()
        {
            try
            {
                Negocio negocioEncontrado = await  _repositorio.Obtener(n => n.IdNegocio == 1);
                return negocioEncontrado;
            }
            catch
            {
                throw;
            }
        }
    }
}
