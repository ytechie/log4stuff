using System;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Owin.Security.DataHandler;
using Newtonsoft.Json;

namespace Log4stuff.Web.Models
{
    public static class LogEventExtensions
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0,
                                                      DateTimeKind.Utc);

        public static string ToJson(this LogEvent logEvent)
        {
            return JsonConvert.SerializeObject(logEvent);
        }

        public static void PopulateFromJson(this LogEvent logEvent, string logEventJson)
        {
            JsonConvert.PopulateObject(logEventJson, logEvent);
        }

        public static DateTime UnixTimeToDateTime(string text)
        {
            var seconds = double.Parse(text, CultureInfo.InvariantCulture);
            return Epoch.AddMilliseconds(seconds);
        }

        public static void PopulateFromLog4JXml(this LogEvent logEvent, string logEventXml)
        {
            var xlinq = XDocument.Parse(logEventXml.Replace("log4j:", ""));

            var elements = xlinq.Descendants().ToList();

            logEvent.Level = elements[0].Attribute("level").Value;

            var timestampString = elements[0].Attribute("timestamp").Value;
            logEvent.Timestamp = UnixTimeToDateTime(timestampString);

            logEvent.Logger = elements[0].Attribute("logger").Value;
            logEvent.Thread = elements[0].Attribute("thread").Value;

            logEvent.Message = elements.Single(x => x.Name == "message").Value;

            var properties = elements.Single(x => x.Name == "properties").Descendants().ToList();
            foreach (var property in properties)
            {
                var name = property.Attribute("name").Value;
                if (name == "ApplicationId")
                {
                    logEvent.ApplicationId = property.Attribute("value").Value;
                }
                else
                {
                    logEvent.Metadata.Add(name, property.Attribute("value").Value);
                }
            }
        }
    }
}