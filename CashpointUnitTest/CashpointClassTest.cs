namespace CashpointUnitTest
{
    using System.Collections.Generic;

    using Cashpoint;
    using NUnit.Framework;

    [TestFixture]
    public class CashpointClassTest
    {
        [Test]
        public void AddBanknoteTest()
        {
            var dic = new Dictionary<uint, uint>();
            var target = new Cashpoint(true, dic);
            
            target.AddBanknote(5, 1);
            Assert.AreEqual(5, target.Total, "Добавление единственной банкноты не было произведено");
            target.AddBanknote(50, 1);
            Assert.AreEqual(55, target.Total, "Добавление второй банкноты не было произведено");

            for (var i = 0; i < 255; i++)
            {
                target.AddBanknote(7, 1);
            }

            Assert.AreEqual(1840, target.Total, "Добавление 255-ти банкнот не было произведено");
            target.AddBanknote(1, 60);
            Assert.AreEqual(1900, target.Total, "Добавление 60-ти банкнот не было произведено");
        }

        [Test]
        public void CanGrantTest()
        {
            var dic = new Dictionary<uint, uint>();
            var target = new Cashpoint(true, dic);
            Assert.IsTrue(target.CanGrant(0), "Банкомат не может выдать 0");

            target.AddBanknote(5, 1);
            Assert.IsTrue(target.CanGrant(5), "Банкомат не может выдать 5=5");

            target.AddBanknote(6, 1);
            Assert.IsFalse(target.CanGrant(10), "Банкомат выдал 10 != 6+5");

            target.AddBanknote(3, 1);
            Assert.IsTrue(target.CanGrant(8), "Банкомат не смог выдать 5+3=8");

            target.AddBanknote(15, 1);
            target.AddBanknote(14, 1);

            Assert.IsTrue(target.CanGrant(28), "Банкомат не смог выдать 5+3+6+14=28");
            Assert.IsFalse(target.CanGrant(16), "Банкомат как-то смог выдать 16 != 14+3");
            Assert.IsFalse(target.CanGrant(44), "Банкомат как-то смог 44 > 5+6+3+15+14");

            Assert.IsTrue(target.CanGrant(target.Total), "Банкомат не смог выдать всю свою сумму(банкноты не повторяются)");

            target.AddBanknote(50, 2);
            target.AddBanknote(10, 10);
            target.RemoveBanknote(10, 5);
            Assert.IsTrue(target.CanGrant(150), "Банкомат не смог выдать 150");
            target.RemoveBanknote(50, 1);
            target.RemoveBanknote(10, 3);
            Assert.IsTrue(target.CanGrant(70), "Банкомат не смог выдать 70");
        }

        [Test]
        public void RemoveBanknoteTest()
        {
            var dic = new Dictionary<uint, uint>();
            var target = new Cashpoint(true, dic);
            target.OnError += delegate { };
            target.RemoveBanknote(5, 1);
            Assert.AreEqual(0u, target.Total, "Извлечена несуществующая купюра из пустого банкомата");

            target.AddBanknote(7, 2);
            target.RemoveBanknote(3, 1);
            Assert.AreEqual(14u, target.Total, "Извлечена несуществующая купюра");

            target.RemoveBanknote(7, 2);
            Assert.AreEqual(0u, target.Total, "Купюры извлечены не полностью");
        }

        [Test]
        public void TotalTest()
        {
            var dic = new Dictionary<uint, uint>();
            var target = new Cashpoint(true, dic);
            Assert.AreEqual(0u, target.Total, "Новый банкомат оказался не пустой");
        }

        [Test]
        public void CountTest()
        {
            var dic = new Dictionary<uint, uint>();
            var target = new Cashpoint(true, dic);
            Assert.AreEqual(0u, target.Count, "Новый банкомат оказался не пустой");
        }
    }
}
