// See https://aka.ms/new-console-template for more information
//Console.WriteLine(Console.BackgroundColor);
//Console.WriteLine(Console.WindowHeight);
//Console.WriteLine(Console.WindowWidth);

using System;
using System.Diagnostics;

string opened_directory = "G:\\Il mio Drive\\Università";
int index_list = 0;
ConsoleKeyInfo   key_info;
ConsoleModifiers mod;

Console.BufferHeight  = Console.WindowHeight + 1; // one line is used as a buffer 
Console.BufferWidth   = Console.WindowWidth;
Console.CursorVisible = false;

string[] files = Directory.GetFiles(opened_directory);
string[] dirs = Directory.GetDirectories(opened_directory);
string[] split;
refresh();
do {
    key_info = Console.ReadKey(true);
    mod = key_info.Modifiers; // alt, shift, ctrl modifiers information
    if (mod > 0) {
        // If one or more modifiers have been pressed.

    } else {
        // None modifiers have been pressed
        if (key_info.Key.Equals(ConsoleKey.DownArrow)) {
            index_list++;
            switch_style_lines(index_list, index_list + 1);
        } else if (key_info.Key.Equals(ConsoleKey.UpArrow)) {
            index_list--;
            switch_style_lines(index_list + 2, index_list + 1);
        } else if (key_info.Key.Equals(ConsoleKey.Enter)){
            Process.Start(new ProcessStartInfo(files[index_list]) { UseShellExecute = true });
        } else {
            refresh();
            Console.WriteLine("dunno");
        }
    }
} while (true);

void switch_style_lines(int first, int second) {
    System.Console.CursorLeft = 0;
    System.Console.CursorTop  = first;
    ConsoleColor cf_first  = Console.ForegroundColor;
    ConsoleColor cb_first  = Console.BackgroundColor;
    System.Console.CursorTop  = second;
    ConsoleColor cf_second = Console.ForegroundColor;
    ConsoleColor cb_second = Console.BackgroundColor;
    
    ConsoleColor cf_temp   = cf_second;
    ConsoleColor cb_temp   = cb_second;
    
    Console.CursorTop = second;
    Console.ForegroundColor = cf_first;
    Console.Write("ads");
    //Console.MoveBufferArea(0, first, Console.BufferWidth, 1, 0, Console.BufferHeight - 1);
    //Console.MoveBufferArea(0, second, Console.BufferWidth, 1, 0, first);
    //Console.MoveBufferArea(0, Console.BufferHeight - 1, Console.BufferWidth, 1, 0, second);

}

void refresh() {
    Console.Title = opened_directory + " - Dot-Commander";
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
        if (i == Console.BufferHeight - 3) {
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
