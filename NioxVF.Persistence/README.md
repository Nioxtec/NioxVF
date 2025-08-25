# NioxVF.Persistence

## 📋 Descripción

El proyecto `NioxVF.Persistence` contiene la capa de persistencia del sistema NioxVF, proporcionando interfaces, entidades y configuraciones base para el acceso a datos.

## 🏗️ Arquitectura

### Estructura del Proyecto

```
NioxVF.Persistence/
├── Interfaces/           # Interfaces de repositorio
├── Entities/            # Entidades de dominio para persistencia
│   └── Base/           # Entidades base
├── Context/            # Contextos de Entity Framework
└── Services/           # Servicios de persistencia
```

## 🔧 Interfaces

### IRepository<T>

Interfaz base genérica que define las operaciones CRUD básicas:

```csharp
public interface IRepository<T> where T : AuditEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
}
```

### IInvoiceRepository

Interfaz específica para la gestión de facturas:

```csharp
public interface IInvoiceRepository : IRepository<InvoiceEntity>
{
    Task<InvoiceEntity?> GetBySeriesAndNumberAsync(string sellerNif, string series, string number);
    Task<IEnumerable<InvoiceEntity>> GetBySellerAsync(string sellerNif);
    Task<IEnumerable<InvoiceEntity>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<string?> GetLastNumberInSeriesAsync(string sellerNif, string series);
}
```

### IHashChainRepository

Interfaz para la gestión de la cadena criptográfica de hashes:

```csharp
public interface IHashChainRepository : IRepository<HashChainEntity>
{
    Task<string?> GetPreviousHashAsync(string sellerNif, string series);
    Task UpdateChainAsync(string sellerNif, string series, string? prevHash, string currentHash);
    Task<IEnumerable<HashChainEntity>> GetChainBySellerAsync(string sellerNif);
}
```

### ISeriesLockRepository

Interfaz para el control de concurrencia por series:

```csharp
public interface ISeriesLockRepository : IRepository<SeriesLockEntity>
{
    Task<bool> TryAcquireLockAsync(string sellerNif, string series, TimeSpan timeout);
    Task ReleaseLockAsync(string sellerNif, string series);
    Task<bool> IsLockedAsync(string sellerNif, string series);
    Task CleanupExpiredLocksAsync();
}
```

## 🏛️ Entidades

### AuditEntity

Entidad base que proporciona campos de auditoría automática:

```csharp
public abstract class AuditEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}
```

### InvoiceEntity

Entidad para la persistencia de facturas:

```csharp
public class InvoiceEntity : AuditEntity
{
    public string SellerNif { get; set; } = string.Empty;
    public string SellerName { get; set; } = string.Empty;
    public string Series { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public string? Text { get; set; }
    public string Type { get; set; } = "F1";
    public string? PrevHash { get; set; }
    public string? Hash { get; set; }
    public decimal TotalTaxBase { get; set; }
    public decimal TotalTaxAmount { get; set; }
    public decimal TotalSurcharge { get; set; }
    public decimal Total { get; set; }
    
    // Navegación
    public List<TaxItemEntity> Taxes { get; set; } = new();
}
```

### TaxItemEntity

Entidad para los impuestos de las facturas:

```csharp
public class TaxItemEntity : AuditEntity
{
    public int InvoiceId { get; set; }
    public decimal TaxBase { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal? SurchargeRate { get; set; }
    public decimal? SurchargeAmount { get; set; }
    
    // Navegación
    public InvoiceEntity Invoice { get; set; } = null!;
}
```

### HashChainEntity

Entidad para la cadena criptográfica:

```csharp
public class HashChainEntity : AuditEntity
{
    public string SellerNif { get; set; } = string.Empty;
    public string Series { get; set; } = string.Empty;
    public string CurrentHash { get; set; } = string.Empty;
    public string? PreviousHash { get; set; }
    public DateTime LastUpdated { get; set; }
}
```

### SeriesLockEntity

Entidad para el control de concurrencia:

```csharp
public class SeriesLockEntity : AuditEntity
{
    public string SellerNif { get; set; } = string.Empty;
    public string Series { get; set; } = string.Empty;
    public string LockId { get; set; } = string.Empty;
    public DateTime AcquiredAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
}
```

## 🔄 Contexto de Base de Datos

