using UploadThings.Services.SettingServices;

namespace UploadThings.Services.Factories
{
    public class JsonOverideFactory : SettingServicesFactory
    {
        public override ISettingServices UpdateAppSetting(string section, string key, string newValue, string jsonFileName)
        {
            return new JsonOveride(section, key, newValue, jsonFileName);
        }
    }
}
