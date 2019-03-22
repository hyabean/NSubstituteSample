using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;

namespace Demo3
{
    public class Class1
    {
        [Test]
        public void Test_SettingReturnValue_ReturnsValueWithSpecifiedArguments()
        {
            var calculator = Substitute.For<ICalculator>();
            calculator.Add(1, 2).Returns(3);
            Assert.AreEqual(3, calculator.Add(1, 2));
        }

        [Test]
        public void Test_SettingReturnValue_ReturnsDefaultValueWithDifferentArguments()
        {
            var calculator = Substitute.For<ICalculator>();

            // 设置调用返回值为3
            calculator.Add(1, 2).Returns(3);

            Assert.AreEqual(3, calculator.Add(1, 2)); 
            Assert.AreEqual(3, calculator.Add(1, 2));

            // 当使用不同参数调用时,返回值不是3
            Assert.AreNotEqual(3, calculator.Add(3, 6));
        }

        [Test]
        public void Test_SettingReturnValue_ReturnsValueFromProperty()
        {
            var calculator = Substitute.For<ICalculator>();

            calculator.Mode.Returns("DEC");
            Assert.AreEqual("DEC", calculator.Mode);

            calculator.Mode = "HEX";
            Assert.AreEqual("HEX", calculator.Mode);
        }
        

        [Test]
        public void Test_ReturnForSpecificArgs_UseArgumentsMatcher()
        {
            var calculator = Substitute.For<ICalculator>();

            // 当第一个参数是任意int类型的值，第二个参数是5时返回。
            calculator.Add(Arg.Any<int>(), 5).Returns(10);
            Assert.AreEqual(10, calculator.Add(123, 5));
            Assert.AreEqual(10, calculator.Add(-9, 5));
            Assert.AreNotEqual(10, calculator.Add(-9, -9));

            // 当第一个参数是1，第二个参数小于0时返回。
            calculator.Add(1, Arg.Is<int>(x => x < 0)).Returns(345);
            Assert.AreEqual(345, calculator.Add(1, -2));
            Assert.AreNotEqual(345, calculator.Add(1, 2));

            // 当两个参数都为0时返回。
            calculator.Add(Arg.Is(0), Arg.Is(0)).Returns(99);
            Assert.AreEqual(99, calculator.Add(0, 0));
        }
        
        [Test]
        public void Test_ReturnForAnyArgs_ReturnForAnyArgs()
        {
            var calculator = Substitute.For<ICalculator>();

            calculator.Add(1, 2).ReturnsForAnyArgs(100);
            Assert.AreEqual(100, calculator.Add(1, 2));
            Assert.AreEqual(100, calculator.Add(-7, 15));
        }


        [Test]
        public void Test_ReturnForAnyArgs_ReturnForAnyArgs1()
        {
            var calculator = Substitute.For<ICalculator>();

            calculator.Add(Arg.Any<int>(), Arg.Any<int>()).ReturnsForAnyArgs(100);
            Assert.AreEqual(100, calculator.Add(1, 2));
            Assert.AreEqual(100, calculator.Add(-7, 15));
        }

        [Test]
        public void Test_ReturnFromFunction_ReturnSum()
        {
            var calculator = Substitute.For<ICalculator>();

            calculator
              .Add(Arg.Any<int>(), Arg.Any<int>())
              .Returns(x => (int)x[0] + (int)x[1]);

            Assert.AreEqual(2, calculator.Add(1, 1));
            Assert.AreEqual(50, calculator.Add(20, 30));
            Assert.AreEqual(9275, calculator.Add(-73, 9348));
        }


        [Test]
        public void Test_ReturnFromFunction_CallInfo()
        {
            var foo = Substitute.For<IFoo>();
            foo.Bar(0, "").ReturnsForAnyArgs(x => "Hello " + x.Arg<string>());
            Assert.AreEqual("Hello World", foo.Bar(1, "World"));
        }

        [Test]
        public void Test_ReturnFromFunction_GetCallbackWhenever()
        {
            var calculator = Substitute.For<ICalculator>();

            var counter = 0;
            calculator
              .Add(0, 0)
              .ReturnsForAnyArgs(x =>
              {
                  counter++;
                  return 0;
              });

            calculator.Add(7, 3);
            calculator.Add(2, 2);
            calculator.Add(11, -3);
            Assert.AreEqual(3, counter);
        }

