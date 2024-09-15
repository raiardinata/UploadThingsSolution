namespace UploadThings.Services.Factories
{
    public abstract class SettingServicesFactory
    {
        public abstract ISettingServices UpdateAppSetting(string section, string key, string newValue, string jsonFileName);
    }
}
