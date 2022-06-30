// See https://aka.ms/new-console-template for more information
// Console.WriteLine(Console.BackgroundColor);
// Console.WriteLine(Console.WindowHeight);
// Console.WriteLine(Console.WindowWidth);

using System.Diagnostics;
using System.Text.RegularExpressions;

const float RESET_SEATCH_TIME = 0.8f; //seconds

ConsoleKeyInfo key_info;
ConsoleModifiers mod;
string[] split;
string search_str = "";
DateTime prev_time = DateTime.Now;
TimeSpan diff;
Regex alphanum_regex = new Regex(@"^[a-zA-Z0-9\s,]*$");
List<string> history_dirs = new List<string>();

int rows_of_a_page = Console.WindowHeight;
int cols_of_a_page = Console.WindowWidth;
Console.TreatControlCAsInput = true; // also treats the other modifiers as input

//string open_directory      = "G:\\Il mio Drive\\Università";
history_dirs.Add("G:\\Il mio Drive\\Università");
int index_list      = 0;
int prev_index_list = 0;

bool left_db_focus = true; // specifies if the focus is on the
                           // left directory box

//int height = Console.BufferHeight  = Console.WindowHeight; // one line is used as a buffer 
//int width  = Console.BufferWidth   = Console.WindowWidth;

/*string[] files = Directory.GetFiles(history_dirs.Last<string>());
string[] dirs  = Directory.GetDirectories(history_dirs.Last<string>());
string[] list  = new string[files.Length + dirs.Length]; 
dirs.CopyTo(list, 0);
files.CopyTo(list, dirs.Length);*/


DotCommander.DirectoryBox db_left  = new DotCommander.DirectoryBox(60, 0, 120, 30, "G:\\Il mio Drive\\Università");
DotCommander.DirectoryBox db_right = new DotCommander.DirectoryBox(0,  0, 120, 30, "G:\\Il mio Drive\\Università");

