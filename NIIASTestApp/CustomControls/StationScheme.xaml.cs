using NIIASTestApp.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NIIASTestApp.CustomControls
{
    /// <summary>
    /// UserControl, рисующий схему станции и все сопутствующие элементы
    /// </summary>
    public partial class StationScheme : UserControl
    {

        #region Inner Vars
        // todo: в рамках текущего присложения допустимо указать параметры в виде хардкода,
        // но в реальном приложении можно создать dependency property для настройки ключевых параметров UserControl

        /// <summary>
        /// Коэффициент масштабирования по оси Х. 
        /// Используется для автомасштабирования параметров отображаемых элементов
        /// в зависимости от их количества и парамета Width StationScheme
        /// </summary>
        double _xScale;

        /// <summary>
        /// Коэффициент масштабирования по оси Y. 
        /// Используется для автомасштабирования параметров отображаемых элементов
        /// в зависимости от их количества и параметра Height StationScheme
        /// </summary>
        double _yScale;

        /// <summary>
        /// Мультипликатор для размера узлов на схеме
        /// </summary>
        const float _pRadiusMult = 0.04f;

        /// <summary>
        /// Мультипликатор для толщины линий схемы
        /// </summary>
        const float _sThicknessMult = 0.05f;

        /// <summary>
        /// Мультипликатор для размера выделенных узлов для нахождения пути
        /// </summary>
        const float _ppRadiusMult = 0.3f;

        /// <summary>
        /// Мультипликатор для толщины подписей узлов
        /// </summary>
        const float _psThicknessMult = 0.02f;

        /// <summary>
        /// Мультипликатор для позиционирования подписей узлов по оси X
        /// </summary>
        const float _pOffsetXMult = 0.2f;

        /// <summary>
        /// Мультипликатор для позиционирования подписей узлов по оси Y
        /// </summary>
        const float _pOffsetYMult = 0.3f;

        /// <summary>
        /// Мультипликатор для размера шрифта подписей узлов
        /// </summary>
        const float _pFontSizeMult = 0.8f;

        /// <summary>
        /// Opacity для подписей узлов
        /// </summary>
        const float _psOpacity = 0.7f;

        /// <summary>
        /// Opacity для заливки парков
        /// </summary>
        const float _parkOpacity = 0.3f;

        /// <summary>
        /// FontSize для наименований парков
        /// </summary>
        const byte _parkFontSize = 40;

        /// <summary>
        /// Мультипликатор для толщины путей
        /// </summary>
        const float _tThicknessMult = 0.1f;

        /// <summary>
        /// Кисть для наименования парка
        /// </summary>
        Brush _parkNameBrush = Brushes.Black;

        /// <summary>
        /// Хранит пары: наименование объекта → список UIElement, 
        /// отображаемых для данного объекта (Path, Polygon, TextBlock и т.д.)
        /// Для включения интерактивности при переключении параметров пользователем в ComboBox
        /// </summary>
        Dictionary<string, List<UIElement>> _nameUIElementsPairs = new Dictionary<string, List<UIElement>>();


        #endregion


        #region Ctors

        public StationScheme()
        {
            InitializeComponent();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Рисует схему станции.
        /// Схема автоматически масштабируется
        /// </summary>
        /// <param name="st"> объект станции</param>
        public void StationDrawing(Station st, bool canAddPointsSigns = false)
        {
            /*Вычислить коэффициенты масштабирования*/
            XYScalesSet(st);


            /*добавить в GeometryGroup все отрезки и точки станции*/
            GeometryGroup gg = SegmentsToGeometryCtor(st.Segments);


            /*добавить GeometryGroup в свойство Path.Data и вывести на холст*/
            Path p = PathCtor(gg, Brushes.Snow, _sThicknessMult);
            canvas.Children.Add(p);


            /*Добавить подписи точек (применяется для дополнительного окна поиска пути)*/
            if (canAddPointsSigns)
            {
                List<Point> points = PointsXYTranslate(st.StationPoints);
                double x;
                double y;
                gg = new GeometryGroup();
                for (int i = 0; i < points.Count(); i++)
                {
                    FormattedText text = new FormattedText($"T{i}({st.StationPoints[i]})",
                        CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        new Typeface(FontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal),
                        _yScale * _pFontSizeMult,
                        Brushes.Black,
                        null,
                        VisualTreeHelper.GetDpi(this).PixelsPerDip);

                    x = points[i].X - _xScale * _pOffsetXMult;
                    y = points[i].Y + _yScale * _pOffsetYMult;

                    Geometry g = text.BuildGeometry(new Point(x, y));
                    gg.Children.Add(g);
                }

                p = PathCtor(gg, Brushes.Black, _psThicknessMult, _psOpacity);
                canvas.Children.Add(p);
            }
        }

        /// <summary>
        /// Выполняет заливку парка цветом
        /// </summary>
        /// <param name="st">объект станции</param>
        /// <param name="parkName">наименование парка</param>
        /// <param name="color">необходимый цвет заливки</param>
        public void ParkHighlighting(Station st, string parkName, Brush color)
        {
            /*Если парк уже ранее заливался, то заменить цвет,
             *если нет - отобразить заливку и записать значения в словарь*/
            if (_nameUIElementsPairs.ContainsKey(parkName))
            {
                Polygon polygon = _nameUIElementsPairs[parkName][0] as Polygon;
                Path path = _nameUIElementsPairs[parkName][1] as Path;
                TextBlock tb = _nameUIElementsPairs[parkName][2] as TextBlock;
                polygon.Fill = color;
                path.Stroke = color == Brushes.Transparent ? color : GetTrackBrush();
                tb.Foreground = color == Brushes.Transparent ? color : _parkNameBrush;
            }
            else
            {
                XYScalesSet(st);

                Park park = st.NameParkPairs[parkName];

                /*Отобразить заливку*/
                List<Point> points = PointsXYTranslate(park.BoundsPoints);
                Polygon polygon = new Polygon();
                foreach (var point in points)
                {
                    polygon.Points.Add(point);
                }
                polygon.Fill = color;
                polygon.Opacity = _parkOpacity;

                canvas.Children.Add(polygon);

                /*Отобразить наименование*/
                TextBlock tb = new TextBlock();
                tb.Text = parkName;
                tb.FontFamily = Application.Current.FindResource("MainFont") as FontFamily;
                tb.FontSize = _parkFontSize;
                tb.Margin = new Thickness(park.CentralPoint.X * _xScale, park.CentralPoint.Y * _yScale, 0, 0);
                tb.Opacity = _parkOpacity;
                tb.Foreground = _parkNameBrush;
                canvas.Children.Add(tb);

                /*Отобразить пути*/
                List<Segment> segments = new List<Segment>();
                foreach (var list in park.NameTrackPairs.Values.ToList()) segments.AddRange(list);
                GeometryGroup gg = SegmentsToGeometryCtor(segments);
                Path path = PathCtor(gg, GetTrackBrush(), _tThicknessMult);
                canvas.Children.Add(path);

                /*добавить значения в словарь*/
                _nameUIElementsPairs.Add(parkName, new List<UIElement> { polygon, path, tb });
            }
        }

        /// <summary>
        /// выделяет цветом выбранные начальную и конечную точки кратчайшего пути
        /// </summary>
        /// <param name="cbName">имя combobox, из которого была выбрана точка, для записи в словарь</param>
        /// <param name="point">выбранная точка</param>
        /// <param name="brush">цвет заливки</param>
        public void PointHighlighting(string cbName, Point point, Brush brush)
        {
            Point p = PointXYTranslate(point);
            Ellipse e;
            double width = _xScale * _ppRadiusMult;
            double x = p.X - width / 2;
            double y = p.Y - width / 2;

            if (_nameUIElementsPairs.ContainsKey(cbName))
            {
                e = _nameUIElementsPairs[cbName][0] as Ellipse;
                e.Margin = new Thickness(x, y, 0, 0);
            }
            else
            {
                e = new Ellipse();
                e.Fill = brush;
                e.Width = width;
                e.Height = width;
                e.Margin = new Thickness(x, y, 0, 0);
                _nameUIElementsPairs.Add(cbName, new List<UIElement> { e });
                canvas.Children.Add(e);
            }
        }

        /// <summary>
        /// Отображает кратчайший путь
        /// </summary>
        /// <param name="pathSegments"> список участков кратчайшего пути для отображения</param>
        public void SPathDrawing(IEnumerable<Segment> pathSegments)
        {
            string name = "SPath";
            Path p = PathCtor(SegmentsToGeometryCtor(pathSegments), Brushes.LightSkyBlue, _tThicknessMult);
            if (_nameUIElementsPairs.ContainsKey(name))
            {
                canvas.Children.Remove(_nameUIElementsPairs[name][0]);
                _nameUIElementsPairs[name].Clear();
                _nameUIElementsPairs[name].Add(p);
            }
            else _nameUIElementsPairs.Add(name, new List<UIElement>() { p });
            canvas.Children.Add(p);
        }

        /// <summary>
        /// Очищает все дочерние элементы canvas и словарь
        /// </summary>
        public void Clear()
        {
            canvas.Children.Clear();
            _nameUIElementsPairs.Clear();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Производит масштабирование координат исходных точек объекта станции 
        /// в новые координаты для отображения в корректном масштабе
        /// </summary>
        /// <param name="pts"> исходные точки станции</param>
        /// <returns></returns>
        private List<Point> PointsXYTranslate(IEnumerable<Point> pts)
        {
            List<Point> points = new List<Point>();
            foreach (var pt in pts) points.Add(new Point(pt.X * _xScale, pt.Y * _yScale));
            return points;
        }

        /// <summary>
        /// Производит трансляцию координат X и Y для точек к масштабу схемы.
        /// Приводит 
        /// </summary>
        /// <param name="p"> точка, координаты которой нужно транслировать</param>
        /// <returns></returns>
        private Point PointXYTranslate(Point p) => new Point(p.X * _xScale, p.Y * _yScale);

        /// <summary>
        /// Вычисляет коеффициенты масштабирования для осей X и Y
        /// и записывает результаты в закрытые переменные _xScale и _yScale
        /// </summary>
        /// <param name="st"> ссылка на текущий объект станции</param>
        private void XYScalesSet(Station st)
        {
            _xScale = ActualWidth / Math.Abs(st.XBounds[1] + st.XBounds[0]);
            _yScale = st.YBounds[1] == st.YBounds[0] ?
                ActualHeight / (2 * st.YBounds[0]) :
                ActualHeight / Math.Abs(st.YBounds[1] + st.YBounds[0]);
        }

        /// <summary>
        /// Конструктор геометрий из списка участков.
        /// Создает объекты линий и точек для отображения
        /// </summary>
        /// <param name="segments">список участков для отображения</param>
        /// <returns></returns>
        private GeometryGroup SegmentsToGeometryCtor(IEnumerable<Segment> segments)
        {
            GeometryGroup gg = new GeometryGroup();
            List<Point> points;

            foreach (Segment s in segments)
            {
                points = PointsXYTranslate(s.Points);
                if (s.SegmentType == SegmentTypes.Line)
                {
                    LineGeometry lg = new LineGeometry();
                    lg.StartPoint = points[0];
                    lg.EndPoint = points[1];
                    gg.Children.Add(lg);
                }
                else
                {
                    PathGeometry pg = new PathGeometry();
                    PathFigure pf = new PathFigure();
                    ArcSegment a = new ArcSegment();
                    pf.StartPoint = points[0];
                    a.Point = points[1];
                    a.Size = s.ArcSize;
                    a.SweepDirection = s.IsArcAlong ? SweepDirection.Clockwise : SweepDirection.Counterclockwise;
                    pf.Segments.Add(a);
                    pg.Figures.Add(pf);
                    gg.Children.Add(pg);
                }

                foreach (Point point in points)
                {
                    EllipseGeometry eg = new EllipseGeometry();
                    eg.Center = point;
                    eg.RadiusX = _xScale * _pRadiusMult;
                    eg.RadiusY = eg.RadiusX;
                    gg.Children.Add(eg);
                }
            }

            return gg;
        }

        /// <summary>
        /// формирует объект Path из GeometryGroup
        /// </summary>
        /// <param name="gg">сформированный объект GeometryGroup</param>
        /// <param name="brush">цвет Path</param>
        /// <param name="thicknessMult">мульрипликатор толщины линий path</param>
        /// <param name="opacity">коеффициент непрозрачности</param>
        /// <returns></returns>
        private Path PathCtor(GeometryGroup gg, Brush brush, double thicknessMult, double opacity = 1)
        {
            Path p = new Path();
            p.Data = gg;
            p.Stroke = brush;
            p.StrokeThickness = _xScale * thicknessMult;
            p.Opacity = opacity;
            return p;
        }

        /// <summary>
        /// Возвращает 1 случайный цвет из 5.
        /// Используется для покраски путей, входящих в парк.
        /// Используется выборка случайного цвета для отличия путей,
        /// входящих в разные парки
        /// </summary>
        /// <returns></returns>
        private Brush GetTrackBrush()
        {
            Random rand = new Random();
            int r = rand.Next(4);
            Brush[] brshs = new Brush[]
            {
                Brushes.Crimson,
                Brushes.Blue,
                Brushes.Fuchsia,
                Brushes.Lime,
                Brushes.PapayaWhip
            };

            return brshs[r];
        }
        #endregion
    }
}
