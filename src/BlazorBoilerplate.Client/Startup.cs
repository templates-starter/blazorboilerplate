using BlazorBoilerplate.CommonUI;
using BlazorBoilerplate.CommonUI.Services.Contracts;
using BlazorBoilerplate.CommonUI.Services.Implementations;
using BlazorBoilerplate.CommonUI.States;
using BlazorBoilerplate.Shared.AuthorizationDefinitions;

using Blazor.Fluxor;
using MatBlazor;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;

using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace BlazorBoilerplate.Client
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthorizationCore(config =>
            {
                config.AddPolicy(Policies.IsAdmin, Policies.IsAdminPolicy());
                config.AddPolicy(Policies.IsUser, Policies.IsUserPolicy());
                config.AddPolicy(Policies.IsReadOnly, Policies.IsUserPolicy());
               // config.AddPolicy(Policies.IsMyDomain, Policies.IsMyDomainPolicy());  Only works on the server end
            });
            services.AddScoped<AuthenticationStateProvider, IdentityAuthenticationStateProvider>();
            services.AddScoped<IAuthorizeApi, AuthorizeApi>();
            services.AddLoadingBar();
            services.Add(new ServiceDescriptor(typeof(IUserProfileApi), typeof(UserProfileApi), ServiceLifetime.Scoped));
            services.AddScoped<AppState>();
            services.AddMatToaster(config =>
            {
                config.Position = MatToastPosition.BottomRight;
                config.PreventDuplicates = true;
                config.NewestOnTop = true;
                config.ShowCloseButton = true;
                config.MaximumOpacity = 95;
                config.VisibleStateDuration = 3000;
            });

            services.AddFluxor(options =>
            {
                options.UseDependencyInjection(typeof(Startup).Assembly);
                options.AddMiddleware<Blazor.Fluxor.ReduxDevTools.ReduxDevToolsMiddleware>();
            });
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            //WebAssemblyHttpMessageHandler.DefaultCredentials = FetchCredentialsOption.Include; //Not sure if we need this https://github.com/aspnet/AspNetCore/issues/17115 Preview4
            app.UseLoadingBar();
            app.AddComponent<App>("app");
        }
    }
}
