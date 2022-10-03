using System;
using System.Drawing;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace code
{
    class UNI
    {
        public static TcpClient ClientUser = new TcpClient();
        public static string IP;
        public static int Role;
        public static string RPS;
        public static string ORPS;
        public static IPAddress ChosenIP;
        public static string VSIP;
    }



    class init
    {


        public static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.SetWindowSize(40,15);
            Console.WriteLine("Host = 1 | Join = 2");
            string op = Console.ReadLine();

            if (op == "1") {
                
                ServerSideSync();
                UNI.Role = 1;
                GameInit();

            }
            if (op == "2") { 
                ClientSideSync();
                UNI.Role = 2;
                GameInit();
            }


        }



        private static void GameInit()
        {
            int runtime = 0;
            while (runtime == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("============================");
                Console.WriteLine("Rock Paper Scissors! [1/2/3]");
                UNI.RPS = Console.ReadLine();


                if (UNI.Role == 1)
                {
                    ServerListenForOpponent();

                    EvalResults();
                }

                if (UNI.Role == 2)
                {
                    Console.Write("Waiting on Opponent \n");
                    ClientSendData();

                    EvalResults();
                }


                Thread.Sleep(500);
            }





        }



        private static void ServerSideSync()
        {

            string hostname = Dns.GetHostName();// this will get your local computers hostname.
            IPHostEntry ipEntry = Dns.GetHostEntry(hostname);
            IPAddress myip = ipEntry.AddressList[0];//use for loop to display desired ip address
            int i = 0;
            int j = 0;
            IPAddress[] Valid_IPv4 = new IPAddress[6];

            
            while (i < ipEntry.AddressList.Length)
            {
                if (ipEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    
                        Valid_IPv4[j] = ipEntry.AddressList[i];
                        Console.Write("Your IP is: ");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(ipEntry.AddressList[i]);
                        Console.ForegroundColor = ConsoleColor.White;
                    j++;
                }

                i++;
            }

            /*
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface nic in nics)
    
                Console.WriteLine(nic.Name);

            Console.ReadLine();
            */
       

            UNI.ChosenIP = Valid_IPv4[0]; // gets 1st ip out of list of ipv4's

            /* Initializes the Listener */
            TcpListener ListeningPort = new TcpListener(UNI.ChosenIP, 8001);


            /* Start Listeneting at the specified port */
            Console.Write("Waiting for Connection");
            ListeningPort.Start();


            Socket Socket = ListeningPort.AcceptSocket();


            byte[] ByteStream = new byte[100];

            int Data = Socket.Receive(ByteStream);
            UNI.VSIP = System.Text.Encoding.ASCII.GetString(ByteStream);
            

            

            ASCIIEncoding asen = new ASCIIEncoding();
            

            byte[] ByteStream2 = new byte[100];
            int Data2 = Socket.Receive(ByteStream2);
            UNI.IP = Convert.ToString(ByteStream2);
            string p2input = System.Text.Encoding.ASCII.GetString(ByteStream);
            UNI.ORPS = Convert.ToString(p2input[0]);

            for (int index = 0; index < Data2; index++)
                Console.Write(Convert.ToChar(ByteStream2[index]));

            Socket.Send(asen.GetBytes("Reply:Connected to " + hostname + "\n"));
            /* clean up */
            Socket.Close();
            ListeningPort.Stop();

        }
        private static void ClientSideSync()
        {
            //------------------------------
            UNI.ClientUser = new TcpClient();
            Console.WriteLine("Enter IP");
            UNI.IP = Console.ReadLine();


            UNI.ClientUser.Connect(UNI.IP, 8001);

            string hostname = Dns.GetHostName();

            IPHostEntry ipEntry = Dns.GetHostEntry(hostname);
            IPAddress myip = ipEntry.AddressList[0];//use for loop to display desired ip address
            int i = 0;
            int j = 0;
            IPAddress[] Valid_IPv4 = new IPAddress[6];
            while (i < ipEntry.AddressList.Length)
            {
                if (ipEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    Valid_IPv4[j] = ipEntry.AddressList[i];

                    j++;
                }

                i++;
            }
            ASCIIEncoding asen = new ASCIIEncoding();

            UNI.VSIP = Convert.ToString(Valid_IPv4[0]); // gets 1st ip out of list of ipv4's
            Stream ustm = UNI.ClientUser.GetStream();
            byte[] ba2 = asen.GetBytes(UNI.VSIP);
            ustm.Write(ba2, 0, ba2.Length);
            //vs ip os sent...
            //now servside to recieve it...



            String str = "\nReply: Connected to " + hostname + "\n";
            Stream stm = UNI.ClientUser.GetStream();

            
            byte[] ba = asen.GetBytes(str);
            //Console.WriteLine("Transmitting.....");

            stm.Write(ba, 0, ba.Length);

            byte[] bb = new byte[100];
            int k = stm.Read(bb, 0, 100);
            for (int index = 0; index < k; index++)
                Console.Write(Convert.ToChar(bb[index]));
           
            
            
            UNI.ClientUser.Close();
        }
        private static void ServerListenForOpponent()
        {



            ASCIIEncoding asen = new ASCIIEncoding();
            /* Initializes the Listener */
            TcpListener ListeningPort = new TcpListener(UNI.ChosenIP, 8001);


            /* Start Listeneting at the specified port */
            Console.Write("Waiting on Opponent \n");
            ListeningPort.Start();


            Socket Socket = ListeningPort.AcceptSocket();


            byte[] ByteStream = new byte[100];

            int Data = Socket.Receive(ByteStream);

            
            string p2input = System.Text.Encoding.ASCII.GetString(ByteStream);
            UNI.ORPS = Convert.ToString(p2input[0]);
          
            //Socket.Send(asen.GetBytes("Your Message was sent"));

            /* clean up */
            Socket.Close();
            ListeningPort.Stop();
            ServerSendData();
        }
        private static void ClientListenForOpponent()
        {


            ASCIIEncoding asen = new ASCIIEncoding();

            string hostname = Dns.GetHostName();// this will get your local computers hostname.
            IPHostEntry ipEntry = Dns.GetHostEntry(hostname);
            IPAddress myip = ipEntry.AddressList[0];//use for loop to display desired ip address
            int i = 0;
            int j = 0;
            IPAddress[] Valid_IPv4 = new IPAddress[6];
            while (i < ipEntry.AddressList.Length)
            {
                if (ipEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    Valid_IPv4[j] = ipEntry.AddressList[i];

                    j++;
                }

                i++;
            }

            UNI.ChosenIP = Valid_IPv4[0]; // gets 1st ip out of list of ipv4's

            /* Initializes the Listener */
            TcpListener ListeningPort = new TcpListener(UNI.ChosenIP, 8001);


            /* Start Listeneting at the specified port */
            ListeningPort.Start();


            Socket Socket = ListeningPort.AcceptSocket();


            byte[] ByteStream = new byte[100];

            int Data = Socket.Receive(ByteStream);

            Console.ForegroundColor = ConsoleColor.Yellow;
            string p2input = System.Text.Encoding.ASCII.GetString(ByteStream);
            UNI.ORPS = Convert.ToString(p2input[0]);
        
            Console.ForegroundColor = ConsoleColor.White;


            //Socket.Send(asen.GetBytes("Your Message was sent"));

            /* clean up */
            Socket.Close();
            ListeningPort.Stop();

        }

        private static void ServerSendData()
        {

            

            UNI.ClientUser = new TcpClient();
          
            //Console.WriteLine(UNI.VSIP);
            
            UNI.ClientUser.Connect(UNI.VSIP, 8001);
            // use the ipaddress as in the server program



            string hostname = Dns.GetHostName();
            String str = UNI.RPS;
            Stream stm = UNI.ClientUser.GetStream();

            ASCIIEncoding asen = new ASCIIEncoding();
            byte[] ba = asen.GetBytes(str);
            //Console.WriteLine("Transmitting.....");

            stm.Write(ba, 0, ba.Length);

            byte[] bb = new byte[100];
            int k = stm.Read(bb, 0, 100);



            UNI.ClientUser.Close();

            

        }
        private static void ClientSendData()
        {

            

            UNI.ClientUser = new TcpClient();
            int lc = 0; // lc * 2 = total seconds of wait
            while (lc < 10) {
                try
                {
                    UNI.ClientUser.Connect(UNI.IP, 8001);
                    lc = 5;
                }
                catch  { 
                
                }
                lc++;
            }
            // use the ipaddress as in the server program

           

            string hostname = Dns.GetHostName();
            String str = UNI.RPS;
            Stream stm = UNI.ClientUser.GetStream();

            ASCIIEncoding asen = new ASCIIEncoding();
            byte[] ba = asen.GetBytes(str);
            //Console.WriteLine("Transmitting.....");

            stm.Write(ba, 0, ba.Length);

            byte[] bb = new byte[100];
            int k = stm.Read(bb, 0, 100);

         

            UNI.ClientUser.Close();

            ClientListenForOpponent();

        }

        

        private static void EvalResults()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(" [You]     ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[P2]");
            Console.ForegroundColor = ConsoleColor.Yellow;

            if (UNI.RPS == "1")
            {
                
                if (UNI.ORPS == "1")
                {
                    Console.WriteLine("Rock vs Rock");
                    Console.WriteLine("Its a TIE!");
                }

                if (UNI.ORPS == "2")
                {
                    Console.WriteLine("Rock vs Paper");
                    Console.WriteLine("You lost!");

                }

                if (UNI.ORPS == "3")
                {
                    Console.WriteLine("Rock vs Scissors");
                    Console.WriteLine("You Won!");
                }

            }

            if (UNI.RPS == "2") 
            {

                if (UNI.ORPS == "1")
                {
                    Console.WriteLine("Paper vs Rock");
                    Console.WriteLine("You Won!");
                }

                if (UNI.ORPS == "2")
                {
                    Console.WriteLine("Paper vs Paper");
                    Console.WriteLine("Its a TIE!");
                }

                if (UNI.ORPS == "3")
                {
                    Console.WriteLine("Paper vs Scissors");
                    Console.WriteLine("You lost!");
                }




            }

            if (UNI.RPS == "3")
            {

                if (UNI.ORPS == "1")
                {
                    Console.WriteLine("Scissors vs Rock");
                    Console.WriteLine("You lost!");
                }

                if (UNI.ORPS == "2")
                {
                    Console.WriteLine("Scissors vs Paper");
                    Console.WriteLine("You Won!");
                }

                if (UNI.ORPS == "3")
                {

                    Console.WriteLine("Scissors vs Scissors");
                    Console.WriteLine("Its a TIE!");
                }



            }

        }

        
    }

   
}