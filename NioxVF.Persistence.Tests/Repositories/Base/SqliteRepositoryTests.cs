using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NioxVF.Persistence.Entities;
using NioxVF.Persistence.Sqlite.Context;
using NioxVF.Persistence.Sqlite.Repositories;
using Xunit;

namespace NioxVF.Persistence.Tests.Repositories.Base;

/// <summary>
/// Tests unitarios para SqliteRepository usando SqliteInvoiceRepository como implementación concreta
/// </summary>
public class SqliteRepositoryTests : IDisposable
{
    private readonly DbContextOptions<SqliteDbContext> _options;
    private readonly Mock<ILogger<SqliteInvoiceRepository>> _loggerMock;

    public SqliteRepositoryTests()
    {
        // Configurar base de datos en memoria para tests
        _options = new DbContextOptionsBuilder<SqliteDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _loggerMock = new Mock<ILogger<SqliteInvoiceRepository>>();
    }

    /// <summary>
    /// Test para verificar que AddAsync agrega una entidad correctamente
    /// </summary>
    [Fact]
    public async Task AddAsync_ShouldAddEntity()
    {
        // Arrange
        using var context = new SqliteDbContext(_options);
        var repository = new SqliteInvoiceRepository(context, _loggerMock.Object);
        var invoice = new InvoiceEntity
        {
            SellerNif = "B00000000",
            SellerName = "Test Company",
            Series = "A",
            Number = "00001",
            IssueDate = DateTime.UtcNow,
            TotalTaxBase = 100.00m,
            TotalTaxAmount = 21.00m,
            TotalSurcharge = 0.00m,
            Total = 121.00m
        };

        // Act
        var result = await repository.AddAsync(invoice);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(0, result.Id);
        Assert.Equal("B00000000", result.SellerNif);
        Assert.Equal("Test Company", result.SellerName);
    }

