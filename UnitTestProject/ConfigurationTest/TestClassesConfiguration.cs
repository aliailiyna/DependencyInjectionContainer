namespace UnitTestProject.ConfigurationTest
{
    public interface ICommon { }
    public class First : ICommon { }

    public class Second : ICommon { }



    public interface ICorrect { };
    public class ClassCorrect : ICorrect
    {
    }

    public interface IGeneric<T>{ };
    public class ClassGeneric<T> : IGeneric<T>
    {
    }

    public interface IGenericParam<T> where T : ICorrect{ };
    public class ClassGenericParam<T> : IGenericParam<T> where T : ICorrect
    {
    }

    public interface IForInterface { };
    public interface IAbstract : IForInterface
    {
    }

    public interface IForAbstractClass { };
    public abstract class ClassAbstract : IForAbstractClass
    {
    }

    public interface IWrongConstructor { };
    public class ClassWrongConstructor : IWrongConstructor
    {
        public ClassWrongConstructor(int num)
        {

        }
    }
}
