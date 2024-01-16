using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Newtonsoft.Json;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    public class NegocioController : Controller
    {
        private readonly IMapper _mapper;
        private readonly INegocioService _negocioService;

        public NegocioController(IMapper mapper, INegocioService negocioService)
        {
            _mapper = mapper;
            _negocioService = negocioService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Obtener()
        {
            GenericResponse<VMNegocio> genericResponse = new GenericResponse<VMNegocio>();

            try
            {
                VMNegocio negocio = _mapper.Map<VMNegocio>(await _negocioService.Obtener());

                genericResponse.Estado = true;
                genericResponse.Objeto = negocio;
            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarCambios([FromForm] IFormFile logo, [FromForm] string modelo)
        {
            GenericResponse<VMNegocio> genericResponse = new GenericResponse<VMNegocio>();

            try
            {
                VMNegocio negocio = JsonConvert.DeserializeObject<VMNegocio>(modelo);

                string nombreLogo = "";
                Stream logoStream = null;

                if (logo != null)
                {
                    string nombreEnCodigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(logo.FileName);
                    nombreLogo = string.Concat(nombreEnCodigo, extension);
                    logoStream = logo.OpenReadStream();
                }

                Negocio negocioEditado = await _negocioService.GuardarCambios(_mapper.Map<Negocio>(negocio), logoStream, nombreLogo);

                negocio = _mapper.Map<VMNegocio>(negocioEditado);
                
                genericResponse.Estado = true;
                genericResponse.Objeto = negocio;
            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }
    }
}
