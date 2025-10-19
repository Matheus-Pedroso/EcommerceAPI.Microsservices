namespace Mango.Web.Utility;

public class StaticDetails
{
    public static string CouponAPIBase { get; set; } = string.Empty;
    public static string AuthAPIBase { get; set; } = string.Empty;
    public static string ProductAPIBase { get; set; } = string.Empty;
    public static string CartAPIBase { get; set; } = string.Empty;
    public static string OrderAPIBase { get;set; } = string.Empty;

    public const string RoleAdmin = "ADMIN";
    public const string RoleCustomer = "CUSTOMER";
    public const string TokenCookie = "JWTToken";
    public enum ApiType
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public const string Status_Pending = "Pending";
    public const string Status_Approved = "Approved";
    public const string Status_ReadyForPickup = "ReadyForPickup";
    public const string Status_Completed = "Completed";
    public const string Status_Refunded = "Refunded";
    public const string Status_Cancelled = "Cancelled";
}
