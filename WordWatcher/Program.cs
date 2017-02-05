using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace WordWatcher
{
    public class Page
    {
        public string Url;
        public string Content;
        public int WordCounter;

        public Page(string url)
        {
            Url = url;
        }

        public void DownloadContent()
        {
            WebRequest request = WebRequest.Create(Url);
            WebResponse response = request.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream());
            Content = sr.ReadToEnd();

            response.Close();
            sr.Close();
        }

        public async Task DownloadContentAsync()
        {
            WebRequest request = WebRequest.Create(Url);

            try
            {
                Task<WebResponse> T_response = request.GetResponseAsync();
                Console.WriteLine("Downloading while in method...");    // this doesn't wait for the result of GetResponseAsync()
                WebResponse response = await T_response;

                // Read response
                StreamReader sr = new StreamReader(response.GetResponseStream());
                try
                {
                    Content = await sr.ReadToEndAsync();
                }
                finally
                {
                    response.Close();
                    sr.Close();
                }
            }
            catch (WebException e)
            {
                Console.WriteLine("Error: {0}", e);
                Content = "";
            }            
        }

        public void SetContent(string acontent)
        {
            Content = acontent;
        }

        public void CountWord()
        {
            char[] SplitChars = {' ', ',', ';', '<', '>', '/', '\t', '\n', '.', '#', '=' };
            WordCounter = Content.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries).Count();
        }

        public async Task ProcessURLAsync()
        {
            await DownloadContentAsync();
            CountWord();
            Console.WriteLine("There is {0} words in my page {1}", WordCounter, Url);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<Page> UrlList = new List<Page> {
                                    new Page("http://www.google.fr"),
                                    new Page("http://www.facebook.fr"),
                                    new Page("http://www.lemonde.fr"),
                                    new Page("http://www.meteofrance.fr")
                                    };

            IEnumerable<Task> downloadTasksQuery = from page in UrlList select page.ProcessURLAsync();
            Task[] downloadTasks = downloadTasksQuery.ToArray();
            Task.WaitAll(downloadTasks);        // OK for void returning Task but how to do it it Task<int> is returned
            Console.WriteLine("Press enter to exit...");
            Console.ReadKey();
        }
    }
}
