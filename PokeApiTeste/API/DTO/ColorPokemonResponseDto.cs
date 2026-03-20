using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PokeApiTeste.DTO
{
    public sealed class ColorPokemonResponseDto
    {
        [Required]
        public string Color { get; set; } = default!;

        [Required]
        public List<string> PokemonNames { get; set; } = new();

    }
}