    /// <summary>
    /// Test para verificar que GetByIdAsync retorna la entidad correcta
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ShouldReturnEntity()
    {
        // Arrange
        using var context = new SqliteDbContext(_options);
        var repository = new SqliteInvoiceRepository(context, _loggerMock.Object);
        var invoice = new InvoiceEntity
        {
            SellerNif = "B00000000",
            SellerName = "Test Company",
            Series = "A",
            Number = "00001",
            IssueDate = DateTime.UtcNow,
            TotalTaxBase = 100.00m,
            TotalTaxAmount = 21.00m,
            TotalSurcharge = 0.00m,
            Total = 121.00m
        };

        var addedInvoice = await repository.AddAsync(invoice);

        // Act
        var result = await repository.GetByIdAsync(addedInvoice.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(addedInvoice.Id, result.Id);
        Assert.Equal("B00000000", result.SellerNif);
    }

    /// <summary>
    /// Test para verificar que GetByIdAsync retorna null cuando no existe
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenEntityDoesNotExist()
    {
        // Arrange
        using var context = new SqliteDbContext(_options);
        var repository = new SqliteInvoiceRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Test para verificar que GetAllAsync retorna todas las entidades
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEntities()
    {
        // Arrange
        using var context = new SqliteDbContext(_options);
        var repository = new SqliteInvoiceRepository(context, _loggerMock.Object);

        var invoice1 = new InvoiceEntity
        {
            SellerNif = "B00000000",
            SellerName = "Test Company 1",
            Series = "A",
            Number = "00001",
            IssueDate = DateTime.UtcNow,
            TotalTaxBase = 100.00m,
            TotalTaxAmount = 21.00m,
            TotalSurcharge = 0.00m,
            Total = 121.00m
        };

        var invoice2 = new InvoiceEntity
        {
            SellerNif = "B00000001",
            SellerName = "Test Company 2",
            Series = "B",
            Number = "00001",
            IssueDate = DateTime.UtcNow,
            TotalTaxBase = 200.00m,
            TotalTaxAmount = 42.00m,
            TotalSurcharge = 0.00m,
            Total = 242.00m
        };

        await repository.AddAsync(invoice1);
        await repository.AddAsync(invoice2);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    /// <summary>
    /// Test para verificar que UpdateAsync actualiza la entidad correctamente
    /// </summary>
    [Fact]
    public async Task UpdateAsync_ShouldUpdateEntity()
    {
        // Arrange
        using var context = new SqliteDbContext(_options);
        var repository = new SqliteInvoiceRepository(context, _loggerMock.Object);
        var invoice = new InvoiceEntity
        {
            SellerNif = "B00000000",
            SellerName = "Test Company",
            Series = "A",
            Number = "00001",
            IssueDate = DateTime.UtcNow,
            TotalTaxBase = 100.00m,
            TotalTaxAmount = 21.00m,
            TotalSurcharge = 0.00m,
            Total = 121.00m
        };

        var addedInvoice = await repository.AddAsync(invoice);
        addedInvoice.SellerName = "Updated Company";

        // Act
        await repository.UpdateAsync(addedInvoice);

        // Assert
        var updatedInvoice = await repository.GetByIdAsync(addedInvoice.Id);
        Assert.NotNull(updatedInvoice);
        Assert.Equal("Updated Company", updatedInvoice.SellerName);
    }

    /// <summary>
    /// Test para verificar que DeleteAsync elimina la entidad correctamente
    /// </summary>
    [Fact]
    public async Task DeleteAsync_ShouldDeleteEntity()
    {
        // Arrange
        using var context = new SqliteDbContext(_options);
        var repository = new SqliteInvoiceRepository(context, _loggerMock.Object);
        var invoice = new InvoiceEntity
        {
            SellerNif = "B00000000",
            SellerName = "Test Company",
            Series = "A",
            Number = "00001",
            IssueDate = DateTime.UtcNow,
            TotalTaxBase = 100.00m,
            TotalTaxAmount = 21.00m,
            TotalSurcharge = 0.00m,
            Total = 121.00m
        };

        var addedInvoice = await repository.AddAsync(invoice);

        // Act
        await repository.DeleteAsync(addedInvoice.Id);

        // Assert
        var deletedInvoice = await repository.GetByIdAsync(addedInvoice.Id);
        Assert.Null(deletedInvoice);
    }

    /// <summary>
    /// Test para verificar que ExistsAsync retorna true cuando la entidad existe
    /// </summary>
    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenEntityExists()
    {
        // Arrange
        using var context = new SqliteDbContext(_options);
        var repository = new SqliteInvoiceRepository(context, _loggerMock.Object);
        var invoice = new InvoiceEntity
        {
            SellerNif = "B00000000",
            SellerName = "Test Company",
            Series = "A",
            Number = "00001",
            IssueDate = DateTime.UtcNow,
            TotalTaxBase = 100.00m,
            TotalTaxAmount = 21.00m,
            TotalSurcharge = 0.00m,
            Total = 121.00m
        };

        var addedInvoice = await repository.AddAsync(invoice);

        // Act
        var exists = await repository.ExistsAsync(addedInvoice.Id);

        // Assert
        Assert.True(exists);
    }

    /// <summary>
    /// Test para verificar que ExistsAsync retorna false cuando la entidad no existe
    /// </summary>
    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenEntityDoesNotExist()
    {
        // Arrange
        using var context = new SqliteDbContext(_options);
        var repository = new SqliteInvoiceRepository(context, _loggerMock.Object);

        // Act
        var exists = await repository.ExistsAsync(999);

        // Assert
        Assert.False(exists);
    }

    /// <summary>
    /// Test para verificar que FindAsync retorna entidades que coinciden con el predicado
    /// </summary>
    [Fact]
    public async Task FindAsync_ShouldReturnMatchingEntities()
    {
        // Arrange
        using var context = new SqliteDbContext(_options);
        var repository = new SqliteInvoiceRepository(context, _loggerMock.Object);

        var invoice1 = new InvoiceEntity
        {
            SellerNif = "B00000000",
            SellerName = "Test Company 1",
            Series = "A",
            Number = "00001",
            IssueDate = DateTime.UtcNow,
            TotalTaxBase = 100.00m,
            TotalTaxAmount = 21.00m,
            TotalSurcharge = 0.00m,
            Total = 121.00m
        };

        var invoice2 = new InvoiceEntity
        {
            SellerNif = "B00000001",
            SellerName = "Test Company 2",
            Series = "B",
            Number = "00001",
            IssueDate = DateTime.UtcNow,
            TotalTaxBase = 200.00m,
            TotalTaxAmount = 42.00m,
            TotalSurcharge = 0.00m,
            Total = 242.00m
        };

        await repository.AddAsync(invoice1);
        await repository.AddAsync(invoice2);

        // Act
        var result = await repository.FindAsync(e => e.SellerNif == "B00000000");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("B00000000", result.First().SellerNif);
    }

    /// <summary>
    /// Test para verificar que FirstOrDefaultAsync retorna la primera entidad que coincide
    /// </summary>
    [Fact]
    public async Task FirstOrDefaultAsync_ShouldReturnFirstMatchingEntity()
    {
        // Arrange
        using var context = new SqliteDbContext(_options);
        var repository = new SqliteInvoiceRepository(context, _loggerMock.Object);

        var invoice1 = new InvoiceEntity
        {
            SellerNif = "B00000000",
            SellerName = "Test Company 1",
            Series = "A",
            Number = "00001",
            IssueDate = DateTime.UtcNow,
            TotalTaxBase = 100.00m,
            TotalTaxAmount = 21.00m,
            TotalSurcharge = 0.00m,
            Total = 121.00m
        };

        var invoice2 = new InvoiceEntity
        {
            SellerNif = "B00000000",
            SellerName = "Test Company 2",
            Series = "B",
            Number = "00001",
            IssueDate = DateTime.UtcNow,
            TotalTaxBase = 200.00m,
            TotalTaxAmount = 42.00m,
            TotalSurcharge = 0.00m,
            Total = 242.00m
        };

        await repository.AddAsync(invoice1);
        await repository.AddAsync(invoice2);

        // Act
        var result = await repository.FirstOrDefaultAsync(e => e.SellerNif == "B00000000");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("B00000000", result.SellerNif);
    }

    /// <summary>
    /// Test para verificar que FirstOrDefaultAsync retorna null cuando no hay coincidencias
    /// </summary>
    [Fact]
    public async Task FirstOrDefaultAsync_ShouldReturnNull_WhenNoMatches()
    {
        // Arrange
        using var context = new SqliteDbContext(_options);
        var repository = new SqliteInvoiceRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.FirstOrDefaultAsync(e => e.SellerNif == "NONEXISTENT");

        // Assert
        Assert.Null(result);
    }

    public void Dispose()
    {
        // Cleanup se maneja automáticamente con using statements
    }
}
