namespace CesiZen.Presentation.BackOffice.Core;
public static partial class Extensions {
    public static bool IsRoot(this IConfiguration configuration, string mailAddress) =>
        !string.IsNullOrEmpty(configuration["Root:MailAddress"]) &&
        string.Equals(mailAddress, configuration["Root:MailAddress"], StringComparison.OrdinalIgnoreCase);
}