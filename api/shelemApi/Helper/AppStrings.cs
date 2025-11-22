namespace shelemApi.Helper;
public static class AppStrings
{
    public static IConfiguration ConfigureAppStrings(this IConfiguration configuration)
    {
        ApiKey = configuration.GetSection("apiKey").Value;
        ApiUrl = configuration.GetSection("apiUrl").Value;
        MainUrl = configuration.GetSection("mainUrl").Value;
        ClientUrl = configuration.GetSection("clientUrl").Value;
        return configuration;
    }

    public static string ApiKey { get; set; }
    public static string ApiUrl { get; set; }
    public static string MainUrl { get; set; }
    public static string ClientUrl { get; set; }

}
