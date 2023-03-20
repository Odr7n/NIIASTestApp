using System.Collections.Generic;
using System.Windows;

namespace NIIASTestApp.Model
{
    /// <summary>
    /// Базовый класс для станций.
    /// Определяет базовый конструктор и 
    /// базовые абстрактные методы для последующего наследования
    /// </summary>
    public abstract class Station
    {

        #region Props
        private string _name;
        /// <summary>
        /// Наименование станции
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// Словарь, хранящий наименования парков и ссылки на объекты соответствующих парков
        /// </summary>
        public Dictionary<string, Park> NameParkPairs { get; set; }

        /// <summary>
        /// Список участков, входящий в станцию
        /// </summary>
        public List<Segment> Segments { get; set; }

        /// <summary>
        /// Содержит значение крайней левой и крайней правой точек по оси X
        /// для масштабирования на визуальной схеме
        /// </summary>
        public double[] XBounds { get; set; }

        /// <summary>
        /// Содержит значение крайней верхней и крайней нижней точек по оси Y
        /// для масштабирования на визуальной схеме
        /// </summary>
        public double[] YBounds { get; set; }

        /// <summary>
        /// Список узловых точек для отображения на схеме
        /// </summary>
        public List<Point> StationPoints { get; set; }

        #endregion

        #region Ctors
        public Station(string name)
        {
            _name = name;
            Segments = new List<Segment>();
            NameParkPairs = new Dictionary<string, Park>();
            StationPoints = new List<Point>();
            //StationPoints = new Dictionary<Point, string>();
            XBounds = new double[2];
            YBounds = new double[2];
        }

        #endregion

        #region Methods
        /// <summary>
        /// Метод для добавления и удаления участков
        /// </summary>
        /// <param name="segments"> список добавляемых участков</param>
        /// <param name="canAdd">true → добавить участок, false →  удалить участок</param>
        public abstract void SegmentsManager(IEnumerable<Segment> segments, bool canAdd = false);

        /// <summary>
        /// Метод для добавления и удаления парков
        /// </summary>
        /// <param name="parkName">Наименование парка</param>
        /// <param name="canAdd">true → добавить участок, false →  удалить участок</param>
        public abstract void ParksManager(string parkName, bool canAdd = false);

        /// <summary>
        /// Метод для поиска кратчайшего пути
        /// </summary>
        /// <param name="startPoint">Начальная точка</param>
        /// <param name="endPoint"> Конечная точка</param>
        /// <returns></returns>
        public abstract List<Segment> GetShortestPath(Point startPoint, Point endPoint);


        #endregion
    }
}
