namespace Cashpoint
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    public class CashpointInstaller : IWindsorInstaller
{
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
        //container.Register(Classes.FromThisAssembly()
        //                    .Where(Component.IsInSameNamespaceAs<CashpointSmallInput>())
        //                    .WithService.DefaultInterfaces()
        //                    .LifestyleTransient());
        container.Register(Component
                            .For<ICashpoint>()
                            .ImplementedBy<CashpointSmallInput>()
                            .Named("CashpointSmallInput"));

        container.Register(Component
                            .For<ICashpoint>()
                            .ImplementedBy<CashpointLargeInput>()
                            .Named("CashpointLargeInput"));
    }
}
}
