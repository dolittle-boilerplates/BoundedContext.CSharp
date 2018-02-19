using System;
using doLittle.Assemblies;
using doLittle.Assemblies.Rules;
using doLittle.DependencyInversion;
using doLittle.DependencyInversion.Conventions;
using doLittle.Execution;
using doLittle.Hosting;

namespace Source
{
    public interface IFoo { }

    [Singleton]
    public class Foo : IFoo { }

    [Singleton]
    public class Blah
    {

    }

    public class Provider : ICanProvideBindings
    {
        public void Provide(IBindingProviderBuilder builder)
        { }
    }

    public class Convention : IBindingConvention
    {
        public bool CanResolve(Type service)
        {
            return false;
        }

        public void Resolve(Type service, IBindingBuilder bindingBuilder)
        {

        }
    }

    public class AssemblySpecifier : ICanSpecifyAssemblies
    {
        /// <inheritdoc/>
        public void Specify(IAssemblyRuleBuilder builder)
        {
            builder.ExcludeAssembliesStartingWith(
                "Autofac",
                "System",
                "mscorlib",
                "Microsoft",
                "SQLite",
                "StackExchange",
                "Newtonsoft",
                "Remotion",
                "Serilog",
                "MongoDB",
                "Swashbuckle"
            );
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var host = Host.CreateBuilder().Build();

            var h2 = host.Container.Get<IHost>();
            var foo = host.Container.Get<IFoo>();
        }
    }
}




        /*
        {
            var before = DateTimeOffset.UtcNow;

            var host = Host.CreateBuilder().Build().Run();

            var after = DateTimeOffset.UtcNow;
            Console.WriteLine("Time : " + after.Subtract(before));

            var foo = host.Container.Get<IFoo>();
            var foo2 = host.Container.Get<IFoo>();

            var blah = host.Container.Get<Blah>();
            var blah2 = host.Container.Get<Blah>();
        }
        */
