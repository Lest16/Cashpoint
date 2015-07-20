namespace Cashpoint
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;

    public class CashpointSmallInput : ICashpoint
    {
        [DataMember(Name = "banknotes")]
        private Dictionary<uint, byte> banknotes = new Dictionary<uint, byte>();

        [NonSerialized]
        private uint total;

        [NonSerialized]
        private uint count;

        private uint[] granted = { 1 };

        public CashpointSmallInput()
        {
            this.granted = new uint[] { 1 };
            this.total = 0;
            this.count = 0;
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

        public static CashpointSmallInput LoadFromXmlFile(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentException("Should be not null" + filename);
            }

            using (var stream = new FileStream(filename, FileMode.Open))
            {
                var xmlSerializer = new DataContractSerializer(typeof(CashpointSmallInput));
                var cashpoint = (CashpointSmallInput)xmlSerializer.ReadObject(stream);
                cashpoint.total = 0;
                cashpoint.count = 0;

                foreach (var b in cashpoint.banknotes.Keys)
                {
                    cashpoint.total += b;
                    cashpoint.count++;
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

            return this.granted[value] > 0;
        }

        ICashpoint ICashpoint.LoadFromXmlFile(string filename)
        {
            return LoadFromXmlFile(filename);
        }

        public void SaveToXmlFile(string filename)
        {
            using (var writer = XmlWriter.Create(filename, new XmlWriterSettings { Indent = true }))
            {
                var xmlSerializer = new DataContractSerializer(typeof(CashpointSmallInput));
                xmlSerializer.WriteObject(writer, this);
            }
        }

        public void AddBanknote(uint value)
        {
            if (this.banknotes.ContainsKey(value))
            {
                this.banknotes[value]++;
            }
            else
            {
                this.banknotes.Add(value, 1);
            }

            this.total += value;
            this.count++;
            Array.Resize(ref this.granted, (int)this.total + 1);
            for (var i = (int)this.total; i >= 0; i--)
            {
                if (this.granted[i] != 0)
                {
                    this.granted[i + value]++;
                }
            }
        }

        public void AddBanknote(uint value, byte countBanknote)
        {
            Array.Resize(ref this.granted, (int)this.total + 1);
            for (var i = 0; i < countBanknote; i++)
            {
                this.AddBanknote(value);
            }
        }

        public void RemoveBanknote(uint value)
        {
            Array.Resize(ref this.granted, (int)this.total + 1);
            if (this.CanGrant(value))
            {
                for (var i = 0; i < (int)this.total; i++)
                {
                    if (this.granted[i] != 0)
                    {
                        this.granted[i + value]--;
                    }
                }
            }

            if (!this.banknotes.ContainsKey(value))
            {
                return;
            }

            if (this.banknotes[value] == 1)
            {
                this.banknotes.Remove(value);
            }
            else
            {
                this.banknotes[value]--;
            }

            this.total -= value;
            this.count--;
        }

        public void RemoveBanknote(uint value, byte countBanknote)
        {
            Array.Resize(ref this.granted, (int)this.total + 1);
            for (var i = 0; i < countBanknote; i++)
            {
                this.RemoveBanknote(value);
            }
        }
    }
}
