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
                Console.WriteLine("Downloading {0}...", Url);    // this doesn't wait for the result of GetResponseAsync()
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

        public void ConsoleWatcher()
        {
            do
            {
                string input = Console.ReadLine();
                Url = input;
            } while (Url != "stop");
        }

        public async Task ConsoleWatcherAsync()
        {
            Console.WriteLine("ConsoleWatcher Thread started");
            await Task.Run(new Action(ConsoleWatcher));
            Console.WriteLine("ConsoleWatcher Thread stopped");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Page p = new Page("http://www.google.fr");
            string last_url = "";

            p.ConsoleWatcherAsync();
            
            while (p.Url != "stop") {
                if (p.Url != last_url) {
                    Task.WaitAll(p.ProcessURLAsync());
                    last_url = p.Url;
                }
            }
            
            Console.WriteLine("Press enter to exit...");
            Console.ReadKey();
        }
    }
}
