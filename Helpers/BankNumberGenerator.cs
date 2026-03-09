public static class BankNumberGenerator
{
    public static string GenerateAccountNumber()
    {
        string rndNumber = Random.Shared.Next(900000000, 1000000000).ToString();
        return "dK-JoNaS-" + rndNumber;
    }
    public static string GenerateCustomerNumber()
    {
        string guid = Guid.NewGuid().ToString();
        string partialGuid = guid.Substring(guid.Length - 12);
        return "mac-jonas-" + partialGuid;
    }
}
