using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Map_Player
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.Title = "Map Player";
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("attempting to run the map player");
            Player player;
            try
            {
                player = new Player(/*args[0]*/"");
                player.Run();
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("error");
            }
            return 0;
        }
    }
}
