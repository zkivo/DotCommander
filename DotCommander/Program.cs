// See https://aka.ms/new-console-template for more information
//Console.WriteLine(Console.BackgroundColor);
//Console.WriteLine(Console.WindowHeight);
//Console.WriteLine(Console.WindowWidth);
Console.Clear();

string opened_directory = "G:\\Il mio Drive\\Università";

box();
Console.ReadKey();

void box()
{
    Console.Clear();
    Console.CursorTop  = 0;
    Console.CursorLeft = 0;
    Console.BackgroundColor = (ConsoleColor) 0; //gray
    Console.ForegroundColor = (ConsoleColor) ConsoleColor.Blue;
    string[] files = Directory.GetFiles(opened_directory);
    string[] dirs = Directory.GetDirectories(opened_directory);
    string[] split;
    Console.WriteLine("   " + opened_directory + "   ");
    Console.BackgroundColor = (ConsoleColor) 0; //gray
    Console.ForegroundColor = (ConsoleColor) 15;
    foreach (string file in files)
    {
        split = file.Split('\\');
        Console.WriteLine("" + split[split.Length - 1]);
    }
    //for (int i = 0; i < Console.WindowHeight; i++)
    //{
    //    for (int j = 0; j < Console.WindowWidth; j++)
    //    {
    //        if (i == 0 || i == Console.WindowHeight - 1)
    //        {
    //            Console.Write("\u2500");
    //        } else
    //        {
    //            if (j == 0 || j == Console.WindowWidth - 1)
    //            {
    //                Console.Write("\u2502");
    //            } else
    //            {
    //                Console.Write(" ");
    //            }
    //        }
    //    }
    //}
}
