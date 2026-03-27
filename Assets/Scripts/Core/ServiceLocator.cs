using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
    static readonly Dictionary<System.Type, object> services = new();

    public static void Register<T>(T service)
    {
        Type type = typeof(T);

        if(services.ContainsKey(type)) return;

        services[type] = service;
        Debug.Log($"[ServiceLocator] Registered: {typeof(T).Name}");
    }

    public static T Get<T>()
    {
        if (services.TryGetValue(typeof(T), out var service))
        {
            return (T)service;
        }
        Debug.LogWarning($"[ServiceLocator] Service not found: {typeof(T).Name}");
        return default;
    }
}