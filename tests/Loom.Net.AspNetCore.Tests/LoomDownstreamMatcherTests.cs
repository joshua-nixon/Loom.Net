using Loom.Net.AspNetCore.Exceptions;
using Loom.Net.AspNetCore.Tests.Helpers;
using Microsoft.AspNetCore.Http;

namespace Loom.Net.AspNetCore.Tests;

public sealed class LoomDownstreamMatcherTests
{
    [Fact]
    public void TryGetDownstream_MatchesPrefixedRouteAndPreservesQueryString()
    {
        var matcher = LoomDownstreamMatcherHelper.CreateFromRoutes(
            ("users/{user-id}", "v1/users/{user-id}")
        );

        var context = new DefaultHttpContext();

        context.Request.Path = "/api/users/42";
        context.Request.QueryString = new QueryString("?include=orders");

        var matched = matcher.TryGetDownstream(context, out var route);

        Assert.True(matched);
        Assert.NotNull(route);
        Assert.Same(context, route.Context);
        Assert.Equal(new Uri("http://localhost:8080/v1/users/42?include=orders"), route.DownstreamUri);
    }

    [Fact]
    public void TryGetDownstream_DoesNotMatchPartialPath()
    {
        var matcher = LoomDownstreamMatcherHelper.CreateFromRoutes(
            ("users/{user-id}", "v1/users/{user-id}")
        );

        var context = new DefaultHttpContext();

        context.Request.Path = "/api/users/42/extra";

        var matched = matcher.TryGetDownstream(context, out var route);

        Assert.Null(route);
        Assert.False(matched);
    }

    [Fact]
    public void TryGetDownstream_ThrowsWhenMultipleRoutesMatch()
    {
        var matcher = LoomDownstreamMatcherHelper.CreateFromRoutes(
            ("users/{user-id}", "v1/users/{user-id}"),
            ("users/jeff", "v1/users/jeff")
        );

        var context = new DefaultHttpContext();

        context.Request.Path = "/api/users/special";

        Assert.Throws<LoomAspNetCoreInvalidOperationException>(() => matcher.TryGetDownstream(context, out _));
    }

    [Fact]
    public void TryGetDownstream_ThrowsWhenRequestPathIsMissing()
    {
        var matcher = LoomDownstreamMatcherHelper.CreateFromRoutes(
            ("users/{user-id}", "v1/users/{user-id}")
        );
        
        var context = new DefaultHttpContext();

        Assert.Throws<LoomAspNetCoreInvalidOperationException>(() => matcher.TryGetDownstream(context, out _));
    }
}
