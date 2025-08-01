using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using T.TNet;

namespace T.Pages
{
    public class DashboardModel : PageModel
    {
        public int curInstanceId { get; private set; }

        public IActionResult OnGet()
        {
            return RedirectToPage("Index");
            /*

            if (int.TryParse(Request.Cookies["Dashboard/curInstanceId"], out int result)) curInstanceId = result;
            else curInstanceId = 0;

            return Page();

            string? val = Request.Cookies["password"];

            if (val == "myPass") return Page();
            else return RedirectToPage("Index");*/
        }

        public IActionResult OnPostChangeInstance(string instanceId)
        {
            if (!int.TryParse(instanceId, out _)) return Page();

            CookieOptions options = new()
            {
                Expires = DateTimeOffset.Now.AddDays(1)
            };

            Response.Cookies.Append("Dashboard/curInstanceId", instanceId, options);

            return Page();
        }

        public IActionResult OnPostCreateInstance(string port, string name, string gameId)
        {
            if (!int.TryParse(port, out int portParse)) Debug.Write("test");
            if (!int.TryParse(gameId, out int gameIdParse)) Console.Write("test");

            Console.Write("test");

            TNetInstanceManager.CreateInstance(portParse, gameIdParse).name = name;

            return Page();
        }
    }
}
