using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PokeApiTeste.Application.Exceptions;
using PokeApiTeste.Context;
using PokeApiTeste.DTO;
using PokeApiTeste.DTOs;
using PokeApiTeste.Infrastructure.Integrations.PokeApi;
using PokeApiTeste.Integrations.PokeApi;
using PokeApiTeste.Model;

namespace PokeApiTeste.Service
{
    public class PokemonService : IPokemonService
    {

        private readonly AppDbContext _context;
        private readonly IPokeApiClient _pokeApi;
        private readonly IMapper _mapper;

        public PokemonService(AppDbContext appDbContext, IPokeApiClient pokeApiClient, IMapper mapper)
        {
            _context = appDbContext;
            _pokeApi = pokeApiClient;
            _mapper = mapper;
        }

        public async Task<ColorPokemonResponseDto> GetByColorAsync(
            string colorName, CancellationToken cancellationToken = default)
        {
            var color = NormalizeColorName(colorName);

            var existing = await FindColorInDatabaseAsync(color, cancellationToken);
            if (existing is not null)
                return _mapper.Map<ColorPokemonResponseDto>(existing);

            var apiResponse = await FetchFromPokeApiAsync(color, cancellationToken);

            PokemonColor colorEntity = await PersistFromApiAsync(apiResponse, cancellationToken);

            return _mapper.Map<ColorPokemonResponseDto>(colorEntity);
        }

        private static string NormalizeColorName(string colorName)
        {
            if (colorName is null)
                throw new ArgumentNullException(nameof(colorName));

            var color = colorName.Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(color))
                throw new ArgumentException("A cor não pode ser vazia");

            return color;
        }

        private async Task<PokemonColor?> FindColorInDatabaseAsync(string colorName, CancellationToken ct)
        {
            return await _context.PokemonColors
                                 .Include(pc => pc.PokemonSpecies)
                                 .FirstOrDefaultAsync(pc => pc.Name == colorName, ct);

        }

        private async Task<PokeApiPokemonColorResponse> FetchFromPokeApiAsync(string colorName, CancellationToken ct)
        {
            var response = await _pokeApi.GetByColorAsync(colorName, ct);

            if (response is null)
                throw new PokeApiException();

            if (response.PokemonSpecies == null || !response.PokemonSpecies.Any())
                throw new PokeApiException("PokéAPI retornou sem pokémons para essa cor");

            return response;
        }

        private async Task<PokemonColor> PersistFromApiAsync(PokeApiPokemonColorResponse apiResponse, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(apiResponse.Name))
                throw new PokeApiException("Nome da cor inválido vindo da API");

            var color = new PokemonColor
            {
                Name = apiResponse.Name
            };

            var speciesValidas = apiResponse.PokemonSpecies
                .Where(s => !string.IsNullOrWhiteSpace(s.Name))
                .Select(s => s.Name.Trim().ToLowerInvariant())
                .Distinct();

            color.PokemonSpecies = speciesValidas
                .Select(name => new PokemonSpecies(name)
                {
                    Color = color
                })
                .ToList();

            _context.PokemonColors.Add(color);
            await _context.SaveChangesAsync(ct);

            return color;
        }
    }
}