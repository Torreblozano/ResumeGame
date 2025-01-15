using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LocalizationsManager : CustomSingleton<LocalizationsManager>
{
    public string currentLanguage => CurrentLanguage();
    public int currentLanguageIndex => CurrentLanguageIndex();

    private string CurrentLanguage()
    {
        Locale currentLocale = LocalizationSettings.SelectedLocale;
        return currentLocale.Identifier.Code;
    }

    private int CurrentLanguageIndex()
    {
        Locale currentLocale = LocalizationSettings.SelectedLocale;

        Dictionary<string, int> languageKeyToInt = new Dictionary<string, int>
        {
            { "en", 0 },
            { "es", 1 }
        };

        string currentLanguageKey = currentLocale.Identifier.Code;
        return languageKeyToInt.TryGetValue(currentLanguageKey, out int currentLanguageInt) ? currentLanguageInt : -1; // Return -1 if not found
    }

    public async UniTask ChangeLanguage(int localeID)
    {
        await SetLocale(localeID);
    }

    public async UniTask ChangeLanguage(string localeID)
    {
        int index = localeID == "en" ? 0 : 1;
        await SetLocale(index);
    }

    private async UniTask SetLocale(int localeID)
    {
        await LocalizationSettings.InitializationOperation;

        if (localeID < 0 || localeID >= LocalizationSettings.AvailableLocales.Locales.Count)
        {
            print("Invalid locale ID: " + localeID);
            return;
        }

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeID];
    }

    public async UniTask<string> GetDialog(string key)
    {
        var table = await LocalizationSettings.StringDatabase.GetTableAsync("Dialogs");

        if (table == null)
        {
            print("Dialogs table not found.");
            return "";
        }

        var entry = table.GetEntry(key);
        return entry?.Value ?? string.Empty;
    }

    public async UniTask<string> GetString(string key)
    {
        var table = await LocalizationSettings.StringDatabase.GetTableAsync("GameLocalizations");

        if (table == null)
        {
            print("GameLocalizations table not found.");
            return "";
        }

        var entry = table.GetEntry(key);
        return entry?.Value ?? string.Empty;
    }
}
