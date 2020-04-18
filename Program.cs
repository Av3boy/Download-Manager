using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Diagnostics;
using VideoLibrary;
using System.Media;
using System.Runtime.InteropServices;

namespace Download
{
    class Program
    {

        public static Thread thread;
        public static string errorMsg = "Your input choice isn't any of the possible choices. \n" +
                            "Please try choosing one of the following: \n" +
                            "1: Download a file from server. \n" +
                            "2. Download Audio from youtube. \n" +
                            "3. Download Video with audio from youtube. \n";

        static void Main(string[] args)
        {

            //User can cancel the operation at any moment
            thread = new Thread(() => {
                ListenForKey();
            });
            thread.Start();

            Console.WriteLine("Hello. This Program is used to download files from multiple sources. \n" +
                "First please choose an download method: \n" +
                "1: Download a file from server. \n" +
                "2. Download Audio from youtube. \n" +
                "3. Download Video with audio from youtube. \n \n" +
                "Also typing 'help' prints out possible commands.");

            string temp = Console.ReadLine();

            if (int.TryParse(temp, out int choice))
                MainLoop(false, choice);
            else
            {
                Console.WriteLine();
                MainLoop(true, choice);
            }

            Console.ReadLine();
        }

        public static void MainLoop(bool looping, int choice)
        {

            if (looping == true)
            {
                string temp = Console.ReadLine();
                if (int.TryParse(temp, out choice))
                {
                    if (choice != 1 || choice != 2 || choice != 3)
                    {
                        Console.WriteLine();

                        MainLoop(true, 0);
                    }
                    else
                    {
                        Console.WriteLine(errorMsg);
                        MainLoop(false, choice);
                    }
                }
                else
                {
                    Console.WriteLine(errorMsg);
                }
            }

            switch (choice)
            {
                case 1:

                    Console.WriteLine("You have chosen to download a file from the server. \n" +
                        "Next, please insert the file that you would like to download. \n");
                    string serverFileToDownload = Console.ReadLine();

                    Console.WriteLine("Now, please specify the file path you'd like to give the file.\n ");
                    string pathToDownloadServerFile = GetValidPath();

                    DownloadFile(serverFileToDownload, pathToDownloadServerFile);
                    break;

                case 2:
                    Console.WriteLine("You have chosen to download video from YouTube. \n" +
                        "Next, please insert the link to the video that you would like to download.\n");
                    string youtubeUrl = Console.ReadLine();

                    Console.WriteLine("Now, please specify the file path you'd like to give the video.\n");
                    string pathToyoutubeVideo = GetValidPath();

                    DownloadYoutube(youtubeUrl, pathToyoutubeVideo);
                    break;

                case 3:
                    Console.WriteLine("Unfortunately the current version of the project does not include this functionality.");
                    // TODO: add download Video with audio from youtube functionality.
                    break;

                default:
                    Console.WriteLine(errorMsg);
                    MainLoop(true, 0);
                    break;
            }
        }

        public static void ListenForKey()
        {
            while (true)
            {
                //
                //string input = Console.ReadLine();

                /*switch (input)
                {
                    case "help":
                        Console.WriteLine("'quit' exits the program. \n" +
                            "'cancel' cancels the ongoing function. \n " +
                            "''");
                        break;

                    case "cancel":
                        Console.WriteLine("Are you sure you want to cancel your ongoing function? \n" +
                            "Type 'Y' to cancel function. \n Type 'N' to continue with ongoing function. \n");

                        string cancellation = Console.ReadLine();



                        break;

                    case "quit":
                        Environment.Exit(0);
                        break;

                    default:
                        break;
                }*/
            }
        }

        public static void DownloadFile(string file, string path)
        {
            thread = new Thread(() => {

                string file_ = Encoding.UTF8.GetString(DownloadFromServer(file));
                if (path.EndsWith(@"\") || path.EndsWith("/"))
                {
                    path.Substring(path.Length - 1);
                }
                else
                    path += @"\";

                File.WriteAllText(path + file_, file_);
                Console.WriteLine(file_);

            });// Download(file, path));

            thread.Start();
            Wait();
        }

        public static void DownloadYoutube(string youtubeUrl, string pathToDownloaded)
        {
            thread = new Thread(() => {

                YouTube youtube = YouTube.Default;
                Video vid = youtube.GetVideo(youtubeUrl);

                if (pathToDownloaded.EndsWith(@"\") || pathToDownloaded.EndsWith("/"))
                {
                    pathToDownloaded.Substring(pathToDownloaded.Length - 1);
                }
                else
                    pathToDownloaded += @"\";

                File.WriteAllBytes(pathToDownloaded + vid.FullName, vid.GetBytes());   
                
                Console.WriteLine(pathToDownloaded + vid.FullName);
                Console.WriteLine(vid.Title);
            });// Download(file, path));

            thread.Start();
            Wait();
        }

        static void Wait()
        {
            int i = 0;
            while (thread.IsAlive)
            {
                Console.WriteLine("Time Elapsed: {0}", i);
                Thread.Sleep(1000);
                i++;
            }
        }

        static string GetValidPath()
        {
            string path = Console.ReadLine();
            if (Uri.IsWellFormedUriString(path, UriKind.Absolute))
            {
                Console.WriteLine("The inserted path: '" + path + "' is not valid. \n" +
                    "Please try again.");

                return GetValidPath();

                //C:\Users\Root\Desktop
            }
            else
                return path;
        }

        static byte[] DownloadFromServer(string file)
        {
            WebClient request = new WebClient();
            ServerUtils server = new ServerUtils();

            string ftpString = "ftp://" + server.host + "/";
            NetworkCredential ftpCredentials = new NetworkCredential(server.username, server.password);

            string url = ftpString + "/Programs/" + file;
            request.Credentials = ftpCredentials;

            Console.WriteLine("downloading..");

            try
            {
                byte[] data = request.DownloadData(url);
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new byte[0];
            }
        }

        /*static void Play()
        {
            //Frequenz/Audio/videoplayback.mp3
            byte[] sound = DownloadFromServer("Frequenz/Audio/videoplayback.mp3");

            SoundPlayer player = new SoundPlayer(@"C:\Users\Root\Desktop\test.wav");
            player.Play();
        }*/
    }
}
