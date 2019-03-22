using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;

namespace NSubstituteSample
{
    /// <summary>
    /// 参考文档 http://www.cnblogs.com/gaochundong/archive/2013/05/21/nsubstitute_get_started.html
    /// </summary>
    class Program
    {
        [Test]
        public void Test_GetStarted_GetSubstitute()
        {
            ICalculator calculator = Substitute.For<ICalculator>();

            calculator.Add(1, 2);
        }

        [Test]
        public void Test_GetStarted_ReturnSpecifiedValue()
        {
            ICalculator calculator = Substitute.For<ICalculator>();
            calculator.Add(1, 2).Returns(3);

            int actual = calculator.Add(1, 2);
            Assert.AreEqual(3, actual);
        }

        [Test]
        public void Test_GetStarted_ReceivedSpecificCall()
        {
            ICalculator calculator = Substitute.For<ICalculator>();
            calculator.Add(1, 2);

            calculator.Received().Add(1, 2);
            calculator.DidNotReceive().Add(5, 7);
        }

        [Test]
        //[ExpectedException(typeof(ReceivedCallsException))]
        public void Test_GetStarted_DidNotReceivedSpecificCall()
        {
            ICalculator calculator = Substitute.For<ICalculator>();
            calculator.Add(5, 7);

            calculator.Received().Add(1, 2);
        }

        [Test]
        public void Test_GetStarted_SetPropertyValue()
        {
            ICalculator calculator = Substitute.For<ICalculator>();

            calculator.Mode.Returns("DEC");
            Assert.AreEqual("DEC", calculator.Mode);

            calculator.Mode = "HEX";
            Assert.AreEqual("HEX", calculator.Mode);
        }

        [Test]
        public void Test_GetStarted_MatchArguments()
        {
            ICalculator calculator = Substitute.For<ICalculator>();

            calculator.Add(10, -5);

            calculator.Received().Add(10, Arg.Any<int>());
            calculator.Received().Add(10, Arg.Is<int>(x => x < 0));
        }

        [Test]
        public void Test_GetStarted_PassFuncToReturns()
        {
            ICalculator calculator = Substitute.For<ICalculator>();
            calculator
               .Add(Arg.Any<int>(), Arg.Any<int>())
               .Returns(x => (int)x[0] + (int)x[1]);

            int actual = calculator.Add(5, 10);

            Assert.AreEqual(15, actual);
        }

        [Test]
        public void Test_GetStarted_MultipleValues()
        {
            ICalculator calculator = Substitute.For<ICalculator>();
            calculator.Mode.Returns("HEX", "DEC", "BIN");

            Assert.AreEqual("HEX", calculator.Mode);
            Assert.AreEqual("DEC", calculator.Mode);
            Assert.AreEqual("BIN", calculator.Mode);
        }

        [Test]
        public void Test_GetStarted_RaiseEvents()
        {
            ICalculator calculator = Substitute.For<ICalculator>();
            bool eventWasRaised = false;

            calculator.PoweringUp += (sender, args) =>
            {
                eventWasRaised = true;
            };

            calculator.PoweringUp += Raise.Event();

            Assert.IsTrue(eventWasRaised);
        }
    }

    public interface ICalculator
    {
        int Add(int a, int b);
        string Mode { get; set; }
        event EventHandler PoweringUp;
    }
}
