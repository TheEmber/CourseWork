using CourseWork;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CourseWork.Tests;

public class BasicTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public BasicTests(WebApplicationFactory<Program> factory)
    {
        string baseDirectory = AppContext.BaseDirectory;
        string contentRootPath = Path.Combine(baseDirectory, "../../..");
        factory = factory.WithWebHostBuilder(builder => builder.UseContentRoot(contentRootPath));
        _factory = factory;
    }
    [Theory]
    [InlineData("/")]
    [InlineData("/Home/Privacy")]
    [InlineData("/Flight/List")]
    public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(url);

        response.EnsureSuccessStatusCode();
        Assert.Equal("text/html; charset=utf-8",
            response.Content.Headers.ContentType.ToString());
    }
}