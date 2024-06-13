using System.Text;
using ImageAgregationService.Exceptions.ConfigExceptions;
using Newtonsoft.Json;

namespace ImageAgregationService.Singletones
{
    public class ConfigReader
    {
        public async Task<List<string>> GetBuckets()
        {
            try
            {
                var json = string.Empty;
                using (var fs = File.OpenRead("Configs/bucketconfig.json"))
                using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                    json = await sr.ReadToEndAsync().ConfigureAwait(false);
                var configJson = JsonConvert.DeserializeObject<List<string>>(json);
      
                if (configJson == null)
                {
                    throw new GetConfigException("configJson was null");
                }

                return configJson;
            }
            catch (Exception ex)
            {
                throw new GetConfigException("Failed to get buckets from config file!",ex);
            }
        }
    }
}