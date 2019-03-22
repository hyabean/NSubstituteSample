using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;

namespace Demo2
{
    public class Class1
    {
        [Test]
        public void Test_CreatingSubstitute_MultipleInterfaces()
        {
            var command = Substitute.For<ICommand, IDisposable>();

            var runner = new CommandRunner(command);
            runner.RunCommand();

            command.Received().Execute();
            ((IDisposable)command).Received().Dispose();
        }


        [Test]
        public void Test_CreatingSubstitute_SpecifiedOneClassType()
        {
            var substitute = Substitute.For(
                  new[] { typeof(ICommand), typeof(IDisposable), typeof(SomeClassWithCtorArgs) },
                  new object[] { 5, "hello world" }
                );
            Assert.IsInstanceOf<ICommand>(substitute);
            Assert.IsInstanceOf<IDisposable>(substitute);
            Assert.IsInstanceOf<SomeClassWithCtorArgs>(substitute);
        }

        [Test]
        public void Test_CreatingSubstitute_ForDelegate()
        {
            var func = Substitute.For<Func<string>>();
            func().Returns("hello");
            Assert.AreEqual("hello", func());
        }
    }

    public interface ICommand : IDisposable
    {
        void Execute();
    }

    public class CommandRunner
    {
        private readonly ICommand _command;

        public CommandRunner(ICommand command)
        {
            _command = command;
        }

        public void RunCommand()
        {
            _command.Execute();
            _command.Dispose();
        }
    }

    public class SomeClassWithCtorArgs : IDisposable
    {
        public SomeClassWithCtorArgs(int arg1, string arg2)
        {
        }

        public void Dispose()
        {
            // Method intentionally left empty.
        }
    }


}
