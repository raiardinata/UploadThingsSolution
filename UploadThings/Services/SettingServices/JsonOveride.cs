using Newtonsoft.Json.Linq;

namespace UploadThings.Services.SettingServices
{
    public class JsonOveride : ISettingServices
    {
        private readonly string _section;
        private readonly string _key;
        private readonly string _newValue;
        private readonly string _filePath;
        public JsonOveride(string section, string key, string newValue, string jsonFileName)
        {
            _section = section;
            _key = key;
            _newValue = newValue;
            _filePath = Path.Combine(Directory.GetCurrentDirectory(), $"{jsonFileName}");
        }
        public Exception UpdateAppSetting()
        {

            var json = File.ReadAllText(_filePath);
            var jsonObj = JObject.Parse(json);
            var sectionObj = jsonObj[_section] as JObject;

            if (sectionObj != null)
            {
                sectionObj[_key] = _newValue;
            }
            else
            {
                jsonObj[_section] = new JObject(new JProperty(_key, _newValue));
            }

            File.WriteAllText(_filePath, jsonObj.ToString());

            return new Exception("null");
        }
    }


}
