namespace TestesProjetosPokeApi;

using System.Linq.Expressions;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using PokeApiTeste.Context;
using PokeApiTeste.DTO;
using PokeApiTeste.DTOs;
using PokeApiTeste.Infrastructure.Integrations.PokeApi;
using PokeApiTeste.Integrations.PokeApi;
using PokeApiTeste.Mapper;
using PokeApiTeste.Model;
using PokeApiTeste.Service;

[TestClass]
public class UnitTest
{
    [TestMethod]
    public async Task GetByColorAsync_DeveRetornarPokemonsCadastradosNoBanco_QuandoCorValida()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        using var context = new AppDbContext(options);
        context.Database.EnsureCreated();

        PokemonColor entity = new PokemonColor { Name = "red" };
        context.PokemonColors.Add(entity);
        await context.SaveChangesAsync();

        var clientMock = new Mock<IPokeApiClient>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });

        var mapper = config.CreateMapper();

        var service = new PokemonService(context, clientMock.Object, mapper);

        var result = await service.GetByColorAsync("red");

        Assert.IsNotNull(result);

        clientMock.Verify(
            x => x.GetByColorAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [TestMethod]
    public async Task GetByColorAsync_DeveRetornarPokemonsDiretamenteDaApi_QuandoCorValida()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        using var context = new AppDbContext(options);
        context.Database.EnsureCreated();

        var clientMock = new Mock<IPokeApiClient>();

        clientMock
            .Setup(x => x.GetByColorAsync("red", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PokeApiPokemonColorResponse
            {
                Id = 1,
                Name = "red",
                PokemonSpecies = new List<PokeApiColors>
                {
                    new PokeApiColors
                    {
                        Name = "pikachu",
                        Url = "https://pokeapi.co/api/v2/pokemon-species/pikachu"
                    },
                    new PokeApiColors
                    {
                        Name = "charmander",
                        Url = "https://pokeapi.co/api/v2/pokemon-species/charmander"
                    }
                }
            });

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });

        var mapper = config.CreateMapper();

        var service = new PokemonService(context, clientMock.Object, mapper);

        var result = await service.GetByColorAsync("red");

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.PokemonNames.Count);

        clientMock.Verify(
            x => x.GetByColorAsync("red", It.IsAny<CancellationToken>()),
            Times.Once
        );

        var saved = context.PokemonColors.First();

        Assert.AreEqual("red", saved.Name);

        Assert.AreEqual(1, context.PokemonColors.Count());
    }

    [TestMethod]
    public async Task GetByColorAsync_NaoDeveDuplicarDados_QuandoExistirNoBanco()
    {

        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        using var context = new AppDbContext(options);
        context.Database.EnsureCreated();

        var clientMock = new Mock<IPokeApiClient>();

        clientMock
            .Setup(x => x.GetByColorAsync("red", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PokeApiPokemonColorResponse
            {
                Id = 1,
                Name = "red",
                PokemonSpecies = new List<PokeApiColors>
                {
                    new PokeApiColors
                    {
                        Name = "pikachu",
                        Url = "https://pokeapi.co/api/v2/pokemon-species/pikachu"
                    },
                    new PokeApiColors
                    {
                        Name = "charmander",
                        Url = "https://pokeapi.co/api/v2/pokemon-species/charmander"
                    }
                }
            });

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });

        var mapper = config.CreateMapper();

        var service = new PokemonService(context, clientMock.Object, mapper);

        await service.GetByColorAsync("red");

        await service.GetByColorAsync("red");

        Assert.AreEqual(1, context.PokemonColors.Count());
    }

    [TestMethod]
    public async Task GetByColorAsync_NaoDeveAceitarParametroNull()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        using var context = new AppDbContext(options);
        context.Database.EnsureCreated();

        var clientMock = new Mock<IPokeApiClient>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });

        var mapper = config.CreateMapper();

        var service = new PokemonService(context, clientMock.Object, mapper);

        await Assert.ThrowsExceptionAsync<ArgumentException>(
            () => service.GetByColorAsync(null)
        );

        clientMock.Verify(
            x => x.GetByColorAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }
}