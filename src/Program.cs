using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteMapGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Global.InitConfig();

            Console.WriteLine(string.Format("Start Processing {0}", ConfigurationManager.AppSettings["rootDomain"]));
            Console.WriteLine(string.Format("Maximum entries {0}", Global._maxValue.ToString()));

            HTMLUtility.GenerateSiteMap(ConfigurationManager.AppSettings["rootDomain"]);


            Console.WriteLine("Process end... press enter to exist");
            Console.ReadLine();


        }
    }
}
