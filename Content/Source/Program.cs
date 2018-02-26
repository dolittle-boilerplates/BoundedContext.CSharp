using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using doLittle.Assemblies;
using doLittle.Assemblies.Rules;
using doLittle.Commands;
using doLittle.Commands.Coordination;
using doLittle.DependencyInversion;
using doLittle.DependencyInversion.Conventions;
using doLittle.Events.Storage;
using doLittle.Execution;
using doLittle.Hosting;
using doLittle.Logging;
using doLittle.Queries;
using doLittle.Queries.Coordination;
using doLittle.ReadModels;
using doLittle.Runtime.Events.Publishing;
using doLittle.Runtime.Events.Storage;
using doLittle.Runtime.Execution;
using doLittle.Security;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;

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
        public void Provide(IBindingProviderBuilder builder) { }
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

    public class MyCommand : ICommand
    {

    }

    public class NullBindings : ICanProvideBindings
    {
        public void Provide(IBindingProviderBuilder builder)
        {
            builder.Bind<IEventStore>().To<NullEventStore>().Singleton();
            builder.Bind<IEventSourceVersions>().To<NullEventSourceVersions>().Singleton();
            builder.Bind<IEventEnvelopes>().To<EventEnvelopes>();
            builder.Bind<IEventSequenceNumbers>().To<NullEventSequenceNumbers>();
            builder.Bind<ICanSendCommittedEventStream>().To<NullCommittedEventStreamSender>().Singleton();
            builder.Bind<ExecutionContextPopulator>().To((ExecutionContext, details)=> { });
            builder.Bind<ClaimsPrincipal>().To(()=> new ClaimsPrincipal(new ClaimsIdentity()));
            builder.Bind<CultureInfo>().To(()=> CultureInfo.InvariantCulture);
            builder.Bind<ICallContext>().To(new DefaultCallContext());
            builder.Bind<ICanResolvePrincipal>().To(new DefaultPrincipalResolver());
        }
    }

    public class MyReadModel : IReadModel
    {

    }

    public class MyQuery : IQueryFor<MyReadModel>
    {
        public IQueryable<MyReadModel> Query => new MyReadModel[0].AsQueryable();

    }

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            //services.AddMvcCore().AddApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });

            services.AddDolittle();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseMvc();

        }
    }


    public static class ContainerBuilderExtensions
    {

        public static void AddDolittle(this ContainerBuilder containerBuilder)
        {

        }
    }
    
   

    class Program
    {
        static void Main(string[] args)
        {
            //var host = Host.CreateBuilder().Build();

            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build()
                .Run();

            /*
            var queryCoordinator = host.Container.Get<IQueryCoordinator>();
            var query = new MyQuery();
            var pagingInfo = new PagingInfo();
            var result = queryCoordinator.Execute(query,pagingInfo);


            var commandCoordinator = host.Container.Get<ICommandCoordinator>();
            var command = new MyCommand();
            commandCoordinator.Handle(command);


            var h2 = host.Container.Get<IHost>();
            var foo = host.Container.Get<IFoo>();*/
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