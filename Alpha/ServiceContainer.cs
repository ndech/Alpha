namespace Alpha
{
    using System;
    using System.Collections.Generic;

    public class ServiceContainer : IServiceProvider
    {
        private readonly Dictionary<Type, object> _services;

        public ServiceContainer()
        {
            _services = new Dictionary<Type, object>();
        }

        public void AddService(Type serviceType, object item)
        {
            _services[serviceType] = item;
        }

        public void RemoveService(Type serviceType)
        {
            _services.Remove(serviceType);
        }

        public object GetService(Type serviceType)
        {
            return _services[serviceType];
        }

        public T GetService<T>() where T : IService
        {
            return (T)_services[typeof(T)];
        }
    }
}
