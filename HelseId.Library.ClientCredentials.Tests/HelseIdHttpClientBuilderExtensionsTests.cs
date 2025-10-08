using System.Reflection;
using HelseId.Library.ClientCredentials.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HelseId.Library.ClientCredentials.Tests;

[TestFixture]
public class HelseIdHttpClientBuilderExtensionsTests
{
    [Test]
    public void AddHelseIdClientCredentials_sets_up_HttpClient_with_HelseIdDPoPDelegatingHandler()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddHttpClient("HelseID").AddHelseIdClientCredentials("scope");

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        var helseIDHttpClient = httpClientFactory.CreateClient("HelseID");

        var httpMessageHandler = GetHttpMessageHandlerFromClient(helseIDHttpClient);
        
        HandlerOrInnerHandlerShouldBeOfType<HelseIdDPoPDelegatingHandler>(httpMessageHandler as DelegatingHandler);
    }

    private static HttpMessageHandler? GetHttpMessageHandlerFromClient(HttpClient httpClient)
    {
        var t = httpClient.GetType().BaseType;
        var f = t.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        var handlerPrivateField = httpClient.GetType().BaseType.GetField("_handler", BindingFlags.NonPublic | BindingFlags.Instance);
        var handler = handlerPrivateField.GetValue(httpClient);
        return handler as HttpMessageHandler;
    }

    private static void HandlerOrInnerHandlerShouldBeOfType<THandler>(DelegatingHandler? delegatingHandler)
    {
        if (delegatingHandler is THandler)
        {
            return;
        }

        if (delegatingHandler != null && delegatingHandler.InnerHandler is DelegatingHandler innerHandler)
        {
            HandlerOrInnerHandlerShouldBeOfType<THandler>(innerHandler);
        }
        else
        {
            Assert.Fail($"Handler or InnerHandler is not of type {typeof(THandler)}");
        }
    }
}
