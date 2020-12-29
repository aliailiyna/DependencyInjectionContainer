using Microsoft.VisualStudio.TestTools.UnitTesting;
using DependencyInjectorLibrary;
using DependencyInjectorLibrary.Exceptions;
using UnitTestProject.ConfigurationTest;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTestConfiguration
    {
        [TestMethod]
        public void ConfigurationCorrect()
        {
            Configuration configuration = new Configuration();
            configuration.Register<ICorrect, ClassCorrect>();
        }

        [TestMethod]
        public void ConfigurationCorrectOpen()
        {
            Configuration configuration = new Configuration();
            configuration.Register(typeof(ICorrect), typeof(ClassCorrect));
        }

        [TestMethod]
        public void ConfigurationCorrectGenericParam()
        {
            Configuration configuration = new Configuration();
            configuration.Register(typeof(ICorrect), typeof(ClassCorrect));
            configuration.Register<IGeneric<ICorrect>, ClassGeneric<ICorrect>>();
        }

        [TestMethod]
        public void ConfigurationCorrectGenericParamExceptionUnregistered()
        {
            Configuration configuration = new Configuration();
            Assert.ThrowsException<ConfigurationException>(() => configuration.Register(typeof(ClassGeneric<ICorrect>),
                typeof(IGeneric<ICorrect>)));
        }

        [TestMethod]
        public void ConfigurationCorrectOpenGeneric()
        {
            Configuration configuration = new Configuration();
            configuration.Register(typeof(IGeneric<>), typeof(ClassGeneric<>));
        }

        [TestMethod]
        public void ConfigurationCorrectOpenGenericParam()
        {
            Configuration configuration = new Configuration();
            configuration.Register(typeof(IGeneric<ICorrect>), typeof(ClassGeneric<ICorrect>));
        }

        [TestMethod]
        public void ConfigurationExceptionInherit()
        {
            Configuration configuration = new Configuration();
            Assert.ThrowsException<ConfigurationException>(() => configuration.Register(typeof(ClassCorrect), typeof(ICorrect)));
        }

        [TestMethod]
        public void ConfigurationExceptionInheritGeneric()
        {
            Configuration configuration = new Configuration();
            Assert.ThrowsException<ConfigurationException>(() => configuration.Register(typeof(ClassGeneric<>), typeof(IGeneric<>)));
        }

        [TestMethod]
        public void ConfigurationExceptionInheritGenericParam()
        {
            Configuration configuration = new Configuration();
            configuration.Register(typeof(ICorrect), typeof(ClassCorrect));
            Assert.ThrowsException<ConfigurationException>(() => configuration.Register(typeof(ClassGeneric<ICorrect>), 
                typeof(IGeneric<ICorrect>)));
        }

        [TestMethod]
        public void ConfigurationExceptionAbstractInterfaceOpen()
        {
            Configuration configuration = new Configuration();
            Assert.ThrowsException<ConfigurationException>(() => configuration.Register(typeof(IForInterface), typeof(IAbstract)));
        }

        [TestMethod]
        public void ConfigurationExceptionAbstractClass()
        {
            Configuration configuration = new Configuration();
            Assert.ThrowsException<ConfigurationException>(() => configuration.Register<IForAbstractClass, ClassAbstract>());
        }

        [TestMethod]
        public void Asself()
        {
            Configuration configuration = new Configuration();
            configuration.Register<ClassCorrect, ClassCorrect>();
        }

        [TestMethod]
        public void AsselfOpen()
        {
            Configuration configuration = new Configuration();
            configuration.Register(typeof(ClassCorrect), typeof(ClassCorrect));
        }

        [TestMethod]
        public void AsselfGeneric()
        {
            Configuration configuration = new Configuration();
            configuration.Register(typeof(ClassCorrect), typeof(ClassCorrect));
            configuration.Register<ClassGeneric<ClassCorrect>, ClassGeneric<ClassCorrect>>();
        }

        [TestMethod]
        public void AsselfOpenGeneric()
        {
            Configuration configuration = new Configuration();
            configuration.Register(typeof(ClassGeneric<>), typeof(ClassGeneric<>));
        }

        [TestMethod]
        public void AsselfOpenGenericParams()
        {
            Configuration configuration = new Configuration();
            configuration.Register(typeof(ClassCorrect), typeof(ClassCorrect));
            configuration.Register(typeof(ClassGeneric<ClassCorrect>), typeof(ClassGeneric<ClassCorrect>));
        }

        [TestMethod]
        public void ExceptionDuplicate()
        {
            Configuration configuration = new Configuration();
            configuration.Register(typeof(ICommon), typeof(First));
            Assert.ThrowsException<ConfigurationException>(() => configuration.Register<ICommon, First>());
        }

        [TestMethod]
        public void ExceptionDuplicateName()
        {
            Configuration configuration = new Configuration();
            configuration.Register(typeof(ICommon), typeof(First), "Same");
            Assert.ThrowsException<ConfigurationException>(() => configuration.Register<ICommon, Second>("Same"));
        }

        [TestMethod]
        public void ExceptionDuplicateDifferentName()
        {
            Configuration configuration = new Configuration();
            configuration.Register(typeof(ICommon), typeof(First), "First");
            Assert.ThrowsException<ConfigurationException>(() => configuration.Register<ICommon, First>("Second"));
        }

        [TestMethod]
        public void ExceptionDuplicateSameName()
        {
            Configuration configuration = new Configuration();
            configuration.Register(typeof(ICommon), typeof(First), "Same");
            Assert.ThrowsException<ConfigurationException>(() => configuration.Register<ICommon, Second>("Same"));
        }

        [TestMethod]
        public void ExceptionWrongConstructor()
        {
            Configuration configuration = new Configuration();
            Assert.ThrowsException<ConfigurationException>(() => configuration.Register<IWrongConstructor, ClassWrongConstructor>());
            Assert.ThrowsException<ConfigurationException>(() => configuration.Register(typeof(IWrongConstructor), typeof(ClassWrongConstructor)));
        }
    }
}
