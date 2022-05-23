// See https://aka.ms/new-console-template for more information
//Console.WriteLine(Console.BackgroundColor);
//Console.WriteLine(Console.WindowHeight);
//Console.WriteLine(Console.WindowWidth);

using System.Diagnostics;

string opened_directory = "G:\\Il mio Drive\\Università";
int index_list = 0;
ConsoleKeyInfo   key_info;
ConsoleModifiers mod;

Console.BufferHeight = Console.WindowHeight;
Console.BufferWidth  = Console.WindowWidth;

string[] files = Directory.GetFiles(opened_directory);
string[] dirs = Directory.GetDirectories(opened_directory);
string[] split;

do {
    box();
    key_info = Console.ReadKey(true);
    mod = key_info.Modifiers; // alt, shift, ctrl modifiers information
    if (mod > 0) {
        // If one or more modifiers have been pressed.

    } else {
        // None modifiers have been pressed
        if (key_info.Key.Equals(ConsoleKey.DownArrow)) {
            index_list++;
        } else if (key_info.Key.Equals(ConsoleKey.UpArrow)) {
            index_list--;
        } else if (key_info.Key.Equals(ConsoleKey.Enter)){
            Process.Start(new ProcessStartInfo(files[index_list]) { UseShellExecute = true });
        } else {
            Console.WriteLine("dunno");
        }
    }
} while (true);


void box() {
    Console.Clear();
    Console.CursorTop  = 0;
    Console.CursorLeft = 0;
    Console.BackgroundColor = ConsoleColor.Black; 
    Console.ForegroundColor = ConsoleColor.Blue;
    Console.WriteLine("   " + opened_directory + "   ");
    Console.BackgroundColor = ConsoleColor.Black;
    Console.ForegroundColor = ConsoleColor.White;
    //int index = files.Length % index_list;
    int i = 0;
    foreach (string file in files) {
        if (i == Console.BufferHeight - 2) {
            return;
        } else if (i == index_list) {
            // this record must be highlighted because the cursor is here
            Console.ForegroundColor = ConsoleColor.Yellow;
        } else Console.ForegroundColor = ConsoleColor.White;
        split = file.Split('\\');
        Console.WriteLine("" + split[split.Length - 1]);
        i++;
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
