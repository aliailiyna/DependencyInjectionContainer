using System.Collections.Generic;
using DependencyInjectorLibrary.Help;

namespace UnitTestProject.InjectorTest
{
    public interface ICommon { }
    public class First : ICommon
    {
    }
    public class Second : ICommon
    {
    }

    public interface ICommonFrameAttr 
    {
        public ICommon First { get; }
        public ICommon Second { get; }
    }
    public class CommonFrameAttr : ICommonFrameAttr
    {
        public ICommon First { get; }
        public ICommon Second { get; }

        public CommonFrameAttr([ImplName("Second")] ICommon second, [ImplName("First")] ICommon first)
        {
            First = first;
            Second = second;
        }
    }

    public interface ICommonFrameEnumerable
    {
        public IEnumerable<ICommon> All { get; }
    }

    class CommonFrameEnumerable : ICommonFrameEnumerable
    {
        public IEnumerable<ICommon> All { get; }

        public CommonFrameEnumerable(IEnumerable<ICommon> all)
        {
            All = all;
        }
    }

    public interface IFirstPattern { }
    public class FirstPattern : IFirstPattern { }

    public interface ISecondPattern { }
    public class SecondPattern : ISecondPattern { }


    public interface ISmallPattern<IT1> where IT1 : IFirstPattern
    {
        public IT1 First { set; get; }
    }
    public class SmallPattern<IT1> : ISmallPattern<IT1> where IT1 : IFirstPattern
    { 
        public IT1 First { set; get; }
        //public SmallPattern(IT1 first)
        //{
            //First = first;
        //}
    }

    public interface IBigPattern<IT1, IT2> //where IT1 : IFirstPattern where IT2 : ISecondPattern
    {
        public IT1 First { set; get; }
        public IT2 Second { set; get; }
    }
    public class BigPattern<IT1, IT2> : IBigPattern<IT1, IT2> //where IT1 : IFirstPattern where IT2 : ISecondPattern
    {
        public IT1 First { set; get; }
        public IT2 Second { set; get; }

        public BigPattern()
        {
        }

        public BigPattern(IT1 first)
        {
            First = first;
        }

        public BigPattern(IT2 second)
        {
            Second = second;
        }
        public BigPattern(IT1 first, IT2 second)
        {
            First = first;
            Second = second;
        }
    }

    public interface INestedOne 
    {
        public INestedTwo Nested { get; }
    }

    public class NestedOne : INestedOne
    {
        public INestedTwo Nested { get; }

        public NestedOne(INestedTwo nested)
        {
            Nested = nested;
        }
    }

    public interface INestedTwo 
    {
        public INestedThree Nested { get; }
    }

    public class NestedTwo : INestedTwo
    {
        public INestedThree Nested { get; }

        public NestedTwo(INestedThree nested)
        {
            Nested = nested;
        }
    }

    public interface INestedThree { }

    public class NestedThree : INestedThree
    {

    }
}