        [Test]
        public void Test_ReturnFromFunction_UseAndDoesAfterReturns()
        {
            var calculator = Substitute.For<ICalculator>();

            var counter = 0;
            calculator
              .Add(0, 0)
              .ReturnsForAnyArgs(x => 0)
              .AndDoes(x => counter++);

            calculator.Add(7, 3);
            calculator.Add(2, 2);
            Assert.AreEqual(2, counter);
        }


        [Test]
        public void Test_MultipleReturnValues_ReturnMultipleValues()
        {
            var calculator = Substitute.For<ICalculator>();

            calculator.Mode.Returns("DEC", "HEX", "BIN");
            Assert.AreEqual("DEC", calculator.Mode);
            Assert.AreEqual("HEX", calculator.Mode);
            Assert.AreEqual("BIN", calculator.Mode);
        }

        [Test]
        //[ExpectedException(typeof(Exception))]
        public void Test_MultipleReturnValues_UsingCallbacks()
        {
            var calculator = Substitute.For<ICalculator>();

            calculator.Mode.Returns(x => "DEC", x => "HEX", x => { throw new Exception(); });
            Assert.AreEqual("DEC", calculator.Mode);
            Assert.AreEqual("HEX", calculator.Mode);
            Assert.Throws< Exception>(() => { var _ = calculator.Mode; });
        }


        [Test]
        public void Test_ReplaceReturnValues_ReplaceSeveralTimes()
        {
            var calculator = Substitute.For<ICalculator>();

            calculator.Mode.Returns("DEC,HEX,OCT");
            calculator.Mode.Returns(x => "???");
            calculator.Mode.Returns("HEX");
            calculator.Mode.Returns("BIN");

            Assert.AreEqual("BIN", calculator.Mode);
        }


        [Test]
        public void Test_CheckReceivedCalls_CallReceived()
        {
            //Arrange
            var command = Substitute.For<ICommand>();
            var something = new SomethingThatNeedsACommand(command);

            //Act
            something.DoSomething();

            //Assert
            command.Received().Execute();
        }
        [Test]
        public void Test_CheckReceivedCalls_CallDidNotReceived()
        {
            //Arrange
            var command = Substitute.For<ICommand>();
            var something = new SomethingThatNeedsACommand(command);

            //Act
            something.DontDoAnything();

            //Assert
            command.DidNotReceive().Execute();
        }

        [Test]
        public void Test_CheckReceivedCalls_CallReceivedNumberOfSpecifiedTimes()
        {
            // Arrange
            var command = Substitute.For<ICommand>();
            var repeater = new CommandRepeater(command, 3);

            // Act
            repeater.Execute();

            // Assert
            // 如果仅接收到2次或者4次，这里会失败。
            command.Received(3).Execute();
        }


        [Test]
        public void Test_CheckReceivedCalls_CallReceivedWithSpecificArguments()
        {
            var calculator = Substitute.For<ICalculator>();

            //calculator.Add(1, 2).Returns(3);
            //calculator.Add(-100, 100).Returns(0);

            calculator.Add(1, 2);

            // 检查接收到了第一个参数为任意值，第二个参数为2的调用
            calculator.Received().Add(Arg.Any<int>(), 2);

            calculator.Add(-100, 100);
            // 检查接收到了第一个参数小于0，第二个参数为100的调用
            calculator.Received().Add(Arg.Is<int>(x => x < 0), 100);
            // 检查未接收到第一个参数为任意值，第二个参数大于等于500的调用
            calculator
              .DidNotReceive()
              .Add(Arg.Any<int>(), Arg.Is<int>(x => x >= 500));
        }

        [Test]
        public void Test_CheckReceivedCalls_IgnoringArguments()
        {
            var calculator = Substitute.For<ICalculator>();

            calculator.Add(1, 3);

            calculator.ReceivedWithAnyArgs().Add(1, 1);
            calculator.DidNotReceiveWithAnyArgs().Subtract(0, 0);
        }

        [Test]
        public void Test_CheckReceivedCalls_CheckingCallsToPropeties()
        {
            var calculator = Substitute.For<ICalculator>();

            var _ = calculator.Mode;
            calculator.Mode = "TEST";

            // 检查接收到了对属性 getter 的调用
            // 这里需要使用临时变量以通过编译
            var __ = calculator.Received().Mode;

            // 检查接收到了对属性 setter 的调用，参数为"TEST"
            calculator.Received().Mode = "TEST";
        }

