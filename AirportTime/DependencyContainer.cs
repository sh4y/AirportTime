using System;
using System.Collections.Generic;

/// <summary>
/// A simple dependency injection container
/// </summary>
public class DependencyContainer
{
    private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();
    
    /// <summary>
    /// Registers an implementation for an interface
    /// </summary>
    /// <typeparam name="TInterface">The interface type</typeparam>
    /// <param name="implementation">The implementation instance</param>
    public void Register<TInterface>(object implementation)
    {
        _services[typeof(TInterface)] = implementation;
    }
    
    /// <summary>
    /// Gets a registered service
    /// </summary>
    /// <typeparam name="T">The service type</typeparam>
    /// <returns>The service instance</returns>
    public T Get<T>()
    {
        if (_services.TryGetValue(typeof(T), out var service))
        {
            return (T)service;
        }
        
        throw new InvalidOperationException($"Service of type {typeof(T).Name} is not registered");
    }
    
    /// <summary>
    /// Checks if a service is registered
    /// </summary>
    /// <typeparam name="T">The service type</typeparam>
    /// <returns>True if registered</returns>
    public bool IsRegistered<T>()
    {
        return _services.ContainsKey(typeof(T));
    }
}