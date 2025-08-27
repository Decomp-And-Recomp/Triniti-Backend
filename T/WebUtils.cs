namespace T;

public class WebUtils
{
    public static string? GetIp(HttpContext context)
    {
        string? ip = context.Request.Headers["HTTP_X_FORWARDED_FOR"].FirstOrDefault();

        if (string.IsNullOrEmpty(ip))
        {
            var ipAddr = context.Connection.RemoteIpAddress;

            if (ipAddr == null) return null;

            ip = ipAddr.IsIPv4MappedToIPv6 ? ipAddr.MapToIPv4().ToString() : ipAddr.ToString();
        }

        return ip;
    }
}
