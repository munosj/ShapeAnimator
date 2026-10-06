using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Shapes;

namespace ShapeAnimator.Model
{
    internal class Circle: GameShape
    {       
        

        public Ellipse? UnderlyingEllipse;

        public int Radius;

        public Circle(int x, int y, int radius)
        {
            this.X = x;
            this.Y = y;

            Radius = radius;
            Size = radius;
        }
    }
}
