using Microsoft.Toolkit.Uwp.Notifications;

namespace NovaLauncher.Web.Utils
{
    internal static class Notifications
    {
        public static void ShowToast(string message, string title, ToastDuration toastDuration = ToastDuration.Short)
        {
            try
            {
                new ToastContentBuilder()
                    .AddText(title)
                    .AddText(message)
                    .SetToastDuration(toastDuration)
                    .Show();
            }
            catch
            {
            }
        }
    }
}
