using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using cv = NIIASTestApp.VM.CommonVars;

namespace NIIASTestApp.Model
{
    /// <summary>
    /// Поддерживающий класс для хранения методов функций, используемых для вычислений
    /// </summary>
    public class StationFuncs
    {
        /// <summary>
        /// Эвристическая функция для расчета оценки от текущей точки до конечной
        /// </summary>
        /// <param name="startP">текущая точка</param>
        /// <param name="endP">конечная точка</param>
        /// <returns></returns>
        public double GetShortestLength(Point startP, Point endP)
        {
            if (startP != endP) return Math.Abs(startP.X - endP.X) + Math.Abs(startP.Y - endP.Y);
            else return 0;
        }

        /// <summary>
        /// Обновляет границы станции или парка. Перегруженный метод.
        /// Обновление по списку участков
        /// </summary>
        /// <param name="segments"> список участков станции</param>
        /// <param name="xBounds">значения координат X самой левой и самой правой точек</param>
        /// <param name="yBounds">значения координат Y самой левой и самой правой точек</param>
        public void BoundsRefresh(IEnumerable<Segment> segments, double[] xBounds, double[] yBounds)
        {
            foreach (var s in segments)
            {
                foreach (var p in s.Points) BoundsRefresh(p, xBounds, yBounds);
            }
        }

        /// <summary>
        /// Обновляет границы станции или парка. Перегруженный метод/
        /// Обновление по одной новой точке
        /// </summary>
        /// <param name="p">новая точка, значения X и Y которой сравниваются</param>
        /// <param name="xBounds">значения координат X самой левой и самой правой точек</param>
        /// <param name="yBounds">значения координат Y самой левой и самой правой точек</param>
        public void BoundsRefresh(Point p, double[] xBounds, double[] yBounds)
        {
            double value;
            double[] bounds;
            for (int i = 0; i < 2; i++)
            {
                value = i == 0 ? p.X : p.Y;
                bounds = i == 0 ? xBounds : yBounds;

                if (bounds[0] == 0) bounds[0] = bounds[1] = value;
                else
                {
                    if (value < bounds[0]) bounds[0] = value;
                    else if (value > bounds[1]) bounds[1] = value;
                }
            }
        }

        /// <summary>
        /// Упорядочивает точки в списке по часовой стрелке.
        /// Используется для user friendly отображения названий точек на схеме 
        /// для более приятного и быстрого для пользователя выбора начальной и конченой точек,
        /// а также для правильного построения многоугольника для заливки парка
        /// </summary>
        /// <param name="points">список точек объекта</param>
        /// <param name="xBounds">значения координат X самой левой и самой правой точек</param>
        /// <param name="yBounds">значения координат Y самой левой и самой правой точек</param>
        /// <returns></returns>
        public List<Point> GetPointsOrderByClockwise(IEnumerable<Point> points, double[] xBounds, double[] yBounds)
        {
            List<Point> points0 = points as List<Point>;

            double xOffset = xBounds[0] > 0 ? -1 * xBounds.Average() : 0;
            double yOffset = yBounds[0] > 0 ? -1 * yBounds.Average() : 0;

            List<Point> points1 = new List<Point>();
            foreach (Point p in points0) points1.Add(new Point(p.X + xOffset, p.Y + yOffset));

            points0.Clear();
            points0 = points1.OrderBy(p => Math.Atan2(p.Y, p.X)).ToList();

            points1.Clear();
            foreach (Point p in points0) points1.Add(new Point(p.X - xOffset, p.Y - yOffset));

            return points1;
        }

        /// <summary>
        /// Метод используется для записи логов при отладке алгоритмов
        /// </summary>
        /// <param name="log"></param>
        public void WriteLog(string log)
        {
            string logPath = cv.logFilePath;
            using (StreamWriter writer = new StreamWriter(logPath))
            {
                writer.Write(log);
            }
        }
    }
}
