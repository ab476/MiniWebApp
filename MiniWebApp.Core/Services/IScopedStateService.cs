namespace MiniWebApp.Core.Services;

/// <summary>
/// Defines a contract for a scoped state container that stores data by key or by type.
/// <br/>
/// <b>Note:</b> This service is intended to live for the duration of a single user session or request.
/// </summary>
public interface IScopedStateService
{
    /// <summary>
    /// Stores a value associated with a specific string key.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The unique identifier.</param>
    /// <param name="value">The value to store.</param>
    void Set<T>(string key, T value);

    /// <summary>
    /// Stores a value using the type <typeparamref name="T"/> as the key.
    /// <br/>
    /// <example>
    /// <code>state.Set(new UserProfile { Name = "Alice" });</code>
    /// </example>
    /// </summary>
    void Set<T>(T value);

    /// <summary>
    /// Retrieves a value by key. Returns <see langword="default"/> if not found.
    /// </summary>
    T? Get<T>(string key);

    /// <summary>
    /// Retrieves a value using the type <typeparamref name="T"/> as the key.
    /// </summary>
    T? Get<T>();

    /// <summary>
    /// Attempts to retrieve a value by key.
    /// </summary>
    /// <returns><see langword="true"/> if found; otherwise <see langword="false"/>.</returns>
    bool TryGet<T>(string key, out T? value);

    /// <summary>
    /// Attempts to retrieve a value using the type <typeparamref name="T"/> as the key.
    /// </summary>
    bool TryGet<T>(out T? value);

    /// <summary>
    /// Removes an entry by type.
    /// </summary>
    bool Remove<T>();

    /// <summary>
    /// Removes an entry by string key.
    /// </summary>
    bool Remove(string key);

    /// <summary>
    /// Wipes all data from the current scope.
    /// </summary>
    void Clear();
}