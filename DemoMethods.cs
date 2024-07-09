using System.Net;
using System.Net.Http;

namespace Project_Async_WPF
{
    public class DemoMethods
    {
        public static List<WebsiteDataModel> RunDownloadSync()
        {
            List<string> websites = PrepData();
            List<WebsiteDataModel> output = new List<WebsiteDataModel>();

            foreach (string site in websites)
            {
                WebsiteDataModel results = DownloadWebsite(site);
                output.Add(results);
            }

            return output;
        }

        private static WebsiteDataModel DownloadWebsite(string websiteURL)
        {
            using (WebClient client = new WebClient())
            {
                string content = client.DownloadString(websiteURL);
                return new WebsiteDataModel
                {
                    WebsiteURL = websiteURL,
                    WebsiteContent = content
                };
            }
        }
        public static async Task<List<WebsiteDataModel>> RunDownloadAsync(IProgress<ProgressReportModel> progress)
        {
            List<string> websites = PrepData();
            List<WebsiteDataModel> output = new List<WebsiteDataModel>();
            ProgressReportModel report = new ProgressReportModel();

            foreach (string site in websites)
            {
                WebsiteDataModel results = await DownloadWebsiteAsync(site);
                output.Add(results);

                report.SitesDownloaded = new List<WebsiteDataModel>(output); // Ensure a new list instance
                report.PercentageComplete = (output.Count * 100) / websites.Count;
                progress.Report(report);
            }

            return output;
        }

        private static List<string> PrepData()
        {
            // Your logic to prepare the data
            return new List<string> { "https://example.com", "https://example.org" }; // Sample data
        }

        private static async Task<WebsiteDataModel> DownloadWebsiteAsync(string site)
        {
            // Logic to download the website data asynchronously
            var httpClient = new HttpClient();
            string content = await httpClient.GetStringAsync(site);
            return new WebsiteDataModel { WebsiteURL = site, WebsiteContent = content };
        }
    }
}
