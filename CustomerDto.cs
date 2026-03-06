public class CustomerDto
{
    public string Username { get; set; }
    public string Password { get; set; } // Plain text password - INSECURE!
    public string FullName { get; set; }
    public string CustomerNum { get; set; }
    // Navigation property
}