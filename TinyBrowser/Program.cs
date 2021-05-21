using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace TinyBrowser {
    class Program {
        
        static string FindTextBetweenTags(string original, string start, string end) {
            var titleIndex = original.IndexOf(start);
            string title = string.Empty;
            if (titleIndex != -1) {
                titleIndex += start.Length;
                var titleEndIndex = original.IndexOf(end);
                if (titleEndIndex > titleIndex) {
                    title = original[titleIndex..titleEndIndex];
                }
            }

            return title;
        }
        
        static void Main(string[] args) {
            
                const string host = "acme.com";
                const string uri = "/";
                const int port = 80;
                
                var tcpClient = new TcpClient(host, port);
                var stream = tcpClient.GetStream();
                var streamWriter = new StreamWriter(stream, Encoding.ASCII);

                var request = $"GET {uri} HTTP/1.1\r\nHost: {host}\r\n\r\n";
                streamWriter.Write(request);
                streamWriter.Flush();

                var streamReader = new StreamReader(stream);
                var response = streamReader.ReadToEnd();

                var uriBuilder = new UriBuilder(null, host) {Path = uri};
                Console.WriteLine($"Opened {uriBuilder}");

                var titleText = FindTextBetweenTags(response, "<title>", "</title>");
                Console.WriteLine("Title: " + titleText);

                var links = GetLinks(response);
                
                foreach (var link in links) {
                    Console.Write($"{links.IndexOf(link)}");
                    // Console.WriteLine($" {links.Count}");
                }

                // stream.Close();
                // tcpClient.Close();
        }

        private static List<string[]> GetLinks(string response) {
            var links = new List<string[]>();
            var regex = new Regex("<a href=[\"|'](?<link>.*?)[\"|'].*?>(<b>|<img.*?>)?(?<name>.*?)(</b>)?</a>", RegexOptions.None);
            if (!regex.IsMatch(response)) return links;

            foreach (Match match in regex.Matches(response)) {
                links.Add(new[] {match.Groups["title"].Value, match.Groups["link"].Value});
            }
            return links;
        }

    }
}