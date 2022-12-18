using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;

namespace WebScraper
{
    class youtubeScraper
    {
        static void Main(string[] args)
        {
            TextWriter tw = new StreamWriter("data.csv");
            TextWriter twJson = new StreamWriter("data.json");
            IWebDriver driver = new ChromeDriver();
            string app = " ";
            Console.Write("What do you want to search for?(Job/Video/Schedule) ");
            app = Console.ReadLine().ToLower();
            string jsonData = "[\n";
            void addTocsv(string input, Boolean newLine)
            {
                if (newLine)
                {
                    tw.WriteLine(input);
                }
                else
                {
                    tw.Write(input + ",");
                }
            }
            void addToJson(string identifier, string data, Boolean startObject, Boolean endObject)
            {
                if (startObject)
                {
                    jsonData += "{\n\"" + identifier + "\": \"" + data + "\",\n";
                }
                else if (endObject)
                {
                    jsonData += "\"" + identifier + "\": \"" + data + "\"\n },";
                }
                else
                {
                    jsonData += "\"" + identifier + "\": \"" + data + "\",\n";
                }
            }
            if (app == "job")
            {
                addTocsv("Job title", false);
                addTocsv("Company", false);
                addTocsv("Location", false);
                addTocsv("keywords", false);
                addTocsv("Link", true);
                Console.Write("Search keyword: ");
                string keyword = Console.ReadLine();
                driver.Navigate().GoToUrl("https://www.ictjob.be/en/search-it-jobs?keywords=" + keyword);
                Thread.Sleep(4000);
                var privacyButton = driver.FindElement(By.XPath("//*[@id=\"body-ictjob\"]/div[2]/a"));
                privacyButton.Click();
                var dateFilter = driver.FindElement(By.XPath("/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[1]/div[2]/div/div[2]/span[2]/a"));
                dateFilter.Click();
                Thread.Sleep(10000);
                int counter = 1;
                while (counter < 7)
                {
                    if (counter != 4)
                    {
                        string jobTitle = driver.FindElement(By.XPath("html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[2]/div[1]/div/ul/li[" + counter + "]/span[2]/a/h2")).Text;
                        string company = driver.FindElement(By.XPath("/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[2]/div[1]/div/ul/li[" + counter + "]/span[2]/span[1]")).Text;
                        string location = driver.FindElement(By.XPath("/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[2]/div[1]/div/ul/li[" + counter + "]/span[2]/span[2]/span[2]/span/span")).Text;
                        string keywords = driver.FindElement(By.XPath("/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[2]/div[1]/div/ul/li[" + counter + "]/span[2]/span[3]")).Text;
                        string link = driver.FindElement(By.XPath("/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[2]/div[1]/div/ul/li[" + counter + "]/span[2]/a")).GetAttribute("href");
                        Console.WriteLine("Job title: " + jobTitle);
                        Console.WriteLine("Company: " + company);
                        Console.WriteLine("location: " + location);
                        Console.WriteLine("keywords: " + keywords);
                        Console.WriteLine("link: " + link);
                        addTocsv(jobTitle, false);
                        addTocsv(company, false);
                        addTocsv(location.Replace(",", ""), false);
                        addTocsv(keywords.Replace(",", ""), false);
                        addTocsv(link, true);
                        addToJson("Job Title", jobTitle, true, false);
                        addToJson("Company", company, false, false);
                        addToJson("Location", location, false, false);
                        addToJson("Keywords", keywords, false, false);
                        addToJson("Link", link, false, true);
                    }
                    counter++;
                }
            }
            else if (app == "video")
            {
                Console.Write("Search Term: ");
                string searchTerm = Console.ReadLine();
                driver.Navigate().GoToUrl("https://www.youtube.com/results?search_query=" + searchTerm + "&sp=CAISAhAB");
                var rejectButton = driver.FindElement(By.XPath("/html/body/ytd-app/ytd-consent-bump-v2-lightbox/tp-yt-paper-dialog/div[4]/div[2]/div[6]/div[1]/ytd-button-renderer[1]/yt-button-shape/button/yt-touch-feedback-shape/div/div[2]"));
                rejectButton.Click();
                Thread.Sleep(500);
                int counter = 0;
                int evenCounter = 0;
                var titleList = driver.FindElements(By.XPath("//*[@id=\"video-title\"]/yt-formatted-string"));
                var linkList = driver.FindElements(By.XPath("//*[@id=\"video-title\"]"));
                var uploaderList = driver.FindElements(By.XPath("//*[@id=\"text\"]/a"));
                addTocsv("Link", false);
                addTocsv("Title", false);
                addTocsv("Uploader", false);
                addTocsv("Views", true);
                while ((counter) < 5)
                {
                    string views = driver.FindElement(By.XPath("/html/body/ytd-app/div[1]/ytd-page-manager/ytd-search/div[1]/ytd-two-column-search-results-renderer/div[2]/div/ytd-section-list-renderer/div[2]/ytd-item-section-renderer/div[3]/ytd-video-renderer[" + (counter + 1) + "]/div[1]/div/div[1]/ytd-video-meta-block/div[1]/div[2]/span[1]")).Text;
                    string link = linkList[counter].GetAttribute("href");
                    string title = titleList[counter].Text;
                    string uploader = uploaderList[(evenCounter + 1)].Text;
                    Console.WriteLine(((counter + 1)) + ".");
                    Console.WriteLine("Link: " + link);
                    Console.WriteLine("Title: " + title);
                    Console.WriteLine("Uploader: " + uploader);
                    Console.WriteLine("Views: " + views);
                    addTocsv(link, false);
                    addTocsv(title.Replace(",", ""), false);
                    addTocsv(uploader, false);
                    addTocsv(views, true);
                    addToJson("Link", link,true,false);
                    addToJson("Title", title.Replace("\"",""),false,false);
                    addToJson("Uploader", uploader, false, false);
                    addToJson("Views", views, false, true);
                    evenCounter += 2;
                    counter++;
                }
            }
            else if (app == "schedule")
            {
                Console.Write("What league do you want to see the results of (lec/lck/lcs/lpl)? ");
                string response = Console.ReadLine().ToLower();
                try
                {
                    driver.Navigate().GoToUrl("https://lolesports.com/schedule?leagues=" + response);
                    Thread.Sleep(1000);
                    int times = 0;
                    var scoresTeam1 = driver.FindElements(By.ClassName("scoreTeam1"));
                    var scoresTeam2 = driver.FindElements(By.ClassName("scoreTeam2"));

                    var teams = driver.FindElements(By.ClassName("tricode"));
                    times = teams.Count();
                    int count = 0;
                    int matchNumber = times - 1;
                    int team1Count = scoresTeam1.Count();
                    int team2Count = scoresTeam2.Count();
                    while (count < 10)
                    {
                        if (count % 2 == 0)
                        {
                            var team2 = teams[matchNumber - count].Text;
                            var team2Score = scoresTeam2[team2Count - 1].Text;
                            Console.Write(team2 + " ");
                            Console.Write(team2Score);
                            Console.Write(" vs ");
                            team2Count--;
                            addTocsv(team2, false);
                            addTocsv(team2Score, false);
                            addToJson("Team 2", team2,true,false);
                            addToJson("Score team 2", team2Score,false,false);
                        }
                        else
                        {
                            var team1 = teams[matchNumber - count].Text;
                            var team1Score = scoresTeam1[team1Count - 1].Text;
                            Console.Write(team1Score + " ");
                            Console.Write(team1);
                            Console.WriteLine();
                            addTocsv(team1Score, false);
                            addTocsv(team1, true);
                            addToJson("Team 1", team1, false, false);
                            addToJson("Score team 1", team1Score, false, true);
                            team1Count--;
                        }
                        count++;
                    }
                }
                catch
                {
                    Console.WriteLine("A non-valid league was given");
                }
            }
            jsonData = jsonData.Substring(0,jsonData.Length - 1);
            jsonData += "\n]";
            twJson.Write(jsonData);
            twJson.Close();
            tw.Close();
            driver.Close();
        }
    }
}