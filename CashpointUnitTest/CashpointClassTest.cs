namespace CashpointUnitTest
{
    using System.IO;

    using Cashpoint;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    [TestClass]
    public class CashpointClassTest
    {
        [TestMethod]
        public void AddBanknoteTest()
        {
            var target = new Cashpoint();
            target.AddBanknoteLargeInput(5);
            Assert.AreEqual((uint)5, target.Total, "Добавление единственной банкноты не было произведено");
            target.AddBanknoteLargeInput(50);
            Assert.AreEqual((uint)55, target.Total, "Добавление второй банкноты не было произведено");

            for (var i = 0; i < 255; i++)
            {
                target.AddBanknoteLargeInput(7);
            }

            Assert.AreEqual((uint)1840, target.Total, "Добавление 255-ти банкнот не было произведено");
            target.AddBanknoteLargeInput(1, 60);
            Assert.AreEqual((uint)1900, target.Total, "Добавление 60-ти банкнот не было произведено");
        }

        [TestMethod]
        public void CanGrantTest()
        {
            var target = new Cashpoint();
            //Assert.IsTrue(target.CanGrant(0), "Банкомат не может выдать 0");

            //target.AddBanknote(5);
            //Assert.IsTrue(target.CanGrant(5), "Банкомат не может выдать 5=5");

            //target.AddBanknote(6);
            //Assert.IsFalse(target.CanGrant(10), "Банкомат выдал 10 != 6+5");

            //target.AddBanknote(3);
            //Assert.IsTrue(target.CanGrant(8), "Банкомат не смог выдать 5+3=8");

            //target.AddBanknote(15);
            //target.AddBanknote(14);

            //Assert.IsTrue(target.CanGrant(28), "Банкомат не смог выдать 5+3+6+14=28");
            //Assert.IsFalse(target.CanGrant(16), "Банкомат как-то смог выдать 16 != 14+3");
            //Assert.IsFalse(target.CanGrant(44), "Банкомат как-то смог 44 > 5+6+3+15+14");

            //Assert.IsTrue(target.CanGrant(target.Total), "Банкомат не смог выдать всю свою сумму(банкноты не повторяются)");

            //target = new Cashpoint();

            //target.AddBanknote(7, 5);

            //Assert.IsTrue(target.CanGrant(14), "Банкомат не смог выдать 7+7=14");
            //Assert.IsTrue(target.CanGrant(35), "Банкомат не смог выдать 7+7+7+7+7=35");

            //target.RemoveBanknote(7, 4);

            //Assert.IsFalse(target.CanGrant(21), "Банкомат выдал 21 != 7");
            //Assert.IsTrue(target.CanGrant(7), "Банкомат не смог выдать 7=7");

            //target.AddBanknote(5, 3);

            //Assert.IsTrue(target.CanGrant(22), "Банкомат не смог выдать 7+5+5+5=22");

            //target.RemoveBanknote(5);

            //Assert.IsFalse(target.CanGrant(22), "Банкомат выдал 21 != 17");

            //Assert.IsTrue(target.CanGrant(target.Total), "Банкомат не смог выдать всю свою сумму(банкноты повторяются)");

            //var target = new Cashpoint();
            target.AddBanknoteLargeInput(50, 2);
            target.AddBanknoteLargeInput(10, 10);
            target.RemoveBanknoteLargeInput(10, 5);
            Assert.IsTrue(target.CanGrant(150), "Банкомат не смог выдать 150");
            target.RemoveBanknoteLargeInput(50, 1);
            target.RemoveBanknoteLargeInput(10, 3);
            Assert.IsTrue(target.CanGrant(70), "Банкомат не смог выдать 70");
        }

        [TestMethod]
        public void LoadFromXmlFileTest()
        {
            const string Filename = "testdata1.xml";
            var target = Cashpoint.LoadFromXmlFile(Filename);
            Assert.AreEqual(target.Total, 897u, "Из файла testdata1.xml загружено неверное количество денег");

            Assert.IsTrue(target.CanGrant(500u), "Банкомат не смог выдать 400 из файла testdata1.xml");
            Assert.IsFalse(target.CanGrant(404u), "Банкомат как-то смог выдать 440 из файла testdata1.xml");
        }

        [TestMethod]
        public void RemoveBanknoteTest()
        {
            var target = new Cashpoint();
            target.RemoveBanknoteLargeInput(5);
            Assert.AreEqual(0u, target.Total, "Извлечена несуществующая купюра из пустого банкомата");

            target.AddBanknoteLargeInput(7, 2);
            target.RemoveBanknoteLargeInput(3);
            Assert.AreEqual(14u, target.Total, "Извлечена несуществующая купюра");

            target.RemoveBanknoteLargeInput(7, 2);
            Assert.AreEqual(0u, target.Total, "Купюры извлечены не полностью");
        }

        [TestMethod]
        public void SaveToXmlFileTest()
        {
            var target = new Cashpoint();
            target.AddBanknoteLargeInput(1);
            target.AddBanknoteLargeInput(5, 2);
            const string Filename = "testdata2.xml";
            target.SaveToXmlFile(Filename);
            Assert.AreEqual(File.ReadAllText("testdata3.xml"), File.ReadAllText(Filename), "Купюры в файле отличны от ожидаемых");
        }

        [TestMethod]
        public void TotalTest()
        {
            var target = new Cashpoint();
            Assert.AreEqual(0u, target.Total, "Новый банкомат оказался не пустой");
        }

        [TestMethod]
        public void CountTest()
        {
            var target = new Cashpoint();
            Assert.AreEqual(0u, target.Count, "Новый банкомат оказался не пустой");
        }
    }
}
