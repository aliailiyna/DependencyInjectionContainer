using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DependencyInjectorLibrary.Creators;
using DependencyInjectorLibrary.Exceptions;
using DependencyInjectorLibrary.Help;

namespace DependencyInjectorLibrary
{
    public class DependencyInjector
    {
        private readonly Dictionary<Type, List<ImplementationData>> dependencyToImplementations;
        private readonly List<RegisteredDependencyInfo> openGenerics;

        public DependencyInjector(Configuration dependencies)
        {
            dependencyToImplementations = new Dictionary<Type, List<ImplementationData>>();
            openGenerics = new List<RegisteredDependencyInfo>();
            InitializeDependenciesDictionary(dependencies);
        }

        // start initialization of injector
        private void InitializeDependenciesDictionary(Configuration dependencies)
        {
            foreach (List<RegisteredDependencyInfo> dependencyInfos in dependencies.RegisteredDependencies.Values)
            {
                foreach (RegisteredDependencyInfo dependencyInfo in dependencyInfos)
                {
                    if (dependencyInfo.DependencyType.IsGenericTypeDefinition)
                    {
                        openGenerics.Add(dependencyInfo);
                    }
                    else
                    {
                        AddDependencyInfoToDictionary(dependencyInfo);
                    }
                }
            }
        }

        private void AddDependencyInfoToDictionary(RegisteredDependencyInfo dependencyInfo)
        {
            List<ImplementationData> implementationDatas;
            if (dependencyToImplementations.ContainsKey(dependencyInfo.DependencyType))
            {
                implementationDatas = dependencyToImplementations[dependencyInfo.DependencyType];
            }
            else
            {
                implementationDatas = new List<ImplementationData>();
                dependencyToImplementations.Add(dependencyInfo.DependencyType, implementationDatas);
            }


            IImplementationCreator? provider = null;
            switch (dependencyInfo.Lifetime)
            {
                case Lifetime.Instance:
                    provider = CreateTransientProvider(dependencyInfo.ImplementationType);
                    break;
                case Lifetime.Singleton:
                    provider = CreateSingletonProvider(dependencyInfo.ImplementationType);
                    break;
            }

            if (provider != null)
            {
                implementationDatas.Add(new ImplementationData(provider, dependencyInfo.DependencyType,
                    dependencyInfo.ImplementationType, dependencyInfo.Name));
            }
        }

        private IImplementationCreator CreateTransientProvider(Type implementationType)
        {
            return new InstanceCreator(this, implementationType);
        }

        private IImplementationCreator CreateSingletonProvider(Type implementationType)
        {
            return new SingletonCreator(CreateTransientProvider(implementationType));
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T), null);
        }

        public T Resolve<T>(object implementationNmae)
        {
            return (T)Resolve(typeof(T), implementationNmae);
        }

        internal object Resolve(Type dependencyType, object? implementationName)
        {
            bool isEnumerable = false;
            if (dependencyType.IsGenericType && !dependencyToImplementations.ContainsKey(dependencyType))
            {
                Type genericDefinition = dependencyType.GetGenericTypeDefinition();
                if (genericDefinition.Equals(typeof(IEnumerable<>)))
                {
                    dependencyType = dependencyType.GetGenericArguments()[0];
                    isEnumerable = true;
                }

                if (dependencyType.IsGenericType && !dependencyToImplementations.ContainsKey(dependencyType))
                {
                    ProcessOpenGenericType(dependencyType, implementationName);
                }
            }

            if (!dependencyToImplementations.ContainsKey(dependencyType))
            {
                throw new NotRegisteredException(dependencyType);
            }

            List<ImplementationData> implementationDatas = dependencyToImplementations[dependencyType];
            if (isEnumerable)
            {
                return GetAllImplementations(implementationDatas);
            }
            else
            {
                return GetRequiredImplementation(implementationDatas, implementationName, dependencyType);
            }

        }

        // dynamically add dependencyes for open generic
        private void ProcessOpenGenericType(Type dependencyType, object? implementationName)
        {
            Type openGeneric = dependencyType.GetGenericTypeDefinition();
            List<RegisteredDependencyInfo> genericImplementations = openGenerics.Where(dependencyInfo =>
                dependencyInfo.DependencyType.Equals(openGeneric)).ToList();
            if (genericImplementations.Count == 0)
                return;
            RegisteredDependencyInfo? dependencyInfo = null;
            if (implementationName == null)
            {
                dependencyInfo = genericImplementations[0];
            }
            else
            {
                genericImplementations = genericImplementations
                    .Where(dependencyInfo => dependencyInfo.Name != null &&
                        dependencyInfo.Name.Equals(implementationName)).ToList();
                if (genericImplementations.Count != 0)
                {
                    dependencyInfo = genericImplementations[0];
                }
            }
            if (dependencyInfo == null)
                return;
            //dependencyInfo = CreateOpenGenericInfoCopy(dependencyInfo, dependencyType.GetGenericArguments()[0]);
            dependencyInfo = CreateOpenGenericInfoCopy(dependencyInfo, dependencyType.GetGenericArguments());
            if (!dependencyToImplementations.ContainsKey(dependencyInfo.DependencyType))
            {
                AddDependencyInfoToDictionary(dependencyInfo);
            }
        }

        private RegisteredDependencyInfo CreateOpenGenericInfoCopy(RegisteredDependencyInfo dependencyInfo,
            Type[] genericDependencyType)
        {
            return new RegisteredDependencyInfo(
                dependencyInfo.DependencyType.MakeGenericType(genericDependencyType),
                dependencyInfo.ImplementationType.MakeGenericType(genericDependencyType),
                dependencyInfo.Lifetime,
                dependencyInfo.Name
            );
        }

        private IList GetAllImplementations(List<ImplementationData> implementationDatas)
        {
            IList implementations = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(implementationDatas[0].DependencyType));

            foreach (ImplementationData implementationData in implementationDatas)
            {
                implementations.Add(implementationData.Creator.CreateImplementation());
            }
            return implementations;
        }

        private object GetRequiredImplementation(List<ImplementationData> implementationDatas,
            object? implementationName, Type dependencyType)
        {
            ImplementationData implementationData = GetImplementationData(implementationDatas,
                implementationName, dependencyType);
            return implementationData.Creator.CreateImplementation();
        }

        private ImplementationData GetImplementationData(List<ImplementationData> implementationDatas,
            object? implementationName, Type dependencyType)
        {
            ImplementationData implementationData;
            if (implementationName == null)
            {
                implementationData = implementationDatas[0];
            }
            else
            {
                implementationData = implementationDatas.Find(implementationData =>
                    implementationData.Name != null && implementationData.Name.Equals(implementationName));
                if (implementationData == null)
                {
                    throw new NotRegisteredException(dependencyType, implementationName);
                }
            }
            return implementationData;
        }
    }

    class ImplementationData
    {
        public IImplementationCreator Creator
        { get; }

        public Type DependencyType
        { get; }

        public Type ImplementationType
        { get; }

        public object? Name
        { get; }

        public ImplementationData(IImplementationCreator provider, Type dependencyType,
            Type implementationType, object? name)
        {
            Creator = provider;
            DependencyType = dependencyType;
            ImplementationType = implementationType;
            Name = name;
        }
    }
}
