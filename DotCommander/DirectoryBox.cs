using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotCommander {

    internal class DirectoryBox {

        private const float RESET_SEATCH_TIME = 0.8f; //seconds
        //private static int rows_of_a_page = Console.WindowHeight;
        //private static int cols_of_a_page = Console.WindowWidth;

        private (int x, int y) top_left;     // coordinate of the buffer
        private (int x, int y) bottom_right;
        private List<string> history_dirs;
        private List<string> list;
        private int index_list;
        //private int prev_index_list;
        private DateTime prev_time;
        private string   search_str;
        private string   blank_line;
        private int cols_of_the_box;
        private int rows_of_the_box;
        private bool focus;

        public DirectoryBox(int top_left_x, int top_left_y, int bottom_right_x, int bottom_right_y, string path, bool focus) {
            this.top_left.x = top_left_x;
            this.top_left.y = top_left_y;
            this.bottom_right.x = bottom_right_x;
            this.bottom_right.y = bottom_right_y;
            this.history_dirs = new List<string>();
            this.history_dirs.Add(path);
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

        public void enter_pressed() {
            if (File.Exists(list[index_list])) {
                Process.Start(new ProcessStartInfo(list[index_list]) { UseShellExecute = true });
            } else if (Directory.Exists(list[index_list])) {
                change_dir(list[index_list]);
            } else {
                Console.Write("ERROR: 6364");
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
                Console.SetCursorPosition(top_left.x, top_left.y + i + 1);
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
                } else if (Directory.Exists(element)) {
                    if (i == this.index_list && this.focus) {
                        // this record must be highlighted because the cursor is here
                        DirectoryBox.set_directory_color(true);
                    } else {
                        DirectoryBox.set_directory_color(false);
                    }
                    Console.Write("\\");
                } else {
                    Console.Write("ERROR: 3756");
                    return;
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
            
            Console.SetCursorPosition(top_left.x, top_left.y + i_from + 1);
            if (File.Exists(list[i_from])) {
                set_file_color(false);
            } else if (Directory.Exists(list[i_from])) {
                set_directory_color(false);
                Console.Write("\\");
            } else {
                Console.WriteLine("ERROR: 6254");
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

            Console.SetCursorPosition(top_left.x, top_left.y + i_to + 1);
            if (File.Exists(list[i_to])) {
                set_file_color(true);
            } else if (Directory.Exists(list[i_to])) {
                set_directory_color(true);
                Console.Write("\\");
            } else {
                Console.WriteLine("ERROR: 6255");
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
            history_dirs.Add(path);
            index_list = 0;
            clear_directory_box();
            list.Clear();
            foreach (string temp in Directory.GetDirectories(path)) {
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

        public void set_console_cursor() {
            if (index_list > 0) {
                Console.SetCursorPosition(top_left.x, top_left.y + index_list + 1);
            } else {
                Console.SetCursorPosition(top_left.x, top_left.y);
            }
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
