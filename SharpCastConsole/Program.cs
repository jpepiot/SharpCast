namespace SharpCastConsole {
    using System;
    using System.Diagnostics;
    using SharpCast;

    class Program {

        static void Main(string[] args) {

            const string host = "192.168.1.58"; // ChromeCast host
            const string contentUrl = "http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4";
            const string contentType = "video/mp4";
            const string title = "Big buck dunny";
            const string thumbnailUrl = "http://www.ni-mate.com/wp-content/uploads/2012/12/Very_BBB_xmas1_8bit_600px_wide.jpg";

            try {
                //Trace.Listeners.Add(new ConsoleTraceListener());

                Player client = new Player(host);
                client.Connect();

                client.MediaStatusChanged += (sender, status) =>
                {
                    Console.WriteLine("New player state :" + status.PlayerState);
                };

                client.LoadVideo(new Uri(contentUrl), contentType, new MovieMediaMetadata {
                    Title = title,
                    Images = new[] { new Image { Url = thumbnailUrl } }
                });
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }
    }
}
