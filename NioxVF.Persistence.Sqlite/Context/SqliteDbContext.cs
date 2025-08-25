using Microsoft.EntityFrameworkCore;
using NioxVF.Persistence.Context;

namespace NioxVF.Persistence.Sqlite.Context;

/// <summary>
/// Contexto de Entity Framework Core para SQLite
/// </summary>
public class SqliteDbContext : NioxVFDbContext
{
    /// <summary>
    /// Constructor para inyección de dependencias
    /// </summary>
    /// <param name="options">Opciones de configuración del contexto</param>
    public SqliteDbContext(DbContextOptions<SqliteDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Constructor para configuración manual
    /// </summary>
    public SqliteDbContext()
    {
    }

    /// <summary>
    /// Configuración específica para SQLite
    /// </summary>
    /// <param name="optionsBuilder">Constructor de opciones</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        // Configurar SQLite si no está configurado
        if (!optionsBuilder.IsConfigured)
        {
            // Por defecto, usar base de datos en memoria para desarrollo
            optionsBuilder.UseSqlite("Data Source=:memory:");
        }
    }

    /// <summary>
    /// Configuración del modelo específica para SQLite
    /// </summary>
    /// <param name="modelBuilder">Constructor del modelo</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuraciones específicas para SQLite
        // SQLite no soporta algunos tipos de datos de SQL Server, por lo que
        // Entity Framework Core los mapea automáticamente
    }
}
