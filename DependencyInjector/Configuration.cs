using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DependencyInjectorLibrary.Exceptions;
using DependencyInjectorLibrary.Help;

namespace DependencyInjectorLibrary
{
    public class Configuration
    {
        private readonly static string NO_SUITABLE_CONSTRUCTOR_MESSAGE_FORMAT = "Для типа {0} не найден подходящий конструктор.";
        private readonly static string INHERITANCE_ERROR_MESSAGE_FORMAT = "Тип {0} должен наследоваться от {1}.";
        private readonly static string IMPL_ALREADY_REGISTERED_MESSAGE_FORMAT = "Реализация {0} уже была зарегистрирована.";
        private readonly static string ABSTRACT_IMPL_MESSAGE_FORMAT = "Реализация {0} не должна быть абстрактной.";

        private const Lifetime defaultLifetime = Lifetime.Instance;

        internal Dictionary<Type, List<RegisteredDependencyInfo>> RegisteredDependencies
        { get; }

        public Configuration()
        {
            RegisteredDependencies = new Dictionary<Type, List<RegisteredDependencyInfo>>();
        }

        public void Register<TDependency, TImplementation>(Lifetime lifetime = defaultLifetime) where TImplementation : TDependency
        {
            privateRegister(typeof(TDependency), typeof(TImplementation), lifetime, null);
        }

        public void Register<TDependency, TImplementation>(object name) where TImplementation : TDependency
        {
            privateRegister(typeof(TDependency), typeof(TImplementation), defaultLifetime, name);
        }

        public void Register<TDependency, TImplementation>(Lifetime lifetime, object name) where TImplementation : TDependency
        {
            privateRegister(typeof(TDependency), typeof(TImplementation), lifetime, name);
        }

        public void Register(Type dependency, Type implementation, Lifetime lifetime = defaultLifetime)
        {
            ValidateInharitance(dependency, implementation);
            privateRegister(dependency, implementation, lifetime, null);
        }

        public void Register(Type dependency, Type implementation, object name)
        {
            ValidateInharitance(dependency, implementation);
            privateRegister(dependency, implementation, defaultLifetime, name);
        }

        public void Register(Type dependency, Type implementation, Lifetime lifetime, object name)
        {
            ValidateInharitance(dependency, implementation);
            privateRegister(dependency, implementation, lifetime, name);
        }

        private void ValidateInharitance(Type dependency, Type implementation)
        {
            //if ((!dependency.IsGenericTypeDefinition && dependency.IsAssignableFrom(implementation)) || 
            //!dependency.OpenIsAssignableFrom(implementation))
            //if (!dependency.IsAssignableFrom(implementation))
            {
                //throw new ConfigurationException(string.Format(INHERITANCE_ERROR_MESSAGE_FORMAT, implementation.Name, dependency.Name));
            }
            if (dependency.IsAssignableFrom(implementation)) return;
            if (dependency.OpenIsAssignableFrom(implementation)) return;
            throw new ConfigurationException(string.Format(INHERITANCE_ERROR_MESSAGE_FORMAT, implementation.Name, dependency.Name));
        }

        private void privateRegister(Type dependency, Type implementation, Lifetime lifetime, object? name)
        {
            ValidateNotAbstract(implementation);
            ValidateConstructors(implementation);

            RegisteredDependencyInfo dependencyInfo = new RegisteredDependencyInfo(dependency, implementation, lifetime, name);
            if (!RegisteredDependencies.ContainsKey(dependency))
            {
                RegisteredDependencies.Add(dependency, new List<RegisteredDependencyInfo>() { dependencyInfo });
            }
            else
            {
                List<RegisteredDependencyInfo> dependencyInfos = RegisteredDependencies[dependency];
                ValidateNotImplDuplication(dependencyInfos, implementation, dependencyInfo.Name);
                dependencyInfos.Add(dependencyInfo);
            }
        }

        private void ValidateNotAbstract(Type implementation)
        {
            if (implementation.IsAbstract)
            {
                throw new ConfigurationException(string.Format(ABSTRACT_IMPL_MESSAGE_FORMAT, implementation.Name));
            }
        }

        private void ValidateNotImplDuplication(List<RegisteredDependencyInfo> dependencyInfos, Type implementation, object? implementationName)
        {
            if (dependencyInfos.Select(dependency => dependency.ImplementationType).Any(type => type.Equals(implementation))
                || (implementation.Name != null &&
                dependencyInfos.Select(dependency => dependency.Name)
                .Where(name => name != null).Any(name => name.Equals(implementationName)))
                )
            {
                throw new ConfigurationException(string.Format(IMPL_ALREADY_REGISTERED_MESSAGE_FORMAT, implementation.Name));
            }
        }

        private void ValidateConstructors(Type implementation)
        {
            if (!ContainsSuitableConstructor(implementation))
            {
                throw new ConfigurationException(string.Format(NO_SUITABLE_CONSTRUCTOR_MESSAGE_FORMAT, implementation.Name));
            }
        }

        // 
        private bool ContainsSuitableConstructor(Type implementationType)
        {
            ConstructorInfo suitableConstructor = implementationType.GetConstructors(BindingFlags.NonPublic |
                BindingFlags.Public | BindingFlags.Instance).OrderBy(constructor => constructor.GetParameters().Length).Last();
            return suitableConstructor.GetParameters().All(parameter =>
                parameter.ParameterType.IsInterface || parameter.ParameterType.IsClass);
        }
    }
}
