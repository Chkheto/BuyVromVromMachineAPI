namespace MyVromVromMachineAPI.models;

public static class Roles
{
    public const string Buyer = "Buyer";
    public const string Seller = "Seller";
    public const string Admin = "Admin";

    private static readonly string[] AdminEmails =
    {
        "chkhetianisandro@gmail.com",
        "gegagego68@gmail.com"
    };

    public static bool IsAdminEmail(string? email) =>
        email != null && AdminEmails.Contains(email.Trim(), StringComparer.OrdinalIgnoreCase);
}