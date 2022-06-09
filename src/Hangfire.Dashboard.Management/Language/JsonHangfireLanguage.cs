using System;
using System.Linq;

namespace Hangfire.Dashboard.Management
{
    /// <summary>
    /// 翻译
    /// </summary>
    public static class JsonHangfireLanguage
    {
        private static System.Collections.Concurrent.ConcurrentDictionary<string, string> keyValuesaa = new System.Collections.Concurrent.ConcurrentDictionary<string, string>();

        public static string TranslatLanguage(System.Globalization.CultureInfo culture, string name)
        {
            return keyValuesaa.GetOrAdd(culture?.Name + "$$$" + name, f =>
             {
                 culture = culture ?? System.Globalization.CultureInfo.CurrentUICulture;
                 while (culture != null && !string.IsNullOrWhiteSpace(culture.Name))
                 {
                     var language = loadCultureLanguages(culture);
                     var translat = getTranslat(name, language);
                     if (translat != null) return translat;
                     culture = culture.Parent;
                 }
                 return getTranslat(name, defaultCulture);
             });
        }

        private static string getTranslat(string name, CultureLanguages culture)
        {
            string translat = null;
            if (culture?.Translates?.TryGetValue(name, out translat) == true)
                return translat;
            else
            {
                foreach (var item in culture?.TranslateRegexs ?? new TranslateRegexs[] { })
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(name, item.Pattern))
                    {
                        var timeoutInSecondsGroups = System.Text.RegularExpressions.Regex.Match(name, item.Pattern).Groups;
                        var t = item.Translate;
                        foreach (var n in item.Names)
                        {
                            t = t.Replace($"{{{n}}}", timeoutInSecondsGroups[n].Value);
                        }
                        return t;
                    }
                }
            }
            return null;
        }

        private static CultureLanguages defaultCulture = loadCultureLanguage("en");

        private static CultureLanguages loadCultureLanguage(string name)
        {
            var assembly = typeof(JsonHangfireLanguage).Assembly;
            var resourceNames = assembly.GetManifestResourceNames();

            var resourceName = resourceNames.FirstOrDefault(f => f.Equals($"Hangfire.Dashboard.Management.Content.Language.{name}.json", StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(resourceName))
            {
                var stream = assembly.GetManifestResourceStream(resourceName);
                var json = new System.IO.StreamReader(stream).ReadToEnd();
                var culture = Newtonsoft.Json.JsonConvert.DeserializeObject<CultureLanguages>(json);
                return culture;
            }
            return null;
        }

        private static System.Collections.Concurrent.ConcurrentDictionary<string, CultureLanguages> keyValues = new System.Collections.Concurrent.ConcurrentDictionary<string, CultureLanguages>();

        private static CultureLanguages loadCultureLanguages(System.Globalization.CultureInfo culture)
        {
            return keyValues.GetOrAdd(culture.Name, ff =>
             {
                 return loadCultureLanguage(culture.Name);
             });
        }

        public static void AddCultureLanguages(CultureLanguages culture)
        {
            keyValues.AddOrUpdate(culture.Culture, culture, (name, oldCulture) => culture);
        }
    }

    public class CultureLanguages
    {
        public string Culture { get; set; }
        public System.Collections.Generic.Dictionary<string, string> Translates { get; set; }
        public TranslateRegexs[] TranslateRegexs { get; set; }
    }

    public class TranslateRegexs
    {
        public string[] Names { get; set; }
        public string Pattern { get; set; }
        public string Translate { get; set; }
    }
}