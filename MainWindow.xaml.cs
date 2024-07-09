using Project_Async_WPF;
using System.Diagnostics;
using System.Net;
using System.Windows;

namespace DemoAsync_WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void executaSync_Click(object sender, RoutedEventArgs e)
        {
            var watch = Stopwatch.StartNew();

            var results = DemoMethods.RunDownloadSync();
            PrintResults(results);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            resultsWindow.Text += $"Tempo de execução Total: {elapsedMs} ms{Environment.NewLine}";
        }

        private async void executaAsync_Click(object sender, RoutedEventArgs e)
        {
            Progress<ProgressReportModel> progress = new Progress<ProgressReportModel>();
            progress.ProgressChanged += ReportProgress;

            var watch = Stopwatch.StartNew();

            var results = await DemoMethods.RunDownloadAsync(progress);
            PrintResults(results);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            resultsWindow.Text += $"Tempo de execução Total: {elapsedMs} ms";
        }

        private void ReportProgress(object sender, ProgressReportModel e)
        {
            dashboardProgress.Value = e.PercentageComplete;
            PrintResults(e.SitesDownloaded);
        }

        private void PrintResults(List<WebsiteDataModel> results)
        {
            resultsWindow.Text = string.Empty;
            foreach (var item in results)
            {
                resultsWindow.Text += $"{item.WebsiteURL} downloaded: {item.WebsiteContent.Length} characters{Environment.NewLine}";
            }
        }

        private async void ParallelAsync_Click(object sender, RoutedEventArgs e)
        {
            var watch = Stopwatch.StartNew();

            var results = await RunDownloadParallelAsync();
            PrintResults(results);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            resultsWindow.Text += $"Tempo de execução Total: {elapsedMs} ms";
        }

        private void cancelOperation_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private List<string> PrepData()
        {
            List<string> output = new List<string>
            {
                "https://www.yahoo.com",
                "https://www.google.com",
                "https://www.microsoft.com",
                "https://www.cnn.com",
                "https://www.codeproject.com"
            };

            resultsWindow.Text = "";

            return output;
        }

        private async Task RunDownloadSync()
        {
            List<string> websites = PrepData();

            foreach (string site in websites)
            {
                WebsiteDataModel results = DownloadWebsite(site);
                ReportWebsiteInfo(results);
            }
        }

        private WebsiteDataModel DownloadWebsite(string websiteURL)
        {
            WebsiteDataModel output = new WebsiteDataModel();
            WebClient client = new WebClient();

            output.WebsiteURL = websiteURL;
            output.WebsiteContent = client.DownloadString(websiteURL);

            return output;
        }

        private void ReportWebsiteInfo(WebsiteDataModel data)
        {
            resultsWindow.Text += $"O site {data.WebsiteURL} fez download de: {data.WebsiteContent.Length} caracteres. {Environment.NewLine}";
        }

        private async Task RunDownloadAsync()
        {
            List<string> websites = PrepData();

            foreach (string site in websites)
            {
                WebsiteDataModel results = await Task.Run(() => DownloadWebsite(site));
                ReportWebsiteInfo(results);
            }
        }

        private async Task<List<WebsiteDataModel>> RunDownloadParallelAsync()
        {
            List<string> websites = PrepData();
            List<Task<WebsiteDataModel>> tasks = new List<Task<WebsiteDataModel>>();

            foreach (string site in websites)
            {
                tasks.Add(DownloadWebsiteAsync(site));
            }

            var results = await Task.WhenAll(tasks);

            return results.ToList(); // Return results as a List
        }

        private async Task<WebsiteDataModel> DownloadWebsiteAsync(string websiteURL)
        {
            WebsiteDataModel output = new WebsiteDataModel();
            WebClient client = new WebClient();

            output.WebsiteURL = websiteURL;
            output.WebsiteContent = await client.DownloadStringTaskAsync(websiteURL);

            return output;
        }
    }
}
