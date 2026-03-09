using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public IActionResult Login(string username, string password)
    {
        var customer = _context.Customers
            .FirstOrDefault(c => c.Username == username);

        if (customer != null && BCrypt.Net.BCrypt.Verify(password, customer.PasswordHashed))
        {
            Response.Cookies.Append("CustomerId", customer.Id.ToString());
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

        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        if (hashedPassword.Length < 8)
        {
            ViewBag.Error = "Lösenordet måste vara 8 tecken långt";
            return View();
        }

        if (!hashedPassword.Any(char.IsNumber) && !hashedPassword.Any(char.IsSymbol))
        {
            ViewBag.Error = "Lösenordet måste innehålla siffror och specialtecken";
            return View();
        }

        if (!hashedPassword.Any(char.IsUpper))
        {
            ViewBag.Error = "Lösenordet måste innehålla storbokstav";
            return View();
        }

        if (hashedPassword == username)
        {
            ViewBag.Error = "Lösenordet får inte vara samma som användarnamn";
            return View();
        }

        var customer = new Customer
        {
            Username = username,
            PasswordHashed = hashedPassword,
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

        Response.Cookies.Append("CustomerId", customer.Id.ToString());
        return RedirectToAction("Dashboard", "Account");
    }

    public IActionResult Logout()
    {
        Response.Cookies.Delete("CustomerId");
        return RedirectToAction("Login");
    }
}
