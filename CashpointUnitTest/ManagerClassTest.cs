namespace CashpointUnitTest
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    using Cashpoint;

    using Castle.Core.Logging;

    using NUnit.Framework;

    [TestFixture]
    public class ManagerClassTest
    {
        [Test]
        public void StartTest()
        {
            var outputString = this.Start("1\n33\n2");
            var regex = new Regex(@"(Total: \d+)");
            Assert.IsTrue(regex.IsMatch(outputString), "Добавление или удаление не было произведено");
            outputString = this.Start("2\n33\n2");
            Assert.IsTrue(regex.IsMatch(outputString), "Добавление или удаление не было произведено");
            outputString = this.Start("3");
            regex = new Regex(@"(Total: 0)");
            Assert.IsTrue(regex.IsMatch(outputString), "Распознавание некорректной команды");
        }

        public string Start(string testString)
        {
            var ms1 = new MemoryStream(Encoding.Default.GetBytes(testString));
            var sr = new StreamReader(ms1);
            var ms2 = new MemoryStream();
            var sw = new StreamWriter(ms2);
            ILogger logger = new NullLogger();
            var target = new Manager(
                new Cashpoint(true, new Dictionary<uint, uint>(), logger),
                new StubRepository<CashpointState>(),
                sr,
                sw,
                false);
            target.Start();
            var byffer = ms2.GetBuffer();
            var outputString = Encoding.Default.GetString(byffer);
            return outputString;
        }
    }
}
