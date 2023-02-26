using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Treug1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool IsDragging = false;
        private Point mouseOffset;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsDragging = true;
            Ellipse ellipse = (Ellipse)sender;
            mouseOffset = e.GetPosition(ellipse);

            ellipse.MouseMove += ellipse_MouseMove;
            ellipse.MouseLeftButtonUp += ellipse_MouseUp;

            ellipse.CaptureMouse();

        }

        private void ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsDragging)
            {
                Ellipse ellipse = (Ellipse)sender;
                Point p = e.GetPosition(Canv);

                ellipse.SetValue(Canvas.TopProperty, p.Y - mouseOffset.Y);
                ellipse.SetValue(Canvas.LeftProperty, p.X - mouseOffset.X);

                DrawTreug();
            }
        }

        private void ellipse_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (IsDragging)
            {
                Ellipse ellipse = (Ellipse)sender;

                ellipse.MouseMove -= ellipse_MouseMove;
                ellipse.MouseLeftButtonUp -= ellipse_MouseUp;

                ellipse.ReleaseMouseCapture();
                IsDragging = false;
            }

            //DrawTreug();
        }

        private void DrawTreug()
        {
            Line l1 = FindCreateLine(Canv, "LineAB");
            Line l2 = FindCreateLine(Canv, "LineAC");
            Line l3 = FindCreateLine(Canv, "LineBC");

            LinkE(EA, EB, l1);
            LinkE(EA, EC, l2);
            LinkE(EB, EC, l3);

            Ellipse e1 = FindCreateEllipse(Canv, "E1", Brushes.Red, 8.0);
            Ellipse e2 = FindCreateEllipse(Canv, "E2", Brushes.Green, 8.0);
            Ellipse e3 = FindCreateEllipse(Canv, "E3", Brushes.Blue, 8.0);
            
            /* ортоцентр треугольников */
            double d1, d2, d3;
            Point p1 = new Point(), p2 = new Point(), p3 = new Point();

            d1 = SetO(ref p1, EA, EB, EC);
            d2 = SetO(ref p2, EA, EC, EB);
            d3 = SetO(ref p3, EB, EC, EA);

            SetCircleO(e1, p1, 8);
            SetCircleO(e2, p2, 8);
            SetCircleO(e3, p3, 8);

            /* описанные окружности */
            Ellipse c1 = FindCreateEllipse(Canv, "C1", (Brush)null, 0);
            SetCircleO(c1, p1, d1);
            Ellipse c2 = FindCreateEllipse(Canv, "C2", (Brush)null, 0);
            SetCircleO(c2, p2, d2);
            Ellipse c3 = FindCreateEllipse(Canv, "C3", (Brush)null, 0);
            SetCircleO(c3, p3, d3);
        }

        /****************
         Описание
           Построение окружности c1 с центром = е1, диаметром = d1
         ***************/
        private void SetCircleO(Ellipse c, Point p, double d)
        {
            
            c.SetValue(Canvas.LeftProperty, p.X - 0.5*d);
            c.SetValue(Canvas.WidthProperty, d);
            c.SetValue(Canvas.TopProperty, p.Y - 0.5*d);
            c.SetValue(Canvas.HeightProperty, d);
        }

        /***************
         Описание
           Построение точки центра правильного треугольника с основанием EA-EB, вершина по другую сторону от точки EC
         ***************/
        private double SetO(ref Point p, Shape EA, Shape EB, Shape EC)
        {
            double[] lefts = new double[3];
            double[] widths = new double[3];
            double[] tops = new double[3];
            double[] heights = new double[3];
            double[] strokeThickness = new double[3];

            lefts[0] = Double.Parse(EA.GetValue(Canvas.LeftProperty).ToString());
            lefts[1] = Double.Parse(EB.GetValue(Canvas.LeftProperty).ToString());
            lefts[2] = Double.Parse(EC.GetValue(Canvas.LeftProperty).ToString());
            widths[0] = Double.Parse(EA.GetValue(Canvas.WidthProperty).ToString());
            widths[1] = Double.Parse(EB.GetValue(Canvas.WidthProperty).ToString());
            widths[2] = Double.Parse(EC.GetValue(Canvas.WidthProperty).ToString());
            tops[0] = Double.Parse(EA.GetValue(Canvas.TopProperty).ToString());
            tops[1] = Double.Parse(EB.GetValue(Canvas.TopProperty).ToString());
            tops[2] = Double.Parse(EC.GetValue(Canvas.TopProperty).ToString());
            heights[0] = Double.Parse(EA.GetValue(Canvas.HeightProperty).ToString());
            heights[1] = Double.Parse(EB.GetValue(Canvas.HeightProperty).ToString());
            heights[2] = Double.Parse(EC.GetValue(Canvas.HeightProperty).ToString());
            strokeThickness[0] = Double.Parse(EA.StrokeThickness.ToString());
            strokeThickness[1] = Double.Parse(EB.StrokeThickness.ToString());
            strokeThickness[2] = Double.Parse(EC.StrokeThickness.ToString());

            Point A = new Point(lefts[0] + 0.5*widths[0] + 0*strokeThickness[0],
                                tops[0] + 0.5*heights[0] + 0*strokeThickness[0]
                                );
            Point B = new Point(lefts[1] + 0.5*widths[1] + 0*strokeThickness[1],
                                tops[1] + 0.5*heights[1] + 0*strokeThickness[1]
                                );
            Point C = new Point(lefts[2] + 0.5 * widths[2] + 0*strokeThickness[2],
                                tops[2] + 0.5 * heights[2] + 0*strokeThickness[2]
                                );

            p = GeometryOp.FindO(A, B, C);
            
            double d;
            d = Math.Sqrt(Math.Pow(p.X - B.X, 2) + Math.Pow(p.Y - B.Y, 2));
            d = 2.0 * d + 0 * strokeThickness[2];

            return d;
        }

        private Line FindCreateLine(Canvas c, string AName)
        {
            Line l = (Line)null;
            foreach (UIElement e in c.Children)
            {
                if (e is Line && ((Line)e).Name == AName)
                    l = (Line)e;
            }

            if (l == (Line)null)
            {
                l = new Line();
                l.Name = AName;
                l.Stroke = Brushes.Black;
                l.StrokeThickness = 3;
                l.IsHitTestVisible = false;
                
                c.Children.Add(l);
            }

            return l;
        }

        private Ellipse FindCreateEllipse(Canvas c, string AName, Brush fill, Double d)
        {
            Ellipse el = (Ellipse)null;
            foreach (UIElement e in c.Children)
            {
                if (e is Ellipse && ((Ellipse)e).Name == AName)
                    el = (Ellipse)e;
            }

            if (el == (Ellipse)null)
            {
                el = new Ellipse();
                el.Name = AName;
                el.Width = d;
                el.Height = d;
                el.Stroke = Brushes.Black;
                el.StrokeThickness = 1;
                el.Fill = fill;
                el.IsHitTestVisible = false;

                c.Children.Add(el);
            }

            return el;
        }

        private Rectangle FindCreateRect(Canvas c, string AName, Brush fill, Double d)
        {
            var el = (Rectangle)null;
            foreach (UIElement e in c.Children)
            {
                if (e is Rectangle && ((Rectangle)e).Name == AName)
                    el = (Rectangle)e;
            }

            if (el == (Rectangle)null)
            {
                el = new Rectangle();
                el.Name = AName;
                el.Width = d;
                el.Height = d;
                el.Stroke = Brushes.Black;
                el.StrokeThickness = 1;
                el.Fill = fill;
                el.IsHitTestVisible = false;

                c.Children.Add(el);
            }

            return el;
        }

        private void LinkE(Ellipse A, Ellipse B, Line l)
        {
            l.X1 = Double.Parse(A.GetValue(Canvas.LeftProperty).ToString()) + 0.5*Double.Parse(A.GetValue(Canvas.WidthProperty).ToString());
            l.Y1 = Double.Parse(A.GetValue(Canvas.TopProperty).ToString()) + 0.5 * Double.Parse(A.GetValue(Canvas.HeightProperty).ToString());
            l.X2 = Double.Parse(B.GetValue(Canvas.LeftProperty).ToString()) + 0.5 * Double.Parse(B.GetValue(Canvas.WidthProperty).ToString());
            l.Y2 = Double.Parse(B.GetValue(Canvas.TopProperty).ToString()) + 0.5 * Double.Parse(B.GetValue(Canvas.HeightProperty).ToString());
        }
    }
}
