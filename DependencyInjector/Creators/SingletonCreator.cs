namespace DependencyInjectorLibrary.Creators
{
    internal class SingletonCreator : IImplementationCreator
    {
        private readonly IImplementationCreator creator;
        private readonly object locker;
        private volatile object? instance;

        public SingletonCreator(IImplementationCreator creator)
        {
            this.creator = creator;
            locker = new object();
        }

        public object CreateImplementation()
        {
            if (instance == null)
            {
                lock (locker)
                {
                    if (instance == null)
                    {
                        instance = creator.CreateImplementation();
                    }
                }
            }
            return instance;
        }
    }
}
