using System;

namespace DependencyInjectorLibrary.Help
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ImplNameAttribute : Attribute
    {
        public object Name
        { get; }

        public ImplNameAttribute(object name)
        {
            Name = name;
        }
    }
}
