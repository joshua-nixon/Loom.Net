using System.Web;
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
    public void TryGetDownstream_MergesQueryStringWithTemplateTakingPrecedence()
    {
        var matcher = LoomDownstreamMatcherHelper.CreateFromRoutes(
            ("users/{user-id}", "v1/users/{user-id}?id={user-id}&view=full")
        );

        var context = new DefaultHttpContext();
        context.Request.Path = "/api/users/42";
        context.Request.QueryString = new QueryString("?id=99&lang=en&tag=one&tag=two");

        var matched = matcher.TryGetDownstream(context, out var route);

        Assert.True(matched);
        Assert.NotNull(route);

        var downstreamQuery = HttpUtility.ParseQueryString(route.DownstreamUri.Query);

        Assert.Equal("42", downstreamQuery["id"]);
        Assert.Equal("full", downstreamQuery["view"]);
        Assert.Equal("en", downstreamQuery["lang"]);
        Assert.Equal(["one", "two"], downstreamQuery.GetValues("tag") ?? []);
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
        context.Request.Path = "/api/users/jeff";

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
