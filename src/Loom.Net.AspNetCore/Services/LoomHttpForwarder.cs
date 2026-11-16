using System.Net;
using Loom.Net.AspNetCore.Models;

namespace Loom.Net.AspNetCore.Services
{
    internal sealed class LoomHttpForwarder : IDisposable
    {
        private readonly HttpMessageInvoker _messageInvoker;

        public LoomHttpForwarder()
        {
            var handler = new SocketsHttpHandler
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.None,
                UseCookies = false,
            };

            _messageInvoker = new HttpMessageInvoker(handler, disposeHandler: true);
        }

        public async Task ForwardAsync(MatchedDownstreamRoute downstreamRoute)
        {
            var request = downstreamRoute.Context.Request;
            var cancellationToken = downstreamRoute.Context.RequestAborted;

            using var proxyRequest = new HttpRequestMessage(new HttpMethod(request.Method), downstreamRoute.DownstreamUri);
            using var proxyResponse = await _messageInvoker.SendAsync(proxyRequest, cancellationToken);

            downstreamRoute.Context.Response.StatusCode = (int)proxyResponse.StatusCode;

            await proxyResponse.Content.CopyToAsync(downstreamRoute.Context.Response.Body, cancellationToken);
        }

        public void Dispose()
        {
            _messageInvoker?.Dispose();
        }
    }
}