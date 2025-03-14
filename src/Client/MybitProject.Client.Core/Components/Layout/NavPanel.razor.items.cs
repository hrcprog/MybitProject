﻿namespace MybitProject.Client.Core.Components.Layout;

public partial class NavPanel
{
    private void CreateNavItems()
    {
        allNavItems =
        [
            new()
            {
                Text = Localizer[nameof(AppStrings.Home)],
                IconName = BitIconName.Home,
                Url = Urls.HomePage,
            },
            new()
            {
                Text = Localizer[nameof(AppStrings.Todo)],
                IconName = BitIconName.ToDoLogoOutline,
                Url = Urls.TodoPage,
            },
            new()
            {
                Text = Localizer[nameof(AppStrings.Settings)],
                IconName = BitIconName.Equalizer,
                Url = Urls.SettingsPage,
                AdditionalUrls =
                [
                    $"{Urls.SettingsPage}/{Urls.SettingsSections.Profile}",
                    $"{Urls.SettingsPage}/{Urls.SettingsSections.Account}",
                    $"{Urls.SettingsPage}/{Urls.SettingsSections.Tfa}",
                    $"{Urls.SettingsPage}/{Urls.SettingsSections.Sessions}",
                ]
            },
            new()
            {
                Text = Localizer[nameof(AppStrings.Terms)],
                IconName = BitIconName.EntityExtraction,
                Url = Urls.TermsPage,
            }
        ];

        if (AppPlatform.IsBlazorHybrid)
        {
            // Currently, the "About" page is absent from the Client/Core project, rendering it inaccessible on the web platform.
            // In order to exhibit a sample page that grants direct access to native functionalities without dependence on
            // dependency injection (DI) or publish-subscribe patterns, the "About" page is integrated within Blazor
            // hybrid projects like Client/Maui.

            allNavItems.Add(new()
            {
                Text = Localizer[nameof(AppStrings.AboutTitle)],
                IconName = BitIconName.Info,
                Url = Urls.AboutPage,
            });
        }
    }
}
