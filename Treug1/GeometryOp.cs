using System;
using System.Windows;

namespace Treug1
{
    class GeometryOp
    {
        static public Point Rotate2D(Point p, double angle)
        {
            Point p2 = new Point
            {
                X = p.X * Math.Cos(angle) - p.Y * Math.Sin(angle),
                Y = 0.0 + p.X * Math.Sin(angle) + p.Y * Math.Cos(angle)
            };
            return p2;
        }

        static public Point Shift2D(Point p, Point d)
        {
            Point p2 = new Point
            {
                X = p.X + d.X,
                Y = p.Y + d.Y
            };
            return p2;
        }

        static public Point Neg(Point p)
        {
            Point p2 = new Point(-p.X, -p.Y);
            return p2;
        }

        static public Point FindM(Point A, Point B)
        {
            double a, eps = 0.0001;

            //сдвиг
            Point B1 = Shift2D(B, Neg(A));

            //поворот
            if (Math.Abs(B1.X) > eps)
                a = Math.Atan(B1.Y / B1.X);
            else
                a = Math.PI / 2;

            Point B2 = Rotate2D(B1, -a);

            //искомая вершина
            Point M2 = new Point
            {
                X = B2.X / 2,
                Y = B2.X / Math.Sqrt(2)
            };

            //поворот обратно
            Point M1 = Rotate2D(M2, a);

            //сдвиг обратно
            Point M = Shift2D(M1, A);

            return M;
        }

        /********************
         Описание
           Поиск центра правильного треугольника с основанием AB, построенном по другую сторону относительно
         точки C
         ********************/
        static public Point FindO(Point A, Point B, Point C)
        {
            double a, eps = 0.0001;

            //сдвиг
            Point B1 = Shift2D(B, Neg(A));
            Point C1 = Shift2D(C, Neg(A));

            //поворот
            if (Math.Abs(B1.X) > eps)
                a = Math.Atan(B1.Y / B1.X);
            else
                a = Math.PI / 2;

            Point B2 = Rotate2D(B1, -a);
            Point C2 = Rotate2D(C1, -a);

            //искомая вершина: h = sqrt(a^2 - (a/2)^2) = a*sqrt(3)/2; My = 1/3*h = a/sqrt(3)/2
            Point M2 = new Point
            {
                X = B2.X / 2,
                Y = B2.X / Math.Sqrt(3) / 2
            };
            M2.Y = Math.Abs(M2.Y) * (-Math.Sign(C2.Y));


            //поворот обратно
            Point M1 = Rotate2D(M2, a);

            //сдвиг обратно
            Point M = Shift2D(M1, A);
            return M;
        }
    }
}
