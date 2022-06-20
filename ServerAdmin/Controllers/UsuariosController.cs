using Logica;
using Microsoft.AspNetCore.Mvc;
using ServerAdminLogicInterface;
using ServerAdmin.DTOs;

namespace ServerAdmin.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly ILogicaUsuario _userLogic;
        public UsuariosController(ILogicaUsuario usarioLogica)
        {
            _userLogic = usarioLogica;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_userLogic.GetAll().Result);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] string id)
        {
            try
            {
                var usuario = _userLogic.GetById(id).Result;
                return Ok(usuario);
            }
            catch (NullReferenceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] UsuarioDTO usuarioDTO)
        {
            var usuario = usuarioDTO.CrearUsuario();
            return Content(_userLogic.Insert(usuario).Result);
        }

        [HttpPut("{name}")]
        public IActionResult Put([FromRoute] string name, [FromBody] UsuarioDTO usuarioDTO)
        {
            usuarioDTO.PNomUsu = name;
            var usuario = usuarioDTO.CrearUsuario();
            return Content(_userLogic.Update(usuario).Result);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] string id)
        {
            _userLogic.Delete(id);
            return Ok();
        }
    }
}