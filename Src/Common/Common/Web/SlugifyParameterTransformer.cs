using Microsoft.AspNetCore.Routing;
using System.Globalization;
using System.Text.RegularExpressions;

namespace AI.Common.Web;

public class SlugifyParameterTransformer : IOutboundParameterTransformer
{
    public string TransformOutbound(object value)
    {
        // Slugify value
        return value == null
            ? null
            : Regex.Replace(value.ToString() ?? string.Empty, "([a-z])([A-Z])", "$1-$2").ToLower(CultureInfo.CurrentCulture);
    }
}