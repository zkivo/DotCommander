// See https://aka.ms/new-console-template for more information
// Console.WriteLine(Console.BackgroundColor);
// Console.WriteLine(Console.WindowHeight);
// Console.WriteLine(Console.WindowWidth);

using System.Diagnostics;

ConsoleKeyInfo key_info;
ConsoleModifiers mod;
string[] split;

int rows_of_a_page = Console.WindowHeight;
Console.TreatControlCAsInput = true;

string last_open_directory = "G:\\Il mio Drive\\Università";
string open_directory      = "G:\\Il mio Drive\\Università";
int index_list = 0;
int prev_index_list = 0;

//int height = Console.BufferHeight  = Console.WindowHeight; // one line is used as a buffer 
//int width  = Console.BufferWidth   = Console.WindowWidth;

string[] files = Directory.GetFiles(open_directory);
string[] dirs  = Directory.GetDirectories(open_directory);
string[] list  = new string[files.Length + dirs.Length]; 
dirs.CopyTo(list, 0);
files.CopyTo(list, dirs.Length);

refresh();
do {
    key_info = Console.ReadKey(true);
    mod = key_info.Modifiers; // alt, shift, ctrl modifiers information alt=1,shift=2,ctrl=4
    if (mod == ConsoleModifiers.Alt) {
        // just alt have been pressed
        if (key_info.Key.Equals(ConsoleKey.LeftArrow)) {
            // go to previous path
            change_dir(last_open_directory);
        }
    } else {
        // None modifiers have been pressed
        if (key_info.Key.Equals(ConsoleKey.DownArrow)) {
            if (index_list < list.Length - 1) {
                switch_highlight(index_list, index_list + 1);
                index_list++;
            }
        } else if (key_info.Key.Equals(ConsoleKey.UpArrow)) {
            if (index_list > 0) {
                switch_highlight(index_list, index_list - 1);
                index_list--;
            }
        } else if (key_info.Key.Equals(ConsoleKey.Enter)) {
            /* if it is a directory change the current dir
             * otherwise open the file or program */
            if (File.Exists(list[index_list])) {
                Process.Start(new ProcessStartInfo(list[index_list]) { UseShellExecute = true });
            } else if (Directory.Exists(list[index_list])) {
                change_dir(list[index_list]);
            } else {
                Console.Write("dunno 2");
            }
        } else if (key_info.Key.Equals(ConsoleKey.PageDown)) {
            prev_index_list = index_list;
            if (index_list + rows_of_a_page >= list.Length) index_list = list.Length - 1;
            else index_list += rows_of_a_page;
            switch_highlight(prev_index_list, index_list);

        } else if (key_info.Key.Equals(ConsoleKey.PageUp)) {
            prev_index_list = index_list;
            if (index_list - rows_of_a_page < 0) index_list = 0;
            else index_list -= rows_of_a_page;
            switch_highlight(prev_index_list, index_list);

        } else {
            Console.Beep();
        }
        if (index_list == 0) {
            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = false;
        }
    }
} while (true);

void change_dir(string path) {
    last_open_directory = open_directory;
    open_directory = path;
    index_list = 0;
    prev_index_list = 0;
    files = Directory.GetFiles(open_directory);
    dirs = Directory.GetDirectories(open_directory);
    list = new string[files.Length + dirs.Length];
    dirs.CopyTo(list, 0);
    files.CopyTo(list, dirs.Length);
    refresh();
}

void switch_highlight(int i_from, int i_to) {
    i_from++; //index from
    i_to++;   //index to
    Console.ResetColor();
    Console.ForegroundColor = ConsoleColor.White;
    try {
        Console.CursorLeft = 0;
        Console.CursorTop  = i_from;
        Console.Write(list[i_from - 1].Split("\\").Last<string>());
    } catch (Exception e) {
        //nothing
    }
    Console.ForegroundColor = ConsoleColor.Yellow;
    try {
        Console.CursorLeft = 0;
        Console.CursorTop  = i_to;
        Console.Write(list[i_to - 1].Split("\\").Last<string>());
    } catch (Exception e) {
        //nothing
    }
    Console.CursorVisible = false;
    //Console.MoveBufferArea(0, first, Console.BufferWidth, 1, 0, Console.BufferHeight - 1);
    //Console.MoveBufferArea(0, second, Console.BufferWidth, 1, 0, first);
    //Console.MoveBufferArea(0, Console.BufferHeight - 1, Console.BufferWidth, 1, 0, second);
}

void refresh() {
    Console.Title = open_directory + " - Dot-Commander";
    Console.Clear();
    Console.CursorTop  = 0;
    Console.CursorLeft = 0;
    Console.BackgroundColor = ConsoleColor.Black; 
    Console.ForegroundColor = ConsoleColor.Blue;
    Console.Write("   " + open_directory + "   ");
    Console.BackgroundColor = ConsoleColor.Black;
    Console.ForegroundColor = ConsoleColor.White;
    //int index = files.Length % index_list;
    int i = 0;
    foreach (string element in list) {
        if (i == list.Length - 1) {
            break;
        } else if (i == index_list) {
            // this record must be highlighted because the cursor is here
            Console.ForegroundColor = ConsoleColor.Yellow;
        } else Console.ForegroundColor = ConsoleColor.White;
        split = element.Split('\\');
        Console.CursorLeft = 0;
        Console.CursorTop = i + 1;
        Console.Write("" + split[split.Length - 1]);
        i++;
    }
    Console.SetCursorPosition(0, 0);
    Console.CursorVisible = false;
}
