using Microsoft.AspNetCore.Mvc;
using Tracker_Money.Services;

namespace Tracker_Money.Controllers
{
    public class LanguageController : Controller
    {
        private readonly ILanguageService _languageService;

        public LanguageController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        /// <summary>
        /// Changes the current language and redirects back to the referring page
        /// </summary>
        /// <param name="culture">The culture code (e.g., "en" or "ar")</param>
        /// <param name="returnUrl">The URL to return to after changing the language</param>
        /// <returns>Redirect to the referring page or home page</returns>
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            // Set the selected culture in the cookie
            _languageService.SetLanguage(culture);

            // Redirect to the referring URL or home page if not specified
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = Url.Action("Index", "Home");
            }

            return LocalRedirect(returnUrl);
        }
    }
}