//refresh();
db_left.draw();
db_right.draw();
do {
    key_info = Console.ReadKey(true);
    mod = key_info.Modifiers; // alt, shift, ctrl modifiers information alt=1,shift=2,ctrl=4
    if (mod > 0) {
        // some modifiers have been pressed
        if (mod == ConsoleModifiers.Control) {
            //just ctrl has been pressed
            if (key_info.Key.Equals(ConsoleKey.C)) {
                // go to previous path
                Environment.Exit(0);
            }
        } else if (mod == ConsoleModifiers.Alt) {
            // just alt has been pressed
        } else if (mod == ConsoleModifiers.Shift) {
            // just shift has been pressed
        } else {
            // a combination of the mods has been performed
        }
    } else {
        // None modifiers have been pressed
        if (key_info.Key.Equals(ConsoleKey.Enter)) {
            /* if it is a directory change the current dir
             * otherwise open the file or program */
            if (left_db_focus) db_left.enter_pressed();
            else db_right.enter_pressed();

        } else if (is_alphanumeric(key_info.KeyChar.ToString())) {
            // enters here if the input is alphanumeric
            // this code checks the list to find what
            // people write
            if   (left_db_focus) db_left.typed_alphanumeric();
            else db_right.typed_alphanumeric();

        } else if (key_info.Key.Equals(ConsoleKey.DownArrow)) {
            if (index_list < list.Length - 1) {
                switch_highlight(index_list, index_list + 1);
                db.switch_highlight(index_list, index_list + 1);
                index_list++;
            }
        } else if (key_info.Key.Equals(ConsoleKey.UpArrow)) {
            if (index_list > 0) {
                switch_highlight(index_list, index_list - 1);
                db.switch_highlight(index_list, index_list - 1);
                index_list--;
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

        } else if (key_info.Key.Equals(ConsoleKey.Backspace)) {
            string str_go_to;
            int num;
            try {
                num = history_dirs.Count;
                if (num > 1) {
                    // not in the first dir of the history
                    history_dirs.RemoveAt(num - 1);
                    str_go_to = history_dirs.Last<string>();
                    history_dirs.RemoveAt(num - 2);
                    change_dir(str_go_to);
                    db.change_dir(str_go_to);
                } 
            } catch(Exception e) {
                Console.Beep();
            }
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
    history_dirs.Add(path);
    //open_directory = path;
    index_list = 0;
    prev_index_list = 0;
    files = Directory.GetFiles(path);
    dirs = Directory.GetDirectories(path);
    list = new string[files.Length + dirs.Length];
    dirs.CopyTo(list, 0);
    files.CopyTo(list, dirs.Length);
    //refresh();
}

void switch_highlight(int i_from, int i_to) {
    /*
     * looks where index_from is pointing to and set the color
     * based on wheter it is a file or a directory
     * this part does not highlight
     */
    // ------ FROM -------
    int cursor_left = 0;
    if (i_from >= dirs.Length) {
        // index from points a file
        set_file_color(false);
    } else {
        // points to a directory
        cursor_left++;
        set_directory_color(false);
    }
    try {
        Console.SetCursorPosition(0, i_from + 1);
        if (cursor_left > 0) Console.Write("\\");
        Console.CursorLeft = cursor_left;
        Console.Write(list[i_from].Split("\\").Last<string>());
    } catch (Exception e) {
        //nothing
    }
    /*
     * looks where index_to is pointing to and set the color
     * based on wheter it is a file or a directory
     * this part highlights
     */
    // ------ TO -------
    cursor_left = 0;
    if (i_to >= dirs.Length) {
        // index from points a file
        set_file_color(true);
    } else {
        // points to a dir
        cursor_left++;
        set_directory_color(true);
    }
    try {
        Console.SetCursorPosition(0, i_to + 1);
        if (cursor_left > 0) Console.Write("\\");
        Console.CursorLeft = cursor_left;
        Console.Write(list[i_to].Split("\\").Last<string>());
    } catch (Exception e) {
        //nothing
    }
    Console.CursorVisible = false;
    //Console.MoveBufferArea(0, first, Console.BufferWidth, 1, 0, Console.BufferHeight - 1);
    //Console.MoveBufferArea(0, second, Console.BufferWidth, 1, 0, first);
    //Console.MoveBufferArea(0, Console.BufferHeight - 1, Console.BufferWidth, 1, 0, second);
}

void refresh() {
    Console.Title = history_dirs.Last<string>() + " - Dot-Commander";
    Console.ResetColor();
    Console.Clear();
    Console.CursorTop  = 0;
    Console.CursorLeft = 0;
    Console.BackgroundColor = ConsoleColor.Black; 
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write("   " + history_dirs.Last<string>() + "   ");
    Console.BackgroundColor = ConsoleColor.Black;
    Console.ForegroundColor = ConsoleColor.White;
    //int index = files.Length % index_list;
    int i = 0;
    foreach (string dir in dirs) {
        if (i == list.Length) {
            break;
        } else if (i == index_list) {
            // this record must be highlighted because the cursor is here
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.Yellow;
        } else {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
        }
        split = dir.Split('\\');
        Console.CursorLeft = 0;
        Console.CursorTop = i + 1;
        Console.Write("\\" + split[split.Length - 1]);
        i++;
    }
    foreach (string file in files) {
        if (i == list.Length) {
            break;
        } else if (i == index_list) {
            // this record must be highlighted because the cursor is here
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
        } else {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
        }
        split = file.Split('\\');
        Console.CursorLeft = 0;
        Console.CursorTop = i + 1;
        Console.Write("" + split[split.Length - 1]);
        i++;
    }
    Console.SetCursorPosition(0, 0);
    Console.CursorVisible = false;
}

void search_string(string str) {
    int i = 0;
    foreach (string element in list) {
        if (element.Split("\\").Last<string>().StartsWith(str, StringComparison.CurrentCultureIgnoreCase)) {
            // str is in the list
            switch_highlight(index_list, i);
            index_list = i;
            break;
        }
        i++;
    }
}

bool is_alphanumeric(string str) {
    return alphanum_regex.IsMatch(str);
}

void set_directory_color(bool highlight) {
    if (highlight) {
        // this record must be highlighted because the index is here
        Console.BackgroundColor = ConsoleColor.DarkGray;
        Console.ForegroundColor = ConsoleColor.Yellow;
    } else {
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.DarkYellow;
    }
}

void set_file_color(bool highlight) {
    if (highlight) {
        // this record must be highlighted because the cursor is here
        Console.BackgroundColor = ConsoleColor.DarkBlue;
        Console.ForegroundColor = ConsoleColor.White;
    } else {
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.DarkGreen;
    }
}