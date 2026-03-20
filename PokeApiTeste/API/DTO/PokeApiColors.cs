using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PokeApiTeste.DTOs
{
    public sealed class PokeApiColors
    {
        [Required]
        public string Name { get; set; } = default!;

        [Required]
        public string Url { get; set; } = default!;


    }
}