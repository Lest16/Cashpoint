namespace Cashpoint
{
    using Castle.Windsor;
    using Castle.Windsor.Installer;

    public static class Program
    {
        public static void Main()
        {
            var container = new WindsorContainer();
            container.Install(FromAssembly.This());
            var manager = container.Resolve<ConsoleManager>();
            manager.Start();
        }
    }
}
