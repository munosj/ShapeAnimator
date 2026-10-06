using System.Windows.Shapes;

namespace ShapeAnimator.Model
{
    internal class GameTriangle : GameShape
    {
        public Polygon? UnderlyingTriangle;

        public GameTriangle(int x, int y, int size)
        {
            X = x;
            Y = y;
            Size = size;
        }
    }
}