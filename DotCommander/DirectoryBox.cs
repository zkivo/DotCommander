using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotCommander {

    internal class DirectoryBox {

        private (int x, int y) top_left;     // coordinate of the buffer
        private (int x, int y) bottom_right;
        private List<string> history_dirs;
        private List<string> list;
        private int index_list;
        private int prev_index_list;

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
        }

        public void draw() {

            string[] split;
            int i = 0;

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
            Console.SetCursorPosition(0, 0);
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
            this.index_list = i_to;
}

        public void change_dir(string path) {
            history_dirs.Add(path);
            index_list = 0;
            prev_index_list = 0;
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
