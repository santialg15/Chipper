using Logica;
using Microsoft.AspNetCore.Mvc;
using ServerAdmin.DTOs;
using ServerAdminLogicInterface;

namespace ServerAdmin.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChipsController : Controller
    {
        private readonly ILogicaPublicaciones _chipsLogic;
        public ChipsController(ILogicaPublicaciones chipsLogic)
        {
            _chipsLogic = chipsLogic;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_chipsLogic.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var chip = _chipsLogic.GetById("string");
            return Ok(chip);
        }

        [HttpPost]
        public IActionResult Post([FromBody] PublicacionDTO publicacionDTO)
        {
            var publicacion = publicacionDTO.CrearPublicacion();
            return Ok(_chipsLogic.Insert(publicacion));
        }

        [HttpPut("{id}")]
        public IActionResult Put([FromRoute] Guid id, [FromBody] Publicacion chip)
        {
            chip.Id = id;
            return Ok(_chipsLogic.Update(chip));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] Guid id)
        {
            _chipsLogic.Delete("string");
            return Ok();
        }

        [HttpPost("{id}/Respuestas")]
        public IActionResult CreateAnswer([FromRoute] Guid id, [FromBody] RespuestaDTO respuestaDTO)
        {
            var respuesta = respuestaDTO.CrearRespuesta();
            return Ok(_chipsLogic.CreateAnswer(id,respuesta));
        }

        [HttpDelete("{id}/Respuestas")]
        public IActionResult DeleteAnswer([FromRoute] Guid id,[FromQuery] Guid idRespuesta)
        {
            _chipsLogic.DeleteAnswer(id, idRespuesta);
            return Ok();
        }
    }
}
