using Polygon1.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Polygon1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool IsDragging;
        private Point mouseOffset;
        private List<Vertex> vertices = new List<Vertex> ();
        private Vertex divider;
        private Vertex intersection1, intersection2;
        const double ellipseDiameter = 13.0;
        const double eps = 0.0001;

        public MainWindow()
        {
            InitializeComponent();
            vertices.Add(new Vertex(0, 100));
            vertices.Add(new Vertex(120, 130));
            vertices.Add(new Vertex(140, 90));
            vertices.Add(new Vertex(110, 31));
            vertices.Add(new Vertex(40, 10));

            divider = new Vertex(150, 0);
        }

        public override void EndInit()
        {
            base.EndInit();
            DrawVertices();
            DrawPolygon();
            CalcPolygonSquare();
            DrawDivider();
            DrawIntersectionPoints();
        }

        /// <summary>
        /// Рисуем все грани
        /// </summary>
        private void DrawPolygon()
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                int next_idx = (i == (vertices.Count-1) ? 0 : i+1);
                DrawLine(this.Canv, i, vertices.ElementAt(i), vertices.ElementAt(next_idx));
            }
        }

        /// <summary>
        /// Рисуем вертикальный разделитель
        /// </summary>
        private void DrawDivider()
        {
            if (divider != null)
            {
                var a = new Vertex(divider.x, 0);
                var b = new Vertex(divider.x, 1200);
                DrawLine(this.Canv, 1000, a, b, Brushes.Red);
            }
        }

        /// <summary>
        /// Рисуем точки пересечения разделителя и многоугольника
        /// </summary>
        private void DrawIntersectionPoints()
        {
            intersection1 = null;
            intersection2 = null;

            for (int i = 0; i < vertices.Count; i++)
            {
                int next_idx = (i == vertices.Count-1) ? 0 : (i+1);
                var a = vertices.ElementAt(i);
                var b = vertices.ElementAt(next_idx);

                if (a.x <= divider.x && b.x >= divider.x)
                {
                    intersection1 = FindIntersection(a, b, divider.x);
                }

                if (a.x >= divider.x && b.x <= divider.x)
                {
                    intersection2 = FindIntersection(b, a, divider.x);
                }
            }

            if (intersection1 != null)
            {
                DrawEllipse(this.Canv, 1000, intersection1.x, intersection1.y, Colors.Red);
            }

            if (intersection2 != null)
            {
                DrawEllipse(this.Canv, 1001, intersection2.x, intersection2.y, Colors.RosyBrown);
            }
        }

        /// <summary>
        /// Поиск пересечения грани AB с линией x = x0.
        /// Предположение: a.x < x0 < y.x
        /// </summary>
        private Vertex FindIntersection(Vertex a, Vertex b, int x0)
        {
            int y0;

            if (a.x > x0 || b.x < x0)
            {
                throw new ArgumentException($"x0 should be between a.x, b.x. ax={a.x}, x0={x0}, bx={b.x}");
            }

            //if (a.x == b.x)
            //{
            //    y0 = a.y;
            //}
            //else
            {
                y0 = (int)(1.0*a.y + 1.0*(b.y - a.y) / (b.x - a.x) * (x0 - a.x));
            }

            return new Vertex(x0, y0);
        }

        private void sHeader_Click(object sender, RoutedEventArgs e)
        {
            DrawPolygon();
        }

        /// <summary>
        /// Создание и отображение линии
        /// </summary>
        private void DrawLine(Canvas canv, int index, Vertex a, Vertex b, Brush lineBrush = null)
        {
            string lineName = "Line" + index;
            Line element = null;

            // Find line
            foreach (UIElement e in canv.Children)
            {
                if (e is Line line && line.Name == lineName)
                {
                    element = line;
                }
            }
            
            // Create line
            if (element == null)
            {
                element = new Line()
                {
                    Name = lineName,
                    Stroke = lineBrush ?? Brushes.Black,
                    StrokeThickness = 3,
                    IsHitTestVisible = false
                };

                canv.Children.Add(element);
            }

            element.X1 = a.x;
            element.Y1 = a.y;
            element.X2 = b.x;
            element.Y2 = b.y;
        }

        /// <summary>
        /// Создание и отображение эллипса
        /// </summary>
        private void DrawEllipse(Canvas canv, int index, int left, int top, Color? ellipseColor = null)
        {
            SolidColorBrush ellipseBrush = new SolidColorBrush(ellipseColor ?? Colors.Azure);
            string ellipseName = "Ellipse" + index;

            Ellipse element = null;

            // Find Ellipse
            foreach (UIElement e in canv.Children)
            {
                if (e is Ellipse ellipse && ellipse.Name == ellipseName)
                {
                    element = ellipse;
                    break;
                }
            }

            // Create Ellipse
            if (element == null)
            {
                element = new Ellipse()
                {
                    Width = ellipseDiameter,
                    Height = ellipseDiameter,
                    StrokeThickness = 2,
                    Fill = ellipseBrush,
                    Stroke = Brushes.Black,
                    Name = ellipseName
                };


                element.MouseLeftButtonDown += new MouseButtonEventHandler(this.ellipse_MouseLeftButtonDown);

                canv.Children.Add(element);
            }

            // Set coordinates
            Canvas.SetLeft(element, left - ellipseDiameter/2);
            Canvas.SetTop(element, top - ellipseDiameter/2);

        }

        private void ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsDragging = true;
            Ellipse ellipse = (Ellipse)sender;
            mouseOffset = e.GetPosition(ellipse);

            ellipse.MouseMove += ellipse_MouseMove;
            ellipse.MouseLeftButtonUp += Ellipse_MouseLeftButtonUp;

            ellipse.CaptureMouse();
        }

        private void Ellipse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsDragging)
            {
                Ellipse ellipse = (Ellipse)sender;

                ellipse.MouseMove -= ellipse_MouseMove;
                ellipse.MouseLeftButtonUp -= Ellipse_MouseLeftButtonUp;

                ellipse.ReleaseMouseCapture();
                IsDragging = false;
            }
        }

        private void ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsDragging)
            {
                Ellipse ellipse = (Ellipse)sender;
                Point p = e.GetPosition(this.Canv);
                double elp_x = p.X - mouseOffset.X;
                double elp_y = p.Y - mouseOffset.Y;

                string ellipseName = ellipse.Name;
                int index = int.Parse(ellipseName.Substring("Ellipse".Length));
                
                if (index >= 1000)
                    return;

                ellipse.SetValue(Canvas.LeftProperty, elp_x);
                ellipse.SetValue(Canvas.TopProperty, elp_y);

                vertices.ElementAt(index).x = (int)(elp_x + ellipseDiameter / 2);
                vertices.ElementAt(index).y = (int)(elp_y + ellipseDiameter / 2);

                DrawPolygon();
                //sHeader.Content = string.Format($"{ellipse.Name}: {elp_x + ellipseDiameter/2}, {elp_y + ellipseDiameter/2}");

                CalcPolygonSquare();
                DrawDivider();
                DrawIntersectionPoints();
            }
        }

        /// <summary>
        /// Рисуем все вершины
        /// </summary>
        private void DrawVertices()
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                DrawEllipse(this.Canv, i, vertices.ElementAt(i).x, vertices.ElementAt(i).y);
            }
        }

        /// <summary>
        /// Вычисление площади полигона
        /// </summary>
        /// <returns></returns>
        private double CalcPolygonSquare()
        {
            double totalSquare = 0.0;
            StringBuilder sLog = new StringBuilder();

            for (int i = 0; i < vertices.Count; i++)
            {
                var v = vertices.ElementAt(i);

                int next_idx = (i == vertices.Count - 1) ? 0 : i+1;
                var next_v = vertices.ElementAt(next_idx);
                double currentSquare = TrapezoidSquare(v, next_v);
                totalSquare += currentSquare;

                sLog.Append($"{i}: {currentSquare}; ");
            }
            
            sLog.Append($" => total: {totalSquare}");
            sHeader.Content = sLog.ToString();

            return totalSquare;
        }

        /// <summary>
        /// Площадь трапеции по вершинам "верхней" стороны
        /// </summary>
        /// <param name="v"></param>
        /// <param name="next_v"></param>
        /// <returns></returns>
        private double TrapezoidSquare(Vertex v, Vertex next_v)
        {
            var s = 0.5*(v.y + next_v.y)*(next_v.x - v.x);
            return s;
        }
    }
}
