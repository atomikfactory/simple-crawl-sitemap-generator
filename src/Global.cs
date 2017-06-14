using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteMapGenerator
{
    public static class Global
    {
        public static List<HREFLink> GlobalList { get; set; }
        public static List<HREFLink> BufferList { get; set; }
        public static List<HREFLink> NextLink { get; set; }

        public static List<string> _excludedFiles { get; set; }
        public static int _maxValue { get; set; }
        public static bool _crawlOnlyMyDomain { get; set; }
        public static bool _generateSiteMap { get; set; }
        public static bool _generateUrlTxtFile { get; set; }


        public static void InitConfig()
        {

            _excludedFiles = GenerateExcludeFileList();

            int maxEntries = 0;
            int.TryParse(ConfigurationManager.AppSettings["maxEntries"], out maxEntries);
            _maxValue = maxEntries;

            _crawlOnlyMyDomain = bool.Parse(ConfigurationManager.AppSettings["crawlOnlyMyDomain"]);
            _generateSiteMap = bool.Parse(ConfigurationManager.AppSettings["GenerateSiteMap"]);
            _generateUrlTxtFile = bool.Parse(ConfigurationManager.AppSettings["generateLinksTxtFile"]);

        }

        private static List<string> GenerateExcludeFileList()
        {
            var concatResult = new List<string>();
            var excludedMediaFiles = ConfigurationManager.AppSettings["excludedMediaFiles"].Split('|').Select(x => x.Trim().ToLower()).ToList();
            var excludedOtherFiles = ConfigurationManager.AppSettings["excludedOtherFiles"].Split('|').Select(x => x.Trim().ToLower()).ToList();

            return concatResult.Concat(excludedMediaFiles).Concat(excludedOtherFiles).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
        }

    }
}
