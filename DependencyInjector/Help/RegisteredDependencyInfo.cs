using System;

namespace DependencyInjectorLibrary.Help
{
    public enum Lifetime
    {
        Instance,
        Singleton
    }

    class RegisteredDependencyInfo
    {
        public Type DependencyType
        { get; }

        public Type ImplementationType
        { get; }

        public Lifetime Lifetime
        { get; }

        public object? Name
        { get; }

        public RegisteredDependencyInfo(Type dependencyType, Type implementationType, Lifetime lifetime, object? name)
        {
            DependencyType = dependencyType;
            ImplementationType = implementationType;
            Lifetime = lifetime;
            Name = name;
        }
    }
}
