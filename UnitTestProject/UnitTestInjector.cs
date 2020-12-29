using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using DependencyInjectorLibrary;
using DependencyInjectorLibrary.Exceptions;
using DependencyInjectorLibrary.Help;
using UnitTestProject.InjectorTest;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTestInjector
    {
        [TestMethod]
        public void InstanseDifferent()
        {
            Configuration configuration = new Configuration();
            configuration.Register<ICommon, First>(Lifetime.Instance);
            DependencyInjector injector = new DependencyInjector(configuration);

            ICommon objFirst = injector.Resolve<ICommon>();
            ICommon objSecond = injector.Resolve<ICommon>();

            Assert.IsFalse(objFirst == objSecond);
        }

        [TestMethod]
        public void InstanseDifferentAsself()
        {
            Configuration configuration = new Configuration();
            configuration.Register<First, First>(Lifetime.Instance);
            DependencyInjector injector = new DependencyInjector(configuration);

            First objFirst = injector.Resolve<First>();
            First objSecond = injector.Resolve<First>();

            Assert.IsFalse(objFirst == objSecond);
        }

        [TestMethod]
        public void SingletoneSame()
        {
            Configuration configuration = new Configuration();
            configuration.Register<ICommon, First>(Lifetime.Singleton);
            DependencyInjector injector = new DependencyInjector(configuration);

            ICommon objFirst = injector.Resolve<ICommon>();
            ICommon objSecond = injector.Resolve<ICommon>();

            Assert.IsTrue(objFirst == objSecond);
        }

        [TestMethod]
        public void SingletoneSameAsself()
        {
            Configuration configuration = new Configuration();
            configuration.Register<First, First>(Lifetime.Singleton);
            DependencyInjector injector = new DependencyInjector(configuration);

            First objFirst = injector.Resolve<First>();
            First objSecond = injector.Resolve<First>();

            Assert.IsTrue(objFirst == objSecond);
        }

        [TestMethod]
        public void NamedCorrect()
        {
            Configuration configuration = new Configuration();
            configuration.Register<ICommon, First>("First");
            configuration.Register<ICommon, Second>("Second");
            DependencyInjector injector = new DependencyInjector(configuration);

            ICommon obj;

            obj = injector.Resolve<ICommon>("First");
            Assert.IsInstanceOfType(obj, typeof(First));

            obj = injector.Resolve<ICommon>("Second");
            Assert.IsInstanceOfType(obj, typeof(Second));
        }

        [TestMethod]
        public void NamedCorrectWithout()
        {
            Configuration configuration = new Configuration();
            configuration.Register<ICommon, First>("First");
            configuration.Register<ICommon, Second>("Second");
            DependencyInjector injector = new DependencyInjector(configuration);

            ICommon obj = injector.Resolve<ICommon>();
            Assert.IsInstanceOfType(obj, typeof(First));
        }

        [TestMethod]
        public void ExceptionRegister()
        {
            Configuration configuration = new Configuration();
            DependencyInjector injector = new DependencyInjector(configuration);

            Assert.ThrowsException<NotRegisteredException>(() => injector.Resolve<ICommon>());
        }

        [TestMethod]
        public void ExceptionNamed()
        {
            Configuration configuration = new Configuration();
            configuration.Register<ICommon, First>("First");
            configuration.Register<ICommon, Second>("Second");
            DependencyInjector injector = new DependencyInjector(configuration);

            Assert.ThrowsException<NotRegisteredException>(() => injector.Resolve<ICommon>("Same"));
        }


        [TestMethod]
        public void AllCorrect()
        {
            Configuration configuration = new Configuration();
            configuration.Register<ICommon, First>(Lifetime.Singleton, "First");
            configuration.Register<ICommon, Second>(Lifetime.Singleton, "Second");
            DependencyInjector injector = new DependencyInjector(configuration);

            ICommon objFirst = injector.Resolve<ICommon>("First");
            ICommon objSecond = injector.Resolve<ICommon>("Second");
            IEnumerable <ICommon> objects = injector.Resolve<IEnumerable<ICommon>>();
            List<ICommon> objectsList = new List<ICommon>();
            foreach (var obj in objects)
            {
                objectsList.Add(obj);
            }

            List<ICommon> objectsCheck = new List<ICommon>() { objFirst, objSecond };
            CollectionAssert.AreEquivalent(objectsList, objectsCheck);
        }

        [TestMethod]
        public void ExceptionAll()
        {
            Configuration configuration = new Configuration();
            DependencyInjector injector = new DependencyInjector(configuration);

            Assert.ThrowsException<NotRegisteredException>(() => injector.Resolve<IEnumerable<ICommon>>());
        }

        [TestMethod]
        public void NameAttributeInConstructor()
        {
            Configuration configuration = new Configuration();
            configuration.Register<ICommon, First>(Lifetime.Singleton, "First");
            configuration.Register<ICommon, Second>(Lifetime.Singleton, "Second");
            configuration.Register<ICommonFrameAttr, CommonFrameAttr>();
            DependencyInjector injector = new DependencyInjector(configuration);

            ICommon objFirst = injector.Resolve<ICommon>("First");
            ICommon objSecond = injector.Resolve<ICommon>("Second");
            ICommonFrameAttr objFrame = injector.Resolve<ICommonFrameAttr>();

            Assert.AreEqual(objFirst, objFrame.First);
            Assert.AreEqual(objSecond, objFrame.Second);
        }

        [TestMethod]
        public void IEnumerableInConstructor()
        {
            Configuration configuration = new Configuration();
            configuration.Register(typeof(ICommon), typeof(First), Lifetime.Singleton, "First");
            configuration.Register(typeof(ICommon), typeof(Second), Lifetime.Singleton, "Second");
            configuration.Register(typeof(ICommonFrameEnumerable), typeof(CommonFrameEnumerable));
            DependencyInjector injector = new DependencyInjector(configuration);

            IEnumerable<ICommon> objects = injector.Resolve<IEnumerable<ICommon>>();
            List<ICommon> objectsList = new List<ICommon>();
            foreach (var obj in objects)
            {
                objectsList.Add(obj);
            }

            ICommonFrameEnumerable objectFrame = injector.Resolve<ICommonFrameEnumerable>();
            IEnumerable<ICommon> protoCheckList = objectFrame.All;
            List<ICommon> checkList = new List<ICommon>();
            foreach (var obj in protoCheckList)
            {
                checkList.Add(obj);
            }

            CollectionAssert.AreEquivalent(objectsList, checkList);
        }


        [TestMethod]
        public void Generic()
        {
            Configuration configuration = new Configuration();
            configuration.Register<IFirstPattern, FirstPattern>();
            configuration.Register<ISmallPattern<IFirstPattern>, SmallPattern<IFirstPattern>>();
            DependencyInjector injector = new DependencyInjector(configuration);

            ISmallPattern<IFirstPattern> main = injector.Resolve<ISmallPattern<IFirstPattern>>();

            //Assert.IsInstanceOfType(main.First, typeof(FirstPattern));
            Assert.IsInstanceOfType(main, typeof(SmallPattern<IFirstPattern>));
        }

        [TestMethod]
        public void OpenGeneric()
        {
            Configuration configuration = new Configuration();
            configuration.Register(typeof(IFirstPattern), typeof(FirstPattern));
            configuration.Register(typeof(ISmallPattern<>), typeof(SmallPattern<>));
            DependencyInjector injector = new DependencyInjector(configuration);

            ISmallPattern<IFirstPattern> main = injector.Resolve<ISmallPattern<IFirstPattern>>();

            //Assert.IsInstanceOfType(main.First, typeof(FirstPattern));
            Assert.IsInstanceOfType(main, typeof(SmallPattern<IFirstPattern>));
        }

        [TestMethod]
        public void OpenGenericParam()
        {
            Configuration configuration = new Configuration();
            configuration.Register(typeof(IFirstPattern), typeof(FirstPattern));
            configuration.Register(typeof(ISmallPattern<IFirstPattern>), typeof(SmallPattern<IFirstPattern>));
            DependencyInjector injector = new DependencyInjector(configuration);

            ISmallPattern<IFirstPattern> main = injector.Resolve<ISmallPattern<IFirstPattern>>();

            //Assert.IsInstanceOfType(main.First, typeof(FirstPattern));
            Assert.IsInstanceOfType(main, typeof(SmallPattern<IFirstPattern>));
        }

        [TestMethod]
        public void GenericTwo()
        {
            Configuration configuration = new Configuration();
            configuration.Register<IFirstPattern, FirstPattern>();
            configuration.Register<ISecondPattern, SecondPattern>();
            configuration.Register<IBigPattern<IFirstPattern, ISecondPattern>, BigPattern<IFirstPattern, ISecondPattern>>();
            DependencyInjector injector = new DependencyInjector(configuration);

            IBigPattern<IFirstPattern, ISecondPattern> main = injector.Resolve<IBigPattern<IFirstPattern, ISecondPattern>>();

            Assert.IsInstanceOfType(main.First, typeof(FirstPattern));
            Assert.IsInstanceOfType(main.Second, typeof(SecondPattern));
            Assert.IsInstanceOfType(main, typeof(IBigPattern<IFirstPattern, ISecondPattern>));
        }

        [TestMethod]
        public void OpenGenericTwo()
        {
            Configuration configuration = new Configuration();
            configuration.Register(typeof(IFirstPattern), typeof(FirstPattern));
            configuration.Register(typeof(ISecondPattern), typeof(SecondPattern));
            configuration.Register(typeof(IBigPattern<,>), typeof(BigPattern<,>));
            DependencyInjector injector = new DependencyInjector(configuration);

            IBigPattern<IFirstPattern, ISecondPattern> main = injector.Resolve<IBigPattern<IFirstPattern, ISecondPattern>>();

            Assert.IsInstanceOfType(main.First, typeof(FirstPattern));
            Assert.IsInstanceOfType(main.Second, typeof(SecondPattern));
            Assert.IsInstanceOfType(main, typeof(BigPattern<IFirstPattern, ISecondPattern>));
        }

        [TestMethod]
        public void OpenGenericTwoParam()
        {
            Configuration configuration = new Configuration();
            configuration.Register(typeof(IFirstPattern), typeof(FirstPattern));
            configuration.Register(typeof(ISecondPattern), typeof(SecondPattern));
            configuration.Register(typeof(IBigPattern<IFirstPattern, ISecondPattern>), typeof(BigPattern<IFirstPattern, ISecondPattern>));
            DependencyInjector injector = new DependencyInjector(configuration);

            IBigPattern<IFirstPattern, ISecondPattern> main = injector.Resolve<IBigPattern<IFirstPattern, ISecondPattern>>();

            Assert.IsInstanceOfType(main.First, typeof(FirstPattern));
            Assert.IsInstanceOfType(main.Second, typeof(SecondPattern));
            Assert.IsInstanceOfType(main, typeof(BigPattern<IFirstPattern, ISecondPattern>));
        }

        [TestMethod]
        public void Nested()
        {
            Configuration configuration = new Configuration();
            configuration.Register(typeof(INestedOne), typeof(NestedOne));
            configuration.Register(typeof(INestedTwo), typeof(NestedTwo));
            configuration.Register(typeof(INestedThree), typeof(NestedThree));
            DependencyInjector injector = new DependencyInjector(configuration);

            INestedOne main = injector.Resolve<INestedOne>();

            Assert.IsInstanceOfType(main, typeof(NestedOne));
            Assert.IsInstanceOfType(main.Nested, typeof(NestedTwo));
            Assert.IsInstanceOfType(main.Nested.Nested, typeof(NestedThree));
        }
    }
}
