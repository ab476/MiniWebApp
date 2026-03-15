using System.Collections.Concurrent;

namespace MiniWebApp.Core.Services;

/// <inheritdoc cref="IScopedStateService"/>
public class ScopedStateService : IScopedStateService
{
    private readonly ConcurrentDictionary<string, object?> _store = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Generates a unique key based on the type hierarchy.
    /// <br/>
    /// Priority: <c>AssemblyQualifiedName</c> → <c>FullName</c> → <c>Name</c>
    /// </summary>
    private static string GetKey<T>() =>
        typeof(T).AssemblyQualifiedName ?? typeof(T).FullName ?? typeof(T).Name;

    /// <inheritdoc />
    public void Set<T>(string key, T value) => _store[key] = value;

    /// <inheritdoc />
    public void Set<T>(T value) => Set(GetKey<T>(), value);

    /// <inheritdoc />
    public T? Get<T>(string key) => _store.TryGetValue(key, out var value) ? (T?)value : default;

    /// <inheritdoc />
    public T? Get<T>() => Get<T>(GetKey<T>());

    /// <inheritdoc />
    public bool TryGet<T>(string key, out T? value)
    {
        if (_store.TryGetValue(key, out var storedValue))
        {
            try
            {
                value = (T?)storedValue;
                return true;
            }
            catch (InvalidCastException) { /* Handle type mismatch if needed */ }
        }
        value = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryGet<T>(out T? value) => TryGet(GetKey<T>(), out value);

    /// <inheritdoc />
    public bool Remove<T>() => _store.TryRemove(GetKey<T>(), out _);

    /// <inheritdoc />
    public bool Remove(string key) => _store.TryRemove(key, out _);

    /// <inheritdoc />
    public void Clear() => _store.Clear();
}