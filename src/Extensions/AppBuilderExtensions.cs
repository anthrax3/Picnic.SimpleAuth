using Microsoft.AspNetCore.Builder;

namespace Picnic.SimpleAuth.Extensions
{
    public static class AppBuilderExtensions
    {
        public static IApplicationBuilder UsePicnicSimpleAuth(this IApplicationBuilder app, CookiePolicyOptions cookiePolicyOptions = null)
        {
            app.UseCookiePolicy(cookiePolicyOptions ?? new CookiePolicyOptions());
            return app;
        }
    }
}