        [Test]
        public void Test_CheckReceivedCalls_CheckingCallsToIndexers()
        {
            var dictionary = Substitute.For<IDictionary<string, int>>();
            dictionary["test"] = 1;

            dictionary.Received()["test"] = 1;
            dictionary.Received()["test"] = Arg.Is<int>(x => x < 5);
        }

        [Test]
        public void Test_CheckReceivedCalls_CheckingEventSubscriptions()
        {
            var command = Substitute.For<ICommand>();
            var watcher = new CommandWatcher(command);

            command.Executed += Raise.Event();

            Assert.IsTrue(watcher.DidStuff);
        }

        [Test]
        public void Test_CheckReceivedCalls_MakeSureWatcherSubscribesToCommandExecuted()
        {
            var command = Substitute.For<ICommand>();
            var watcher = new CommandWatcher(command);

            // 不推荐这种方法。
            // 更好的办法是测试行为而不是具体实现。
            command.Received().Executed += watcher.OnExecuted;
            // 或者有可能事件处理器是不可访问的。
            command.Received().Executed += Arg.Any<EventHandler>();
        }

        [Test]
        public void Test_ClearReceivedCalls_ForgetPreviousCalls()
        {
            var command = Substitute.For<ICommand>();
            var runner = new OnceOffCommandRunner(command);

            // 第一次运行
            runner.Run();
            command.Received().Execute();

            // 忘记前面对command的调用
            command.ClearReceivedCalls();

            // 第二次运行
            runner.Run();
            command.DidNotReceive().Execute();
        }


        [Test]
        public void Test_ArgumentMatchers_IgnoringArguments()
        {
            var calculator = Substitute.For<ICalculator>();

            calculator.Add(Arg.Any<int>(), 5).Returns(7);

            Assert.AreEqual(7, calculator.Add(42, 5));
            Assert.AreEqual(7, calculator.Add(123, 5));
            Assert.AreNotEqual(7, calculator.Add(1, 7));
        }


        [Test]
        public void Test_ArgumentMatchers_MatchSubTypes()
        {
            IFormatter formatter = Substitute.For<IFormatter>();

            formatter.Format(new object());
            formatter.Format("some string");

            formatter.Received().Format(Arg.Any<object>());
            formatter.Received().Format(Arg.Any<string>());
            formatter.DidNotReceive().Format(Arg.Any<int>());
        }

        [Test]
        public void Test_ArgumentMatchers_ConditionallyMatching()
        {
            var calculator = Substitute.For<ICalculator>();

            calculator.Add(1, -10);

            // 检查接收到第一个参数为1，第二个参数小于0的调用
            calculator.Received().Add(1, Arg.Is<int>(x => x < 0));
            // 检查接收到第一个参数为1，第二个参数为 -2、-5和-10中的某个数的调用
            calculator
              .Received()
              .Add(1, Arg.Is<int>(x => new[] { -2, -5, -10 }.Contains(x)));
            // 检查未接收到第一个参数大于10，第二个参数为-10的调用
            calculator.DidNotReceive().Add(Arg.Is<int>(x => x > 10), -10);
        }

        [Test]
        public void Test_ArgumentMatchers_ConditionallyMatchingThrowException()
        {
            IFormatter formatter = Substitute.For<IFormatter>();

            formatter.Format(Arg.Is<string>(x => x.Length <= 10)).Returns("matched");

            Assert.AreEqual("matched", formatter.Format("short"));
            Assert.AreNotEqual("matched", formatter.Format("not matched, too long"));

            // 此处将不会匹配，因为在尝试访问 null 的 Length 属性时会抛出异常，
            // 而 NSubstitute 会假设其为不匹配并隐藏掉异常。
            Assert.AreNotEqual("matched", formatter.Format(null));
        }

        [Test]
        public void Test_ArgumentMatchers_MatchingSpecificArgument()
        {
            var calculator = Substitute.For<ICalculator>();

            calculator.Add(0, 42);

            // 这里可能不工作，NSubstitute 在这种情况下无法确定在哪个参数上应用匹配器
            //calculator.Received().Add(0, Arg.Any<int>());

            calculator.Received().Add(Arg.Is(0), Arg.Any<int>());
        }


        [Test]
        public void Test_CallbacksWhenDo_PassFunctionsToReturns()
        {
            var calculator = Substitute.For<ICalculator>();

            var counter = 0;
            calculator
              .Add(0, 0)
              .ReturnsForAnyArgs(x => 0)
              .AndDoes(x => counter++);

            calculator.Add(7, 3);
            calculator.Add(2, 2);
            calculator.Add(11, -3);
            Assert.AreEqual(3, counter);
        }


