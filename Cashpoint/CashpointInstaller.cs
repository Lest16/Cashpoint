﻿namespace Cashpoint
{
    using global::Cashpoint.Properties;

    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    using MongoDB.Driver;

    public class CashpointInstaller : IWindsorInstaller
{
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<Cashpoint>()
                                .UsingFactoryMethod<Cashpoint>(() => new Cashpoint(Settings.Default.LimitForSmallInput > 5000)));
            container.Register(Component.For<ConsoleManager>());
            container.Register(Component.For<MongoClient>().UsingFactoryMethod(() => new MongoClient(Settings.Default.connectionString)));
        }
}
}
