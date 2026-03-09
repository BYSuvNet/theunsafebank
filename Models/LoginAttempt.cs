using System;
using System.Runtime.InteropServices.Marshalling;
namespace theunsafebank.Models;
public class LoginAttempt
{
    public int Id {get;set;}
    public string? Username { get; set; }
    public DateTime LoginTime { get; set; } = DateTime.Now;
    public bool IsSuccess { get; set; }
}