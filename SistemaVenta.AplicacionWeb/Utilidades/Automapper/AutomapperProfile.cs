using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.Entity;
using System.Globalization;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace SistemaVenta.AplicacionWeb.Utilidades.Automapper
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            #region Rol
            CreateMap<Rol, VMRol>().ReverseMap();
            #endregion

            #region Usuario
            CreateMap<Usuario, VMUsuario>()
            .ForMember(destino => destino.EsActivo, opt => opt.MapFrom(origen => origen.EsActivo == true ? 1 : 0))
            .ForMember(destino => destino.NombreRol, opt => opt.MapFrom(origen => origen.IdRolNavigation.Descripcion));

            //Por si se necesita otras caracteristicas
            CreateMap<VMUsuario, Usuario>()
            .ForMember(destino => destino.EsActivo, opt => opt.MapFrom(origen => origen.EsActivo == 1 ? true : false))
            .ForMember(destino => destino.IdRolNavigation, opt => opt.Ignore()); //Esto es para que ignore la conversión de Rol a Usuario. Para que no realice alguna conversión.
            #endregion

            #region Negocio
            CreateMap<Negocio, VMNegocio>()
            .ForMember(destino => destino.PorcentajeImpuesto, opt => opt.MapFrom(origen => Convert.ToString(origen.PorcentajeImpuesto.Value, new CultureInfo("es-GT"))));

            CreateMap<VMNegocio, Negocio>()
            .ForMember(destino => destino.PorcentajeImpuesto, opt => opt.MapFrom(origen => Convert.ToString(origen.PorcentajeImpuesto, new CultureInfo("es-GT"))));
            #endregion

            #region  Categoria
            CreateMap<Categoria, VMCategoria>()
            .ForMember(destino => destino.EsActivo, opt => opt.MapFrom(origen => origen.EsActivo == true ? 1 : 0));

            CreateMap<VMCategoria, Categoria>()
            .ForMember(destino => destino.EsActivo, opt => opt.MapFrom(origen => origen.EsActivo == true ? 1 : 0));
            #endregion

            #region Producto

            CreateMap<Producto, VMProducto>()
            .ForMember(destino => destino.EsActivo, opt => opt.MapFrom(origen => origen.EsActivo == true ? 1 : 0))
            .ForMember(destino => destino.NombreCategoria, opt => opt.MapFrom(origen => origen.IdCategoriaNavigation.Descripcion))
            .ForMember(destino => destino.Precio, opt => opt.MapFrom(origen => Convert.ToString(origen.Precio.Value, new CultureInfo("es-GT"))));

            CreateMap<VMProducto, Producto>()
             .ForMember(destino => destino.EsActivo, opt => opt.MapFrom(origen => origen.EsActivo))
            .ForMember(destino => destino.IdCategoriaNavigation, opt => opt.Ignore())
            .ForMember(destino => destino.Precio, opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Precio, new CultureInfo("es-GT"))));
            #endregion

            #region Tipo Documento Venta
            CreateMap<TipoDocumentoVenta, VMTipoDocumentoVenta>().ReverseMap();
            #endregion

            #region Venta
            CreateMap<Venta, VMVenta>()
            .ForMember(destino => destino.TipoDocumentoVenta, opt => opt.MapFrom(origen => origen.IdTipoDocumentoVentaNavigation.Descripcion))
            .ForMember(destino => destino.Usuario, opt => opt.MapFrom(origen => origen.IdUsuarioNavigation.Nombre))
            .ForMember(destino => destino.SubTotal, opt => opt.MapFrom(origen => Convert.ToString(origen.SubTotal.Value, new CultureInfo("es-GT"))))
            .ForMember(destino => destino.ImpuestoTotal, opt => opt.MapFrom(origen => Convert.ToString(origen.ImpuestoTotal.Value, new CultureInfo("es-GT"))))
            .ForMember(destino => destino.Total, opt => opt.MapFrom(origen => Convert.ToString(origen.Total.Value, new CultureInfo("es-GT"))))
            .ForMember(destino => destino.FechaRegistro, opt => opt.MapFrom(origen => origen.FechaRegistro.Value.ToString("dd/MM/yyyy")));

            CreateMap<VMVenta, Venta>()
            .ForMember(destino => destino.SubTotal, opt => opt.MapFrom(origen => Convert.ToDecimal(origen.SubTotal, new CultureInfo("es-GT"))))
            .ForMember(destino => destino.ImpuestoTotal, opt => opt.MapFrom(origen => Convert.ToDecimal(origen.ImpuestoTotal, new CultureInfo("es-GT"))))
            .ForMember(destino => destino.Total, opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Total, new CultureInfo("es-GT"))));
            #endregion

            #region Detalle Venta
            CreateMap<DetalleVenta, VMDetalleVenta>()
            .ForMember(destino => destino.Precio, opt => opt.MapFrom(origen => Convert.ToString(origen.Precio.Value, new CultureInfo("es-GT"))))
            .ForMember(destino => destino.Total, opt => opt.MapFrom(origen => Convert.ToString(origen.Total.Value, new CultureInfo("es-GT"))));

            CreateMap<VMDetalleVenta, DetalleVenta>()
            .ForMember(destino => destino.Precio, opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Precio, new CultureInfo("es-GT"))))
            .ForMember(destino => destino.Total, opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Total, new CultureInfo("es-GT"))));

            CreateMap<DetalleVenta, VMReporteVenta>()
            .ForMember(destino => destino.FechaRegistro, opt => opt.MapFrom(origen => origen.IdVentaNavigation.FechaRegistro.Value.ToString("dd/MM/yyyy")))

            .ForMember(destino => destino.NumeroVenta, opt => opt.MapFrom(origen => origen.IdVentaNavigation.NumeroVenta))
            
            .ForMember(destino => destino.TipoDocumento, opt => opt.MapFrom(origen => origen.IdVentaNavigation.IdTipoDocumentoVentaNavigation))

            .ForMember(destino => destino.DocumentoCliente, opt => opt.MapFrom(origen => origen.IdVentaNavigation.DocumentoCliente))

            .ForMember(destino => destino.NombreCliente, opt => opt.MapFrom(origen => origen.IdVentaNavigation.NombreCliente))
            .ForMember(destino => destino.SubTotalVenta, opt => opt.MapFrom(origen => Convert.ToString(origen.IdVentaNavigation.SubTotal.Value, new CultureInfo("es-GT"))))

            .ForMember(destino => destino.ImpuestoTotalVenta, opt => opt.MapFrom(origen => Convert.ToString(origen.IdVentaNavigation.ImpuestoTotal.Value, new CultureInfo("es-GT"))))

            .ForMember(destino => destino.TotalVenta, opt => opt.MapFrom(origen => Convert.ToString(origen.IdVentaNavigation.Total.Value, new CultureInfo("es-GT"))))

            .ForMember(destino => destino.ImpuestoTotalVenta, opt => opt.MapFrom(origen => origen.DescripcionProducto))

            .ForMember(destino => destino.Precio, opt => opt.MapFrom(origen => Convert.ToString(origen.Precio.Value, new CultureInfo("es-GT"))))

            .ForMember(destino => destino.Total, opt => opt.MapFrom(origen => Convert.ToString(origen.Total.Value, new CultureInfo("es-GT"))));

            #endregion
        
            #region Menu
            CreateMap<Menu, VMMenu>()
            .ForMember(destino => destino.SubMenus, opt => opt.MapFrom(origen => origen.InverseIdMenuPadreNavigation));
            #endregion
        }
    }
}