namespace DotNetUtility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Net;
    using System.Net.Cache;
    using System.Diagnostics;
    using System.ComponentModel;
    using System.Windows;

    public class ApplicationInstallerInformation
    {
        public string Title { get; set; }
        public string Version { get; set; }
        public System.Uri DownloadSiteUrl { get; set; }
        public System.Uri InstallerUrl { get; set; }
        public System.Uri ChangeLogUrl { get; set; }
    }

    public sealed class ApplicationUpdateModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string propertyName)
        {
            var t = PropertyChanged;
            if (t != null) { t(this, new PropertyChangedEventArgs(propertyName)); }
        }

        public Uri InformationFileUri { get; private set; }
        public ApplicationInstallerInformation LatestInstallerInformation { get; private set; }
        public int InstallerFileDownloadingProgressPercentage { get; private set; }
        public string DownloadedInstallerFileFullPath { get; private set; }
        public WebClient DownloadWebClient { get; private set; }

        //public SimpleDelegateCommand CancelCommand { get; private set; }
        //public event EventHandler DownloadCanceled;

        public ApplicationUpdateModel(Uri informationFileUri)
        {
            InformationFileUri = informationFileUri;
            //CancelCommand = new SimpleDelegateCommand();
            //CancelCommand.CanPerform = false;
            //CancelCommand.PerformEventHandler += delegate { if (DownloadWebClient != null) { DownloadWebClient.CancelAsync(); } };
        }

        static string GetFileName(Uri uri, string httpWebRequestMethod = "HEAD")
        {
            try
            {
                var fileName = string.Empty;
                if (uri.Scheme.Equals(Uri.UriSchemeHttp) || uri.Scheme.Equals(Uri.UriSchemeHttps))
                {
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
                    httpWebRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                    httpWebRequest.Method = httpWebRequestMethod;
                    httpWebRequest.AllowAutoRedirect = false;
                    var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    if (httpWebResponse.StatusCode.Equals(HttpStatusCode.Redirect)
                        || httpWebResponse.StatusCode.Equals(HttpStatusCode.Moved)
                        || httpWebResponse.StatusCode.Equals(HttpStatusCode.MovedPermanently))
                    {
                        if (httpWebResponse.Headers["Location"] != null)
                        {
                            var location = httpWebResponse.Headers["Location"];
                            fileName = GetFileName(new Uri(location));
                            return fileName;
                        }
                    }
                    var contentDisposition = httpWebResponse.Headers["content-disposition"];
                    if (string.IsNullOrEmpty(contentDisposition) == false)
                    {
                        const string lookForFileName = "filename=";
                        var index = contentDisposition.IndexOf(lookForFileName, StringComparison.CurrentCultureIgnoreCase);
                        if (index >= 0)
                        {
                            fileName = contentDisposition.Substring(index + lookForFileName.Length);
                        }
                        if (fileName.StartsWith("\""))
                        {
                            fileName = fileName.Substring(1, fileName.Length - 2);
                        }
                        fileName = fileName.Split('\"')[0];
                    }
                }
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = System.IO.Path.GetFileName(uri.LocalPath);
                }
                return fileName;
            }
            catch (WebException)
            {
                return GetFileName(uri, "GET");
            }
        }

        public bool CheckInformationFile()
        {
            if (InformationFileUri == null) { return false; }

            try
            {
                var webRequest = WebRequest.Create(InformationFileUri);
                webRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                var webResponse = webRequest.GetResponse();
                var appCastStream = webResponse.GetResponseStream();
                using (var reader = new System.IO.StreamReader(appCastStream))
                {
                    string jsonString = reader.ReadToEnd();
                    LatestInstallerInformation = Newtonsoft.Json.JsonConvert.DeserializeObject<ApplicationInstallerInformation>(jsonString);
                }
                OnPropertyChanged(nameof(LatestInstallerInformation));
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool DownloadInstaller()
        {
            if (LatestInstallerInformation == null) { return false; }
            if (LatestInstallerInformation.InstallerUrl == null) { return false; }

            try
            {
                var tempFolderFullPath = System.IO.Path.GetTempPath();
                var downloadedFileName = GetFileName(LatestInstallerInformation.InstallerUrl);
                DownloadedInstallerFileFullPath = System.IO.Path.Combine(tempFolderFullPath, downloadedFileName);
                OnPropertyChanged(nameof(DownloadedInstallerFileFullPath));
                DownloadWebClient = new WebClient();
                DownloadWebClient.DownloadProgressChanged += (sender, e) =>
                {
                    InstallerFileDownloadingProgressPercentage = e.ProgressPercentage;
                    OnPropertyChanged(nameof(InstallerFileDownloadingProgressPercentage));
                };
                DownloadWebClient.DownloadFileCompleted += (sender, e) =>
                {
                    if (e.Cancelled)
                    {
                        //CancelCommand.CanPerform = false;
                        //var t = DownloadCanceled; if (t != null) { t(this, EventArgs.Empty); }
                        return;
                    }
                    StartSetup();
                };
                DownloadWebClient.DownloadFileAsync(LatestInstallerInformation.InstallerUrl, DownloadedInstallerFileFullPath);
                //CancelCommand.CanPerform = true;
                return true;
            }
            catch (Exception ex)
            {
                //CancelCommand.CanPerform = false;
                Debugger.Break();
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        void StartSetup()
        {
            Trace.Assert(string.IsNullOrEmpty(DownloadedInstallerFileFullPath) == false);

            var processStartInfo = new ProcessStartInfo { FileName = DownloadedInstallerFileFullPath, UseShellExecute = true };
            Process.Start(processStartInfo);

            // NOTE: 強制終了。
            // TODO: この方法で良いかの検討。
            Environment.Exit(0);
        }

        public bool OpenDownloadWebSite()
        {
            if (LatestInstallerInformation == null) { return false; }
            if (LatestInstallerInformation.DownloadSiteUrl == null) { return false; }
            if (string.IsNullOrEmpty(LatestInstallerInformation.DownloadSiteUrl.ToString())) { return false; }

            Process.Start("explorer.exe", "\"" + LatestInstallerInformation.DownloadSiteUrl + "\"");
            return true;
        }
    }
}
