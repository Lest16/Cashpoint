namespace Cashpoint
{
    using Castle.Core.Logging;
    using Castle.Windsor;
    using Castle.Windsor.Installer;

    public static class Program
    {
        public static void Main()
        {
            log4net.Config.XmlConfigurator.Configure();
            var container = new WindsorContainer();
            container.Install(FromAssembly.This());
            var manager = container.Resolve<Manager>();
            manager.Start();
        }
    }
}
