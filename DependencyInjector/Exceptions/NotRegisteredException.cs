using System;

namespace DependencyInjectorLibrary.Exceptions
{
    public class NotRegisteredException : Exception
    {
        private readonly static string MESSAGE_FORMAT = "Зависимость для {0} не зарегистрирована.";
        private readonly static string MESSAGE_WITH_OBJECT_FORMAT = "Зависимость для {0} с именем {1} не зарегистрирована.";
        private readonly string message;

        public override string Message
        {
            get { return message; }
        }

        public NotRegisteredException(Type dependencyType)
        {
            message = string.Format(MESSAGE_FORMAT, dependencyType.Name);
        }

        public NotRegisteredException(Type dependencyType, object name)
        {
            message = string.Format(MESSAGE_WITH_OBJECT_FORMAT, dependencyType.Name, name);
        }
    }
}
