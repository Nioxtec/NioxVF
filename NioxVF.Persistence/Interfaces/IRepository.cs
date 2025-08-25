using System.Linq.Expressions;

namespace NioxVF.Persistence.Interfaces;

/// <summary>
/// Interfaz base para repositorios genéricos que proporciona operaciones CRUD básicas
/// </summary>
/// <typeparam name="T">Tipo de entidad que maneja el repositorio</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Obtiene una entidad por su identificador
    /// </summary>
    /// <param name="id">Identificador de la entidad</param>
    /// <returns>La entidad encontrada o null si no existe</returns>
    Task<T?> GetByIdAsync(int id);

    /// <summary>
    /// Obtiene todas las entidades del tipo especificado
    /// </summary>
    /// <returns>Colección de todas las entidades</returns>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Agrega una nueva entidad al repositorio
    /// </summary>
    /// <param name="entity">Entidad a agregar</param>
    /// <returns>La entidad agregada con su identificador asignado</returns>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// Actualiza una entidad existente
    /// </summary>
    /// <param name="entity">Entidad a actualizar</param>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Elimina una entidad por su identificador
    /// </summary>
    /// <param name="id">Identificador de la entidad a eliminar</param>
    Task DeleteAsync(int id);

    /// <summary>
    /// Verifica si existe una entidad con el identificador especificado
    /// </summary>
    /// <param name="id">Identificador a verificar</param>
    /// <returns>True si existe, false en caso contrario</returns>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Busca entidades que cumplan con el predicado especificado
    /// </summary>
    /// <param name="predicate">Expresión que define las condiciones de búsqueda</param>
    /// <returns>Colección de entidades que cumplen con el predicado</returns>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Obtiene la primera entidad que cumpla con el predicado especificado
    /// </summary>
    /// <param name="predicate">Expresión que define las condiciones de búsqueda</param>
    /// <returns>La primera entidad encontrada o null si no existe</returns>
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
}
