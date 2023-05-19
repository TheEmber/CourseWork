using Microsoft.Extensions.Configuration;
namespace CourseWork;
public static class GlobalVariables
{
    public static string DbPath = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("appsettings")["LocalDb"];
}