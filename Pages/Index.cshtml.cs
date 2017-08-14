using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Text;
using System.Collections.Generic;

namespace RazorMinification
{
    public class IndexModel : PageModel
    {
        public string DT { get; private set; } = DateTime.Now.ToString();

        public List<string> People { get; private set; } = new List<string>() { "Susan", "Catalina", "Diego", "Bob" };

        public void OnGet()
        {
        }
    }
}