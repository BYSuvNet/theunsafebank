using Microsoft.AspNetCore.Mvc;
using theunsafebank.Data;
using theunsafebank.Models;

namespace theunsafebank.Controllers;

public class AuthController : Controller
{
    private readonly BankContext _context;

    public AuthController(BankContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string username, string password )

    {
        DateTime oneMinuteAgo = DateTime.Now.AddMinutes(-1);
        int failedAttempts = _context.LoginAttempts
        .Where(l => l.Username == username && l.IsSuccess == false && l.LoginTime >= oneMinuteAgo)
        .Count();

        if (failedAttempts >= 2)
        {
            ViewBag.Error = "Du har slut på försök. Kontakta kundtjänst eller försök om 1 minut.";
            return View();
        }
        var customer = _context.Customers
            .FirstOrDefault(c => c.Username == username && c.Password == password);

          var Logaiattempts = new LoginAttempt
        {
            Username = username,
            IsSuccess = customer != null 
        };

        _context.LoginAttempts.Add(Logaiattempts);
        _context.SaveChanges();

        if (customer != null )
        {
            HttpContext.Session.SetString("customerId", customer.Id.ToString());
            return RedirectToAction("Dashboard", "Account");
        }

        ViewBag.Error = "Invalid username or password";
        return View();
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(string username, string password, string fullName)
    {
        var existingCustomer = _context.Customers.FirstOrDefault(c => c.Username == username);

        if (existingCustomer != null)
        {
            ViewBag.Error = "Username already exists";
            return View();
        }

        if (password.Length < 8)
        {
            ViewBag.Error = "Lösenordet måste vara 8 tecken långt";
            return View();
        }

        if (!password.Any(char.IsNumber) && !password.Any(char.IsSymbol))
        {
            ViewBag.Error = "Lösenordet måste innehålla siffror och specialtecken";
            return View();
        }

        if (!password.Any(char.IsUpper))
        {
            ViewBag.Error = "Lösenordet måste innehålla storbokstav";
            return View();
        }

        if (password == username)
        {
            ViewBag.Error = "Lösenordet får inte vara samma som användarnamn";
            return View();
        }

        var customer = new Customer
        {
            Username = username,
            Password = password,
            FullName = fullName
        };

        _context.Customers.Add(customer);
        _context.SaveChanges();

        var accountNumber = (1000 + customer.Id).ToString();

        var account = new Account
        {
            AccountNumber = accountNumber,
            Balance = 10000m, // 10,000 SEK
            CustomerId = customer.Id
        };

        if (customer.Id % 10 == 0)
        {
            account.Balance += 10000m;
        }

        _context.Accounts.Add(account);
        _context.SaveChanges();

        HttpContext.Session.SetString("customerId", customer.Id.ToString());
        return RedirectToAction("Dashboard", "Account");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Remove("customerId");
        return RedirectToAction("Login");
    }
}
