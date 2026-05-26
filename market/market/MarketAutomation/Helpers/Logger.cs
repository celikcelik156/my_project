using MarketAutomation.Data;
using MarketAutomation.Models;
using System;

namespace MarketAutomation.Helpers
{
    public static class ActiveUser
    {
        public static int? Id { get; set; }
        public static string Username { get; set; } = string.Empty;
        public static string FullName { get; set; } = string.Empty;
        public static string Role { get; set; } = string.Empty;
        
        public static bool IsAdmin => Role == "Admin";
    }

    public static class Logger
    {
        public static void LogActivity(string actionType, string description)
        {
            try
            {
                using (var context = new MarketDbContext())
                {
                    var log = new ActivityLog
                    {
                        ActionType = actionType,
                        Description = description,
                        LogDate = DateTime.Now,
                        UserId = ActiveUser.Id
                    };
                    context.ActivityLogs.Add(log);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Loglama hatası: {ex.Message}");
            }
        }
    }
}
