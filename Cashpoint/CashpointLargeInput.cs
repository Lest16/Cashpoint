namespace Cashpoint
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;

    [Serializable]
    public class CashpointLargeInput : ICashpoint
    {
        [DataMember(Name = "banknotes")]
        private SortedDictionary<uint, byte> banknotes;

        [NonSerialized]
        private uint count;

        [NonSerialized]
        private uint total;

        [NonSerialized]
        private uint[] granted;

        public CashpointLargeInput()
        {
            this.granted = new uint[] { 1 };
            this.total = 0;
            this.count = 0;
            this.banknotes = new SortedDictionary<uint, byte>();
        }

        public CashpointLargeInput(uint nominal)
        {
            this.granted = new uint[] { 1 };
            this.total = 0;
            this.count = 0;
            this.banknotes = new SortedDictionary<uint, byte>();
        }

        public uint Total
        {
            get
            {
                return this.total;
            }
        }

        public uint Count
        {
            get
            {
                return this.count;
            }
        }

        public static CashpointLargeInput LoadFromXmlFile(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentException("Should be not null", "filename");
            }

            using (var stream = new FileStream(filename, FileMode.Open))
            {
                var xmlSerializer = new DataContractSerializer(typeof(CashpointLargeInput));
                var cashpoint = (CashpointLargeInput)xmlSerializer.ReadObject(stream);
                cashpoint.granted = new uint[] { 1 };
                cashpoint.count = 0;
                cashpoint.total = 0;
                foreach (var b in cashpoint.banknotes)
                {
                    cashpoint.count += b.Value;
                    cashpoint.GrantedAdd(b.Key, b.Value);
                }

                return cashpoint;
            }
        }

        public bool CanGrant(uint value)
        {
            if (value > this.total)
            {
                return false;
            }

            return this.granted[(int)value] > 0;
        }

        ICashpoint ICashpoint.LoadFromXmlFile(string filename)
        {
            return LoadFromXmlFile(filename);
        }

        public void SaveToXmlFile(string filename)
        {
            using (var writer = XmlWriter.Create(filename, new XmlWriterSettings { Indent = true }))
            {
                var xmlSerializer = new DataContractSerializer(typeof(CashpointLargeInput));
                xmlSerializer.WriteObject(writer, this);
            }
        }

        public void AddBanknote(uint value)
        {
            this.AddBanknote(value, 1);
        }

        public void AddBanknote(uint value, byte number)
        {
            if (number == 0)
            {
                return;
            }

            if (this.banknotes.ContainsKey(value))
            {
                if (this.banknotes[value] + number < 256)
                {
                    this.banknotes[value] += number;
                    this.count += number;
                }
            }
            else
            {
                this.banknotes.Add(value, number);
                this.count += number;
            }

            this.GrantedAdd(value, number);
        }

        public void RemoveBanknote(uint value)
        {
            this.RemoveBanknote(value, 1);
        }

        public void RemoveBanknote(uint value, byte number)
        {
            if (number == 0)
            {
                return;
            }

            if (!this.banknotes.ContainsKey(value) || this.banknotes[value] < number)
            {
                return;
            }

            for (var i = this.total; i >= value * this.banknotes[value]; i--)
            {
                if (this.granted[i] > 0 && this.granted[i - (value * this.banknotes[value])] > 0 && (i + value > this.total || this.granted[i + value] == 0))
                {
                    for (var k = 0; k < number; k++)
                    {
                        this.granted[i - (k * value)] -= 1;
                    }
                }
            }

            this.banknotes[value] -= number;
            this.count -= number;
            this.total -= value * number;
            Array.Resize(ref this.granted, (int)this.total + 1);
        }

        private void GrantedAdd(uint value, int number)
        {
            if (number == 0)
            {
                return;
            }

            this.total += value * (uint)number;
            Array.Resize(ref this.granted, (int)this.total + 1);
            for (var i = (int)this.total; i >= (this.banknotes[value] * value); i--)
            {
                if (this.granted[i - (this.banknotes[value] * value)] > 0)
                {
                    for (var j = 0; j < number; j++)
                    {
                        this.granted[i - (j * value)] += 1;
                    }
                }
            }
        }
    }
}
