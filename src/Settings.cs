namespace Sitecore.FakeDb.RainbowSerialization
{
    public static class Settings
    {
        private static string _language { get; set; }

        public static string Language
        {
            get
            {
                if (string.IsNullOrEmpty(_language))
                {
                    if (!string.IsNullOrEmpty(Sitecore.Configuration.Settings.GetSetting("RainbowSerialization_Language")))
                        _language = Sitecore.Configuration.Settings.GetSetting("RainbowSerialization_Language");
                    else
                        _language = "en";
                }

                return _language;
            }
        }
    }
}