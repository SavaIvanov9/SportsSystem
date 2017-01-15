namespace SportsSystem.Importer
{
    using System;
    using SportsSystem.Importer.Core;

    public class Launcher
    {
        public static void Main()
        {
            Console.WriteLine("Press any key to continue and recreate database. Current data will be lost.");
            Console.ReadKey();

            Console.WriteLine(Environment.NewLine + "Creating new database...");

            Engine.Instance.Start();
        }
    }
}
