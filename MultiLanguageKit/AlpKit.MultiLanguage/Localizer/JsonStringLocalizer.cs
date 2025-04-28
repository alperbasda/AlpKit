using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.Globalization;

namespace AlpKit.MultiLanguage.Localizer
{
    public class JsonStringLocalizer : IStringLocalizer
    {
        List<JsonLocalization> localization = new List<JsonLocalization>();
        public JsonStringLocalizer()
        {
            localization = JsonConvert.DeserializeObject<List<JsonLocalization>>(File.ReadAllText(@"localization.json"))!;
        }


        public LocalizedString this[string name]
        {
            get
            {
                var value = GetString(name);
                return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var format = GetString(name);
                var value = string.Format(format ?? name, arguments);
                return new LocalizedString(name, value, resourceNotFound: format == null);
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return localization.Where(l => l.LocalizedValue.Keys.Any(lv => lv == CultureInfo.CurrentCulture.Name)).Select(l => new LocalizedString(l.Key, l.LocalizedValue[CultureInfo.CurrentCulture.Name], true));
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return new JsonStringLocalizer();
        }

        private string GetString(string name)
        {
            try
            {

                var query = localization.Where(l => l.LocalizedValue.Keys.Any(lv => lv == CultureInfo.CurrentCulture.Name));
                var value = query.FirstOrDefault(l => l.Key.ToLower() == name.ToLower());
                if (value == null)
                    return name;
                return value.LocalizedValue[CultureInfo.CurrentCulture.Name];

            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}
