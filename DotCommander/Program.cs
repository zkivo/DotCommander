// See https://aka.ms/new-console-template for more information
//Console.WriteLine(Console.BackgroundColor);
//Console.WriteLine(Console.WindowHeight);
//Console.WriteLine(Console.WindowWidth);
Console.Clear();

string[] files = Directory.GetFiles("G:\\Il mio Drive\\Università");
string[] dirs = Directory.GetDirectories("G:\\Il mio Drive\\Università");

Console.ForegroundColor = (ConsoleColor) 3;

foreach (string file in files)
{
    Console.WriteLine(file);
}

Console.ForegroundColor = (ConsoleColor) 2;

foreach (string a in dirs)
{
    Console.WriteLine(a);
}
box();
Console.ReadKey();

void box()
{
    for (int i = 0; i < Console.WindowHeight; i++)
    {
        for (int j = 0; j < Console.WindowWidth; j++)
        {
            if (i == 0 || i == Console.WindowHeight - 1)
            {
                Console.Write("\u2500");
            } else
            {
                if (j == 0 || j == Console.WindowWidth - 1)
                {
                    Console.Write("\u2502");
                } else
                {
                    Console.Write(" ");
                }
            }
        }
    }
}
