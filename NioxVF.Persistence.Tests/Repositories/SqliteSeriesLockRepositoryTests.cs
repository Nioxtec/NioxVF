using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NioxVF.Persistence.Entities;
using NioxVF.Persistence.Sqlite.Context;
using NioxVF.Persistence.Sqlite.Repositories;
using Xunit;

namespace NioxVF.Persistence.Tests.Repositories;

/// <summary>
/// Tests unitarios para SqliteSeriesLockRepository
/// </summary>
public class SqliteSeriesLockRepositoryTests : IDisposable
{
    private readonly DbContextOptions<SqliteDbContext> _options;
    private readonly Mock<ILogger<SqliteSeriesLockRepository>> _loggerMock;

    public SqliteSeriesLockRepositoryTests()
    {
        // Configurar base de datos en memoria para tests
        _options = new DbContextOptionsBuilder<SqliteDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _loggerMock = new Mock<ILogger<SqliteSeriesLockRepository>>();
    }

    /// <summary>
    /// Test para verificar que TryAcquireLockAsync adquiere el lock correctamente
    /// </summary>
    [Fact]
    public async Task TryAcquireLockAsync_ShouldAcquireLock_WhenNotLocked()
    {
        // Arrange
        using var context = new SqliteDbContext(_options);
        var repository = new SqliteSeriesLockRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.TryAcquireLockAsync("B00000000", "A", TimeSpan.FromMinutes(5));

        // Assert
        Assert.True(result);
        
        var isLocked = await repository.IsLockedAsync("B00000000", "A");
        Assert.True(isLocked);
    }

    /// <summary>
    /// Test para verificar que TryAcquireLockAsync falla cuando ya está bloqueado
    /// </summary>
    [Fact]
    public async Task TryAcquireLockAsync_ShouldFail_WhenAlreadyLocked()
    {
        // Arrange
        using var context = new SqliteDbContext(_options);
        var repository = new SqliteSeriesLockRepository(context, _loggerMock.Object);

        // Adquirir primer lock
        var firstResult = await repository.TryAcquireLockAsync("B00000000", "A", TimeSpan.FromMinutes(5));
        Assert.True(firstResult);

        // Act - Intentar adquirir segundo lock
        var secondResult = await repository.TryAcquireLockAsync("B00000000", "A", TimeSpan.FromMinutes(5));

        // Assert
        Assert.False(secondResult);
    }

    /// <summary>
    /// Test para verificar que ReleaseLockAsync libera el lock correctamente
    /// </summary>
    [Fact]
    public async Task ReleaseLockAsync_ShouldReleaseLock()
    {
        // Arrange
        using var context = new SqliteDbContext(_options);
        var repository = new SqliteSeriesLockRepository(context, _loggerMock.Object);

        // Adquirir lock
        var acquired = await repository.TryAcquireLockAsync("B00000000", "A", TimeSpan.FromMinutes(5));
        Assert.True(acquired);

        // Verificar que está bloqueado
        var isLocked = await repository.IsLockedAsync("B00000000", "A");
        Assert.True(isLocked);

        // Act
        await repository.ReleaseLockAsync("B00000000", "A");

        // Assert
        var isStillLocked = await repository.IsLockedAsync("B00000000", "A");
        Assert.False(isStillLocked);
    }

