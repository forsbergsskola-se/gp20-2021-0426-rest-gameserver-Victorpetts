using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace TinyBrowser {
    class Program {
        static void Main(string[] args) {
            var host = "acme.com";
            var uri = "/";
            var tcpClient = new TcpClient(host, 80);
            var stream = tcpClient.GetStream();
            var streamWriter = new StreamWriter(stream, Encoding.ASCII);

            var request = $"GET {uri} HTTP/1.1\r\nHost: {host}\r\n\r\n";
            streamWriter.Write(request);
            streamWriter.Flush();

            var streamReader = new StreamReader(stream);
            var response = streamReader.ReadToEnd();

            var uriBuilder = new UriBuilder(null, host);
            uriBuilder.Path = uri;
            Console.WriteLine($"Opened {uriBuilder}");
        }
    }
}