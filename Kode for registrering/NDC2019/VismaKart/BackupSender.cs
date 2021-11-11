using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using VismaKart.Utils;
using Windows.Storage;

namespace VismaKart
{
    public static class BackupSender
    {
        public static bool _haveTriedToSend = false;

        public static void TrySendBackup()
        {
            if (_haveTriedToSend) return;
            _haveTriedToSend = true;
            try
            {
                var storageFolder = ApplicationData.Current.LocalFolder;
                var scorefile = storageFolder
                    .GetFileAsync("score.txt")
                    .GetAwaiter().GetResult();

                var text = FileIO.ReadTextAsync(scorefile)
                    .GetAwaiter().GetResult();

                var b = new Backup
                {
                    Data = text
                };

                var client = HttpClientFactory.Create();
                var tokenSrc = new CancellationTokenSource(TimeSpan.FromSeconds(15));
                var result = client.PostAsync($"https://vismakart.azurewebsites.net/api/participant/fulldump",
                        new StringContent(
                            JsonConvert.SerializeObject(b),
                            Encoding.UTF8,
                            "application/json"),
                        tokenSrc.Token);
            }
            catch { }
        }
    }
}