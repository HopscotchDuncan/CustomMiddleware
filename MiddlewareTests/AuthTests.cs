using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using System.Net;
using CustomMiddleware.Middleware;

namespace MiddlewareTests
{
    public class AuthTests : IAsyncLifetime
    {
        IHost? host;
        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task InitializeAsync()
        {
            host = await new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .ConfigureServices(services =>
                        {
                        })
                        .Configure(app =>
                        {
                            app.UseMiddleware<MyAuth>();
                        });
                })
                .StartAsync();
        }

        [Fact]
        public async Task MiddlewareTest_NoCredentials()
        {
            var response = await host.GetTestClient().GetAsync("/");
            Assert.NotEqual(HttpStatusCode.NotFound, response.StatusCode);
            var result = await response.Content.ReadAsStringAsync();
            Assert.Equal("Not Authorized.", result);
        }

        [Fact]
        public async Task MiddlewareTest_RightCredentials()
        {
            var response = await host.GetTestClient().GetAsync("/?username=user1&password=password1");
            Assert.NotEqual(HttpStatusCode.NotFound, response.StatusCode);
            var result = await response.Content.ReadAsStringAsync();
            Assert.Equal("Authorized.", result);
        }

        [Fact]
        public async Task MiddlewareTest_WrongCredentials()
        {
            var response = await host.GetTestClient().GetAsync("/?username=user1");
            Assert.NotEqual(HttpStatusCode.NotFound, response.StatusCode);
            var result = await response.Content.ReadAsStringAsync();
            Assert.Equal("Not Authorized.", result);
        }

        [Fact]
        public async Task MiddlewareTest_WrongCredentials2()
        {
            var response = await host.GetTestClient().GetAsync("/?username=user5&password=password2");
            Assert.NotEqual(HttpStatusCode.NotFound, response.StatusCode);
            var result = await response.Content.ReadAsStringAsync();
            Assert.Equal("Not Authorized.", result);
        }
    }
}