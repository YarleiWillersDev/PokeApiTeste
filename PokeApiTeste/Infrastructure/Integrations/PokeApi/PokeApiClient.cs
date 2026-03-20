using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokeApiTeste.DTOs;
using PokeApiTeste.Infrastructure.Integrations.PokeApi;

namespace PokeApiTeste.Integrations.PokeApi
{
    public sealed class PokeApiClient : IPokeApiClient
    {
        private readonly HttpClient _http;

        public PokeApiClient(HttpClient httpClient)
        {
            _http = httpClient;
        }

        public Task<PokeApiPokemonColorResponse?> GetByColorAsync(string colorName, CancellationToken cancellationToken = default)
        {
                        return _http.GetFromJsonAsync<PokeApiPokemonColorResponse>(
                $"pokemon-color/{colorName}/", 
                cancellationToken);
        }
    }
}