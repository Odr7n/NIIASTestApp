using System.Collections.Generic;
using System.Windows;

namespace NIIASTestApp.Model
{
    /// <summary>
    /// Класс для имплементации различной логики приложения
    /// </summary>
    public class Logic
    {
        #region Methods
        /// <summary>
        /// Создает станцию (хардкод)
        /// </summary>
        /// <returns></returns>
        public Station StationCreateByHardcode()
        {
            StationTest st = new StationTest("Test");

            List<Segment> segments = new List<Segment>();
            Point pS = new Point();
            Point pE = new Point();
            string parkName;


            #region Helpers Methods

            ///<summary>
            /// добавляет следующий участок, начальная точка которого = конечной точке предыдущего участка
            ///</summary>
            void AddChainSegment(double x, double y, bool isArc = false, double radius = default, bool isArcAlong = false)
            {
                pE.X = x;
                pE.Y = y;

                if (isArc) segments.Add(new Segment(pS, pE, SegmentTypes.Arc, new Size(radius, radius), isArcAlong));
                else segments.Add(new Segment(pS, pE));

                pS = pE;
            }

            ///<summary>
            ///Устанавливает начальную точку
            ///</summary>
            void SetStartPoint(double x, double y)
            {
                pS.X = x; pS.Y = y;
            }

            ///<summary>
            ///Устанавливает граничные точки парка и центральную точку
            ///</summary>
            void SetParkPoints()
            {
                Park park = st.NameParkPairs[parkName];
                park.SetBoundsPoints();
                park.SetCentralPoint();
            }

            #endregion


            #region Upper part

            /* Въезд на станцию и прямой участок до поворота */
            SetStartPoint(1, 10);
            AddChainSegment(3, 10);
            AddChainSegment(8, 10);
            AddChainSegment(17, 10);
            AddChainSegment(19, 10);


            /*Съезд 0*/
            SetStartPoint(3, 10);
            AddChainSegment(5, 8);
            AddChainSegment(10, 8);
            AddChainSegment(14, 8);
            AddChainSegment(17, 8);
            AddChainSegment(19, 8);

            /*Съезд 1*/
            SetStartPoint(5, 8);
            AddChainSegment(7, 6);
            AddChainSegment(12, 6);
            AddChainSegment(15, 6);
            AddChainSegment(17, 8);

            /*Съезд 2*/
            SetStartPoint(7, 6);
            AddChainSegment(9, 4);
            AddChainSegment(13, 4);
            AddChainSegment(15, 6);

            /*Съезд 3 → тупик*/
            SetStartPoint(17, 10);
            AddChainSegment(15, 12);
            AddChainSegment(5, 12);

            /*Main → Съезд 0*/
            SetStartPoint(8, 10);
            AddChainSegment(10, 8);

            /*Съезд 1 → Съезд 0*/
            SetStartPoint(12, 6);
            AddChainSegment(14, 8);

            /*Добавить участки в объект станции*/
            st.SegmentsManager(segments, true);
            segments.Clear();

            #endregion

            #region Park0

            /*Создать объект парка и добавить в список парков станции*/
            parkName = "Park0";
            st.ParksManager(parkName, true);

            /*добавить путь "0" в парк*/
            SetStartPoint(3, 10);
            AddChainSegment(17, 10);
            AddChainSegment(19, 10);
            st.NameParkPairs[parkName].TracksManager("0", true, segments);
            segments.Clear();

            /*Добавить путь "1" в парк */
            SetStartPoint(9, 4);
            AddChainSegment(13, 4);
            st.NameParkPairs[parkName].TracksManager("1", true, segments);
            segments.Clear();

            /*установить граничные точки парка для заливки
             и центральную точку для textblock*/
            SetParkPoints();

            #endregion

            #region Middle part

            /*Повороты*/
            SetStartPoint(19, 10);
            AddChainSegment(19, 20, true, 5, true);

            SetStartPoint(19, 8);
            AddChainSegment(19, 22, true, 7, true);

            /*Путь Main из Парка 0 → выезд из станции*/
            SetStartPoint(19, 20);
            AddChainSegment(17, 20);
            AddChainSegment(3, 20);
            AddChainSegment(1, 20);

            /*Съезд 0 → тупик*/
            SetStartPoint(17, 20);
            AddChainSegment(15, 18);
            AddChainSegment(12, 18);

            /*Путь Sec из Парка 0 → в Main*/
            SetStartPoint(19, 22);
            AddChainSegment(17, 22);
            AddChainSegment(5, 22);
            AddChainSegment(3, 20);

            st.SegmentsManager(segments, true);
            segments.Clear();

            #endregion

            #region Park 1

            parkName = "Park1";
            st.ParksManager(parkName, true);

            /*путь 0*/
            SetStartPoint(19, 10);
            AddChainSegment(19, 20, true, 5, true);
            AddChainSegment(17, 20);
            AddChainSegment(3, 20);
            AddChainSegment(1, 20);
            st.NameParkPairs[parkName].TracksManager("0", true, segments);
            segments.Clear();

            /*путь 1*/
            SetStartPoint(19, 22);
            AddChainSegment(5, 22);
            st.NameParkPairs[parkName].TracksManager("1", true, segments);
            segments.Clear();

            SetParkPoints();

            #endregion

            #region Lower part

            /*Съезд 0 → тупик*/
            SetStartPoint(5, 22);
            AddChainSegment(7, 24);
            AddChainSegment(9, 24);
            AddChainSegment(12, 21);
            AddChainSegment(15, 21);

            /*Съезд 1*/
            SetStartPoint(7, 24);
            AddChainSegment(9, 26);
            AddChainSegment(13, 26);
            AddChainSegment(17, 22);


            /*Съезд 2 → тупик*/
            SetStartPoint(9, 26);
            AddChainSegment(11, 28);
            AddChainSegment(14, 28);


            /*Съезд 3 → тупик*/
            SetStartPoint(11, 28);
            AddChainSegment(13, 30);
            AddChainSegment(17, 30);

            st.SegmentsManager(segments, true);
            segments.Clear();

            st.SetStationPointsOrderByClockwise();

            #endregion

            #region Park 2

            parkName = "Park2";
            st.ParksManager(parkName, true);

            /*путь 0*/
            SetStartPoint(9, 24);
            AddChainSegment(12, 21);
            AddChainSegment(15, 21);
            st.NameParkPairs[parkName].TracksManager("0", true, segments);
            segments.Clear();

            /*путь 1*/
            SetStartPoint(13, 30);
            AddChainSegment(17, 30);
            st.NameParkPairs[parkName].TracksManager("1", true, segments);
            segments.Clear();

            /*путь 2*/
            SetStartPoint(5, 22);
            AddChainSegment(7, 24);
            AddChainSegment(9, 26);
            AddChainSegment(13, 26);
            st.NameParkPairs[parkName].TracksManager("2", true, segments);
            segments.Clear();

            SetParkPoints();

            #endregion

            return st;
        }
        #endregion
    }
}
