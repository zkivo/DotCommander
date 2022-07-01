// See https://aka.ms/new-console-template for more information
// Console.WriteLine(Console.BackgroundColor);
// Console.WriteLine(Console.WindowHeight);
// Console.WriteLine(Console.WindowWidth);

using System.Diagnostics;
using System.Text.RegularExpressions;

ConsoleKeyInfo key_info;
ConsoleModifiers mod;
Regex alphanum_regex = new Regex(@"^[a-zA-Z0-9\s,]*$");

Console.TreatControlCAsInput = true; // also treats the other modifiers as input

bool left_db_focus = true; // specifies if the focus is on the left box

DotCommander.DirectoryBox db_left  = new DotCommander.DirectoryBox(0,  0,  60, 30, Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), left_db_focus);
DotCommander.DirectoryBox db_right = new DotCommander.DirectoryBox(60, 0, 120, 30, Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), !left_db_focus);

//refresh();
db_left.draw();
db_right.draw();
db_left.set_console_cursor();
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
            } else if (key_info.Key.Equals(ConsoleKey.L)) {
                if (left_db_focus) db_left.focus_link();
                else db_right.focus_link();
            } else if (key_info.Key.Equals(ConsoleKey.Tab)) {
                left_db_focus = !left_db_focus;
                db_left.switch_focus();
                db_right.switch_focus();
                if (left_db_focus) {
                    db_left.set_console_cursor();
                } else {
                    db_right.set_console_cursor();
                }
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
            if (left_db_focus) db_left.typed_alphanumeric(key_info.KeyChar);
            else db_right.typed_alphanumeric(key_info.KeyChar);
        } else if (key_info.Key.Equals(ConsoleKey.DownArrow)) {
            if (left_db_focus) db_left.down_arrow_pressed();
            else db_right.down_arrow_pressed();
        } else if (key_info.Key.Equals(ConsoleKey.UpArrow)) {
            if (left_db_focus) db_left.up_arrow_pressed();
            else db_right.up_arrow_pressed();
        } else if (key_info.Key.Equals(ConsoleKey.PageDown)) {
            if (left_db_focus) db_left.page_down_pressed();
            else db_right.page_down_pressed();
        } else if (key_info.Key.Equals(ConsoleKey.PageUp)) {
            if (left_db_focus) db_left.page_up_pressed();
            else db_right.page_up_pressed();
        } else if (key_info.Key.Equals(ConsoleKey.Backspace)) {
            if (left_db_focus) db_left.backspace_pressed();
            else db_right.backspace_pressed();
        } else if (key_info.Key.Equals(ConsoleKey.Home)) {
            if (left_db_focus) db_left.home_pressed();
            else db_right.home_pressed();
        } else if (key_info.Key.Equals(ConsoleKey.End)) {
            if (left_db_focus) db_left.end_pressed();
            else db_right.end_pressed();
        } else {
            Console.Beep();
        }
    }
} while (true);

bool is_alphanumeric(string str) {
    return alphanum_regex.IsMatch(str);
}