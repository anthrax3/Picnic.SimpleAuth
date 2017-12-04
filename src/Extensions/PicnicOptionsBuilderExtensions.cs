using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Picnic.Model;
using Picnic.Options;
using Picnic.Service;
using Picnic.SimpleAuth.Areas.Picnic.Controllers;
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

            var picnicOptionsProvider = (IOptions<PicnicOptions>)services.BuildServiceProvider().GetService(typeof(IOptions<PicnicOptions>));
            var effectivePicnicOptions = picnicOptionsProvider.Value;

            picnicOptionsBuilder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options ?? (opts =>
                {
                    opts.Cookie.Name = "picnic-auth";
                    opts.LoginPath = $"/{effectivePicnicOptions.Manage.RoutePrefix}/login";
                }));

            // Tie Policy to Role Claim
            picnicOptionsBuilder.Services.AddAuthorization(opts => opts.AddPolicy("PicnicAuthPolicy", policyOptions =>
            {
                policyOptions.RequireRole("PicnicUser");
                policyOptions.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme);
                policyOptions.Build();
            }));

            // Use Page Store Binding to determine how to bind UserStore
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

            // Route Prefix
            services.Configure<MvcOptions>(opts => opts.Conventions.Insert(0, new PicnicPrefixAppModelConvention(effectivePicnicOptions.Manage.RoutePrefix, typeof(AuthController).Namespace)));

            // View Location Expanders
            services.Configure<RazorViewEngineOptions>(razorViewEngineOptions => razorViewEngineOptions.FileProviders.Add(new EmbeddedFileProvider(typeof(Picnic.SimpleAuth.Model.User).GetTypeInfo().Assembly)));
            
            picnicOptionsBuilder.Services.AddScoped<IUserService, DefaultUserService>();

            return picnicOptionsBuilder;
        }

    }
}