namespace LightInjectOData
{
    using System;

    using LightInject;
    using LightInject.Microsoft.DependencyInjection;
    using Microsoft.AspNet.OData.Builder;
    using Microsoft.AspNet.OData.Extensions;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.OData.Edm;

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }

    public class Startup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //TODO: remove commented lines to enable odata
            //services.AddOData();
            services.AddMvc().AddControllersAsServices();
            var container = new ServiceContainer(new ContainerOptions { EnablePropertyInjection = false });
            container.Register<IService, Service>();
            return container.CreateServiceProvider(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseMvc(cr =>
            {
                //TODO: remove commented lines to enable odata
                //cr.Select().Expand().Filter().OrderBy().MaxTop(null).Count();
                //cr.MapODataServiceRoute("odataroute", "odata", getEdmModel());
            });
        }

        private IEdmModel getEdmModel()
        {
            var builder = new ODataConventionModelBuilder();
            var edmModel = builder.GetEdmModel();
            return edmModel;
        }
    }

    [Route("test")]
    public class TestController : ControllerBase
    {
        public TestController(IService service)
        {
            this.service = service;
        }
        private readonly IService service;

        [Route("getservicevalue")]
        public string GetServiceValue()
        {
            return service.GetValue();
        }
    }

    public interface IService
    {
        string GetValue();
    }
    public class Service : IService
    {
        public string GetValue()
        {
            return "testValue";
        }
    }
}
