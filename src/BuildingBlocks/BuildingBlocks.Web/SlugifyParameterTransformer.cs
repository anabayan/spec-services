using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Routing;

namespace BuildingBlocks.Web;

public class SlugifyParameterTransformer : IOutboundParameterTransformer
{
    /// <summary>
    ///     eg. takes "Hello World" and returns "hello-world"
    ///     or takes "Tracer Observation Site" and returns "tracer-observation-site".
    /// </summary>
    public string TransformOutbound(object value)
    {
        // Slugify value
        return value == null
            ? null
            : Regex.Replace(value.ToString() ?? string.Empty, "([a-z])([A-Z])", "$1-$2").ToLower();
    }
}
