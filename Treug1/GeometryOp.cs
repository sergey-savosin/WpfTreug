using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Treug1
{
    class GeometryOp
    {
        static public Point Rotate2D(Point p, double angle)
        {
            Point p2 = new Point();
            p2.X = p.X*Math.Cos(angle) + p.Y*Math.Sin(angle);
            p2.Y = 0.0 - p.X*Math.Sin(angle) + p.Y*Math.Cos(angle);
            return p2;
        }

        static public Point Shift2D(Point p, Point d)
        {
            Point p2 = new Point();
            p2.X = p.X + d.X;
            p2.Y = p.Y + d.Y;
            return p2;
        }

        static public Point Neg(Point p)
        {
            Point p2 = new Point(-p.X, -p.Y);
            return p2;
        }

        static public Point FindM(Point A, Point B)
        {
            Point B1 = new Point();
            Point B2 = new Point();
            Point M2 = new Point();
            Point M1 = new Point();
            Point M = new Point();

            double a, eps = 0.0001;

            //сдвиг
            B1 = GeometryOp.Shift2D(B, GeometryOp.Neg(A));
            if (Math.Abs(B1.X) > eps)
                a = Math.Atan(B1.Y / B1.X);
            else
                a = Math.PI / 2;

            //поворот
            B2 = GeometryOp.Rotate2D(B1, a);

            //искомая вершина
            M2.X = B2.X / 2;
            M2.Y = B2.X / Math.Sqrt(2);

            //поворот обратно
            M1 = GeometryOp.Rotate2D(M2, -a);

            //сдвиг обратно
            M = GeometryOp.Shift2D(M1, A);
            return M;
        }

        /********************
         Описание
           Поиск центра правильного треугольника с основанием AB, построенном по другую сторону относительно
         точки C
         ********************/
        static public Point FindO(Point A, Point B, Point C)
        {
            Point B1 = new Point();
            Point C1 = new Point();
            Point B2 = new Point();
            Point C2 = new Point();
            Point M2 = new Point();
            Point M1 = new Point();
            Point M = new Point();

            double a, eps = 0.0001;

            //сдвиг
            B1 = GeometryOp.Shift2D(B, GeometryOp.Neg(A));
            C1 = GeometryOp.Shift2D(C, GeometryOp.Neg(A));
            if (Math.Abs(B1.X) > eps)
                a = Math.Atan(B1.Y / B1.X);
            else
                a = Math.PI / 2;

            //поворот
            B2 = GeometryOp.Rotate2D(B1, a);
            C2 = GeometryOp.Rotate2D(C1, a);

            //искомая вершина: h = sqrt(a^2 - (a/2)^2) = a*sqrt(3)/2; My = 1/3*h = a/sqrt(3)/2
            M2.X = B2.X / 2;
            M2.Y = B2.X / Math.Sqrt(3) / 2;
            M2.Y = Math.Abs(M2.Y) * (-Math.Sign(C2.Y));
            

            //поворот обратно
            M1 = GeometryOp.Rotate2D(M2, -a);

            //сдвиг обратно
            M = GeometryOp.Shift2D(M1, A);
            return M;
        }


    }
}
