using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace T.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            /*CookieOptions options = new();
            options.Expires = DateTimeOffset.Now.AddMinutes(5);

            Response.Cookies.Append("password", "myPass", options);*/
        }
    }
}
