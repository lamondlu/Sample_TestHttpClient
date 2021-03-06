using FluentAssertions;
using HttpClientApp.Mock;
using HttpClientApp.Models;
using HttpClientApp.Services;
using Newtonsoft.Json;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HttpClientApp.Test
{
    public class UserServiceTests
    {
        [Fact]
        public async Task WhenACorrectUrlIsProvided_ServiceShouldReturnAlistOfUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User
                {
                    FirstName = "John",
                    LastName = "Doe"
                },
                new User
                {
                    FirstName = "John",
                    LastName = "Deere"
                }
            };

            var httpClientFactoryMock = Substitute.For<IHttpClientFactory>();
            var url = "http://good.uri";
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(users), Encoding.UTF8, "application/json")
            });

            var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);
            httpClientFactoryMock.CreateClient().Returns(fakeHttpClient);

            // Act
            var service = new UserService(httpClientFactoryMock);
            var result = await service.GetUsers(url);

            // Assert
            result
            .Should()
            .BeOfType<List<User>>()
            .And
            .HaveCount(2)
            .And
            .Contain(x => x.FirstName == "John")
            .And
            .Contain(x => x.LastName == "Deere")
            .And
            .Contain(x => x.LastName == "Doe");
        }

        [Fact]
        public async Task WhenABadUrlIsProvided_ServiceShouldReturnNull()
        {
            // Arrange
            var httpClientFactoryMock = Substitute.For<IHttpClientFactory>();
            var url = "http://bad.uri";
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.NotFound
            });
            var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);

            httpClientFactoryMock.CreateClient().Returns(fakeHttpClient);

            // Act
            var service = new UserService(httpClientFactoryMock);
            var result = await service.GetUsers(url);

            // Assert
            result
            .Should()
            .BeNullOrEmpty();
        }
    }
}
