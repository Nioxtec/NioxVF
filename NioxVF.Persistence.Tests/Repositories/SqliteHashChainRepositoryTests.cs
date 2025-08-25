using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NioxVF.Persistence.Entities;
using NioxVF.Persistence.Sqlite.Context;
using NioxVF.Persistence.Sqlite.Repositories;
using Xunit;

namespace NioxVF.Persistence.Tests.Repositories;

/// <summary>
/// Tests unitarios para SqliteHashChainRepository
/// </summary>
public class SqliteHashChainRepositoryTests : IDisposable
{
    private readonly DbContextOptions<SqliteDbContext> _options;
    private readonly Mock<ILogger<SqliteHashChainRepository>> _loggerMock;

    public SqliteHashChainRepositoryTests()
    {
        // Configurar base de datos en memoria para tests
        _options = new DbContextOptionsBuilder<SqliteDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _loggerMock = new Mock<ILogger<SqliteHashChainRepository>>();
    }

    /// <summary>
    /// Test para verificar que GetPreviousHashAsync retorna el hash anterior correcto
    /// </summary>
    [Fact]
    public async Task GetPreviousHashAsync_ShouldReturnPreviousHash()
    {
        // Arrange
        using var context = new SqliteDbContext(_options);
        var repository = new SqliteHashChainRepository(context, _loggerMock.Object);
        
        var hashChain = new HashChainEntity
        {
            SellerNif = "B00000000",
            Series = "A",
            CurrentHash = "ABC123",
            PreviousHash = "XYZ789",
            LastUpdated = DateTime.UtcNow
        };

        await repository.AddAsync(hashChain);

        // Act
        var result = await repository.GetPreviousHashAsync("B00000000", "A");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("ABC123", result);
    }

    /// <summary>
    /// Test para verificar que GetPreviousHashAsync retorna null cuando no existe la cadena
    /// </summary>
    [Fact]
    public async Task GetPreviousHashAsync_ShouldReturnNull_WhenChainDoesNotExist()
    {
        // Arrange
        using var context = new SqliteDbContext(_options);
        var repository = new SqliteHashChainRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetPreviousHashAsync("B00000000", "Z");

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Test para verificar que UpdateChainAsync actualiza la cadena correctamente
    /// </summary>
    [Fact]
    public async Task UpdateChainAsync_ShouldUpdateChain()
    {
        // Arrange
        using var context = new SqliteDbContext(_options);
        var repository = new SqliteHashChainRepository(context, _loggerMock.Object);
        
        var hashChain = new HashChainEntity
        {
            SellerNif = "B00000000",
            Series = "A",
            CurrentHash = "ABC123",
            PreviousHash = "XYZ789",
            LastUpdated = DateTime.UtcNow
        };

        await repository.AddAsync(hashChain);

        // Act
        await repository.UpdateChainAsync("B00000000", "A", "ABC123", "NEW456");

        // Assert
        var updatedChain = await repository.GetPreviousHashAsync("B00000000", "A");
        Assert.NotNull(updatedChain);
        Assert.Equal("NEW456", updatedChain);
    }

    /// <summary>
    /// Test para verificar que UpdateChainAsync crea nueva cadena cuando no existe
    /// </summary>
    [Fact]
    public async Task UpdateChainAsync_ShouldCreateNewChain_WhenChainDoesNotExist()
    {
        // Arrange
        using var context = new SqliteDbContext(_options);
        var repository = new SqliteHashChainRepository(context, _loggerMock.Object);

        // Act
        await repository.UpdateChainAsync("B00000000", "B", "PREV123", "NEW456");

        // Assert
        var result = await repository.GetPreviousHashAsync("B00000000", "B");
        Assert.NotNull(result);
        Assert.Equal("NEW456", result);
    }

    /// <summary>
    /// Test para verificar que GetChainBySellerAsync retorna todas las cadenas del vendedor
    /// </summary>
    [Fact]
    public async Task GetChainBySellerAsync_ShouldReturnAllChainsForSeller()
    {
        // Arrange
        using var context = new SqliteDbContext(_options);
        var repository = new SqliteHashChainRepository(context, _loggerMock.Object);

        var chain1 = new HashChainEntity
        {
            SellerNif = "B00000000",
            Series = "A",
            CurrentHash = "ABC123",
            PreviousHash = "XYZ789",
            LastUpdated = DateTime.UtcNow
        };

        var chain2 = new HashChainEntity
        {
            SellerNif = "B00000000",
            Series = "B",
            CurrentHash = "DEF456",
            PreviousHash = "UVW012",
            LastUpdated = DateTime.UtcNow
        };

        var chain3 = new HashChainEntity
        {
            SellerNif = "B00000001",
            Series = "A",
            CurrentHash = "GHI789",
            PreviousHash = "RST345",
            LastUpdated = DateTime.UtcNow
        };

        await repository.AddAsync(chain1);
        await repository.AddAsync(chain2);
        await repository.AddAsync(chain3);

        // Act
        var result = await repository.GetChainBySellerAsync("B00000000");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, chain => Assert.Equal("B00000000", chain.SellerNif));
    }

    /// <summary>
    /// Test para verificar que GetChainBySellerAsync retorna lista vacía cuando no hay cadenas
    /// </summary>
    [Fact]
    public async Task GetChainBySellerAsync_ShouldReturnEmptyList_WhenNoChainsExist()
    {
        // Arrange
        using var context = new SqliteDbContext(_options);
        var repository = new SqliteHashChainRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetChainBySellerAsync("NONEXISTENT");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    /// <summary>
    /// Test para verificar que UpdateChainAsync maneja transacciones correctamente
    /// </summary>
    [Fact]
    public async Task UpdateChainAsync_ShouldHandleTransactionsCorrectly()
    {
        // Arrange
        using var context = new SqliteDbContext(_options);
        var repository = new SqliteHashChainRepository(context, _loggerMock.Object);

        // Act & Assert - No debería lanzar excepción
        await repository.UpdateChainAsync("B00000000", "A", null, "FIRST_HASH");
        
        var firstHash = await repository.GetPreviousHashAsync("B00000000", "A");
        Assert.Equal("FIRST_HASH", firstHash);

        await repository.UpdateChainAsync("B00000000", "A", "FIRST_HASH", "SECOND_HASH");
        
        var secondHash = await repository.GetPreviousHashAsync("B00000000", "A");
        Assert.Equal("SECOND_HASH", secondHash);
    }

    /// <summary>
    /// Test para verificar que las cadenas se actualizan con timestamps correctos
    /// </summary>
    [Fact]
    public async Task UpdateChainAsync_ShouldUpdateTimestamps()
    {
        // Arrange
        using var context = new SqliteDbContext(_options);
        var repository = new SqliteHashChainRepository(context, _loggerMock.Object);

        var originalTime = DateTime.UtcNow.AddHours(-1);
        
        var hashChain = new HashChainEntity
        {
            SellerNif = "B00000000",
            Series = "A",
            CurrentHash = "ABC123",
            PreviousHash = "XYZ789",
            LastUpdated = originalTime
        };

        await repository.AddAsync(hashChain);

        // Act
        await repository.UpdateChainAsync("B00000000", "A", "ABC123", "NEW456");

        // Assert
        var updatedChain = await repository.GetByIdAsync(hashChain.Id);
        Assert.NotNull(updatedChain);
        Assert.True(updatedChain.LastUpdated > originalTime);
        Assert.True(updatedChain.UpdatedAt > originalTime);
    }

    public void Dispose()
    {
        // Cleanup se maneja automáticamente con using statements
    }
}
