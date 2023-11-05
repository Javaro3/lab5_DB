using lab5.Data;


namespace lab5.Middleware {
    public class DbInitializerMiddleware {
        private readonly RequestDelegate _next;

        public DbInitializerMiddleware(RequestDelegate next) {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext, InsuranceCompanyContext db) {
            if (!(httpContext.Session.Keys.Contains("database"))) {
                DbInitializer.Initialize(db);
                httpContext.Session.SetString("database", "initial");
            }
            return _next.Invoke(httpContext);
        }
    }

    public static class DbInitializerMiddlewareExtensions {
        public static IApplicationBuilder UseDbInitializerMiddleware(this IApplicationBuilder builder) {
            return builder.UseMiddleware<DbInitializerMiddleware>();
        }
    }
}
