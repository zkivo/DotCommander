using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace DotCommander {

    public class DirectoryBox {

        private const float RESET_SEATCH_TIME = 0.8f; //seconds

        private (int x, int y) top_left;     // coordinate of the buffer
        private (int x, int y) bottom_right;
        public  List<string> history_dirs;
        private List<string> list;
        private int index_list;
        private DateTime prev_time;
        private string   search_str;
        private string   blank_line;
        private string   id;
        private int cols_of_the_box;
        private int rows_of_the_box;
        private bool focus;

        public DirectoryBox() { }

        public DirectoryBox(int top_left_x, int top_left_y, int bottom_right_x, int bottom_right_y, bool focus, string id) {
            string path;
            this.id = id;
            read_config_file();
            if (this.history_dirs == null) {
                this.history_dirs = new List<string>();
                this.history_dirs.Add(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            }
            path = history_dirs[history_dirs.Count - 1];
            this.top_left.x = top_left_x;
            this.top_left.y = top_left_y;
            this.bottom_right.x = bottom_right_x;
            this.bottom_right.y = bottom_right_y;
            this.list = new List<string>();
            foreach (string temp in Directory.GetDirectories(path)) {
                this.list.Add(temp);
            }
            foreach (string temp in Directory.GetFiles(path)) {
                this.list.Add(temp);
            }
            this.index_list = 0;
            this.prev_time  = DateTime.Now;
            this.search_str = "";
            this.blank_line = "";
            this.rows_of_the_box = bottom_right_y - top_left_y;
            this.cols_of_the_box = bottom_right_x - top_left_x;
            for (int i = 0; i < cols_of_the_box; i++) {
                this.blank_line += " ";
            }
            this.focus = focus;
        }

        public void refresh_list() {
            this.list.Clear();
            foreach (string temp in Directory.GetDirectories(history_dirs.Last<string>())) {
                this.list.Add(temp);
            }
            foreach (string temp in Directory.GetFiles(history_dirs.Last<string>())) {
                this.list.Add(temp);
            }
        }

        public string get_path_of_indexed_file() {
            return list[index_list];    
        }

        public string get_path_open_directory() {
            return history_dirs.Last<string>();
        }

        public void decrese_index_list() {
            index_list--;
            if (index_list < 0) { index_list = 0; }
        }

        public void read_config_file() {
            try {
                XmlSerializer serializer = new XmlSerializer(typeof(DirectoryBox));
                StreamReader file = new StreamReader(Environment.CurrentDirectory + "\\" + id + "DirectoryBox.config");
                DirectoryBox overview = (DirectoryBox) serializer.Deserialize(file);
                this.history_dirs = new List<string>(overview.history_dirs);
                file.Close();
            } catch (FileNotFoundException e) {

            }
        }

        public void reset_config_file() {
            XmlSerializer serializer = new XmlSerializer(typeof(DirectoryBox));
            
            string path = Environment.CurrentDirectory + "\\" + id + "DirectoryBox.config";
            FileStream file = File.Create(path);

            serializer.Serialize(file, this);
            file.Close();
        }

        public void enter_pressed() {
            if (File.Exists(list[index_list])) {
                Process.Start(new ProcessStartInfo(list[index_list]) {  UseShellExecute = true } );
            } else if (Directory.Exists(list[index_list])) {
                change_dir(list[index_list]);
            } else {
                Console.Write("ERROR: 6364");
            }
        }

        public void delete_pressed() {
            try {
                File.Delete(list[index_list]);
            } catch (Exception e) {
                Console.Beep();
            }
        }

        public void down_arrow_pressed() {
            if (index_list < list.Count - 1) {
                switch_highlight(index_list, index_list + 1);
                index_list++;
            }
        }

        public void up_arrow_pressed() {
            if (index_list > 0) {
                switch_highlight(index_list, index_list - 1);
                index_list--;
            }
        }

        public void page_down_pressed() {
            int prev_index_list = index_list;
            if (index_list + rows_of_the_box >= list.Count) index_list = list.Count - 1;
            else index_list += rows_of_the_box;
            switch_highlight(prev_index_list, index_list);
        }

        public void page_up_pressed() {
            int prev_index_list = index_list;
            if (index_list - rows_of_the_box < 0) index_list = 0;
            else index_list -= rows_of_the_box;
            switch_highlight(prev_index_list, index_list);
        }

        public void backspace_pressed() {
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
                }
            } catch (Exception e) {
                Console.Beep();
            }
        }

        public void home_pressed() {
            switch_highlight(index_list, 0);
            index_list = 0;
        }

        public void end_pressed() {
            switch_highlight(index_list, list.Count - 1);
            index_list = list.Count - 1;
        }

        public void focus_link() {
            ConsoleKeyInfo key_info;
            ConsoleModifiers mod;
            clear_directory_box();
            Console.CursorVisible = true;
            string path = history_dirs.Last<string>();
            do {
                clear_directory_box();
                Console.SetCursorPosition(top_left.x, top_left.y);
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("   " + path);
                list_dirs(path);
                try {
                    /* when writing a string on the console
                     * can happen that the string is bigger of the box,
                     * if this happens we cut it otherwise we write all the string */
                    Console.SetCursorPosition(top_left.x + 3 + path.Substring(0, cols_of_the_box - 4).Length,
                                              top_left.y);
                } catch (System.ArgumentOutOfRangeException e) {
                    Console.SetCursorPosition(top_left.x + 3 + path.Length, top_left.y);
                }
                key_info = Console.ReadKey(true);
                mod = key_info.Modifiers; //alt, shift, ctrl modifiers information alt=1,shift=2,ctrl=4
                if (mod > 0) {
                    // some modifiers have been pressed
                    if (mod == ConsoleModifiers.Control) {
                        if (key_info.Key.Equals(ConsoleKey.Backspace)) {
                            List<string> lista = path.Split("\\").ToList<string>();
                            try {
                                if (lista.Last<string>().Equals("")) {
                                    lista.RemoveAt(lista.Count - 1);
                                }
                                lista.RemoveAt(lista.Count - 1);
                                path = "";
                                foreach (string s in lista) {
                                    path += s + "\\";
                                }
                            } catch (ArgumentOutOfRangeException e) {
                                Console.Beep();
                            }
                        }
                    } else if (mod == ConsoleModifiers.Shift) {
                        if (key_info.KeyChar == ':') {
                            path += ":";
                        } else if (is_alphanumeric(key_info.KeyChar.ToString())) {
                            path += key_info.KeyChar;
                        }
                    }
                } else {
                    if (key_info.Key.Equals(ConsoleKey.Enter)) {
                        if (Directory.Exists(path)) {
                            change_dir(path);
                            break;
                        } else {
                            Console.Beep();
                        }
                    } else if (key_info.Key.Equals(ConsoleKey.Tab)) {
                        path = complete_path(path);
                    } else if (is_alphanumeric(key_info.KeyChar.ToString())) {
                        path += key_info.KeyChar;
                    } else if (key_info.Key.Equals(ConsoleKey.Backspace)) {
                        try {
                            path = path.Substring(0, path.Length - 1);
                        } catch (ArgumentOutOfRangeException e) {
                            Console.Beep();
                        }
                    } else if (key_info.Key.Equals(ConsoleKey.Escape)) {
                        break;
                    } else if (key_info.KeyChar == '\\') {
                        path += "\\";
                    } else {
                        Console.Beep();
                    }
                }
            } while (true);
            clear_directory_box();
            draw();
            reset_console_cursor();
        }

        public static string complete_path(string path) {
            string last = "";
            bool found = false;
            if (!Directory.Exists(path)) {
                List<string> lista = path.Split("\\").ToList<string>();
                try {
                    last = lista.Last<string>();
                    lista.RemoveAt(lista.Count - 1);
                    path = "";
                    foreach (string s in lista) {
                        path += s + "\\";
                    }
                } catch (ArgumentOutOfRangeException e) {
                    //dunno
                }
                try {
                    foreach (string element in Directory.GetDirectories(path)) {
                        if (0 == element.Split("\\").Last<string>().IndexOf(last, StringComparison.OrdinalIgnoreCase)) {
                            path += element.Split("\\").Last<string>() + "\\";
                            found = true;
                            break;
                        }
                    }
                } catch (ArgumentException e) {
                    foreach (string element in Directory.GetLogicalDrives()) {
                        if (0 == element.IndexOf(last, StringComparison.OrdinalIgnoreCase)) {
                            path = element;
                            found = true;
                            break;
                        }
                    }
                } catch (UnauthorizedAccessException e) {
                    Console.Beep();
                }
            } else {
                if (path.Last<char>() != '\\') {
                    path += '\\';
                }
            }
            if (!found) {
                path += last;
            }
            return path;
        } 

        private void list_dirs(string path) {
            set_directory_color(false);
            if (!Directory.Exists(path)) {
                List<string> lista = path.Split("\\").ToList<string>();
                try {
                    lista.RemoveAt(lista.Count - 1);
                    path = "";
                    foreach (string s in lista) {
                        path += s + "\\";
                    }
                } catch (ArgumentOutOfRangeException e) {
                    //dunno
                }
            }
            int i = 1;
            try {
                foreach (string element in Directory.GetDirectories(path)) {
                    Console.SetCursorPosition(top_left.x, top_left.y + i++);
                    Console.Write("\\" + element.Split("\\").Last<string>());
                }
            } catch (ArgumentException e) {
                i = 1;
                foreach (string element in Directory.GetLogicalDrives()) {
                    Console.SetCursorPosition(top_left.x, top_left.y + i++);
                    Console.Write(element);
                }
            } catch (UnauthorizedAccessException e) {
                Console.Beep();
            }
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

        public void typed_alphanumeric(char character) {
            TimeSpan diff = DateTime.Now - prev_time;
            if (diff.TotalSeconds < RESET_SEATCH_TIME) {
                search_str += character.ToString();
            } else {
                // repeat the search
                search_str = character.ToString();
            }
            search_string(search_str);
            prev_time = DateTime.Now;
        }

        public void clear_directory_box() {
            Console.ResetColor();
            int i = 0;
            foreach (string element in this.list) {
                Console.SetCursorPosition(top_left.x, top_left.y + i);
                Console.Write(blank_line);
                i++;
            }
            Console.SetCursorPosition(top_left.x, top_left.y + i);
            Console.Write(blank_line);
        }

        public void draw() {
            int i = 0;
            (int pos_x, int pos_y) = Console.GetCursorPosition();
            /* drawing */
            Console.SetCursorPosition(top_left.x, top_left.y);
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            try {
                /* when writing a string on the console
                 * can happen that the string is bigger of the box,
                 * if this happens we cut it otherwise we write all the string */
                Console.Write("   " + history_dirs.Last<string>().Substring(0, cols_of_the_box - 4));
            } catch (System.ArgumentOutOfRangeException e) {
                Console.Write("   " + history_dirs.Last<string>());
            }
            Console.ResetColor();

            foreach (string element in this.list) {
                if (i == list.Count) {
                    break;
                }
                if (File.Exists(element)) {
                    if (i == this.index_list && this.focus) {
                        // this record must be highlighted because the cursor is here
                        DirectoryBox.set_file_color(true);
                    } else {
                        DirectoryBox.set_file_color(false);
                    }
                    Console.SetCursorPosition(bottom_right.x - 12, top_left.y + i + 1);
                    Console.Write(File.GetLastWriteTime(element).ToString("d"));
                } else if (Directory.Exists(element)) {
                    if (i == this.index_list && this.focus) {
                        // this record must be highlighted because the cursor is here
                        DirectoryBox.set_directory_color(true);
                    } else {
                        DirectoryBox.set_directory_color(false);
                    }
                    Console.SetCursorPosition(bottom_right.x - 12, top_left.y + i + 1);
                    Console.Write(Directory.GetLastWriteTime(element).ToString("d"));
                } else {
                    Console.Write("ERROR: 3756");
                    return;
                }
                Console.SetCursorPosition(top_left.x, top_left.y + i + 1);
                if (Directory.Exists(element)) {
                    Console.Write("\\");
                }
                try {
                    /* when writing a string on the console
                     * can happen that the string is bigger of the box,
                     * if this happens we cut it otherwise we write all the string */
                    Console.Write(list[i].Split("\\").Last<string>().Substring(0, cols_of_the_box - 1));
                } catch (System.ArgumentOutOfRangeException e) {
                    Console.Write(list[i].Split("\\").Last<string>());
                }
                i++;
            }
            if (this.focus) {
                Console.SetCursorPosition(top_left.x, top_left.y + index_list);
            }
            Console.CursorVisible = false;
        }

        public void switch_highlight(int i_from, int i_to) {
            /*
             * looks where index_from is pointing to and set the color
             * based on wheter it is a file or a directory
             * this part does not highlight
             */
            
            // ------ FROM -------
           
            if (File.Exists(list[i_from])) {
                set_file_color(false);
                Console.SetCursorPosition(bottom_right.x - 12, top_left.y + i_from + 1);
                Console.Write(File.GetLastWriteTime(list[i_from]).ToString("d"));
            } else if (Directory.Exists(list[i_from])) {
                set_directory_color(false);
                Console.SetCursorPosition(bottom_right.x - 12, top_left.y + i_from + 1);
                Console.Write(Directory.GetLastWriteTime(list[i_from]).ToString("d"));
            } else {
                Console.WriteLine("ERROR: 6254");
            }
            Console.SetCursorPosition(top_left.x, top_left.y + i_from + 1);
            if (Directory.Exists(list[i_from])) {
                Console.Write("\\");
            }
            try {
                /* when writing a string on the console
                 * can happen that the string is bigger of the box,
                 * if this happens we cut it otherwise we write all the string */
                Console.Write(list[i_from].Split("\\").Last<string>().Substring(0, cols_of_the_box - 1));
            } catch (System.ArgumentOutOfRangeException e) {
                Console.Write(list[i_from].Split("\\").Last<string>());
            }

            // ------ TO -------

            if (File.Exists(list[i_to])) {
                set_file_color(true);
                Console.SetCursorPosition(bottom_right.x - 12, top_left.y + i_to + 1);
                Console.Write(File.GetLastWriteTime(list[i_to]).ToString("d"));
            } else if (Directory.Exists(list[i_to])) {
                set_directory_color(true);
                Console.SetCursorPosition(bottom_right.x - 12, top_left.y + i_to + 1);
                Console.Write(Directory.GetLastWriteTime(list[i_to]).ToString("d"));
            } else {
                Console.WriteLine("ERROR: 6254");
            }
            Console.SetCursorPosition(top_left.x, top_left.y + i_to + 1);
            if (Directory.Exists(list[i_to])) {
                Console.Write("\\");
            }
            try {
                /* when writing a string on the console
                 * can happen that the string is bigger of the box,
                 * if this happens we cut it otherwise we write all the string */
                Console.Write(list[i_to].Split("\\").Last<string>().Substring(0, cols_of_the_box - 1));
            } catch (System.ArgumentOutOfRangeException e) {
                Console.Write(list[i_to].Split("\\").Last<string>());
            }

            Console.CursorVisible = false;
            if (i_to == 0) {
                Console.SetCursorPosition(top_left.x, top_left.y);
            }
            //this.index_list = i_to;
        }

        public void change_dir(string path) {
            string[] dirs; 
            try {
                dirs = Directory.GetDirectories(path);
            } catch (UnauthorizedAccessException e) {
                Console.Beep();
                return;
            }
            history_dirs.Add(path);
            index_list = 0;
            clear_directory_box();
            list.Clear();
            foreach (string temp in dirs) {
                this.list.Add(temp);
            }
            foreach (string temp in Directory.GetFiles(path)) {
                this.list.Add(temp);
            }
            draw();
        }

        public void switch_focus() {
            this.focus = !this.focus;
            clear_directory_box();
            draw();
        }

        public void reset_console_cursor() {
            if (index_list > 0) {
                Console.SetCursorPosition(top_left.x, top_left.y + index_list + 1);
            } else {
                Console.SetCursorPosition(top_left.x, top_left.y);
            }
            Console.CursorVisible = false;
        }

        public static bool is_alphanumeric(string str) {
            Regex alphanum_regex = new Regex(@"^[a-zA-Z0-9\s,]*$");
            return alphanum_regex.IsMatch(str);
        }

        private static void set_directory_color(bool highlight) {
            if (highlight) {
                // this record must be highlighted because the index is here
                Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.ForegroundColor = ConsoleColor.Yellow;
            } else {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
            }
        }

        private static void set_file_color(bool highlight) {
            if (highlight) {
                // this record must be highlighted because the cursor is here
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.ForegroundColor = ConsoleColor.White;
            } else {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.DarkGreen;
            }
        }

    }

}
