using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NioxVF.Persistence.Entities;
using NioxVF.Persistence.Sqlite.Context;
using NioxVF.Persistence.Sqlite.Repositories;
using Xunit;

namespace NioxVF.Persistence.Tests.Repositories;

/// <summary>
/// Tests unitarios para SqliteInvoiceRepository
/// </summary>
public class SqliteInvoiceRepositoryTests : IDisposable
{
    private readonly DbContextOptions<SqliteDbContext> _options;
    private readonly Mock<ILogger<SqliteInvoiceRepository>> _loggerMock;

    public SqliteInvoiceRepositoryTests()
    {
        // Configurar base de datos en memoria para tests
        _options = new DbContextOptionsBuilder<SqliteDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _loggerMock = new Mock<ILogger<SqliteInvoiceRepository>>();
    }

    /// <summary>
    /// Test para verificar que GetBySeriesAndNumberAsync retorna la factura correcta
    /// </summary>
    [Fact]
    public async Task GetBySeriesAndNumberAsync_ShouldReturnInvoice()
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

        await repository.AddAsync(invoice);

        // Act
        var result = await repository.GetBySeriesAndNumberAsync("B00000000", "A", "00001");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("B00000000", result.SellerNif);
        Assert.Equal("A", result.Series);
        Assert.Equal("00001", result.Number);
    }

    /// <summary>
    /// Test para verificar que GetBySeriesAndNumberAsync retorna null cuando no existe
    /// </summary>
    [Fact]
    public async Task GetBySeriesAndNumberAsync_ShouldReturnNull_WhenInvoiceDoesNotExist()
    {
        // Arrange
        using var context = new SqliteDbContext(_options);
        var repository = new SqliteInvoiceRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetBySeriesAndNumberAsync("B00000000", "A", "99999");

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Test para verificar que GetBySellerAsync retorna todas las facturas del vendedor
    /// </summary>
    [Fact]
    public async Task GetBySellerAsync_ShouldReturnAllInvoicesForSeller()
    {
        // Arrange
        using var context = new SqliteDbContext(_options);
        var repository = new SqliteInvoiceRepository(context, _loggerMock.Object);

        var invoice1 = new InvoiceEntity
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

        var invoice2 = new InvoiceEntity
        {
            SellerNif = "B00000000",
            SellerName = "Test Company",
            Series = "A",
            Number = "00002",
            IssueDate = DateTime.UtcNow,
            TotalTaxBase = 200.00m,
            TotalTaxAmount = 42.00m,
            TotalSurcharge = 0.00m,
            Total = 242.00m
        };

        var invoice3 = new InvoiceEntity
        {
            SellerNif = "B00000001",
            SellerName = "Other Company",
            Series = "B",
            Number = "00001",
            IssueDate = DateTime.UtcNow,
            TotalTaxBase = 300.00m,
            TotalTaxAmount = 63.00m,
            TotalSurcharge = 0.00m,
            Total = 363.00m
        };

        await repository.AddAsync(invoice1);
        await repository.AddAsync(invoice2);
        await repository.AddAsync(invoice3);

        // Act
        var result = await repository.GetBySellerAsync("B00000000");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, invoice => Assert.Equal("B00000000", invoice.SellerNif));
    }

    /// <summary>
    /// Test para verificar que GetByDateRangeAsync retorna facturas en el rango de fechas
    /// </summary>
    [Fact]
    public async Task GetByDateRangeAsync_ShouldReturnInvoicesInDateRange()
    {
        // Arrange
        using var context = new SqliteDbContext(_options);
        var repository = new SqliteInvoiceRepository(context, _loggerMock.Object);

        var startDate = DateTime.UtcNow.Date;
        var endDate = startDate.AddDays(7);

        var invoice1 = new InvoiceEntity
        {
            SellerNif = "B00000000",
            SellerName = "Test Company",
            Series = "A",
            Number = "00001",
            IssueDate = startDate.AddDays(1),
            TotalTaxBase = 100.00m,
            TotalTaxAmount = 21.00m,
            TotalSurcharge = 0.00m,
            Total = 121.00m
        };

        var invoice2 = new InvoiceEntity
        {
            SellerNif = "B00000000",
            SellerName = "Test Company",
            Series = "A",
            Number = "00002",
            IssueDate = startDate.AddDays(5),
            TotalTaxBase = 200.00m,
            TotalTaxAmount = 42.00m,
            TotalSurcharge = 0.00m,
            Total = 242.00m
        };

        var invoice3 = new InvoiceEntity
        {
            SellerNif = "B00000000",
            SellerName = "Test Company",
            Series = "A",
            Number = "00003",
            IssueDate = startDate.AddDays(10), // Fuera del rango
            TotalTaxBase = 300.00m,
            TotalTaxAmount = 63.00m,
            TotalSurcharge = 0.00m,
            Total = 363.00m
        };

        await repository.AddAsync(invoice1);
        await repository.AddAsync(invoice2);
        await repository.AddAsync(invoice3);

        // Act
        var result = await repository.GetByDateRangeAsync(startDate, endDate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, invoice => 
        {
            Assert.True(invoice.IssueDate >= startDate);
            Assert.True(invoice.IssueDate <= endDate);
        });
    }

    /// <summary>
    /// Test para verificar que GetLastNumberInSeriesAsync retorna el último número de la serie
    /// </summary>
    [Fact]
    public async Task GetLastNumberInSeriesAsync_ShouldReturnLastNumberInSeries()
    {
        // Arrange
        using var context = new SqliteDbContext(_options);
        var repository = new SqliteInvoiceRepository(context, _loggerMock.Object);

        var invoice1 = new InvoiceEntity
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

        var invoice2 = new InvoiceEntity
        {
            SellerNif = "B00000000",
            SellerName = "Test Company",
            Series = "A",
            Number = "00005",
            IssueDate = DateTime.UtcNow,
            TotalTaxBase = 200.00m,
            TotalTaxAmount = 42.00m,
            TotalSurcharge = 0.00m,
            Total = 242.00m
        };

        var invoice3 = new InvoiceEntity
        {
            SellerNif = "B00000000",
            SellerName = "Test Company",
            Series = "A",
            Number = "00003",
            IssueDate = DateTime.UtcNow,
            TotalTaxBase = 300.00m,
            TotalTaxAmount = 63.00m,
            TotalSurcharge = 0.00m,
            Total = 363.00m
        };

        await repository.AddAsync(invoice1);
        await repository.AddAsync(invoice2);
        await repository.AddAsync(invoice3);

        // Act
        var result = await repository.GetLastNumberInSeriesAsync("B00000000", "A");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("00005", result);
    }

    /// <summary>
    /// Test para verificar que GetLastNumberInSeriesAsync retorna null cuando no hay facturas en la serie
    /// </summary>
    [Fact]
    public async Task GetLastNumberInSeriesAsync_ShouldReturnNull_WhenNoInvoicesInSeries()
    {
        // Arrange
        using var context = new SqliteDbContext(_options);
        var repository = new SqliteInvoiceRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetLastNumberInSeriesAsync("B00000000", "Z");

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Test para verificar que las facturas se cargan con sus TaxItems
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ShouldIncludeTaxItems()
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
            Total = 121.00m,
            Taxes = new List<TaxItemEntity>
            {
                new TaxItemEntity
                {
                    TaxBase = 100.00m,
                    TaxRate = 21.00m,
                    TaxAmount = 21.00m,
                    SurchargeRate = null,
                    SurchargeAmount = null
                }
            }
        };

        var addedInvoice = await repository.AddAsync(invoice);

        // Act
        var result = await repository.GetByIdAsync(addedInvoice.Id);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Taxes);
        Assert.Single(result.Taxes);
        Assert.Equal(100.00m, result.Taxes.First().TaxBase);
        Assert.Equal(21.00m, result.Taxes.First().TaxRate);
    }

    public void Dispose()
    {
        // Cleanup se maneja automáticamente con using statements
    }
}
