using Polygon1.Model;
using System;
using System.Collections.Generic;
using System.Linq;
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
        const double ellipseDiameter = 13.0*3;

        public MainWindow()
        {
            InitializeComponent();
            vertices.Add(new Vertex(0, 0));
            vertices.Add(new Vertex(165, 5));
            vertices.Add(new Vertex(189, 245));
            vertices.Add(new Vertex(5, 165));
        }

        public override void BeginInit()
        {
            base.BeginInit();
            DrawPicture();
        }

        private void DrawPicture()
        {
            // Вершины
            for (int i = 0; i < vertices.Count; i++)
            {
                DrawEllipse(this.Canv, i, vertices.ElementAt(i).x, vertices.ElementAt(i).y);
            }

            // Грани
            for (int i = 1; i < vertices.Count; i++)
            {
                DrawLine(this.Canv, i, vertices.ElementAt(i-1), vertices.ElementAt(i));
            }
        }

        private void sHeader_Click(object sender, RoutedEventArgs e)
        {
            DrawPicture();
        }

        private void DrawLine(Canvas canv, int index, Vertex a, Vertex b)
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
                    Stroke = Brushes.Black,
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

        private void DrawEllipse(Canvas canv, int index, int left, int top)
        {
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
                    Fill = new SolidColorBrush(Colors.Azure),
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

                ellipse.SetValue(Canvas.LeftProperty, elp_x);
                ellipse.SetValue(Canvas.TopProperty, elp_y);

                DrawPolygon();
                sHeader.Content = string.Format($"{ellipse.Name}: {elp_x + ellipseDiameter/2}, {elp_y + ellipseDiameter/2}");
            }
        }

        private void DrawPolygon()
        {
            
        }
    }
}
