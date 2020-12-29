using System;
using System.Linq;
using System.Reflection;
using DependencyInjectorLibrary.Help;

namespace DependencyInjectorLibrary.Creators
{
    internal class InstanceCreator : IImplementationCreator
    {
        private readonly DependencyInjector dependencyInjector;
        private readonly ConstructorInfo suitableConstructor;

        public InstanceCreator(DependencyInjector injector, Type implementationType)
        {
            dependencyInjector = injector;
            suitableConstructor = SelectGreatestParamsCountConstructor(
                implementationType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance));
        }

        // constructor with the greatest number of parameters
        private ConstructorInfo SelectGreatestParamsCountConstructor(ConstructorInfo[] constructorInfos)
        {
            return constructorInfos.OrderBy(constructor => constructor.GetParameters().Length).Last();
        }

        public object CreateImplementation()
        {
            return CreateObjectInstance(suitableConstructor);
        }

        private object CreateObjectInstance(ConstructorInfo suitableConstructor)
        {
            ParameterInfo[] parameterInfos = suitableConstructor.GetParameters();
            object[] parameters = GetParametersByInfos(parameterInfos);
            return suitableConstructor.Invoke(parameters);
        }

        private object[] GetParametersByInfos(ParameterInfo[] parameterInfos)
        {
            object[] parameters = new object[parameterInfos.Length];
            for (int i = 0; i < parameterInfos.Length; i++)
            {
                object? name = TryGetImplementationName(parameterInfos[i]);
                parameters[i] = dependencyInjector.Resolve(parameterInfos[i].ParameterType, name);
            }
            return parameters;
        }

        private object? TryGetImplementationName(ParameterInfo parameterInfo)
        {
            ImplNameAttribute implName = parameterInfo.GetCustomAttribute<ImplNameAttribute>();
            if (implName != null)
            {
                return implName.Name;
            }
            return null;
        }
    }
}
