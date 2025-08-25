using Microsoft.EntityFrameworkCore;
using NioxVF.Persistence.Entities;
using NioxVF.Persistence.Entities.Base;

namespace NioxVF.Persistence.Context;

/// <summary>
/// Contexto base de Entity Framework Core para NioxVF
/// </summary>
public abstract class NioxVFDbContext : DbContext
{
    /// <summary>
    /// Constructor base para el contexto
    /// </summary>
    /// <param name="options">Opciones de configuración del contexto</param>
    protected NioxVFDbContext(DbContextOptions options) : base(options)
    {
    }

    /// <summary>
    /// Constructor para configuración manual
    /// </summary>
    protected NioxVFDbContext()
    {
    }

    /// <summary>
    /// DbSet para entidades de facturas
    /// </summary>
    public DbSet<InvoiceEntity> Invoices { get; set; } = null!;

    /// <summary>
    /// DbSet para entidades de impuestos
    /// </summary>
    public DbSet<TaxItemEntity> TaxItems { get; set; } = null!;

    /// <summary>
    /// DbSet para entidades de cadena de hashes
    /// </summary>
    public DbSet<HashChainEntity> HashChains { get; set; } = null!;

    /// <summary>
    /// DbSet para entidades de locks de series
    /// </summary>
    public DbSet<SeriesLockEntity> SeriesLocks { get; set; } = null!;

    /// <summary>
    /// Configuración del modelo de datos
    /// </summary>
    /// <param name="modelBuilder">Constructor del modelo</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurar entidades
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NioxVFDbContext).Assembly);

        // Configuración global para AuditEntity
        modelBuilder.Entity<AuditEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
        });
    }

    /// <summary>
    /// Configuración del contexto de base de datos
    /// </summary>
    /// <param name="optionsBuilder">Constructor de opciones</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        // Habilitar logging detallado en desarrollo
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
        }
    }

    /// <summary>
    /// Guarda cambios y actualiza timestamps de auditoría
    /// </summary>
    /// <returns>Número de entidades afectadas</returns>
    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    /// <summary>
    /// Guarda cambios de forma asíncrona y actualiza timestamps de auditoría
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Número de entidades afectadas</returns>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Actualiza los campos de auditoría automáticamente
    /// </summary>
    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries<AuditEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
