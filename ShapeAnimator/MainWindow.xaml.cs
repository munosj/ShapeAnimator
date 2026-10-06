using ShapeAnimator.Model;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ShapeAnimator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        int X1, Y1; // Mouse down coordinates
        int X2, Y2; // Mouse up coordinates

        private Brush CurrentColor = Brushes.Red;

        private string CurrentShape = "Circle";

        List<GameShape> Shapes = new List<GameShape>();

        List<Bullet> Bullets = new List<Bullet>();

        Gun Gun = new Gun(400, 350);

        Circle Center = new Circle(0,0,0); // temp marker to show where the center will be
        Circle Rim = new Circle(0,0,0); // temp marker to show the rim
        Line RadialLine = new Line(); // temp marker from center to rim

        DispatcherTimer Timer = new DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();
            Timer.Interval = TimeSpan.FromSeconds(0.1);
            Timer.Tick += Timer_Tick;

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Gun.X = (int)(ShapeCanvas.ActualWidth / 2);
            Gun.Y = (int)(ShapeCanvas.ActualHeight);
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            Step();
        }
        private void StepButton_Click(object sender, RoutedEventArgs e)
        {
            Step();

        }

        private void Step()
        {
            // Move all shapes and handle wall collisions
            foreach (GameShape s in Shapes)
            {
                //move first
                s.X += s.TravelDirectionX * 5;
                s.Y += s.TravelDirectionY * 5;

                // then check for wall collisions
                if (s.X >= ShapeCanvas.ActualWidth - s.Size)
                {
                    s.X = (int)(ShapeCanvas.ActualWidth - s.Size);
                    s.TravelDirectionX *= -1;
                }

                if (s.X <= s.Size)
                {
                    s.X = s.Size;
                    s.TravelDirectionX *= -1;
                }

                if (s.Y >= ShapeCanvas.ActualHeight - s.Size)
                {
                    s.Y = (int)(ShapeCanvas.ActualHeight - s.Size);
                    s.TravelDirectionY *= -1;
                }

                if (s.Y <= s.Size)
                {
                    s.Y = s.Size;
                    s.TravelDirectionY *= -1;
                }
            }

            // Check collisions between shapes
            for (int i = 0; i < Shapes.Count; i++)
            {
                for (int j = i + 1; j < Shapes.Count; j++)
                {
                    GameShape a = Shapes[i];
                    GameShape b = Shapes[j];

                    double distance =
                        Math.Sqrt(
                            (a.X - b.X) * (a.X - b.X) +
                            (a.Y - b.Y) * (a.Y - b.Y));

                    if (distance < a.Size + b.Size)
                    {
                        a.TravelDirectionX *= -1;
                        a.TravelDirectionY *= -1;

                        b.TravelDirectionX *= -1;
                        b.TravelDirectionY *= -1;

                        // Separate shapes
                        a.X += a.TravelDirectionX * 15;
                        a.Y += a.TravelDirectionY * 15;

                        b.X += b.TravelDirectionX * 15;
                        b.Y += b.TravelDirectionY * 15;
                    }
                }
            }
            // Move bullets and remove off-screen bullets
            for (int i = Bullets.Count - 1; i >= 0; i--)
            {
                Bullet b = Bullets[i];

                b.X += b.TravelDirectionX * 10;
                b.Y += b.TravelDirectionY * 10;

                bool hit = false;

                for (int j = Shapes.Count - 1; j >= 0; j--)
                {
                    GameShape s = Shapes[j];

                    double distance =
                        Math.Sqrt(
                            (b.X - s.X) * (b.X - s.X) +
                            (b.Y - s.Y) * (b.Y - s.Y));

                    if (distance < s.Size)
                    {
                        Shapes.RemoveAt(j);

                        hit = true;
                        break;
                    }
                }

                if (hit)
                {
                    Bullets.RemoveAt(i);
                    continue;
                }

                if (b.Y < 0)
                {
                    Bullets.RemoveAt(i);
                }
            }

            RefreshScreen();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                Gun.X -= 10;
            }

            if (e.Key == Key.Right)
            {
                Gun.X += 10;
            }

            if (e.Key == Key.Space)
            {
                FireBullet();
            }

            if (Gun.X < 20)
            {
                Gun.X = 20;
            }

            if (Gun.X > ShapeCanvas.ActualWidth - 20)
            {
                Gun.X = (int)ShapeCanvas.ActualWidth - 20;
            }

            RefreshScreen();
        }
        private void RedButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentColor = Brushes.Red;
        }

        private void BlueButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentColor = Brushes.Blue;
        }

        private void GreenButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentColor = Brushes.Green;
        }

        private void YellowButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentColor = Brushes.Yellow;
        }
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            Timer.Start();
        }
        private void PauseButton_Click(object sender, RoutedEventArgs e)
        { 
            Timer.Stop();
        }

        private void ShotButton_Click(object sender, RoutedEventArgs e)
        {
            FireBullet();

            RefreshScreen();
        }

        private void FireBullet()
        {
            Bullet b = new Bullet();

            b.X = Gun.X;
            b.Y = Gun.Y - 20;

            Bullets.Add(b);
        }

        private void RectangleButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentShape = "Rectangle";
        }

        private void TriangleButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentShape = "Triangle";
        }

        private void CircleButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentShape = "Circle";
        }

        private void RefreshScreen()
        {

            ShapeCanvas.Children.Clear();
            foreach (GameShape s in Shapes)
            {
                if (s is Circle)
                {
                    DrawCircle((Circle)s);
                }
                else if (s is GameRectangle)
                {
                    DrawRectangle((GameRectangle)s);
                }
                else if (s is GameTriangle)
                {
                    DrawTriangle((GameTriangle)s);
                }
            }
            foreach (Bullet b in Bullets)
            {
                DrawBullet(b);
            }

            DrawGun(Gun);
        }

        private void DrawCircle(GameShape s)
        {
            Circle c = (Circle)s;

            DrawCircle(c, (SolidColorBrush)c.ShapeColor);
        }

        private void DrawCircle(Circle c, SolidColorBrush color)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Stroke = color;
            ellipse.StrokeThickness = 1;


            Canvas.SetLeft(ellipse, c.X - c.Radius);
            Canvas.SetTop(ellipse, c.Y - c.Radius);

            ellipse.Width = c.Radius * 2;
            ellipse.Height = c.Radius * 2;


            // hang on to the underlyting ellipse
            // so we can remove from canvas later
            c.UnderlyingEllipse = ellipse; 

            // Put it on the canvas
            ShapeCanvas.Children.Add(ellipse);
        }

        private void DrawRectangle(GameRectangle r)
        {
            System.Windows.Shapes.Rectangle rect =
                new System.Windows.Shapes.Rectangle();

            rect.Stroke = r.ShapeColor;
            rect.StrokeThickness = 1;

            Canvas.SetLeft(rect, r.X - r.Size);
            Canvas.SetTop(rect, r.Y - r.Size);

            rect.Width = r.Size * 2;
            rect.Height = r.Size * 2;

            r.UnderlyingRectangle = rect;

            ShapeCanvas.Children.Add(rect);
        }

        private void DrawTriangle(GameTriangle t)
        {
            Polygon triangle = new Polygon();

            triangle.Stroke = t.ShapeColor;
            triangle.StrokeThickness = 1;

            triangle.Points = new PointCollection()
        {
            new Point(t.X, t.Y - t.Size),
            new Point(t.X - t.Size, t.Y + t.Size),
            new Point(t.X + t.Size, t.Y + t.Size)
        };

            t.UnderlyingTriangle = triangle;

            ShapeCanvas.Children.Add(triangle);
        }

        private void DrawBullet(Bullet b)
        {
            Ellipse bullet = new Ellipse();

            bullet.Fill = Brushes.White;

            bullet.Width = 2.5;
            bullet.Height = 10;

            Canvas.SetLeft(bullet, b.X - 2.5);
            Canvas.SetTop(bullet, b.Y);

            ShapeCanvas.Children.Add(bullet);
        }

        private void DrawGun(Gun g)
        {
            System.Windows.Shapes.Rectangle barrel =
                new System.Windows.Shapes.Rectangle();

            barrel.Fill = Brushes.Gray;

            barrel.Width = 40;
            barrel.Height = 15;

            Canvas.SetLeft(barrel, g.X - 20);
            Canvas.SetTop(barrel, g.Y - 7.5);

            ShapeCanvas.Children.Add(barrel);
        }

        private void RefreshRim()
        {
            // Update the position
            Canvas.SetLeft(Rim.UnderlyingEllipse, Rim.X - Rim.Radius);
            Canvas.SetTop(Rim.UnderlyingEllipse, Rim.Y - Rim.Radius);

            // Update the radius
            Rim.UnderlyingEllipse.Width = Rim.Radius * 2;
            Rim.UnderlyingEllipse.Height = Rim.Radius * 2;
        }

        private void ShapeCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Save the click location
            X1 = (int) e.GetPosition(ShapeCanvas).X;
            Y1 = (int)e.GetPosition(ShapeCanvas).Y;

            // Idea: Draw a temp dot to indicate the center of the circle
            Center.X = X1;
            Center.Y = Y1;
            Center.Radius = 3;
            DrawCircle(Center, Brushes.Aqua);

            // Draw the Rim
            DrawCircle(Rim, Brushes.Aqua);

            // Draw the Radial Line
            RadialLine.Stroke = Brushes.Aqua;
            RadialLine.X1 = X1;
            RadialLine.Y1 = Y1;
            RadialLine.X2 = X1;
            RadialLine.Y2 = Y1;
            ShapeCanvas.Children.Add(RadialLine);
        }

        /// 
        /// ASSIGNMENT 2
        /// 
        /// Can do now:
        /// 
        /// Allow the user to pick color from a color picker
        /// Allow the user to pick shapes (Circle, Rectangle, Triangle, Star)
        /// 
        /// 
        /// More classes needed:
        /// 
        /// Add a "Play" button that, when clicked, will make all the drawned shapes 
        /// move aroud on the screen, and bounce off the top, bottom, left, right border,
        /// and bounce off each other as well
        /// 
        /// Add a "Shoot" button that, when clicked, show a bullet flying up from the
        /// bottom. When the bullet hits a shape, make the shape explodes and disappears
        /// with sound and animation.
        /// 
        /// Allow the user to press keys on the keyboard to change the gun's shooting 
        /// direction
        /// 
        /// Use your creativity to add other features to make it an interesting game
        /// 
 



        private void ShapeCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            int cursor_x = (int)e.GetPosition(ShapeCanvas).X;
            int cursor_y = (int)e.GetPosition(ShapeCanvas).Y;

            // Update rim geometry
            Rim.X = X1;
            Rim.Y = Y1;
            Rim.Radius = (int)Math.Sqrt((cursor_x - X1) * (cursor_x - X1) + (cursor_y - Y1) * (cursor_y - Y1));
            RefreshRim();

            //Update the radial line geometry
            RadialLine.X1 = X1;
            RadialLine.Y1 = Y1;
            RadialLine.X2 = cursor_x;
            RadialLine.Y2 = cursor_y;
        }

        private void ShapeCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // Save the mouse up location
            X2 = (int)e.GetPosition(ShapeCanvas).X;
            Y2 = (int)e.GetPosition(ShapeCanvas).Y;

            int radius = (int) Math.Sqrt((X2 - X1)* (X2 - X1) + (Y2-Y1)* (Y2 - Y1));

            if (CurrentShape == "Circle")
            {
                Circle c = new Circle(X1, Y1, radius);

                c.ShapeColor = CurrentColor;

                Shapes.Add(c);
            }
            else if (CurrentShape == "Rectangle")
            {
                GameRectangle r = new GameRectangle(X1, Y1, radius);

                r.ShapeColor = CurrentColor;

                Shapes.Add(r);
            }
            else if (CurrentShape == "Triangle")
            {
                GameTriangle t = new GameTriangle(X1, Y1, radius);

                t.ShapeColor = CurrentColor;

                Shapes.Add(t);
            }

            RefreshScreen();
        }
    }
}