        [Test]
        public void Test_CallbacksWhenDo_UseWhenDo()
        {
            var counter = 0;
            var foo = Substitute.For<IFoo>();

            foo.When(x => x.SayHello("World"))
              .Do(x => counter++);

            foo.SayHello("World");
            foo.SayHello("World");
            Assert.AreEqual(2, counter);
        }

        [Test]
        public void Test_CallbacksWhenDo_UseWhenDoOnNonVoid()
        {
            var calculator = Substitute.For<ICalculator>();

            var counter = 0;
            calculator.Add(1, 2).Returns(3);
            calculator
              .When(x => x.Add(Arg.Any<int>(), Arg.Any<int>()))
              .Do(x => counter++);

            var result = calculator.Add(1, 2);
            Assert.AreEqual(3, result);
            Assert.AreEqual(1, counter);
        }


        [Test]
        public void Test_ThrowingExceptions_ForVoid()
        {
            var calculator = Substitute.For<ICalculator>();

            // 对无返回值函数
            calculator.Add(-1, -1).Returns(x => { throw new Exception(); });

            // 抛出异常
            Assert.Throws<Exception>(() => { calculator.Add(-1, -1); });
        }

        [Test]
        public void Test_ThrowingExceptions_ForNonVoidAndVoid()
        {
            var calculator = Substitute.For<ICalculator>();

            // 对有返回值或无返回值函数
            calculator
              .When(x => x.Add(-2, -2))
              .Do(x => { throw new Exception(); });

            // 抛出异常
            Assert.Throws<Exception>(() => { calculator.Add(-2, -2); });
        }

