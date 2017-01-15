namespace SportSystem.ConsoleClient
{
    using System;

    public class Program
    {
        /// <summary>
        /// Project for testing purposes
        /// </summary>
        static void Main()
        {
            Console.WriteLine("Press any key to continue and update database.");
            Console.ReadKey();

            Console.WriteLine(Environment.NewLine + "Updating database...");
            Engine.Instance.Start();
        }
    }
}
