// See https://aka.ms/new-console-template for more information
//Console.WriteLine(Console.BackgroundColor);
//Console.WriteLine(Console.WindowHeight);
//Console.WriteLine(Console.WindowWidth);

//string opened_directory = "G:\\Il mio Drive\\Università";
ConsoleKeyInfo   key_info;
ConsoleModifiers mod;

VisualConsole console = new VisualConsole();
console.opened_directory = "G:\\Il mio Drive\\Università";
console.index_list = 0;

int prev_w_height = Console.WindowHeight;
int prev_w_width = Console.WindowWidth;

Thread t_resizing_listener = new Thread(f_resizing_listener);
Console.BufferHeight = Console.WindowHeight;
Console.BufferWidth  = Console.WindowWidth;

t_resizing_listener.Start();

do {
    console.draw();
    key_info = Console.ReadKey(true);
    mod = key_info.Modifiers; // alt, shift, ctrl modifiers information
    if (mod > 0) {
        // If one or more modifiers have been pressed.

    } else {
        // None modifiers have been pressed
        if (key_info.Key.Equals(ConsoleKey.DownArrow)) {
            console.index_list++;
        } else if (key_info.Key.Equals(ConsoleKey.UpArrow)) {
            console.index_list--;
        } else {
            Console.WriteLine("dunno");
        }
    }
} while (true);

void f_resizing_listener() {
    while (true) {
        if (prev_w_height != Console.WindowHeight ||
                prev_w_width != Console.WindowWidth) {
            Console.BufferHeight = Console.WindowHeight;
            Console.BufferWidth = Console.WindowWidth;
            console.draw();
        }
        Thread.Sleep(1000);
    }
}

class VisualConsole {

    public string opened_directory { get; set; }

    public int index_list { get; set; }

    public void draw() {
        lock (this) {
            Console.Clear();
            Console.CursorTop = 0;
            Console.CursorLeft = 0;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Blue;
            string[] files = Directory.GetFiles(opened_directory);
            string[] dirs = Directory.GetDirectories(opened_directory);
            string[] split;
            Console.WriteLine("   " + opened_directory + "   ");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            //int index = files.Length % index_list;
            int i = 0;
            foreach (string file in files) {
                if (i == Console.BufferHeight) return;
                else if (i == index_list) {
                    // this record must be highlighted because the cursor is here
                    Console.ForegroundColor = ConsoleColor.Yellow;
                } else Console.ForegroundColor = ConsoleColor.White;
                split = file.Split('\\');
                Console.WriteLine("" + split[split.Length - 1]);
                i++;
            }
        }
    }
}