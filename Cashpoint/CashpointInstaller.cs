namespace Cashpoint
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using global::Cashpoint.Properties;

    using Castle.Core.Logging;
    using Castle.Facilities.Logging;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    using MongoDB.Driver;

    public class CashpointInstaller : IWindsorInstaller
{
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.AddFacility<LoggingFacility>(f => f.UseLog4Net());
            container.Register(Component.For<MongoRepository>());
            container.Register(Component.For<Manager>().DependsOn(new
                                                                             {
                                                                                 check = Settings.Default.checkConsole
                                                                             }));
            container.Register(
                Component.For<TextReader>().UsingFactoryMethod<TextReader>(
                () =>
                    {
                        var fs = new FileStream("in.txt", FileMode.OpenOrCreate);
                        return new StreamReader(fs);
                    }));
            container.Register(
                Component.For<TextWriter>().UsingFactoryMethod<TextWriter>(
                    () =>
                        {
                            var fs = new FileStream("out.txt", FileMode.OpenOrCreate);
                            return new StreamWriter(fs);
                        }));
            container.Register(Component.For<IRepository<CashpointState>>().UsingFactoryMethod(() => new MongoRepository(new MongoClient(Settings.Default.connectionString))));
            container.Register(Component.For<Cashpoint>()
                                .UsingFactoryMethod<Cashpoint>(
                                    () =>
                                        {
                                            var repository = container.Resolve<IRepository<CashpointState>>();
                                            var banknotes = repository.GetContents();
                                            return new Cashpoint(Settings.Default.LimitForSmallInput > 5000, banknotes == null ? new Dictionary<uint, uint>() : banknotes.Bank.ToDictionary(pair => pair.Key, pair => pair.Value), container.Resolve<ILogger>());
                                        }));
        }
}
}