    /// <summary>
    /// Test para verificar que IsLockedAsync retorna false cuando no hay lock
    /// </summary>
    [Fact]
    public async Task IsLockedAsync_ShouldReturnFalse_WhenNoLockExists()
    {
        // Arrange
        using var context = new SqliteDbContext(_options);
        var repository = new SqliteSeriesLockRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.IsLockedAsync("B00000000", "A");

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Test para verificar que IsLockedAsync retorna true cuando hay lock activo
    /// </summary>
    [Fact]
    public async Task IsLockedAsync_ShouldReturnTrue_WhenLockExists()
    {
        // Arrange
        using var context = new SqliteDbContext(_options);
        var repository = new SqliteSeriesLockRepository(context, _loggerMock.Object);

        // Adquirir lock
        await repository.TryAcquireLockAsync("B00000000", "A", TimeSpan.FromMinutes(5));

        // Act
        var result = await repository.IsLockedAsync("B00000000", "A");

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Test para verificar que CleanupExpiredLocksAsync limpia locks expirados
    /// </summary>
    [Fact]
    public async Task CleanupExpiredLocksAsync_ShouldCleanupExpiredLocks()
    {
        // Arrange
        using var context = new SqliteDbContext(_options);
        var repository = new SqliteSeriesLockRepository(context, _loggerMock.Object);

        // Crear lock expirado manualmente
        var expiredLock = new SeriesLockEntity
        {
            SellerNif = "B00000000",
            Series = "A",
            LockId = "expired-lock",
            AcquiredAt = DateTime.UtcNow.AddHours(-2),
            ExpiresAt = DateTime.UtcNow.AddHours(-1), // Expirado hace 1 hora
            IsActive = true
        };

        await repository.AddAsync(expiredLock);

        // Crear lock válido
        var validLock = new SeriesLockEntity
        {
            SellerNif = "B00000001",
            Series = "B",
            LockId = "valid-lock",
            AcquiredAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(1), // Válido por 1 hora más
            IsActive = true
        };

        await repository.AddAsync(validLock);

        // Verificar que ambos locks existen inicialmente
        // Nota: IsLockedAsync automáticamente limpia locks expirados, por lo que el lock expirado ya no estará activo
        var isExpiredLocked = await repository.IsLockedAsync("B00000000", "A");
        var isValidLocked = await repository.IsLockedAsync("B00000001", "B");
        Assert.False(isExpiredLocked); // Ya debería estar limpio
        Assert.True(isValidLocked);

        // Act
        await repository.CleanupExpiredLocksAsync();

        // Assert
        var isStillExpiredLocked = await repository.IsLockedAsync("B00000000", "A");
        var isStillValidLocked = await repository.IsLockedAsync("B00000001", "B");
        
        Assert.False(isStillExpiredLocked); // Debería estar limpio
        Assert.True(isStillValidLocked);    // Debería seguir activo
    }

    /// <summary>
    /// Test para verificar que los locks tienen timeout configurable
    /// </summary>
    [Fact]
    public async Task TryAcquireLockAsync_ShouldRespectTimeout()
    {
        // Arrange
        using var context = new SqliteDbContext(_options);
        var repository = new SqliteSeriesLockRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.TryAcquireLockAsync("B00000000", "A", TimeSpan.FromSeconds(1));

        // Assert
        Assert.True(result);
        
        // Verificar que el lock expira después del timeout
        await Task.Delay(1100); // Esperar 1.1 segundos
        
        var isStillLocked = await repository.IsLockedAsync("B00000000", "A");
        Assert.False(isStillLocked);
    }

    /// <summary>
    /// Test para verificar que se pueden adquirir locks para diferentes series del mismo vendedor
    /// </summary>
    [Fact]
    public async Task TryAcquireLockAsync_ShouldAllowDifferentSeriesForSameSeller()
    {
        // Arrange
        using var context = new SqliteDbContext(_options);
        var repository = new SqliteSeriesLockRepository(context, _loggerMock.Object);

        // Act
        var lockA = await repository.TryAcquireLockAsync("B00000000", "A", TimeSpan.FromMinutes(5));
        var lockB = await repository.TryAcquireLockAsync("B00000000", "B", TimeSpan.FromMinutes(5));

        // Assert
        Assert.True(lockA);
        Assert.True(lockB);
        
        var isLockedA = await repository.IsLockedAsync("B00000000", "A");
        var isLockedB = await repository.IsLockedAsync("B00000000", "B");
        
        Assert.True(isLockedA);
        Assert.True(isLockedB);
    }

    /// <summary>
    /// Test para verificar que se pueden adquirir locks para diferentes vendedores
    /// </summary>
    [Fact]
    public async Task TryAcquireLockAsync_ShouldAllowDifferentSellers()
    {
        // Arrange
        using var context = new SqliteDbContext(_options);
        var repository = new SqliteSeriesLockRepository(context, _loggerMock.Object);

        // Act
        var lock1 = await repository.TryAcquireLockAsync("B00000000", "A", TimeSpan.FromMinutes(5));
        var lock2 = await repository.TryAcquireLockAsync("B00000001", "A", TimeSpan.FromMinutes(5));

        // Assert
        Assert.True(lock1);
        Assert.True(lock2);
        
        var isLocked1 = await repository.IsLockedAsync("B00000000", "A");
        var isLocked2 = await repository.IsLockedAsync("B00000001", "A");
        
        Assert.True(isLocked1);
        Assert.True(isLocked2);
    }

    /// <summary>
    /// Test para verificar que ReleaseLockAsync no falla cuando no hay lock
    /// </summary>
    [Fact]
    public async Task ReleaseLockAsync_ShouldNotFail_WhenNoLockExists()
    {
        // Arrange
        using var context = new SqliteDbContext(_options);
        var repository = new SqliteSeriesLockRepository(context, _loggerMock.Object);

        // Act & Assert - No debería lanzar excepción
        await repository.ReleaseLockAsync("B00000000", "A");
        
        var isLocked = await repository.IsLockedAsync("B00000000", "A");
        Assert.False(isLocked);
    }

    public void Dispose()
    {
        // Cleanup se maneja automáticamente con using statements
    }
}
