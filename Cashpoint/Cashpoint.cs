namespace Cashpoint
{
    using System;
    using System.Collections.Generic;

    using Castle.Core.Logging;

    public class Cashpoint : ICashpoint
    {
        private static ILogger log;

        private readonly bool isLarge;
    
        private readonly IDictionary<uint, uint> banknotes;

        private uint count;

        private uint total;

        private uint[] granted;

        public Cashpoint(bool isLarge, IDictionary<uint, uint> banknotes, ILogger logger)
        {
            this.banknotes = banknotes;
            this.granted = new uint[] { 1 };
            foreach (var banknote in this.banknotes)
            {
                this.GrantedAdd(banknote.Key, banknote.Value);
            }
            
            this.count = (uint)banknotes.Count;
            this.isLarge = isLarge;
            log = logger;
        }

       public delegate void MessageError();

        public event MessageError OnError;

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

        public IDictionary<uint, uint> Banknotes
        {
            get
            {
                return this.banknotes;
            }
        }

        public void RemoveBanknote(uint value, uint countBankotes)
        {
            if (this.isLarge)
            {
                this.RemoveBanknoteLargeInput(value, countBankotes);
            }
            else
            {
                this.RemoveBanknoteSmallInput(value, countBankotes);
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

        public void AddBanknote(uint value, uint countBanknotes)
        {
            if (this.isLarge)
            {
                this.AddBanknoteLargeInput(value, countBanknotes);
            }
            else
            {
                this.AddBanknoteSmallInput(value, countBanknotes);
            }
        }

        private void AddBanknoteLargeInput(uint value, uint number)
        {
            if (number == 0)
            {
                return;
            }

            if (this.banknotes.ContainsKey(value))
            {
                this.banknotes[value] += number;
                this.count += number;
            }
            else
            {
                this.banknotes.Add(value, number);
                this.count += number;
            }

            this.GrantedAdd(value, number);
            log.Info("Add banknote " + value);
        }

        private void RemoveBanknoteLargeInput(uint value, uint number)
        {
            if (number == 0)
            {
                return;
            }

            if (!this.banknotes.ContainsKey(value) || this.banknotes[value] < number)
            {
                this.OnError();
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

            if (this.banknotes[value] == 1)
            {
                this.banknotes.Remove(value);
            }
            else
            {
                this.banknotes[value] -= number;
            }

            this.count -= number;
            this.total -= value * number;
            log.Info("Remove banknote " + value);
            Array.Resize(ref this.granted, (int)this.total + 1);
        }

        private void GrantedAdd(uint value, uint number)
        {
            if (number == 0)
            {
                return;
            }

            this.total += value * number;
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

        private void AddBanknoteSmallInput(uint value)
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

            log.Info("Add banknote " + value);
        }

        private void AddBanknoteSmallInput(uint value, uint countBanknote)
        {
            Array.Resize(ref this.granted, (int)this.total + 1);
            for (var i = 0; i < countBanknote; i++)
            {
                this.AddBanknoteSmallInput(value);
            }
        }

        private void RemoveBanknoteSmallInput(uint value)
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
                this.OnError();
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
            log.Info("Remove banknote " + value);
        }

        private void RemoveBanknoteSmallInput(uint value, uint countBanknote)
        {
            Array.Resize(ref this.granted, (int)this.total + 1);
            for (var i = 0; i < countBanknote; i++)
            {
                this.RemoveBanknoteSmallInput(value);
            }
        }
    }
}
