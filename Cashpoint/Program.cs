namespace Cashpoint
{
    using System;

    using Castle.Windsor;
    using Castle.Windsor.Installer;

    public static class Program
    {
        public static void Main()
        {

            Console.WriteLine("Input nominal:");
            var nominal = uint.Parse(Console.ReadLine());
            var container = new WindsorContainer();
            container.Install(FromAssembly.This());
            if (nominal > 500)
            {
                var cashpoint = container.Resolve<ICashpoint>("CashpointLargeInput");
                cashpoint.AddBanknote(nominal);
            }
            else
            {
                var cashpoint = container.Resolve<ICashpoint>("CashpointSmallInput");
                cashpoint.AddBanknote(nominal);
            }
        }
    }
}
