using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;
using Tracker_Money.Resources;

namespace Tracker_Money.TagHelpers
{
    /// <summary>
    /// Tag helper for localizing content in views
    /// Usage: <t localize-key="KeyName">Default Text</t>
    /// </summary>
    [HtmlTargetElement("t")]
    public class LocalizeTagHelper : TagHelper
    {
        private readonly IStringLocalizer<SharedResource> _localizer;

        public LocalizeTagHelper(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;
        }

        /// <summary>
        /// The resource key to look up
        /// </summary>
        [HtmlAttributeName("localize-key")]
        public string Key { get; set; }

        /// <summary>
        /// Process the tag helper
        /// </summary>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // Remove the tag from the output
            output.TagName = null;

            // If no key is provided, use the content as the key
            if (string.IsNullOrEmpty(Key))
            {
                // Get the content of the tag
                var content = output.GetChildContentAsync().Result.GetContent().Trim();
                if (!string.IsNullOrEmpty(content))
                {
                    Key = content;
                }
            }

            // Look up the localized string
            if (!string.IsNullOrEmpty(Key))
            {
                var localizedString = _localizer[Key];
                output.Content.SetHtmlContent(localizedString);
            }
        }
    }
}