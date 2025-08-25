using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NioxVF.Persistence.Context;
using NioxVF.Persistence.Entities.Base;
using NioxVF.Persistence.Interfaces;
using NioxVF.Persistence.Sqlite.Context;
using System.Linq.Expressions;

namespace NioxVF.Persistence.Sqlite.Repositories.Base;

/// <summary>
/// Repositorio base genérico para SQLite
/// </summary>
/// <typeparam name="T">Tipo de entidad</typeparam>
public abstract class SqliteRepository<T> : IRepository<T> where T : AuditEntity
{
    protected readonly SqliteDbContext _context;
    protected readonly ILogger<SqliteRepository<T>> _logger;
    protected readonly DbSet<T> _dbSet;

    /// <summary>
    /// Constructor del repositorio
    /// </summary>
    /// <param name="context">Contexto de base de datos</param>
    /// <param name="logger">Logger</param>
    protected SqliteRepository(SqliteDbContext context, ILogger<SqliteRepository<T>> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbSet = context.Set<T>();
    }

    /// <summary>
    /// Obtiene una entidad por su ID
    /// </summary>
    /// <param name="id">ID de la entidad</param>
    /// <returns>Entidad encontrada o null</returns>
    public virtual async Task<T?> GetByIdAsync(int id)
    {
        try
        {
            _logger.LogDebug("Getting entity of type {EntityType} with ID {Id}", typeof(T).Name, id);
            return await _dbSet.FindAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entity of type {EntityType} with ID {Id}", typeof(T).Name, id);
            throw;
        }
    }

    /// <summary>
    /// Obtiene todas las entidades
    /// </summary>
    /// <returns>Colección de entidades</returns>
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        try
        {
            _logger.LogDebug("Getting all entities of type {EntityType}", typeof(T).Name);
            return await _dbSet.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all entities of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <summary>
    /// Agrega una nueva entidad
    /// </summary>
    /// <param name="entity">Entidad a agregar</param>
    /// <returns>Entidad agregada</returns>
    public virtual async Task<T> AddAsync(T entity)
    {
        try
        {
            _logger.LogDebug("Adding entity of type {EntityType}", typeof(T).Name);
            
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            
            var result = await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Successfully added entity of type {EntityType} with ID {Id}", 
                typeof(T).Name, entity.Id);
            
            return result.Entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding entity of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <summary>
    /// Actualiza una entidad existente
    /// </summary>
    /// <param name="entity">Entidad a actualizar</param>
    public virtual async Task UpdateAsync(T entity)
    {
        try
        {
            _logger.LogDebug("Updating entity of type {EntityType} with ID {Id}", typeof(T).Name, entity.Id);
            
            entity.UpdatedAt = DateTime.UtcNow;
            
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Successfully updated entity of type {EntityType} with ID {Id}", 
                typeof(T).Name, entity.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating entity of type {EntityType} with ID {Id}", typeof(T).Name, entity.Id);
            throw;
        }
    }

    /// <summary>
    /// Elimina una entidad por su ID (soft delete)
    /// </summary>
    /// <param name="id">ID de la entidad a eliminar</param>
    public virtual async Task DeleteAsync(int id)
    {
        try
        {
            _logger.LogDebug("Deleting entity of type {EntityType} with ID {Id}", typeof(T).Name, id);
            
            var entity = await GetByIdAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("Entity of type {EntityType} with ID {Id} not found for deletion", typeof(T).Name, id);
                return;
            }

            // Soft delete
            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Successfully deleted entity of type {EntityType} with ID {Id}", 
                typeof(T).Name, id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting entity of type {EntityType} with ID {Id}", typeof(T).Name, id);
            throw;
        }
    }

    /// <summary>
    /// Verifica si una entidad existe por su ID
    /// </summary>
    /// <param name="id">ID de la entidad</param>
    /// <returns>True si existe, false en caso contrario</returns>
    public virtual async Task<bool> ExistsAsync(int id)
    {
        try
        {
            _logger.LogDebug("Checking if entity of type {EntityType} with ID {Id} exists", typeof(T).Name, id);
            return await _dbSet.AnyAsync(e => e.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if entity of type {EntityType} with ID {Id} exists", typeof(T).Name, id);
            throw;
        }
    }

    /// <summary>
    /// Busca entidades que cumplan con un predicado
    /// </summary>
    /// <param name="predicate">Predicado de búsqueda</param>
    /// <returns>Colección de entidades que cumplen el predicado</returns>
    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        try
        {
            _logger.LogDebug("Finding entities of type {EntityType} with predicate", typeof(T).Name);
            return await _dbSet.Where(predicate).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding entities of type {EntityType} with predicate", typeof(T).Name);
            throw;
        }
    }

    /// <summary>
    /// Obtiene la primera entidad que cumpla con un predicado
    /// </summary>
    /// <param name="predicate">Predicado de búsqueda</param>
    /// <returns>Primera entidad que cumple el predicado o null</returns>
    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        try
        {
            _logger.LogDebug("Finding first entity of type {EntityType} with predicate", typeof(T).Name);
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding first entity of type {EntityType} with predicate", typeof(T).Name);
            throw;
        }
    }
}
