// See https://aka.ms/new-console-template for more information
// Console.WriteLine(Console.BackgroundColor);
// Console.WriteLine(Console.WindowHeight);
// Console.WriteLine(Console.WindowWidth);

using System;
using System.Diagnostics;

string opened_directory = "G:\\Il mio Drive\\Università";
int index_list = 0;
ConsoleKeyInfo   key_info;
ConsoleModifiers mod;

int height = Console.BufferHeight  = Console.WindowHeight; // one line is used as a buffer 
int width  = Console.BufferWidth   = Console.WindowWidth;

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
            if (index_list < Console.BufferHeight - 2) {
                switch_highlight(index_list, index_list + 1);
                index_list++;
            }
        } else if (key_info.Key.Equals(ConsoleKey.UpArrow)) {
            if (index_list > 0) {
                switch_highlight(index_list, index_list - 1);
                index_list--;
            }
        } else if (key_info.Key.Equals(ConsoleKey.Enter)){
            Process.Start(new ProcessStartInfo(files[index_list]) { UseShellExecute = true });
        } else {
            Console.Beep();
        }
    }
} while (true);

void switch_highlight(int i_from, int i_to) {
    i_from++; //index from
    i_to++;   //index to
    Console.ResetColor();
    try {
        Console.CursorLeft = 0;
        Console.CursorTop  = i_from;
        Console.Write(files[i_from - 1].Split("\\").Last<string>());
    } catch (Exception e) {

    }
    Console.ForegroundColor = ConsoleColor.Yellow;
    try {
        Console.CursorLeft = 0;
        Console.CursorTop  = i_to;
        Console.Write(files[i_to - 1].Split("\\").Last<string>());
    } catch (Exception e) {

    }
    Console.SetCursorPosition(0, 0);
    Console.ResetColor();
    Console.CursorVisible = false;
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
    Console.Write("   " + opened_directory + "   ");
    Console.BackgroundColor = ConsoleColor.Black;
    Console.ForegroundColor = ConsoleColor.White;
    //int index = files.Length % index_list;
    int i = 0;
    foreach (string file in files) {
        if (i == Console.BufferHeight - 1) {
            return;
        } else if (i == index_list) {
            // this record must be highlighted because the cursor is here
            Console.ForegroundColor = ConsoleColor.Yellow;
        } else Console.ForegroundColor = ConsoleColor.White;
        split = file.Split('\\');
        Console.CursorLeft = 0;
        Console.CursorTop  = i + 1;
        Console.Write("" + split[split.Length - 1]);
        i++;
    }
}
