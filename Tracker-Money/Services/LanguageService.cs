using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using System.Globalization;
using Tracker_Money.Resources;

namespace Tracker_Money.Services
{
    public interface ILanguageService
    {
        /// <summary>
        /// Gets the current culture information
        /// </summary>
        CultureInfo CurrentCulture { get; }

        /// <summary>
        /// Gets the current UI culture information
        /// </summary>
        CultureInfo CurrentUICulture { get; }

        /// <summary>
        /// Gets a value indicating whether the current culture is RTL
        /// </summary>
        bool IsRightToLeft { get; }

        /// <summary>
        /// Gets the list of supported cultures
        /// </summary>
        IList<CultureInfo> SupportedCultures { get; }

        /// <summary>
        /// Sets the culture for the current request
        /// </summary>
        /// <param name="culture">Culture to set</param>
        void SetLanguage(string culture);

        /// <summary>
        /// Gets a localized string resource
        /// </summary>
        /// <param name="key">Resource key</param>
        /// <returns>Localized string</returns>
        string GetLocalizedString(string key);
    }

    public class LanguageService : ILanguageService
    {
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LanguageService(
            IStringLocalizer<SharedResource> localizer,
            IHttpContextAccessor httpContextAccessor)
        {
            _localizer = localizer;
            _httpContextAccessor = httpContextAccessor;
        }

        public CultureInfo CurrentCulture => CultureInfo.CurrentCulture;

        public CultureInfo CurrentUICulture => CultureInfo.CurrentUICulture;

        public bool IsRightToLeft => CurrentUICulture.TextInfo.IsRightToLeft;

        public IList<CultureInfo> SupportedCultures => new List<CultureInfo>
        {
            new CultureInfo("en"),
            new CultureInfo("ar")
        };

        public void SetLanguage(string culture)
        {
            if (string.IsNullOrEmpty(culture))
                return;

            // Validate that the culture is supported
            var supportedCulture = SupportedCultures.FirstOrDefault(c => c.Name == culture);
            if (supportedCulture == null)
                return;

            // Set the cookie with the selected culture
            var cookieValue = CookieRequestCultureProvider.MakeCookieValue(
                new RequestCulture(supportedCulture, supportedCulture));

            var cookie = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                SameSite = SameSiteMode.Lax
            };

            _httpContextAccessor.HttpContext?.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                cookieValue,
                cookie);
        }

        public string GetLocalizedString(string key)
        {
            if (string.IsNullOrEmpty(key))
                return string.Empty;

            return _localizer[key];
        }
    }
}