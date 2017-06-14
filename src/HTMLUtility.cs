using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SiteMapGenerator
{
    public class HTMLUtility
    {
        public static string _rootUrl { get; set; }


        public static void GenerateSiteMap(string rootUrl)
        {
            Global.GlobalList = new List<HREFLink>();

            _rootUrl = rootUrl;


            GetLinks((new HREFLink[] { new HREFLink() { link = _rootUrl, Processed = false } }).ToList());

            Urlset siteMapDoc = new Urlset();
            var urlEntry = new List<UrlsetUrl>();

            Global.GlobalList.OrderBy(l => l.link.Length);

            if (Global._generateUrlTxtFile)
            {
                foreach (var hrefLink in Global.GlobalList)
                {
                    UrlsetUrl entry = new UrlsetUrl();
                    entry.changefreq = ConfigurationManager.AppSettings["changeFrequence"];
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["priority"]))
                        entry.priority = decimal.Parse(ConfigurationManager.AppSettings["priority"]);
                    else
                        entry.priority = 1;

                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["lastModification"]))
                        entry.lastmod = DateTime.Parse(ConfigurationManager.AppSettings["lastModification"]);
                    else
                        entry.lastmod = DateTime.Now;

                    entry.loc = hrefLink.link;

                    urlEntry.Add(entry);


                    System.IO.File.AppendAllText(string.Format("{0}links.txt", ConfigurationManager.AppSettings["linksOutputFolder"]), hrefLink.link + Environment.NewLine);
                }
            }

            if (Global._generateSiteMap)
            {
                siteMapDoc.url = urlEntry.ToArray();

                XmlSerializer serializer = new XmlSerializer(typeof(Urlset));

                using (StreamWriter writer = new StreamWriter(string.Format("{0}sitemap.xml", ConfigurationManager.AppSettings["siteMapOutputFolder"])))
                {
                    serializer.Serialize(writer, siteMapDoc);
                }
            }

        }

        public static void GetLinks(List<HREFLink> hrefLinkList)
        {
            Global.BufferList = new List<HREFLink>();

            Parallel.ForEach(hrefLinkList, linkNext =>
            {

                try
                {
                    HtmlWeb web = new HtmlWeb();
                    HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

                    Console.WriteLine(linkNext.link);

                    doc = web.Load(linkNext.link);

                    if (doc != null && doc.DocumentNode.SelectNodes("//a[@href]") != null)
                    {
                        foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
                        {
                            var process = true;

                            string hrefValue = link.GetAttributeValue("href", string.Empty);

                            var urlToAdd = string.Empty;

                            if (hrefValue.StartsWith("/") && hrefValue.Length > 1)
                            {
                                urlToAdd = _rootUrl + hrefValue;
                            }
                            if (hrefValue.StartsWith("http") || hrefValue.StartsWith("www"))
                            {
                                urlToAdd = hrefValue;
                            }

                            if (string.IsNullOrEmpty(urlToAdd) == false)
                            {

                                //excludedExtension
                                if (!string.IsNullOrEmpty(urlToAdd))
                                {
                                    if (Global._excludedFiles.Contains(Path.GetExtension(new Uri(urlToAdd).AbsoluteUri).ToLower()))
                                        process = false;
                                }


                                if (Global._crawlOnlyMyDomain == true)
                                {
                                    var domain = new Uri(urlToAdd).GetLeftPart(UriPartial.Authority);

                                    if (domain != ConfigurationManager.AppSettings["rootDomain"])
                                        process = false;
                                }

                                if (process && string.IsNullOrEmpty(urlToAdd) == false)
                                {
                                    Global.BufferList.Add(new HREFLink() { link = urlToAdd, Processed = false });
                                }
                            }
                        }

                        Global.BufferList = MergeWithGlobal(Global.BufferList);



                        var hrefLink = Global.GlobalList.Where(l => l.link == linkNext.link).FirstOrDefault();

                        if (hrefLink != null)
                            hrefLink.Processed = true;

                        if (Global.BufferList.Count > 0)
                            GetLinks(Global.BufferList);

                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });


        }

        public static List<HREFLink> MergeWithGlobal(List<HREFLink> lstToMerge)
        {
            Global.NextLink = new List<HREFLink>();

            foreach (var hrefUrl in lstToMerge)
            {
                if (Global.GlobalList.Exists(l => l.link == hrefUrl.link) == false)
                {
                    lock (Global.GlobalList)
                    {
                        if (Global._maxValue != 0 && Global.GlobalList.Count > Global._maxValue)
                            return new List<HREFLink>();

                        Global.GlobalList.Add(hrefUrl);

                        lock (Global.NextLink)
                            Global.NextLink.Add(hrefUrl);
                    }
                }
            }

            return Global.NextLink;

        }
    }



    public class HREFLink
    {
        public string link { get; set; }
        public bool Processed { get; set; }
    }

}


