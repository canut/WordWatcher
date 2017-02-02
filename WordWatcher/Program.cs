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
            // Here implement how you download the content given an URL
            // version 2 could try to implement streams
            WebRequest request = WebRequest.Create(Url);
            WebResponse response = request.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream());
            Content = sr.ReadToEnd();

            response.Close();
            sr.Close();
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
            // Simple test
            Page mypage = new Page("http://www.mysite.com");
            mypage.SetContent("This is my content from my website");
            mypage.CountWord();
            Console.WriteLine("Content: {0}", mypage.Content);
            Console.WriteLine("There is {0} words in my page {1}", mypage.WordCounter, mypage.Url);
            Console.ReadKey();

            // Simple URL download test (sync)
            Page mypage2 = new Page("http://www.google.com");
            mypage2.DownloadContent();
            mypage2.CountWord();
            Console.WriteLine("Content: {0}", mypage2.Content);
            Console.WriteLine("There is {0} words in my page {1}", mypage2.WordCounter, mypage2.Url);
            Console.ReadKey();

            // Simple URL download test (async)

            // Multi downloads by using Threads

        }
    }
}
