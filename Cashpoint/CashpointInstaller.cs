namespace Cashpoint
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using global::Cashpoint.Properties;

    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    using MongoDB.Driver;

    public class CashpointInstaller : IWindsorInstaller
{
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<MongoRepository>());
            container.Register(Component.For<ConsoleManager>());
            container.Register(
                Component.For<TextReader>().UsingFactoryMethod<TextReader>(
                    () =>
                        {
                            var fs = new FileStream("test1.txt", FileMode.OpenOrCreate);
                            return new StreamReader(fs);
                        }));
            container.Register(
                Component.For<TextWriter>().UsingFactoryMethod<TextWriter>(
                    () =>
                        {
                            var fs = new FileStream("test2.txt", FileMode.OpenOrCreate);
                            return new StreamWriter(fs);
                        }));
            container.Register(Component.For<IMongoClient>().UsingFactoryMethod(() => new MongoClient(Settings.Default.connectionString)));
            container.Register(Component.For<Cashpoint>()
                                .UsingFactoryMethod<Cashpoint>(
                                    () =>
                                        {
                                            var repository = container.Resolve<MongoRepository>();
                                            var banknotes = repository.GetContents();
                                            return new Cashpoint(Settings.Default.LimitForSmallInput > 5000, banknotes == null ? new Dictionary<uint, uint>() : banknotes.Bank.ToDictionary(pair => pair.Key, pair => pair.Value));
                                        }));
        }
}
}
