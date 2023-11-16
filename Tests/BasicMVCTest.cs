using CourseWork;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
namespace CourseWork.Tests;
using System;

public class BasicTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public BasicTests(WebApplicationFactory<Program> factory)
    {
        string contentRootPath = Environment.CurrentDirectory;
        factory = factory.WithWebHostBuilder(builder => builder.UseContentRoot(contentRootPath));
        _factory = factory;
    }
    [Theory]
    [InlineData("/")]
    [InlineData("/Home/Privacy")]
    [InlineData("/Flight/List")]
    [InlineData("/Account/Login")]
    [InlineData("/Account/Signup")]
    public async Task EndpointsCorrect(string url)
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(url);

        response.EnsureSuccessStatusCode();
        Assert.Equal("text/html; charset=utf-8",
            response.Content.Headers.ContentType.ToString());
    }
}