using System;
using System.Windows;

namespace NIIASTestApp.Model
{
    /// <summary>
    /// *Перечисление видов сегментов → линия или дуга,
    /// при масштабировании возможно добавление дополнительных видов.
    /// Используется для хранения типа сегмента в структуре Segment, 
    /// а также для расчета и хранения кривизны дуги Size. 
    /// </summary>
    public enum SegmentTypes
    {
        Line, Arc
    }

    /// <summary>
    /// Класс Сегмент.
    /// Хранит необходимые данные о сегменте для отображения на схеме.
    /// Является запакованным, так как не требует создания дочерних классов
    /// </summary>
    public sealed class Segment
    {

        #region Props

        /// <summary>
        /// Список крайних точек участка
        /// </summary>
        private Point[] _points;
        public Point[] Points => _points;

        /// <summary>
        /// Длина участка
        /// </summary>
        private double _length;
        public double Length => _length;

        /// <summary>
        /// Тип участка (в данном примере - линия или дуга).
        /// От типа участка зависит рассчет его длины и крайних точек
        /// </summary>
        private SegmentTypes _segType;
        public SegmentTypes SegmentType => _segType;

        /// <summary>
        /// Радиус дуги, если участок = дуга
        /// </summary>
        public Size ArcSize { get; set; }

        /// <summary>
        /// Хранит ключ поворота для определения его крайних координат 
        /// и корректного отображения на схеме.
        /// true -> поворот по ходу координат X(дуга справа от координаты X), 
        /// false → поворот против координат X (дуга слева от координаты X)
        /// </summary>
        public bool IsArcAlong { get; set; }

        #endregion

        #region Ctors

        public Segment(Point pStart, Point pEnd) : this(pStart, pEnd, SegmentTypes.Line, default, false) { }
        public Segment(Point pStart, Point pEnd, SegmentTypes segmentType, Size size, bool isArcAlong)
        {
            _points = new Point[] { pStart, pEnd };
            _length = LengthGet();
            _segType = segmentType;
            ArcSize = size;
            IsArcAlong = isArcAlong;

            /*рассчитать длину сегмента на основании 
             * координат начальной и конечной точек*/
            double LengthGet()
            {
                StationFuncs mf = new StationFuncs();
                return mf.GetShortestLength(pStart, pEnd);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Возвразает массив минимального и максимльного значения по оси X или Y.
        /// Используется для обновления масштаба схемы.
        /// </summary>
        /// <param name="isX"> true → возвращает массив оси X, false → Y</param>
        /// <returns></returns>
        public double[] BoundsGet(bool isX)
        {
            double[] arr = isX ? new double[] { _points[0].X, _points[1].X } : new double[] { _points[0].Y, _points[1].Y };
            Array.Sort(arr);
            return arr;
        }
        #endregion
    }
}
