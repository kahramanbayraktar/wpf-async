using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace AsyncTestWpfApp
{
    /// <summary>
    /// Source: https://www.youtube.com/watch?v=2moh18sh5p4
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnExecuteSync_Click(object sender, RoutedEventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            RunDownloadSync();

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            txtResults.Text += $"Total execution time: { elapsedMs }";
        }

        private async void BtnExecuteAsync_Click(object sender, RoutedEventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            await RunDownloadAsync();

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            txtResults.Text += $"Total execution time: { elapsedMs }";
        }

        private async void BtnExecuteParallelAsync_Click(object sender, RoutedEventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            await RunDownloadParallelAsync();

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            txtResults.Text += $"Total execution time: { elapsedMs }";
        }

        private void RunDownloadSync()
        {
            var websites = PrepData();

            foreach (var website in websites)
            {
                var results = DownloadWebsite(website);
                ReportWebsiteInfo(results);
            }
        }

        private async Task RunDownloadAsync()
        {
            var websites = PrepData();

            foreach (var website in websites)
            {
                //var result = await Task.Run(() => DownloadWebsite(website)); // Alternative way: Task.Run() makes a synchronous method asynchronous by wrapping it.
                var result = await DownloadWebsiteAsync(website);
                ReportWebsiteInfo(result);
            }
        }

        private async Task RunDownloadParallelAsync()
        {
            var websites = PrepData();
            var tasks = new List<Task<WebsiteDataModel>>();

            foreach (var website in websites)
            {
                tasks.Add(Task.Run(() => DownloadWebsite(website)));
            }

            var results = await Task.WhenAll(tasks);

            foreach (var result in results)
            {
                ReportWebsiteInfo(result);
            }
        }

        private WebsiteDataModel DownloadWebsite(string websiteUrl)
        {
            var output = new WebsiteDataModel();
            var client = new WebClient();

            output.WebsiteUrl = websiteUrl;
            output.WebsiteData = client.DownloadString(websiteUrl);

            return output;
        }

        private async Task<WebsiteDataModel> DownloadWebsiteAsync(string websiteUrl)
        {
            var output = new WebsiteDataModel();
            var client = new WebClient();

            output.WebsiteUrl = websiteUrl;
            output.WebsiteData = await client.DownloadStringTaskAsync(websiteUrl);

            return output;
        }

        private void ReportWebsiteInfo(WebsiteDataModel data)
        {
            txtResults.Text += $"{ data.WebsiteUrl } downloaded: { data.WebsiteData.Length } characters long.{ Environment.NewLine }";
        }

        private List<string> PrepData()
        {
            var output = new List<string>();

            txtResults.Text = "";

            output.Add("https://www.yahoo.com");
            output.Add("https://www.google.com");
            output.Add("https://www.microsoft.com");
            output.Add("https://www.cnn.com");
            output.Add("https://www.codeproject.com");
            output.Add("https://www.stackoverflow.com");

            return output;
        }
    }
}
