namespace Cashpoint
{
    using System;
    using System.IO;
    using System.Linq;

    public class Manager
    {
        private readonly Cashpoint cashpoint;

        private readonly IRepository<CashpointState> repository;

        private readonly TextReader reader;

        private readonly TextWriter writer;

        public Manager(Cashpoint cashpoint, IRepository<CashpointState> repository, TextReader reader, TextWriter writer, bool check)
        {
            this.cashpoint = cashpoint;
            this.repository = repository;
            if (!check)
            {
                this.reader = reader;
                this.writer = writer;
            }
            else
            {
                this.reader = Console.In;
                this.writer = Console.Out;
            }        

            Console.SetIn(reader);
            Console.SetOut(writer);
        }

        public void Start()
        {
            this.cashpoint.OnError += this.ShowMessage;
            string line;
            while (!string.IsNullOrEmpty(line = this.reader.ReadLine()))
            {
                switch (line)
                {
                    case "1":
                        {
                            uint nominal;
                            uint.TryParse(this.reader.ReadLine(), out nominal);
                            uint count;
                            uint.TryParse(this.reader.ReadLine(), out count);
                            this.cashpoint.AddBanknote(nominal, count);
                            this.Save();
                        }

                        break;

                    case "2":
                        {
                            uint nominal;
                            uint.TryParse(this.reader.ReadLine(), out nominal);
                            uint count;
                            uint.TryParse(this.reader.ReadLine(), out count);
                            this.cashpoint.RemoveBanknote(nominal, count);
                            this.Save();
                        }

                        break;
                }
            }

            this.writer.WriteLine("Total: " + this.cashpoint.Total);
            this.writer.Flush();
            this.reader.ReadLine();
            this.writer.Close();
        }

        private void Save()
        {
            this.repository.Clear();
            var state = new CashpointState { Bank = this.cashpoint.Banknotes.ToDictionary(pair => pair.Key, pair => pair.Value) };
            this.repository.Save(state);
        }

        private void ShowMessage()
        {
            this.writer.WriteLine("Error!Banknote does not exist");
        }
    }
}
