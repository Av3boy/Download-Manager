using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Download
{
    public class ServerUtils
    {
        public string host;
        public string username;
        public string password;
        public int port;
        public string database;

        public ServerUtils()
        {
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string startupPath = Path.Combine(path, ".env");

            if (!File.Exists(startupPath))
            {
                Console.WriteLine("A file containing your server credentials could not be found. \n"+
                    "Some of the functionalities in this program use a server to function. \n \n" +
                    
                    "Would you like to specify server details or would you like to continue without specifying a server? \n" +
                    "Press 'Y' to create server configuration file. \n" +
                    "Press 'N' to continue without a configured server. \n");

                string option = Console.ReadLine();

                if (option == "Y")
                {
                    Console.WriteLine("To configure your server details first input your server host: \n");
                    host = "host=" + Console.ReadLine() + ";";
                    Console.WriteLine("\n Next input your username: \n");
                    username = "username=" + Console.ReadLine() + ";";
                    Console.WriteLine("\n Next input your password: \n");
                    password = "username=" + Console.ReadLine() + ";";
                    Console.WriteLine("\n Next input your port: \n");
                    string tempPort = Console.ReadLine();

                    bool looping = true;
                    while (looping)
                    {
                        if (!int.TryParse(tempPort, out port))
                        {
                            Console.WriteLine("The port you imported was not a proper number. Please try again: \n");
                            tempPort = Console.ReadLine();
                        }
                        else
                            looping = false;
                    }

                    string text = host + username + password + port + "port=" + ";";
                    File.WriteAllText(startupPath, text);

                    return;
                }
                if (option == "N")
                    return;
            }

            string creds = File.ReadAllText(startupPath);

            string[] words = creds.Split(';');

            foreach (var word in words)
            {
                string credentialType = word.Split(':')[0];
                string credential = word.Substring(word.IndexOf(':') + 1);

                switch (credentialType)
                {
                    case "host":
                        host = credential;
                        break;
                    case "username":
                        username = credential;
                        break;
                    case "password":
                        password = credential;
                        break;
                    case "port":
                        port = int.Parse(credential);
                        break;
                    case "sqlDataBase":
                        database = credential;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}