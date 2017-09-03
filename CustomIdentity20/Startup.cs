using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomIdentity.Middlewares;
using CustomIdentity20.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CustomIdentity20
{
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
            var userStore = new UserStore();
            services.AddSingleton<IUserStore<ApplicationUser>>(userStore);
            services.AddSingleton<IRoleStore<ApplicationRole>>(userStore);
            services.AddSingleton<IUserPasswordStore<ApplicationUser>>(userStore);

            services.AddAuthentication();

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {

            });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseOptionMiddleware();

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