        [Test]
        public void Test_RaisingEvents_RaiseEvent()
        {
            var engine = Substitute.For<IEngine>();

            var wasCalled = false;
            engine.Idling += (sender, args) => wasCalled = true;

            // 告诉替代实例引发异常，并携带指定的sender和事件参数
            engine.Idling += Raise.EventWith(new object(), new EventArgs());

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void Test_RaisingEvents_RaiseEventButNoMindSenderAndArgs()
        {
            var engine = Substitute.For<IEngine>();

            var wasCalled = false;
            engine.Idling += (sender, args) => wasCalled = true;

            engine.Idling += Raise.Event();
            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void Test_RaisingEvents_ArgsDoNotHaveDefaultCtor()
        {
            var engine = Substitute.For<IEngine>();

            int numberOfEvents = 0;
            engine.LowFuelWarning += (sender, args) => numberOfEvents++;

            // 发送事件，并携带指定的事件参数，未指定发送者
            engine.LowFuelWarning += Raise.EventWith(new LowFuelWarningEventArgs(10));
            // 发送事件，并携带指定的事件参数，并指定发送者
            engine.LowFuelWarning += Raise.EventWith(new object(), new LowFuelWarningEventArgs(10));

            Assert.AreEqual(2, numberOfEvents);
        }

        [Test]
        public void Test_RaisingEvents_RaisingDelegateEvents()
        {
            var sub = Substitute.For<INotifyPropertyChanged>();
            bool wasCalled = false;

            sub.PropertyChanged += (sender, args) => wasCalled = true;

            sub.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(
              this, new PropertyChangedEventArgs("test"));

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void Test_RaisingEvents_RaisingActionEvents()
        {
            var engine = Substitute.For<IEngine>();

            int revvedAt = 0;
            engine.RevvedAt += rpm => revvedAt = rpm;

            engine.RevvedAt += Raise.Event<Action<int>>(123);

            Assert.AreEqual(123, revvedAt);
        }
        [Test]
        public void Test_AutoRecursiveMocks_ManuallyCreateSubstitutes()
        {
            var factory = Substitute.For<INumberParserFactory>();
            var parser = Substitute.For<INumberParser>();
            factory.Create(',').Returns(parser);
            parser.Parse("an expression").Returns(new int[] { 1, 2, 3 });

            var actual = factory.Create(',').Parse("an expression");
            CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, actual);
        }
        [Test]
        public void Test_AutoRecursiveMocks_AutomaticallyCreateSubstitutes()
        {
            var factory = Substitute.For<INumberParserFactory>();
            factory.Create(',').Parse("an expression").Returns(new int[] { 1, 2, 3 });

            var actual = factory.Create(',').Parse("an expression");
            CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, actual);
        }
        [Test]
        public void Test_AutoRecursiveMocks_CallRecursivelySubbed()
        {
            var factory = Substitute.For<INumberParserFactory>();
            factory.Create(',').Parse("an expression").Returns(new int[] { 1, 2, 3 });

            var firstCall = factory.Create(',');
            var secondCall = factory.Create(',');
            var thirdCallWithDiffArg = factory.Create('x');

            Assert.AreSame(firstCall, secondCall);
            Assert.AreNotSame(firstCall, thirdCallWithDiffArg);
        }

        [Test]
        public void Test_AutoRecursiveMocks_SubstituteChains()
        {
            var context = Substitute.For<IContext>();
            context.CurrentRequest.Identity.Name.Returns("My pet fish Eric");
            Assert.AreEqual(
              "My pet fish Eric",
              context.CurrentRequest.Identity.Name);
        }

        [Test]
        public void Test_SetOutRefArgs_SetOutArg()
        {
            // Arrange
            var value = "";
            var lookup = Substitute.For<ILookup>();
            lookup
              .TryLookup("hello", out value)
              .Returns(x =>
              {
                  x[1] = "world!";
                  return true;
              });

            // Act
            var result = lookup.TryLookup("hello", out value);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual("world!", value);
        }

        [Test]
        public void Test_ActionsWithArgumentMatchers_InvokingCallbacks()
        {
            // Arrange
            var cart = Substitute.For<ICart>();
            var events = Substitute.For<IEvents>();
            var processor = Substitute.For<IOrderProcessor>();
            cart.OrderId = 3;
            // 设置 processor 当处理订单ID为3时，调用回调函数，参数为true
            processor.ProcessOrder(3, Arg.Invoke(true));

            // Act
            var command = new OrderPlacedCommand(processor, events);
            command.Execute(cart);

            // Assert
            events.Received().RaiseOrderProcessed(3);
        }

        [Test]
        public void Test_ActionsWithArgumentMatchers_PerformingActionsWithArgs()
        {
            var calculator = Substitute.For<ICalculator>();

            var argumentUsed = 0;
            calculator.Multiply(Arg.Any<int>(), Arg.Do<int>(x => argumentUsed = x));

            calculator.Multiply(123, 42);

            Assert.AreEqual(42, argumentUsed);
        }

        [Test]
        public void Test_ActionsWithArgumentMatchers_PerformingActionsWithAnyArgs()
        {
            var calculator = Substitute.For<ICalculator>();

            var firstArgsBeingMultiplied = new List<int>();
            calculator.Multiply(Arg.Do<int>(x => firstArgsBeingMultiplied.Add(x)), 10);

            calculator.Multiply(2, 10);
            calculator.Multiply(5, 10);

            // 由于第二个参数不为10，所以不会被 Arg.Do 匹配
            calculator.Multiply(7, 4567);

            CollectionAssert.AreEqual(firstArgsBeingMultiplied, new[] { 2, 5 });
        }

        [Test]
        public void Test_ActionsWithArgumentMatchers_ArgActionsCallSpec()
        {
            var calculator = Substitute.For<ICalculator>();

            var numberOfCallsWhereFirstArgIsLessThan0 = 0;

            // 指定调用参数：
            // 第一个参数小于0
            // 第二个参数可以为任意的int类型值
            // 当此满足此规格时，为计数器加1。
            calculator
              .Multiply(
                Arg.Is<int>(x => x < 0),
                Arg.Do<int>(x => numberOfCallsWhereFirstArgIsLessThan0++)
              ).Returns(123);

            var results = new[] {
        calculator.Multiply(-4, 3),
        calculator.Multiply(-27, 88),
        calculator.Multiply(-7, 8),
        calculator.Multiply(123, 2) // 第一个参数大于0，所以不会被匹配
      };

            Assert.AreEqual(3, numberOfCallsWhereFirstArgIsLessThan0); // 4个调用中有3个匹配上
            CollectionAssert.AreEqual(results, new[] { 123, 123, 123, 0 }); // 最后一个未匹配
        }



        [Test]
        public void Test_CheckingCallOrder_CommandRunWhileConnectionIsOpen()
        {
            var connection = Substitute.For<Connection>();
            var command = Substitute.For<CommandClass>();
            var subject = new Controller(connection, command);

            subject.DoStuff();

            Received.InOrder(() =>
            {
                connection.Open();
                command.Run(connection);
                connection.Close();
            });
        }

        [Test]
        public void Test_CheckingCallOrder_SubscribeToEventBeforeOpeningConnection()
        {
            var connection = Substitute.For<Connection>();
            connection.SomethingHappened += () => { /* some event handler */ };
            connection.Open();

            Received.InOrder(() =>
            {
                connection.SomethingHappened += Arg.Any<Action>();
                connection.Open();
            });
        }
    }

