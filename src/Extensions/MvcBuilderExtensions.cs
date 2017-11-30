using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Picnic.Options;
using Picnic.SimpleAuth.Areas.Picnic.Controllers;

namespace Picnic.SimpleAuth.Extensions
{
    public static class MvcBuilderExtensions
    {
        /// <summary>
        /// Specifies that Picnic Simple Auth should be used integrating the component with Mvc
        /// </summary>
        /// <param name="builder">The IMvcBuilder</param>
        /// <param name="prefix">The route prefix to be used</param>
        /// <returns>The IMvcBuilder</returns>
        public static IMvcBuilder UsePicnicSimpleAuth(this IMvcBuilder builder, string prefix = "picnic/")
        {
            builder.Services.Configure<MvcOptions>(opts => opts.Conventions.Insert(0, new PicnicPrefixAppModelConvention(prefix, typeof(AuthController).Namespace)));
            return builder.AddRazorOptions(x => x.FileProviders.Add(new EmbeddedFileProvider(typeof(MvcBuilderExtensions).GetTypeInfo().Assembly)));
        }
    }
}