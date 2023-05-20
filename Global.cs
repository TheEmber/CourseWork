using Microsoft.Extensions.Configuration;
namespace CourseWork;
public static class GlobalVariables
{
    public static readonly string DbPath = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("appsettings")["LocalDb"];
    public const string AuthTypeScheme = "TickNTripCookieScheme";
}