    public class Controller
    {
        private readonly Connection connection;
        private readonly CommandClass command;
        public Controller(Connection connection, CommandClass command)
        {
            this.connection = connection;
            this.command = command;
        }

        public void DoStuff()
        {
            connection.Open();
            command.Run(connection);
            connection.Close();
        }
    }

    public class CommandClass
    {
        public void Run(Connection connection)
        {
            // Method intentionally left empty.
        }
    }

    public class Connection
    {
        public void Open()
        {
            // Method intentionally left empty.
        }

        public void Close()
        {
            // Method intentionally left empty.
        }

        public event Action SomethingHappened;
    }
    public interface IEvents
    {
        void RaiseOrderProcessed(int orderId);
    }

    public interface ICart
    {
        int OrderId { get; set; }
    }

    public interface IOrderProcessor
    {
        void ProcessOrder(int orderId, Action<bool> orderProcessed);
    }

    public class OrderPlacedCommand
    {
        readonly IOrderProcessor orderProcessor;
        readonly IEvents events;
        public OrderPlacedCommand(IOrderProcessor orderProcessor, IEvents events)
        {
            this.orderProcessor = orderProcessor;
            this.events = events;
        }
        public void Execute(ICart cart)
        {
            orderProcessor.ProcessOrder(
                cart.OrderId,
                wasOk => { if (wasOk) events.RaiseOrderProcessed(cart.OrderId); }
            );
        }
    }

    public interface ILookup
{
  bool TryLookup(string key, out string value);
}

    public interface IContext
    {
        IRequest CurrentRequest { get; }
    }

    public interface IRequest
    {
        IIdentity Identity { get; }
        IIdentity NewIdentity(string name);
    }

    public interface IIdentity
    {
        string Name { get; }
        string[] Roles();
    }

    public interface INumberParser
    {
        int[] Parse(string expression);
    }
    public interface INumberParserFactory
    {
        INumberParser Create(char delimiter);
    }

    public interface IEngine
        {
            event EventHandler Idling;
            event EventHandler<LowFuelWarningEventArgs> LowFuelWarning;
            event Action<int> RevvedAt;
        }

        public class LowFuelWarningEventArgs : EventArgs
        {
            public int PercentLeft { get; private set; }
            public LowFuelWarningEventArgs(int percentLeft)
            {
                PercentLeft = percentLeft;
            }
        }

    public interface IFormatter
    {
        string Format(object o);
    }

    public interface IFoo
    {
        string Bar(int a, string b);
        void SayHello(string to);
    }

    public interface ICommand
    {
        void Execute();
        event EventHandler Executed;
    }

    public class OnceOffCommandRunner
    {
        ICommand command;
        public OnceOffCommandRunner(ICommand command)
        {
            this.command = command;
        }
        public void Run()
        {
            if (command == null) return;
            command.Execute();
            command = null;
        }
    }


    public class CommandRepeater
    {
        private readonly ICommand command;
        private readonly int numberOfTimesToCall;
        public CommandRepeater(ICommand command, int numberOfTimesToCall)
        {
            this.command = command;
            this.numberOfTimesToCall = numberOfTimesToCall;
        }

        public void Execute()
        {
            for (var i = 0; i < numberOfTimesToCall; i++) command.Execute();
        }
    }

    public interface ICalculator
    {
        int Add(int a, int b);
        int Subtract(int a, int b);
        int Multiply(int a, int b);
        string Mode { get; set; }
    }

    public class SomethingThatNeedsACommand
    {
        private readonly ICommand command;
        public SomethingThatNeedsACommand(ICommand command)
        {
            this.command = command;
        }
        public void DoSomething() { command.Execute(); }
        public void DontDoAnything()
        {
            // Method intentionally left empty.
        }
    }

    public class CommandWatcher
    {
        readonly ICommand command;
        public CommandWatcher(ICommand command)
        {
            this.command = command;
            this.command.Executed += OnExecuted;
        }
        public bool DidStuff { get; private set; }
        public void OnExecuted(object o, EventArgs e)
        {
            DidStuff = true;
        }
    }

}
