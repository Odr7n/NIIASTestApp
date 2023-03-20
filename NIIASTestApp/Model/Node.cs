using System.Collections.Generic;
using System.Windows;

namespace NIIASTestApp.Model
{
    public class Node
    {
        #region Props

        /// <summary>
        /// Точка, в которой размещается Node
        /// </summary>
        private Point _nodePoint;
        public Point NodePoint => _nodePoint;

        /// <summary>
        /// Хранит ссылку на предыдущий объект Node из которого алгоритм поиска пути пришел в текущий Node
        /// </summary>
        public Node NodePrev { get; set; }

        /// <summary>
        /// Хранит эвристическую оценку до конечной точки 
        /// (метод GetShortestLength в поддерживающем классе StationFunc)
        /// </summary>
        public double RouteLengthE { get; set; }

        /// <summary>
        /// Хранит лучшее значение длины пути от начальной точки
        /// </summary>
        public double RouteLengthS { get; set; }

        /// <summary>
        /// Хранит итоговую оценку Node = RouteLengthE + RouteLengthS
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// Хранит список сегментов, составляющих кратчайший путь до Node от начальной точки
        /// </summary>
        public List<Segment> SegmentsTrack { get; set; }

        /// <summary>
        /// Ключ, который устанавливается в true
        /// если алгоритм пришел в Node второй раз 
        /// и новая оценка оказалась хуже, чем предыдущая.
        /// С помощью пометки алгоритм больше не обратится к этому Node
        /// </summary>
        public bool IsBlocked { get; set; }

        #endregion

        #region Ctors

        public Node(Point nodePoint) : this(nodePoint, null, 0, 0) { }
        public Node(Point nodePoint, Node nodePrev) : this(nodePoint, nodePrev, 0, 0) { }
        public Node(Point nodePoint, Node nodePrev, double routeLengthS, double routeLengthE)
        {
            _nodePoint = nodePoint;
            NodePrev = nodePrev;
            RouteLengthS = routeLengthS;
            RouteLengthE = routeLengthE;
            SegmentsTrack = new List<Segment>();
            IsBlocked = false;
            SetScore();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Вычисляет итоговую оценку Node
        /// </summary>
        public void SetScore() => Score = RouteLengthS + RouteLengthE;

        #endregion
    }
}
