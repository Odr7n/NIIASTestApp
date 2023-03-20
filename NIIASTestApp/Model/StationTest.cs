using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace NIIASTestApp.Model
{
    public class StationTest : Station
    {

        #region Ctors

        public StationTest(string name) : base(name) { }

        #endregion

        #region Public Methods

        /// <summary>
        /// Добавляет или удаляет участки из свойства Segments.
        /// Добавление участков не приводит к добавлению их в парк.
        /// Удаление участков приводит к удалению путей, содержащих эти участки, из соответствующих парков
        /// </summary>
        /// <param name="segment"> ссылка на объект сегмента</param>
        /// <param name="add">true → добавить, false → удалить</param>
        public override void SegmentsManager(IEnumerable<Segment> segments, bool canAdd = false)
        {
            StationFuncs sf = new StationFuncs();

            if (canAdd)
            {
                foreach (var s in segments)
                {
                    if (!Segments.Contains(s))
                    {
                        /*Добавить сегмент в свойство Segments*/
                        Segments.Add(s);


                        /*Добавить узловые точки в свойство StationPoints 
                         * для использования при масштабировании и выборе точек для поиска пути*/
                        StationPointsManager(s, true);


                        foreach (var p in s.Points)
                        {
                            /*Обновить границы станции*/
                            Point pt = s.SegmentType == SegmentTypes.Arc ? new Point(p.X + s.ArcSize.Width / 4, p.Y + s.ArcSize.Height / 4) : p;
                            sf.BoundsRefresh(pt, XBounds, YBounds);
                        }
                    }
                }
                ///*добавить в объект парка ссылку на сегменты*/
                //if (parkName != default) TracksManager(parkName, trackName, true, segments);
            }
            else
            {
                /*Удалить пути из парка и обновить границы парка*/
                foreach (Park park in NameParkPairs.Values.ToList())
                {
                    park.RemoveTracks(segments);
                    park.SetBoundsPoints();
                }

                /*Удалить участки из станции и обновить границы*/
                foreach (var s in segments)
                {
                    if (Segments.Contains(s))
                    {
                        /*если сегмент присутствует в списке → удалить*/
                        Segments.Remove(s);

                        /*удалить точки сегмента из списка точек станции*/
                        StationPointsManager(s);

                        /*Обновить границы станции*/
                        sf.BoundsRefresh(Segments, XBounds, YBounds);
                    }
                }
            }
        }

        /// <summary>
        /// Добавляет или удаляет объект парка
        /// </summary>
        /// <param name="parkName">Наименование парка</param>
        /// <param name="canAdd">true → добавить, false →  удалить</param>
        public override void ParksManager(string parkName, bool canAdd = false)
        {
            if (canAdd)
            {
                /*Если парка не существует → создать парк и добавить в коллекцию*/
                if (!NameParkPairs.ContainsKey(parkName))
                {
                    NameParkPairs.Add(parkName, new Park(parkName));
                }
                else MessageBox.Show("Парк уже существует");
            }
            else
            {
                if (NameParkPairs.ContainsKey(parkName))
                {
                    NameParkPairs[parkName].Dispose();
                    NameParkPairs.Remove(parkName);
                }
            }
        }

        /// <summary>
        /// Возвращает список сегментов кратчайшего пути между точками.
        /// Основан на алгоритме A*. Алгоритм исследует маршрут по A*,
        /// но как только общая оценка score превышает оценку какой либо ранее посещенной, 
        /// но не выбранной Node - бросает текущий Node и переходит на новый с более
        /// лучшим Score
        /// </summary>
        /// <param name="pStart">начальная точка</param>
        /// <param name="pEnd">конечная точка</param>
        /// <returns></returns>
        public override List<Segment> GetShortestPath(Point pStart, Point pEnd)
        {
            #region Vars

            /*Экземпляр класса StationFuncs для вызова функций*/
            StationFuncs sf = new StationFuncs();


            /*Текущая активная точка*/
            Point pActive = pStart;


            /*Cписок всех Node, которые были выбраны, как лучшие 
             * создать и записать в него стартовый Node*/
            List<Node> nodesPicked = new List<Node>() { new Node(pStart) };


            /*Список проверенных Node, для которых вычислены все параметры, 
             * но которые не были выбраны, как лучшие*/
            List<Node> nodesChecked = new List<Node>();


            /*список Node, исследуемых в текущей итерации. 
             * Сделан отдельным списком для более быстрого поиска
             за счет перебора меньшего количества узлов*/
            List<Node> nodesI = new List<Node>();


            /*Список активных сегментов текущей итерации*/
            List<Segment> segmentsI = new List<Segment>();


            /*Лучший Node итерации*/
            Node nodeBest = null;


            /*Лучший параметр Node.Score в текущей итерации*/
            double scoreI = 0;


            /*Переменные для хранения параметров рассматриваемого Node*/
            double routeLengthE = 0;
            double routeLengthS = 0;
            double score = 0;
            Segment segmentPicked = default;

            #endregion

            #region Logic

            while (pActive != pEnd)
            {
                ///<summary>
                /// Локальный метод поиска сегментов с общей точкой
                ///</summary>
                List<Segment> GetSegmentsI(Point p) => Segments.FindAll(s => s.Points.Contains(pActive));


                /*Найти все сегменты segmentsA (segments active), содержащие активную точку*/
                segmentsI = GetSegmentsI(pActive);


                /*Счетчик соседних нод, которые пропускаются 
                 *(находятся в списке nodesPicked или являются заблокаированными Node.IsBlocked=true)
                 *используется для прыжка из тупиковой точки*/
                int nodesPassedCounter = 0;

                /*Ключ нахождения конечной точки*/
                bool isFinished = false;

                /*Сравнить соседние узлы и найти узел с лучшим Score 
                 *(кратчайшее расстояние до конечной точки + длина пути от начальной)*/
                for (int i = 0; i < segmentsI.Count(); i++)
                {
                    ///<summary>
                    /// Локальный метод для обновлении свойства SegmentTrack 
                    /// (список сегментов кратчайшего пути до данного Node)
                    ///</summary>
                    void SetNodeSegmentsTrack(Node node)
                    {
                        node.SegmentsTrack.AddRange(node.NodePrev.SegmentsTrack);
                        node.SegmentsTrack.Add(segmentsI[i]);
                    }


                    /*найти противоположную точку сегмента, относительно активной*/
                    Point p = Array.Find(segmentsI[i].Points, (o) => o != pActive);


                    /*Если следующая точка является конечной - 
                    * записать участок в список и завершить метод*/
                    if (p == pEnd)
                    {
                        nodeBest = new Node(p);
                        nodeBest.NodePrev = nodesPicked.Last();
                        SetNodeSegmentsTrack(nodeBest);
                        segmentPicked = segmentsI[i];
                        isFinished = true;
                        break;
                    }


                    ///<summary>
                    /// Локальный метод. Проверяет все ли соседние ноды недоступны
                    /// (в списке nodesPicked или заблокированы). 
                    /// Если все недоступны → устанавливает score большим, чем максимальный в списке nodesChecked
                    /// для прыжка к лучшему Node из nodesChecked и возвращает bool - значение проверки
                    ///</summary>
                    bool IsNodesPassedCounterFull()
                    {
                        bool isFull = nodesPassedCounter == segmentsI.Count();
                        if (isFull)
                        {
                            scoreI = nodesChecked.Max(n => n.Score) + 1;
                            score = scoreI + 1;
                        }
                        return isFull;
                    }


                    /*Проверить, является ли проверяемый Node ранее выбранным.
                     * Если является → посчитать число таких Node (nodesPassedCounter). 
                     * Если число равно общему числу соседних Node, 
                     * значит нужно перепрыгнуть на Node из листа nodesChecked с лучшим Score.
                     * Для этого значение лучшей оценки итераций scoreI устанавливается большим,
                     * чем максимальное значение Score среди всех Node в листе nodesChecked,
                     * что бы в последующем при сравнении ScoreI c наименьшим Score в nodesChecked
                     * был выбран Node из nodesChecked.
                     * Если не является →  продолжить проверку*/
                    int index = nodesPicked.FindIndex(n => n.NodePoint == p);
                    if (index != -1)
                    {
                        nodesPassedCounter++;
                        if (!IsNodesPassedCounterFull()) continue;
                    }
                    else
                    {
                        /*Если среди проверенных узлов отсутствует узел с данной точкой → 
                         * создать узел и вычислить для него параметры, 
                         * иначе → пересчитать для узла оценку и, 
                         * если оценка лучше предыдущей, перезаписать параметры*/
                        index = nodesChecked.FindIndex(n => n.NodePoint == p);
                        if (index == -1)
                        {
                            routeLengthE = sf.GetShortestLength(p, pEnd);
                            routeLengthS = nodesPicked.Last().RouteLengthS + segmentsI[i].Length;
                            score = routeLengthS + routeLengthE;

                            Node node = new Node(p, nodesPicked.Last(), routeLengthS, routeLengthE);
                            SetNodeSegmentsTrack(node);

                            nodesI.Add(node);

                        }
                        else
                        {
                            /*пересчитать путь от начальной точки*/
                            routeLengthS = nodesPicked.Last().RouteLengthS + segmentsI[i].Length;


                            /*если текущий путь от начальной точки короче предыдущего →
                                обновить значения переменных в текущей ноде*/
                            if (routeLengthS < nodesChecked[index].RouteLengthS)
                            {
                                Node node = nodesChecked[index];
                                node.RouteLengthS = routeLengthS;
                                node.NodePrev = nodesPicked.Last();
                                node.SetScore();

                                node.SegmentsTrack.Clear();
                                SetNodeSegmentsTrack(node);

                                score = node.Score;

                                /*добавить ноду в список нод текущей итерации*/
                                nodesI.Add(node);
                            }
                            else
                            {
                                nodesPassedCounter++;
                                nodesChecked[index].IsBlocked = true;
                                if (!IsNodesPassedCounterFull()) continue;
                            }
                        }


                        /*Обновить лучшую оценку scoreI и выбрать лучший Node*/
                        if (scoreI == 0) RefreshScoreNodeSegment(nodesI.Last());
                        else
                        {
                            if (score == scoreI)
                            {
                                /*Если параметр Score текущей Node такой же как и у ранее выбранной,
                                 * то выбрать ту, у которой меньше значение пути от начальной точки*/
                                nodeBest = nodesI.Find(n0 => n0.RouteLengthS == nodesI.Min(n1 => n1.RouteLengthS));

                                //Segment segBest = segmentsI.Find(s => s.Points.Contains(nodeBest.NodePoint));
                                RefreshScoreNodeSegment(nodeBest/*, segBest*/);
                            }

                            if (score < scoreI) RefreshScoreNodeSegment();
                        }

                        ///<summary>
                        /// Локальный метод. Обновляет значения scoreI и nodeBest
                        ///</summary>
                        void RefreshScoreNodeSegment(Node n = null/*, Segment s = null*/)
                        {
                            scoreI = score;
                            //segmentPicked = s == null ? segmentsI[i] : s;
                            nodeBest = n == null ? nodesI.Last() : n;
                        }
                    }
                }


                /*Если конечная точка была найдена - закончить цикл*/
                if (isFinished) break;


                /*Проверить Node, выбранный лучшим в итерации (c наименьшим score) со всеми Node в списке nodeChecked,
                 Если есть Node с более меньшим Score - перепрыгнуть на него*/
                if (nodesChecked.Count() > 0)
                {
                    double scoreAlt = nodesChecked.Min(n => n.Score);
                    if (scoreAlt < scoreI) nodeBest = nodesChecked.Find(n => n.Score == scoreAlt);
                }

                /*Записать в значение текущей точки значение точки лучшей Node*/
                pActive = nodeBest.NodePoint;


                /*Обновить листы и переменные итерации*/
                nodesPicked.Add(nodeBest);
                nodesChecked.AddRange(nodesI);
                bool wasRem = nodesChecked.Remove(nodeBest);

                nodesI.Clear();
                segmentsI.Clear();
                scoreI = 0;
            }

            return nodeBest.SegmentsTrack;

            #endregion
        }

        /// <summary>
        /// Сортирует список точек парка по часовой стрелке
        /// для более удобного отображения в дополнительном окне WindowSlave
        /// </summary>
        public void SetStationPointsOrderByClockwise()
        {
            StationFuncs sf = new StationFuncs();
            List<Point> pointsOrdered = sf.GetPointsOrderByClockwise(StationPoints, XBounds, YBounds);
            StationPoints.Clear();
            StationPoints = pointsOrdered;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Добавляет или удаляет точки свойства StationPoints
        /// </summary>
        /// <param name="s"> Добавляемый или удаляемый участок, содержащий точки</param>
        /// <param name="canAdd">true → добавить, false →  удалить</param>
        void StationPointsManager(Segment s, bool canAdd = false)
        {
            foreach (Point p in s.Points)
            {
                if (canAdd)
                {
                    if (!StationPoints.Contains(p)) StationPoints.Add(p);
                }
                else
                {
                    if (StationPoints.Contains(p)) StationPoints.Remove(p);
                }
            }
        }

        #endregion
    }
}
