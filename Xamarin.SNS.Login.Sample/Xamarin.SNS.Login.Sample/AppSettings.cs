using System;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using Xamarin.SNS.Login.Sample.Models;
using Xamarin.SNS.Login.Sample.Extensions;

namespace Xamarin.SNS.Login.Sample
{
    public static class AppSettings
    {
        private static ISettings Settings => CrossSettings.Current;

        public static User User
        {
            get => Settings.GetValueOrDefault(nameof(User), default(User));

            set => Settings.AddOrUpdateValue(nameof(User), value);
        }

        public static void RemoveUserData()
        {
            Settings.Remove(nameof(User));
        }
    }
}
