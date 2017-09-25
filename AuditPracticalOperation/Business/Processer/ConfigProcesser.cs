using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ViewModel;
using System.Xml.Serialization;

namespace Business.Processer
{
    [Singleton]
    public class ConfigProcesser
    {
        private const string CONFIG_FILE_NAME = "config";
        private readonly string configFileName;

        public Config GetConfig()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Config));
            using (StreamReader sr = new StreamReader(configFileName))
                return (Config)serializer.Deserialize(sr);
        }

        public void SetConfig(Config config)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Config));
            using (StreamWriter sr = new StreamWriter(configFileName))
                serializer.Serialize(sr, config);
        }

        private ConfigProcesser()
        {
            configFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CONFIG_FILE_NAME);

            if (!File.Exists(configFileName))
            {
                Config config = new Config();
                config.OfficeType = OfficeTypeEnum.NormalMode;
                XmlSerializer serializer = new XmlSerializer(typeof(Config));
                using (StreamWriter sr = new StreamWriter(configFileName))
                    serializer.Serialize(sr, config);
            }
        }
    }
}
