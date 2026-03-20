using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PokeApiTeste.DTO;
using PokeApiTeste.Service;

namespace PokeApiTeste.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PokeApiController : ControllerBase
    {
        private readonly IPokemonService _service;

        public PokeApiController(IPokemonService service)
        {
            _service = service;
        }

        [HttpGet("{colorName}/pokemons")]
        public async Task<ActionResult<ColorPokemonResponseDto>> GetByColor(string colorName)
        {
            var result = await _service.GetByColorAsync(colorName);
            return Ok(result);
        }
    }
}