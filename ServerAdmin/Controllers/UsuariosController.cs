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
        public IActionResult GetById([FromRoute] Guid id)
        {
            var user = _userLogic.GetById(id);
            return Ok(user);
        }

        [HttpPost]
        public IActionResult Post([FromBody] UsuarioDTO usuarioDTO)
        {
            var usuario = usuarioDTO.CrearUsuario();
            return Content(_userLogic.Insert(usuario).Result);
        }

        [HttpPut("{id}")]
        public IActionResult Put([FromRoute] Guid id, [FromBody] Usuario usuario)
        {
            usuario.Id = id;
            return Ok(_userLogic.Update(usuario));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] Guid id)
        {
            _userLogic.Delete(id);
            return Ok();
        }
    }
}