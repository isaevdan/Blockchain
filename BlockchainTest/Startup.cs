using System.Linq;
using BlockchainTest.Bitcoind;
using BlockchainTest.DAL;
using BlockchainTest.DAL.Cache;
using BlockchainTest.DAL.Sql;
using BlockchainTest.Services;
using BlockchainTest.Workers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlockchainTest
{
    public class Startup
    {
        private BaseWorker[] _workers;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddMemoryCache();

            var configRoot = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, false)
                .Build();

            var settings = configRoot.Get<Appsettings>();

            services.AddSingleton(settings);
            services.AddSingleton<IOutputTransactionsRepository, OutputTransactionsRepository>();
            services.AddSingleton<IInputTransactionsRepository, InputTransactionsCache>();
            services.AddSingleton<InputTransactionsRepository>();
            services.AddSingleton<WalletRepository>();
            services.AddSingleton<IWalletRepository, WalletsCache>();
            services.AddSingleton<WalletService>();
            services.AddSingleton(settings.Bitcoind);
            services.AddSingleton(settings.Sql);
            services.AddSingleton(settings.Service);
            services.AddSingleton<BitcoindService>();
            services.AddSingleton<TransactionService>();
            services.AddSingleton<BaseWorker, TransactionsLoader>();
            services.AddSingleton<BaseWorker, WalletLoader>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseStaticFiles(); 
            
            app.UseHttpsRedirection();
            app.UseMvc();

            _workers = app.ApplicationServices.GetServices<BaseWorker>().ToArray();
            foreach (var worker in _workers)
                worker.Start();
        }
    }
}