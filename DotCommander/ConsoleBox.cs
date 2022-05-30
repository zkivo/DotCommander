using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotCommander {
    internal abstract class ConsoleBox {

        private int[]  top_left;
        private int[]  bottom_right;
        private string title;

        ConsoleBox(int top_left_x, int top_left_y, int bottom_right_x, int bottom_right_y, string title) {
            top_left     = new int[2];
            bottom_right = new int[2];
            top_left[0]  = top_left_x;
            top_left[1]  = top_left_y;
            bottom_right[0] = bottom_right_x;
            bottom_right[1] = bottom_right_y;
            this.title = title;
        }

        public abstract void draw();
    }


}
