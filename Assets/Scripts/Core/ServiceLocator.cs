using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
    static readonly Dictionary<System.Type, object> _services = new();

    public static void Register<T>(T service)
    {
        _services[typeof(T)] = service;
        Debug.Log($"[ServiceLocator] Registered: {typeof(T).Name}");
    }

    public static T Get<T>()
    {
        if (_services.TryGetValue(typeof(T), out var service))
        {
            return (T)service;
        }
        Debug.LogWarning($"[ServiceLocator] Service not found: {typeof(T).Name}");
        return default;
    }
}