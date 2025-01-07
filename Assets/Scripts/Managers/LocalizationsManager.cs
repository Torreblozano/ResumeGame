using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LocalizationsManager : CustomSingleton<LocalizationsManager>
{
    public string currentLanguage { get { return CurrentLanguage(); } }

    public int currentLanguageIndex { get { return CurrentLanguageIndex(); } }

    private string CurrentLanguage()
    {
        Locale currentLocale = LocalizationSettings.SelectedLocale;
        return currentLocale.Identifier.Code;
    }

    private int CurrentLanguageIndex()
    {
        Locale currentLocale = LocalizationSettings.SelectedLocale;

        Dictionary<string, int> languageKeyToInt = new Dictionary<string, int>();
        languageKeyToInt.Add("en", 0);
        languageKeyToInt.Add("es", 1);

        string currentLanguageKey = currentLocale.Identifier.Code;
        int currentLanguageInt = languageKeyToInt[currentLanguageKey];
        return currentLanguageInt;
    }

    // public void SaveLanguage(int _localeID) => PlayerPrefsManager.m_currentLanguage = _localeID;

    public async void ChangeLanguage(int _localeID)
    {
        await SetLocale(_localeID);
    }

    public async void ChangeLanguage(string _localeID)
    {
        int index = _localeID == "en" ? 0 : 1;
        await SetLocale(index);
    }

    public async UniTask SetLocale(int _localeID)
    {
        await LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
        //SaveLanguage(_localeID);

    }

    public async UniTask<string> GetDialog(string key)
    {
        var table = await LocalizationSettings.StringDatabase.GetTableAsync("Dialogs");

        if (table == null)
            return "";


        var entry = table.GetEntry(key);
        return entry == null ? string.Empty : entry.Value;
    }

    public async UniTask<string> GetString(string key)
    {
        var table = await LocalizationSettings.StringDatabase.GetTableAsync("GameLocalizations");

        if (table == null)
            return "";


        var entry = table.GetEntry(key);
        return entry == null ? string.Empty : entry.Value;
    }
}
