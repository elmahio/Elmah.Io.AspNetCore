using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace Elmah.Io.AspNetCore.TagHelpers
{
    public class ElmahIoTagHelper(IOptions<ElmahIoOptions> options) : TagHelper
    {
        public ElmahIoOptions Options { get; } = options.Value;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "script";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.SetAttribute("src", $"https://cdn.jsdelivr.net/gh/elmahio/elmah.io.javascript@4.1.0/dist/elmahio.min.js?apiKey={Options.ApiKey}&logId={Options.LogId}");
        }
    }
}
