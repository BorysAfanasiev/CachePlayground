using System.Net.Http.Json;
using System.Text.Json;

namespace IntegrationTests.Fixtures.Tools;

internal class HttpClientTestFacade : IDisposable
{
    private readonly HttpClient Client;

    public HttpClientTestFacade(HttpClient client)
    {
        ArgumentNullException.ThrowIfNull(client, nameof(client));

        Client = client;
    }

    public async Task<HttpResponse<TResponse>> GetAsync<TResponse>(string uri, CancellationToken cancellationToken)
    {
        var response = await this.Client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        await using var content = await response.Content.ReadAsStreamAsync(cancellationToken);

        return new(response.StatusCode, await JsonSerializer.DeserializeAsync<TResponse>(content, options: null, cancellationToken));
    }

    public async Task<HttpResponse<TResponse>> PostAsync<TRequest, TResponse>(string uri, TRequest request, CancellationToken cancellationToken)
    {
        var response = await this.Client.PostAsJsonAsync(uri, request, cancellationToken);
        await using var content = await response.Content.ReadAsStreamAsync(cancellationToken);

        return new(response.StatusCode, await JsonSerializer.DeserializeAsync<TResponse>(content, options: null, cancellationToken));
    }


    //TODO: PUT, DELETE, PATCH
    public void Dispose()
    {
        Client.Dispose();
    }
}