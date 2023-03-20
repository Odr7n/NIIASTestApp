using Microsoft.VisualStudio.TestTools.UnitTesting;
using NIIASTestApp.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace NIIASTestApp.Tests
{
    [TestClass]
    public class StationTests
    {
        #region Shortest path tests

        /// <summary>
        /// Проверяет количество сегментов, 
        /// входящих в кратчайший путь между точками
        /// (1,10) и (12,18)
        /// </summary>
        [TestMethod]
        public void GetShortestPath_From_1_10_To_12_18_Return_SegmentsCount_8()
        {
            /*Входные и выходные параметры*/
            Point pStart = new Point(1, 10);
            Point pEnd = new Point(12, 18);
            int segmentsCountExpected = 8;

            /*Запуск метода*/
            Logic l = new Logic();
            Station st = l.StationCreateByHardcode();
            int segmentsCountActual;
            if (st.StationPoints.Contains(pStart) && st.StationPoints.Contains(pEnd))
            {
                segmentsCountActual = (st.GetShortestPath(pStart, pEnd)).Count;
            }
            else segmentsCountActual = 0;

            /*Проверка*/
            Assert.AreEqual(segmentsCountExpected, segmentsCountActual);
        }

        #endregion

        #region StationScheme tests

        /// <summary>
        /// Проверяет количество сегментов, имеющих хотябы 1 общую точку
        /// с общим количеством сегментов.
        /// Если количество с общими точками меньше, значит какой то сегмент некорректно добавлен
        /// </summary>
        [TestMethod]
        public void StationSchemeTest_SegmentsWithCommonPointsCount_Equal_stationSegments()
        {
            /*Входные и выходные параметры*/
            Logic l = new Logic();
            Station st = l.StationCreateByHardcode();
            int segsWithCommonPointsExpected = st.Segments.Count;
            int segsWithCommonPointsActual = 0;


            /*Получение актуального значения*/
            bool isCommonPointFound;
            foreach (Segment sBase in st.Segments)
            {
                isCommonPointFound = false;
                foreach (Segment sComp in st.Segments)
                {
                    if (sComp == sBase) continue;
                    foreach (Point p in sBase.Points)
                    {
                        if (sComp.Points.Contains(p))
                        {
                            segsWithCommonPointsActual++;
                            isCommonPointFound = true;
                            break;
                        }
                    }
                    if (isCommonPointFound) break;
                }
            }


            /*Проверка*/
            Assert.AreEqual(segsWithCommonPointsExpected, segsWithCommonPointsActual);
        }

        /// <summary>
        /// Проверяет общее количество сегментов. Проверка на целостность
        /// </summary>
        [TestMethod]
        public void StationSchemeTest_SegmentsCount_41()
        {
            /*Входные и выходные параметры*/
            int segmentsCountExpected = 41;


            /*Получение актуального значения*/
            Logic l = new Logic();
            Station st = l.StationCreateByHardcode();
            int segmentsCountActual = st.Segments.Count;


            /*Проверка*/
            Assert.AreEqual(segmentsCountExpected, segmentsCountActual);
        }

        /// <summary>
        /// Проверяет общее количество точек. Проверка на целостность.
        /// </summary>
        [TestMethod]
        public void StationSchemeTest_StationPointsCount_36()
        {
            /*Входные и выходные параметры*/
            int stPointsCountExpected = 41;


            /*Получение актуального значения*/
            Logic l = new Logic();
            Station st = l.StationCreateByHardcode();
            int stPointsCountActual = st.Segments.Count;


            /*Проверка*/
            Assert.AreEqual(stPointsCountExpected, stPointsCountActual);
        }

        #endregion

        #region ParkHighlighting tests 

        /// <summary>
        /// Проверяет количество участков, из которых состоят пути в парке. 
        /// Их должно быть более 1, что бы можно было нарисовать границы.
        /// </summary>
        [TestMethod]
        public void ParksHighlightsTest_TracksCount_MoreThan_1()
        {

            /*Входные и выходные параметры*/
            bool isTracksSegmentsMoreThan1Expected = true;
            bool isTracksSegmentsMoreThan1Actual;


            /*Получение актуального значения*/
            Logic l = new Logic();
            Station st = l.StationCreateByHardcode();
            List<int> parksSegmentsCount = new List<int>();
            int segmentsCount;

            /*Перебрать все парки*/
            foreach (Park park in st.NameParkPairs.Values.ToList())
            {
                segmentsCount = 0;
                /*Перебрать все листы участков, из которых состоят пути
                 *и посчитать количество участков, из которых состоит путь */
                foreach (var list in park.NameTrackPairs.Values.ToList())
                {
                    segmentsCount += list.Count();
                }
                parksSegmentsCount.Add(segmentsCount);
            }

            int parksPassed = 0;
            /*Перебрать полученные результаты, если количество сегментов > 1 
             * увеличить число правильных парков на 1 */
            foreach (int item in parksSegmentsCount)
            {
                if (item > 1) parksPassed++;
            }
            isTracksSegmentsMoreThan1Actual = parksPassed == st.NameParkPairs.Count();


            /*Проверка*/
            Assert.AreEqual(isTracksSegmentsMoreThan1Expected, isTracksSegmentsMoreThan1Actual);
        }


        [TestMethod]
        public void ParkHighlightsTest_HeightWidthIsPositive()
        {
            /*Входные и выходные параметры*/
            bool isHWPositiveExpected = true;
            bool isHWPositiveActual = false;


            /*Получение актуального значения*/
            Logic l = new Logic();
            Station st = l.StationCreateByHardcode();
            double xMin;
            double xMax;
            double yMin;
            double yMax;
            int passedParks = 0;

            foreach (Park park in st.NameParkPairs.Values.ToList())
            {
                xMin = park.BoundsPoints.Min(p => p.X);
                xMax = park.BoundsPoints.Max(p => p.X);
                yMin = park.BoundsPoints.Min(p => p.Y);
                yMax = park.BoundsPoints.Max(p => p.Y);
                if (xMax > xMin && yMax > yMin) passedParks++;
            }
            isHWPositiveActual = st.NameParkPairs.Count() == passedParks;


            /*Проверка*/
            Assert.AreEqual(isHWPositiveExpected, isHWPositiveActual);
        }

        #endregion

    }
}
