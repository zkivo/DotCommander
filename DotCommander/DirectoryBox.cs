﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotCommander {

    internal class DirectoryBox {

        private const float RESET_SEATCH_TIME = 0.8f; //seconds
        private static int rows_of_a_page = Console.WindowHeight;
        private static int cols_of_a_page = Console.WindowWidth;

        private (int x, int y) top_left;     // coordinate of the buffer
        private (int x, int y) bottom_right;
        private List<string> history_dirs;
        private List<string> list;
        private int index_list;
        //private int prev_index_list;
        private DateTime prev_time;
        private string   search_str;
        private string   blank_line;

        public DirectoryBox(int top_left_x, int top_left_y, int bottom_right_x, int bottom_right_y, string path) {
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
            for (int i = 0; i < bottom_right_x - top_left_x; i++) {
                this.blank_line += " ";
            }
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
            if (index_list + rows_of_a_page >= list.Count) index_list = list.Count - 1;
            else index_list += rows_of_a_page;
            switch_highlight(prev_index_list, index_list);
        }

        public void page_up_pressed() {
            int prev_index_list = index_list;
            if (index_list - rows_of_a_page < 0) index_list = 0;
            else index_list -= rows_of_a_page;
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
            Console.SetCursorPosition(top_left.x, top_left.y);
            Console.ResetColor();
            Console.WriteLine(blank_line);
            foreach (string element in this.list) {
                Console.WriteLine(blank_line);
            }
        }

        public void draw() {

            string[] split;
            int i = 0;

            /* drawing */
            Console.SetCursorPosition(top_left.x, top_left.y);
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("   " + history_dirs.Last<string>() + "   ");
            Console.ResetColor();

            foreach (string element in this.list) {
                Console.SetCursorPosition(top_left.x, top_left.y + i + 1);
                split = element.Split('\\');
                if (i == list.Count) {
                    break;
                }
                if (File.Exists(element)) {
                    if (i == this.index_list) {
                        // this record must be highlighted because the cursor is here
                        DirectoryBox.set_file_color(true);
                    } else {
                        DirectoryBox.set_file_color(false);
                    }
                } else if (Directory.Exists(element)) {
                    if (i == this.index_list) {
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
                Console.Write(split[split.Length - 1]);
                i++;
            }
            Console.SetCursorPosition(top_left.x, top_left.y);
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
            Console.Write(list[i_from].Split("\\").Last<string>());
            
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
            Console.Write(list[i_to].Split("\\").Last<string>());
         
            Console.CursorVisible = false;
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
