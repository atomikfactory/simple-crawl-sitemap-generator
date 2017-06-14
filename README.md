# simple-crawl-sitemap-generator
simple crawl &amp; google sitemap generator console application in c#


Overview
-------------------------------------------------------
C# console application used to crawl websites, extract links and generate a google XML Sitemap file and a txt file with the list of links.


Requirements
-------------------------------------------------------

    .Net Framework 4.5.2 or higher
    HtmlAgilityPack(NuGet : Install-Package HtmlAgilityPack -Version 1.4.9.5)
    
Configuration
----------------------------------------------------------

Edit app.config file
* Keys
  * **excludedMediaFiles** : media file extension to exclude (use | as a separator) example : value=".jpg|.jpeg|.bmp|.png|.mp3|.mp4"
  * **excludedOtherFiles** : other extension to exclude (use | as a separator) example : value=".xml|.txt"
  * **maxEntries** :  maximum number of links to extract (zero = unlimited : process can be very long especially if the crawl is not limited to the root domain)
  * **crawlOnlyMyDomain** : value => true or false (determine if the crawl is  limited or not to the root domain)
  * **rootDomain** : website to scan
  * **GenerateSiteMap** : value => true or false
  * **generateLinksTxtFile** : value => true or false
  * **siteMapOutputFolder** & **linksOutputFolder**: output folder for the generated file(s) (example : c:\tmp\) if blank, the output folder will the folder where the application is located.
  

#License
----------------------------------------------------------

MIT license.
