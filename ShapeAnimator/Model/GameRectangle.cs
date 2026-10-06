using System.Windows.Shapes;

namespace ShapeAnimator.Model
{
    internal class GameRectangle : GameShape
    {
        public System.Windows.Shapes.Rectangle? UnderlyingRectangle;

        public GameRectangle(int x, int y, int size)
        {
            X = x;
            Y = y;
            Size = size;
        }
    }
}