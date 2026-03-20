using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokeApiTeste.DTOs;

namespace PokeApiTeste.Infrastructure.Integrations.PokeApi
{
    public interface IPokeApiClient
    {
        Task<PokeApiPokemonColorResponse?> GetByColorAsync(string color, CancellationToken cancellationToken = default);
    }
}