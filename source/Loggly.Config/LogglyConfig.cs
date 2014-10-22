﻿using System;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Net;

namespace Loggly.Config
{
    public class LogglyConfig : ILogglyConfig
    {
        public string ApplicationName { get; set; }
        public string CustomerToken { get; set; }
        public bool ThrowExceptions { get; set; }
        public ITagConfiguration Tags { get; private set; }
        public ITransportConfiguration Transport { get; private set; }
        public ISearchConfiguration Search { get; private set; }

        public bool IsValid
        {
            get { return !string.IsNullOrEmpty(CustomerToken); }
        }

        private LogglyConfig()
        {
            Tags = new TagConfiguration();
            Transport = new TransportConfiguration();
        }

        private static ILogglyConfig _instance;

        public static ILogglyConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    if (LogglyAppConfig.HasAppCopnfig)
                    {
                        _instance = FromAppConfig();
                    }
                    else
                    {
                        _instance = GetNullConfig();
                    }
                }
                return _instance;
            }
            set { _instance = value; }
        }

        private static ILogglyConfig GetNullConfig()
        {
            return new LogglyConfig();
        }

        private static ILogglyConfig FromAppConfig()
        {
            var config = new LogglyConfig();

            config.CustomerToken = LogglyAppConfig.Instance.CustomerToken;
            config.ThrowExceptions = LogglyAppConfig.Instance.ThrowExceptions;
            config.ApplicationName = LogglyAppConfig.Instance.ApplicationName;

            if (LogglyAppConfig.Instance.HasTagConfig)
            {
                config.Tags.SimpleTags.AddRange(LogglyAppConfig.Instance.Tags.Simple.Cast<ISimpleTag>().ToList());
                config.Tags.ComplexTags.AddRange(LogglyAppConfig.Instance.Tags.GetComplexTags());
            }

            config.Transport = TransportAppConfig.ConformToValidConfig(LogglyAppConfig.Instance.Transport);

            config.Search = LogglyAppConfig.Instance.Search;

            return config;
        }
    }
}
