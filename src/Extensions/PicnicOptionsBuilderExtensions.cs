using System;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Picnic.Model;
using Picnic.Options;
using Picnic.Service;
using Picnic.SimpleAuth.Model;
using Picnic.SimpleAuth.Service;
using Picnic.Stores;
using Picnic.Stores.EF;
using Picnic.Stores.Json;

namespace Picnic.SimpleAuth.Extensions
{
    public static class PicnicOptionsBuilderExtensions
    {
        /// <summary>
        /// Specifies that Picnic Simple Auth will be used with Picnic
        /// </summary>
        /// <param name="picnicOptionsBuilder">PicnicOptionsBuilder</param>
        /// <param name="options">Cookie authentication options</param>
        /// <returns>PicnicOptionsBuilder with specified options</returns>
        public static PicnicOptionsBuilder UseSimpleAuth(this PicnicOptionsBuilder picnicOptionsBuilder, Action<CookieAuthenticationOptions> options = null)
        {
            var services = picnicOptionsBuilder.Services;

            var picnicOptions = services.BuildServiceProvider().GetService(typeof(IOptions<PicnicOptions>));

            picnicOptionsBuilder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options ?? (opts =>
                {
                    opts.Cookie.Name = "picnic-auth";
                    opts.LoginPath = "/picnic/login";
                }));

            // Setup Authorization for Picnic
            picnicOptionsBuilder.Services.AddAuthorization(opts => opts.AddPolicy("PicnicAuthPolicy", policyOptions =>
            {
                // NOTE: This example allows anonymous access to the Picnic interface
                // Add your application's policy specifics to control access to the Picnic interface
                policyOptions.RequireRole("PicnicUser");
                policyOptions.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme);
                policyOptions.Build();
            }));

            var pageStoreBinding = picnicOptionsBuilder.Services.FirstOrDefault(x => x.ServiceType == typeof(IGenericStore<Page>));
            if (pageStoreBinding.ImplementationType == typeof(GenericJsonStore<Page>))
            {
                picnicOptionsBuilder.Services.AddScoped<IGenericStore<User>, GenericJsonStore<User>>();
            }
            else
            {
                // Determine the Context Type so that we can automagically use it to create a generic user store
                // that uses the same context type
                var contextType = pageStoreBinding.ImplementationType.GenericTypeArguments[1];
                var implementationType = typeof(GenericEFStore<,>).MakeGenericType(typeof(User), contextType);

                picnicOptionsBuilder.Services.AddScoped(typeof(IGenericStore<User>), implementationType);
            }

            picnicOptionsBuilder.Services.AddScoped<IUserService, DefaultUserService>();

            return picnicOptionsBuilder;
        }

    }
}