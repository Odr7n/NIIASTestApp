using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace NIIASTestApp.Model
{
    public class Park : IDisposable
    {
        #region Vars

        /// <summary>
        /// Границы по оси X [левая правая]
        /// </summary>
        private double[] _xBounds;

        /// <summary>
        /// Границы парка по оси Y [верхняя нижняя]
        /// </summary>
        private double[] _yBounds;

        #endregion

        #region Props

        /// <summary>
        /// Наименование парка
        /// </summary>
        public string Name { get; set; }


        private Dictionary<string, List<Segment>> _nameTrackPairs;
        /// <summary>
        /// Словарь наименований путей и сегмнетов этих путей, входящих в парк
        /// </summary>
        public Dictionary<string, List<Segment>> NameTrackPairs => _nameTrackPairs;


        private List<Point> _boundsPoints;
        /// <summary>
        /// Список точек, расположенных по периметру парка
        /// </summary>
        public List<Point> BoundsPoints => _boundsPoints;


        private Point _centralPoint;
        /// <summary>
        /// Хранит координаты центральной точки парка
        /// для отображения в ней наименования парка на схеме
        /// </summary>
        public Point CentralPoint => _centralPoint;

        #endregion

        #region Ctors

        public Park() : this(default) { }
        public Park(string name)
        {
            Name = name;
            _nameTrackPairs = new Dictionary<string, List<Segment>>();
            _boundsPoints = new List<Point>();
            _xBounds = new double[2];
            _yBounds = new double[2];
        }

        #endregion

        #region Methods Public

        /// <summary>
        /// Метод, добавляющий или удаляющий пути в/из парка
        /// </summary>
        /// <param name="trackName"> имя пути, при необходимости можно подписать путь на схеме</param>
        /// <param name="canAdd">true → добавить путь, false → удалить путь</param>
        /// <param name="segments"> список участков, входящих в путь</param>
        public void TracksManager(string trackName, bool canAdd = false, IEnumerable<Segment> segments = null)
        {
            StationFuncs sf = new StationFuncs();

            if (canAdd)
            {
                /*Проверка, что список участков был передан и не пустой*/
                if (segments != null && segments.Count() > 0)
                {
                    /*Проверка, что участки представляют собой непрерывную ломаную*/
                    List<Segment> sgmnts = segments.ToList();
                    List<Point> pointsChecked = new List<Point>();
                    List<Segment> sgmntsChecked = new List<Segment>();
                    int counter = 0;
                    int countCheck = sgmnts.Count() - 1;

                    //MessageBox.Show($"Попытка добавить Путь {trackName} в Парк {Name}\ncountCheck = {countCheck}");

                    foreach (Segment sCurrent in sgmnts)
                    {
                        foreach (Segment sCompare in sgmnts)
                        {
                            if (sCompare != sCurrent && !sgmntsChecked.Contains(sCompare))
                            {
                                foreach (Point pCur in sCurrent.Points)
                                {
                                    if (pointsChecked.Contains(pCur)) continue;

                                    foreach (Point pComp in sCompare.Points)
                                    {
                                        //MessageBox.Show(
                                        //    $"Park → {Name}\n" +
                                        //    $"trackName → {trackName}\n" +
                                        //    $"sCurrent → {sCurrent.Points[0]} --- {sCurrent.Points[1]}\n" +
                                        //    $"sCompare → {sCompare.Points[0]} --- {sCompare.Points[1]}\n" +
                                        //    $"pCur: {pCur} → pComp: {pComp}\n");

                                        if (pCur == pComp)
                                        {
                                            pointsChecked.Add(pCur);
                                            counter++;
                                            break;
                                        }
                                        //MessageBox.Show($"counter → {counter}");
                                    }
                                }
                            }
                        }

                        sgmntsChecked.Add(sCurrent);

                        //foreach (Point pCur in sCurrent.Points)
                        //{
                        //    pointsChecked.Clear();
                        //    foreach (Segment sCompare in sgmnts)
                        //    {
                        //        if (sCompare!= sCurrent)
                        //        {
                        //            foreach (Point pComp in sCompare.Points)
                        //            {
                        //                if (pointsChecked.Contains(pComp)) break;
                        //                if (pCur == pComp) counter++;
                        //                else pointsChecked.Add(pComp);
                        //                MessageBox.Show(
                        //                    $"Park → {Name}\n" +
                        //                    $"trackName → {trackName}\n" +
                        //                    $"sCurrent → {sCurrent.Points[0]} --- {sCurrent.Points[1]}\n" +
                        //                    $"sCompare → {sCompare.Points[0]} --- {sCompare.Points[1]}\n" +
                        //                    $"pCur: {pCur} → pComp: {pComp}\n " +
                        //                    $"counter → {counter}");
                        //            }
                        //            if (counter == 2) break;
                        //        }
                        //    }
                        //    if (counter == 2) break;
                        //}
                    }


                    if (counter != countCheck)
                    {
                        MessageBox.Show($"! Путь {trackName} не может быть добавлен в парк {Name}.\n" +
                            $"Не все участки имеют смежные точки");
                        return;
                    }



                    /*Добавить путь в парк*/
                    if (!_nameTrackPairs.ContainsKey(trackName)) _nameTrackPairs.Add(trackName, segments.ToList());
                    else _nameTrackPairs[trackName].AddRange(segments);


                    foreach (var s in segments)
                    {
                        //_segments.Add(s);
                        //if (s.SegmentType == SegmentTypes.Arc)
                        //{
                        //    double x0= s.Points[0].X;
                        //    double x1= s.Points[1].X;
                        //    double y0= s.Points[0].Y;
                        //    double y1= s.Points[1].Y;
                        //    double r = s.ArcSize.Width / 2;
                        //    double d = Math.Sqrt(Math.Pow(x0 - x1, 2) + Math.Pow(y0 - y1, 2));
                        //    double h = Math.Sqrt(Math.Pow(r, 2) - Math.Pow(d / 2, 2));
                        //    int k = s.IsArcAlong ? -1 : 1;
                        //    double xc = x0 + (x1 - x0) / 2 + (h * (y1 - y0) / d) * k;
                        //    double yc = y0 + (y1 - y0) / 2 - (h * (x1 - x0) / d) * k;
                        //    Point p = new Point(xc, yc);
                        //    if (!_parkPoints.Contains(p)) _parkPoints.Add(p);
                        //    sf.BoundsRefresh(p, _xBounds, _yBounds);
                        //}

                        foreach (Point p in s.Points)
                        {
                            sf.BoundsRefresh(p, _xBounds, _yBounds);

                            //if (s.SegmentType == SegmentTypes.Arc)
                            //{
                            //    double Offset = s.ArcSize.Width / 3;
                            //    double x = s.IsArcAlong ? p.X + Offset : p.X - Offset;
                            //    AddPoint(new Point(x, p.Y));
                            //}
                            //else AddPoint(p);

                            //void AddPoint(Point point)
                            //{
                            //    //if (!_parkPoints.Contains(point)) _parkPoints.Add(point);
                            //    sf.BoundsRefresh(point, _xBounds, _yBounds);
                            //}
                        }
                    }

                }
                //MessageBox.Show($" Путь {trackName} добавлен в Парк {Name}");
            }
            else
            {
                if (_nameTrackPairs.ContainsKey(trackName))
                {
                    foreach (Segment s in _nameTrackPairs[trackName])
                    {
                        foreach (Point p in s.Points)
                        {
                            if (_boundsPoints.Contains(p)) _boundsPoints.Remove(p);
                        }
                    }
                    _nameTrackPairs.Remove(trackName);
                }
            }
        }

        /// <summary>
        /// Удаляет пути из парка на основании списка участков, из которых состоит путь
        /// </summary>
        /// <param name="segments"></param>
        public void RemoveTracks(IEnumerable<Segment> segments)
        {
            List<string> keysToRemove = new List<string>();
            foreach (Segment s in segments)
            {
                for (int i = 0; i < _nameTrackPairs.Count(); i++)
                {
                    if (_nameTrackPairs.ElementAt(i).Value.Contains(s)) keysToRemove.Add(_nameTrackPairs.ElementAt(i).Key);
                }
            }
            foreach (string key in keysToRemove) _nameTrackPairs.Remove(key);
            keysToRemove.Clear();
        }

        /// <summary>
        /// Устанавливает точки - границы парка и сортирует их в списке по часовой стрелке.
        /// Используется для отображения заливки парка на схеме
        /// </summary>
        public void SetBoundsPoints()
        {
            StationFuncs sf = new StationFuncs();

            List<Point> points = new List<Point>();
            foreach (var list in _nameTrackPairs.Values.ToList())
            {
                foreach (Segment s in list)
                {
                    foreach (Point p in s.Points)
                    {
                        if (_xBounds.Contains(p.X) || _yBounds.Contains(p.Y))
                        {
                            if (s.SegmentType == SegmentTypes.Arc)
                            {
                                double Offset = s.ArcSize.Width / 3;
                                double x = s.IsArcAlong ? p.X + Offset : p.X - Offset;
                                points.Add(new Point(x, p.Y));
                            }
                            else
                            {
                                if (!points.Contains(p)) points.Add(p);
                            }
                        }
                    }
                }
            }

            _boundsPoints = sf.GetPointsOrderByClockwise(points, _xBounds, _yBounds);

            #region V1

            //List<Point> points = new List<Point>();
            //foreach (var p in _parkPoints)
            //{
            //    if (_xBounds.Contains(p.X) || _yBounds.Contains(p.Y)) points.Add(p);
            //}

            //double xOffset = _xBounds[0] > 0 ? -1 * _xBounds.Average() : 0;
            //double yOffset = _yBounds[0] > 0 ? -1 * _yBounds.Average() : 0;

            //List<Point> offsetPoints = new List<Point>();
            //foreach (Point p in points) offsetPoints.Add(new Point(p.X + xOffset, p.Y + yOffset));

            //List<Point> orderedPoints = offsetPoints.OrderBy(p => Math.Atan2(p.X, p.Y)).ToList();
            //foreach (Point p in orderedPoints) _boundsPoints.Add(new Point(p.X - xOffset, p.Y - yOffset));

            #endregion

            #region V2
            //double value;
            //bool isX;
            //double bound;
            //bool sortKey;

            //for (int i = 0; i < 4; i++)
            //{
            //    List<Point> points = new List<Point>();
            //    switch (i)
            //    {
            //        default:
            //        case 0:
            //            isX = false;
            //            bound = _yBounds[0];
            //            sortKey = true;
            //            break;
            //        case 1:
            //            isX = true;
            //            bound = _xBounds[1];
            //            sortKey = true;
            //            break;
            //        case 2:
            //            isX = false;
            //            bound = _yBounds[1];
            //            sortKey = false;
            //            break;
            //        case 3:
            //            isX = true;
            //            bound = _xBounds[0];
            //            sortKey = false;
            //            break;
            //    }

            //    foreach (var p in _parkPoints)
            //    {
            //        value = isX ? p.X : p.Y;
            //        if (!_boundsPoints.Contains(p) && value == bound) points.Add(p);
            //    }

            //    if (sortKey)
            //    {
            //        if (isX) points.Sort((p1, p2) => p1.Y.CompareTo(p2.Y));
            //        else points.Sort((p1, p2) => p1.X.CompareTo(p2.X));
            //    }
            //    else
            //    {
            //        if (isX) points.Sort((p1, p2) => p2.Y.CompareTo(p1.Y));
            //        else points.Sort((p1, p2) => p2.X.CompareTo(p1.X));
            //    }
            //    _boundsPoints.AddRange(points);
            //}

            #endregion
        }

        /// <summary>
        /// Устанавливает центральную точку парка для размещения в ней наименования парка на схеме
        /// </summary>
        public void SetCentralPoint() => _centralPoint = new Point(_xBounds.Average(), _yBounds.Average());

        /// <summary>
        /// Очищает все свойства парка при удалении, что бы быстрее освободить память
        /// </summary>
        public void Dispose()
        {
            _nameTrackPairs.Clear();
            _boundsPoints.Clear();
            Array.Clear(_xBounds, 0, 2);
            Array.Clear(_yBounds, 0, 2);
            Name = default;
        }

        #endregion

    }
}
