using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Newtonsoft.Json;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUsuarioService _service;
        private readonly IRolService _rolService;

        public UsuarioController(IUsuarioService usuarioService, IRolService rolService, IMapper mapper)
        {
            _mapper = mapper;
            _service = usuarioService;
            _rolService = rolService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaRoles()
        {
            List<VMRol> listaRoles = _mapper.Map<List<VMRol>>(await _rolService.Lista());
            return StatusCode(StatusCodes.Status200OK, listaRoles);
        }

        [HttpGet]
        public async Task<IActionResult> ListaUsuarios()
        {
            List<VMUsuario> listaUsuarios = _mapper.Map<List<VMUsuario>>(await _service.Lista());
            // Retornamos el listado de usuarios para facilitar la manipulación de datos.
            // Además, muy importante, se devuelve la data en formato de objeto porque DataTables
            // pide este formato.
            return StatusCode(StatusCodes.Status200OK, new { data = listaUsuarios });
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            // Usamos el generic response para devolver una respuesta estandar.
            GenericResponse<VMUsuario> genericResponse = new GenericResponse<VMUsuario>();
            try
            {
                VMUsuario vMUsuario = JsonConvert.DeserializeObject<VMUsuario>(modelo);

                string nombreFoto = "";
                Stream fotoStream = null;

                if (foto != null)
                {
                    string nombreEnCodigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto = $"{nombreEnCodigo}{extension}";
                    fotoStream = foto.OpenReadStream();
                }

                string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/EnviarClave?correo=[correo]&clave=[clave]";

                Usuario usuarioCreado = await _service.Crear(_mapper.Map<Usuario>(vMUsuario), fotoStream, nombreFoto, urlPlantillaCorreo);

                vMUsuario = _mapper.Map<VMUsuario>(usuarioCreado);

                genericResponse.Estado = true;
                genericResponse.Objeto = vMUsuario;
            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, genericResponse); // Retornamos el objeto generico.
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            // Usamos el generic response para devolver una respuesta estandar.
            GenericResponse<VMUsuario> genericResponse = new GenericResponse<VMUsuario>();
            try
            {
                VMUsuario vMUsuario = JsonConvert.DeserializeObject<VMUsuario>(modelo);

                string nombreFoto = "";
                Stream fotoStream = null;

                if (foto != null)
                {
                    string nombreEnCodigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto = $"{nombreEnCodigo}{extension}";
                    fotoStream = foto.OpenReadStream();
                }

                Usuario usuarioEditado = await _service.Editar(_mapper.Map<Usuario>(vMUsuario), fotoStream, nombreFoto);

                vMUsuario = _mapper.Map<VMUsuario>(usuarioEditado);

                genericResponse.Estado = true;
                genericResponse.Objeto = vMUsuario;
            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, genericResponse); // Retornamos el objeto generico.
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int idUsuario)
        {
             GenericResponse<string> genericResponse = new GenericResponse<string>();

             try
             {
                genericResponse.Estado = await _service.Eliminar(idUsuario);
             }
             catch (Exception ex)
             {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
             }
             return StatusCode(StatusCodes.Status200OK, genericResponse); // Retornamos el objeto generico.
        }
    }
}
