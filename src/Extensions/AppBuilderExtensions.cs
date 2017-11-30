using Microsoft.AspNetCore.Builder;

namespace Picnic.SimpleAuth.Extensions
{
    public static class AppBuilderExtensions
    {
        /// <summary>
        /// Specifies that Picnic Simple Auth should be used - sets up the cookie policy
        /// </summary>
        /// <param name="app">The IApplicationBuilder</param>
        /// <param name="cookiePolicyOptions">Cookie policy options</param>
        /// <returns>The IApplicationBuilder</returns>
        public static IApplicationBuilder UsePicnicSimpleAuth(this IApplicationBuilder app, CookiePolicyOptions cookiePolicyOptions = null)
        {
            app.UseCookiePolicy(cookiePolicyOptions ?? new CookiePolicyOptions());
            return app;
        }
    }
}