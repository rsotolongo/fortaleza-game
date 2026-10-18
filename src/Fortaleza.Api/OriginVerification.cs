using System.Security.Cryptography;
using System.Text;

namespace Fortaleza.Api;

/// <summary>
/// Only serves requests that carry the shared secret CloudFront adds (X-Origin-Verify).
/// It fails closed: inside AWS Lambda a missing secret rejects every request instead of disabling the check.
/// </summary>
public static class OriginVerification
{
    public const string HeaderName = "X-Origin-Verify";

    public static IApplicationBuilder UseOriginVerification(this IApplicationBuilder app, IConfiguration configuration)
    {
        var secret = configuration["ORIGIN_VERIFY"] ?? "";
        var insideLambda = !string.IsNullOrEmpty(configuration["AWS_LAMBDA_FUNCTION_NAME"]);
        if (secret == "" && !insideLambda)
            return app;

        var expected = Encoding.UTF8.GetBytes(secret);
        return app.Use(async (context, next) =>
        {
            var provided = Encoding.UTF8.GetBytes(context.Request.Headers[HeaderName].ToString());
            var valid = expected.Length > 0 && CryptographicOperations.FixedTimeEquals(provided, expected);
            if (!valid)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            }

            await next();
        });
    }
}