### NioxVFDbContext

Contexto base abstracto que proporciona:

- Configuración automática de auditoría
- Soft delete
- Logging detallado
- Configuración de entidades

```csharp
public abstract class NioxVFDbContext : DbContext
{
    public DbSet<InvoiceEntity> Invoices { get; set; }
    public DbSet<TaxItemEntity> TaxItems { get; set; }
    public DbSet<HashChainEntity> HashChains { get; set; }
    public DbSet<SeriesLockEntity> SeriesLocks { get; set; }
}
```

## 📦 Dependencias

### Paquetes NuGet

- `Microsoft.EntityFrameworkCore` (8.0.0)
- `Microsoft.EntityFrameworkCore.Tools` (8.0.0)
- `Microsoft.EntityFrameworkCore.Design` (8.0.0)

### Referencias de Proyecto

- `NioxVF.Domain` - Modelos de dominio

## 🚀 Uso

### Configuración Básica

```csharp
// Registrar en DI container
services.AddScoped<IInvoiceRepository, SqliteInvoiceRepository>();
services.AddScoped<IHashChainRepository, SqliteHashChainRepository>();
services.AddScoped<ISeriesLockRepository, SqliteSeriesLockRepository>();
```

### Ejemplo de Uso

```csharp
public class InvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository;
    
    public InvoiceService(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }
    
    public async Task<InvoiceEntity> CreateInvoiceAsync(InvoiceEntity invoice)
    {
        // Validar que no existe una factura con la misma serie y número
        var existing = await _invoiceRepository.GetBySeriesAndNumberAsync(
            invoice.SellerNif, invoice.Series, invoice.Number);
            
        if (existing != null)
            throw new InvalidOperationException("Invoice already exists");
            
        return await _invoiceRepository.AddAsync(invoice);
    }
}
```

## 🧪 Testing

### Configuración de Tests

```csharp
// Usar base de datos en memoria para tests
var options = new DbContextOptionsBuilder<SqliteDbContext>()
    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
    .Options;

using var context = new SqliteDbContext(options);
var repository = new SqliteInvoiceRepository(context, logger);
```

### Ejecutar Tests

```bash
dotnet test NioxVF.Persistence.Tests
```

## 📊 Índices de Base de Datos

### Invoices
- `IX_Invoices_SellerNif_Series_Number` (Único)
- `IX_Invoices_SellerNif`
- `IX_Invoices_IssueDate`

### TaxItems
- `IX_TaxItems_InvoiceId`

### HashChains
- `IX_HashChains_SellerNif_Series` (Único)
- `IX_HashChains_SellerNif`
- `IX_HashChains_LastUpdated`

### SeriesLocks
- `IX_SeriesLocks_SellerNif_Series` (Único)
- `IX_SeriesLocks_IsActive`
- `IX_SeriesLocks_ExpiresAt`
- `IX_SeriesLocks_LockId`

## 🔒 Auditoría Automática

El contexto automáticamente:

1. **Establece timestamps** al crear entidades
2. **Actualiza timestamps** al modificar entidades
3. **Proporciona campos** para tracking de usuario

```csharp
// Automáticamente establecido
entity.CreatedAt = DateTime.UtcNow;
entity.UpdatedAt = DateTime.UtcNow;

// Al actualizar
entity.UpdatedAt = DateTime.UtcNow;
```

## 🚨 Consideraciones

### Concurrencia

- Usar `SeriesLockRepository` para control de concurrencia por serie
- Implementar retry logic en operaciones críticas
- Configurar timeouts apropiados para locks

### Performance

- Los índices están optimizados para consultas frecuentes
- Usar `Include()` para cargar relaciones cuando sea necesario
- Considerar paginación para grandes volúmenes de datos

### Seguridad

- Validar inputs antes de persistir
- Usar parámetros para evitar SQL injection
- Implementar logging de auditoría para operaciones críticas

## 📝 Changelog

### v1.0.0 (Diciembre 2024)
- ✅ Interfaces base de repositorio
- ✅ Entidades de dominio completas
- ✅ Contexto de Entity Framework
- ✅ Configuración de auditoría automática
- ✅ Tests unitarios completos

---

**NioxVF.Persistence** - Capa de persistencia para el conector Veri*Factu

