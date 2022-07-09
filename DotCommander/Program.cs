using System.Text.RegularExpressions;

ConsoleKeyInfo   key_info;
ConsoleModifiers mod;

Regex alphanum_regex = new Regex(@"^[a-zA-Z0-9\s,]*$");
Console.TreatControlCAsInput = true; // also treats the other modifiers as input

bool   left_db_focus  = true; // specifies if the focus is on the left box
string copy_file_path = "";

DotCommander.DirectoryBox db_left  = new DotCommander.DirectoryBox(0,  0,  60, 30,  left_db_focus, "left_");
DotCommander.DirectoryBox db_right = new DotCommander.DirectoryBox(60, 0, 120, 30, !left_db_focus, "right_");

db_left.draw();
db_right.draw();
db_left.reset_console_cursor();

do {
    key_info = Console.ReadKey(true);
    mod = key_info.Modifiers; // alt, shift, ctrl modifiers information alt=1,shift=2,ctrl=4
    if (mod > 0) {
        // some modifiers have been pressed
        if (mod == ConsoleModifiers.Control) {
            //just ctrl has been pressed
            if (key_info.Key.Equals(ConsoleKey.C)) {
                if (left_db_focus) {
                    copy_file_path = db_left.get_path_of_indexed_file();
                } else {
                    copy_file_path = db_right.get_path_of_indexed_file();
                }
            } else if (key_info.Key.Equals(ConsoleKey.V)) {
                string filename;
                if (copy_file_path != "") {
                    try {
                        if (left_db_focus) {
                            filename = copy_file_path.Split("\\").Last<string>();
                            File.Copy(copy_file_path, db_left.get_path_open_directory() + "\\" + filename);
                        } else {
                            filename = copy_file_path.Split("\\").Last<string>();
                            File.Copy(copy_file_path, db_right.get_path_open_directory() + "\\" + filename);
                        }
                        db_left.clear_directory_box();
                        db_right.clear_directory_box();
                        db_left.refresh_list();
                        db_right.refresh_list();
                        db_left.draw();
                        db_right.draw();
                        if (left_db_focus) {
                            db_left.reset_console_cursor();
                        } else {
                            db_right.reset_console_cursor();
                        }
                    } catch (Exception ex) {
                        Console.Beep();
                    }
                }
            } else if (key_info.Key.Equals(ConsoleKey.L)) {
                if (left_db_focus) db_left.focus_link();
                else db_right.focus_link();
            } else if (key_info.Key.Equals(ConsoleKey.Tab)) {
                left_db_focus = !left_db_focus;
                db_left.switch_focus();
                db_right.switch_focus();
                if (left_db_focus) {
                    db_left.reset_console_cursor();
                } else {
                    db_right.reset_console_cursor();
                }
            } else if (key_info.Key.Equals(ConsoleKey.S)) {
                db_left.reset_config_file();
                db_right.reset_config_file();
            } else if (key_info.Key.Equals(ConsoleKey.M)) {
                string temp;
                string filename;
                try {
                    if (left_db_focus) {
                        temp = db_left.get_path_of_indexed_file();
                        filename = temp.Split("\\").Last<string>();
                        File.Move(temp, db_right.get_path_open_directory() + "\\" + filename);
                        db_left.decrese_index_list();
                    } else {
                        temp = db_right.get_path_of_indexed_file();
                        filename = temp.Split("\\").Last<string>();
                        File.Move(temp, db_left.get_path_open_directory() + "\\" + filename);
                        db_right.decrese_index_list();
                    }
                    db_left.clear_directory_box();
                    db_right.clear_directory_box();
                    db_left.refresh_list();
                    db_right.refresh_list();
                    db_left.draw();
                    db_right.draw();
                    if (left_db_focus) {
                        db_left.reset_console_cursor();
                    } else {
                        db_right.reset_console_cursor();
                    }

                } catch (Exception ex) {
                    Console.Beep();
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
            if (left_db_focus) db_left.enter_pressed();
            else db_right.enter_pressed();
        } else if (is_alphanumeric(key_info.KeyChar.ToString())) {
            if (left_db_focus) db_left.typed_alphanumeric(key_info.KeyChar);
            else db_right.typed_alphanumeric(key_info.KeyChar);
        } else if (key_info.Key.Equals(ConsoleKey.Delete)) {
            if (left_db_focus) {
                db_left.delete_pressed();
                db_left.clear_directory_box();
                db_left.refresh_list();
                db_left.draw();
                db_left.decrese_index_list();
            } else {
                db_right.delete_pressed();
                db_right.clear_directory_box();
                db_right.refresh_list();
                db_right.draw();
                db_right.decrese_index_list();
            }
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
        } else if (key_info.Key.Equals(ConsoleKey.Escape)) {
            Environment.Exit(0);
        } else {
            Console.Beep();
        }
    }
} while (true);

bool is_alphanumeric(string str) {
    return alphanum_regex.IsMatch(str);
}