using Microsoft.AspNetCore.Builder;

namespace LastStand.Dashboard;
public static class DashboardExtensions
{
    public static void SetupDashboard(this WebApplication app)
    {
        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.MapFallbackToFile("/index.html");
    }
}
