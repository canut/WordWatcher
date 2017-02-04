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
    }


    class Program
    {
        static void Main(string[] args)
        {
            // Simple URL download test (async)
            Page mypage3 = new Page("http://www.gooazeerrgle.com");
            Task.WaitAll(mypage3.DownloadContentAsync());

            // Use of Exception but a simple if-statement is better here
            try
            {
                mypage3.CountWord();
                Console.WriteLine("There is {0} words in my page {1}", mypage3.WordCounter, mypage3.Url);
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine("\n\nPage was empty !\nError: {0}", e);
            }
            finally
            {
                Console.ReadKey();
            }
        }
    }
}
