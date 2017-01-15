namespace SportSystem.ConsoleClient
{
    using System;

    public class Program
    {
        static void Main()
        {
            Console.WriteLine("Press any key to continue and recreate database. Current data will be lost.");
            Console.ReadKey();

            Console.WriteLine(Environment.NewLine + "Creating new database...");
            Engine.Instance.Start();
        }
    }
}
