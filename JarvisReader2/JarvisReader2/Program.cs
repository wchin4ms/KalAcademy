using System;

namespace JarvisReader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("FarmLabel: ");
            string farmLabel = Console.ReadLine();
            Console.Write("Site: ");
            string site = Console.ReadLine();
            FarmOverview farm = new FarmOverview(farmLabel);
            Console.WriteLine("------------------");
            Console.Write(farm.InfoString());
        }
    }
}
