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
			.FirstOrDefault(c => c.Username == username && c.Password == password);

		if (customer != null)
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

		string? customerNumber = SetCustomerNumber();

		if (customerNumber == null)
		{
			ViewBag.Error = "Misslyckades att generera kundnummer, var god försök igen.";
			return View();
		}

		var customer = new Customer
		{
			Username = username,
			Password = password,
			FullName = fullName,
			CustomerNumber = customerNumber
		};

		_context.Customers.Add(customer);
		_context.SaveChanges();

		string identifier = "dK-JoNaS-";
		string rndNumber = Random.Shared.Next(900000000, 1000000000).ToString();
		string accountNumber = identifier + rndNumber;

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

	public string? SetCustomerNumber()
	{
		int maxAttempts = 25;
		int attempts = 0;

		while (attempts <= maxAttempts)
		{
			var guid = Guid.NewGuid().ToString();
			string partialGuid = guid.ToString().Substring(guid.Length - 12);

			string customerNumber = "mac-jonas-" + partialGuid;
			customerNumber = "mac-jonas-c1a9284bef8d";

			if (!_context.Customers.Any(c => c.CustomerNumber == customerNumber))
			{
				return customerNumber;
			}

			attempts++;
		}

		return null;
	}
}
