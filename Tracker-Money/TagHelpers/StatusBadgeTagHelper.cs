using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Tracker_Money.TagHelpers
{
    [HtmlTargetElement("status-badge")]
    public class StatusBadgeTagHelper : TagHelper
    {
        /// <summary>
        /// The status value to display in the badge
        /// </summary>
        [HtmlAttributeName("value")]
        public string Value { get; set; }

        /// <summary>
        /// Optional custom CSS class to add to the badge
        /// </summary>
        [HtmlAttributeName("class")]
        public string CssClass { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            
            // Get the appropriate badge class based on status
            var badgeClass = GetBadgeClass(Value);
            
            // Set the CSS class attribute
            output.Attributes.SetAttribute("class", $"{badgeClass} {CssClass}".Trim());
            
            // Set the content of the badge
            output.Content.SetContent(Value);
        }

        /// <summary>
        /// Maps status values to Bootstrap badge classes
        /// </summary>
        /// <param name="status">The status value</param>
        /// <returns>The appropriate Bootstrap badge class</returns>
        private string GetBadgeClass(string status)
        {
            return status?.ToLower() switch
            {
                "draft" => "badge bg-secondary",
                "active" => "badge bg-success",
                "suspended" => "badge bg-warning",
                "completed" => "badge bg-info",
                "cancelled" => "badge bg-danger",
                "defaulted" => "badge bg-dark",
                "paid" => "badge bg-success",
                "overdue" => "badge bg-danger",
                "pending" => "badge bg-warning",
                "upcoming" => "badge bg-info",
                _ => "badge bg-secondary"
            };
        }
    }
}