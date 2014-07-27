using System.Diagnostics.Contracts;

namespace Alpha
{
    using System;
    using System.Collections.Generic;

    class ServiceContainer
    {
        private readonly Dictionary<Type, object> _services;

        public ServiceContainer()
        {
            _services = new Dictionary<Type, object>();
        }

        public void Register<T>(T item) where T : IService
        {
            _services[typeof(T)] = item;
        }

        public void RemoveService<T>() where T : IService
        {
            _services.Remove(typeof(T));
        }
        [Pure]
        public T Get<T>() where T : IService
        {
            return (T)_services[typeof(T)];
        }
    }
}
