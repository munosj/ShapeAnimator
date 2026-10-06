using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace ShapeAnimator.Model
{
    internal abstract class GameShape
    {
        private static Random random = new Random();
        public int X;
        public int Y;

        public int Size;
        public Brush ShapeColor = Brushes.Red;
        public int TravelDirectionX;
        public int TravelDirectionY;

        public GameShape()
        {
            TravelDirectionX = random.Next(2) == 0 ? -1 : 1;
            TravelDirectionY = random.Next(2) == 0 ? -1 : 1;
        }



    }
}

