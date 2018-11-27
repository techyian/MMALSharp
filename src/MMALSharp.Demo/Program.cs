using System;

namespace MMALSharp.Demo
{
    static class Program
    {        
        public static void OperationsHandler()
        {
            MMALCameraConfig.Debug = true;
            
            Console.WriteLine("Please select from an option below.");
            
            Console.WriteLine("1.    Picture");
            Console.WriteLine("2.    Video");
            Console.WriteLine("3.    Utilities");
            Console.WriteLine("4.    Configuration");
            Console.WriteLine("5.    Exit");

            var key = Console.ReadKey();

            var imageOps = new ImageOps();
            var videoOps = new VideoOps();
            
            switch (key.KeyChar)
            {
                case '1':
                    imageOps.Operations();
                    break;
                case '2':
                    videoOps.Operations();
                    break;
            }
        }
        
        static void Main(string[] args)
        {
            Console.WriteLine(@"
              __  __ __  __   _   _    ___ _                  
             |  \/  |  \/  | /_\ | |  / __| |_  __ _ _ _ _ __ 
             | |\/| | |\/| |/ _ \| |__\__ \ ' \/ _` | '_| '_ \
             |_|  |_|_|  |_/_/ \_\____|___/_||_\__,_|_| | .__/
                                                        |_|   
            ");
            
            Console.WriteLine("Welcome to the MMALSharp Demo application.");
            
            OperationsHandler();
        }
    }
}