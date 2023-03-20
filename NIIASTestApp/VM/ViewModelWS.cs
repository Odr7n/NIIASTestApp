using NIIASTestApp.CustomControls;
using NIIASTestApp.Model;
using NIIASTestApp.VM.Commands;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using cv = NIIASTestApp.VM.CommonVars;

namespace NIIASTestApp.VM
{
    /// <summary>
    /// VM для доп.окна
    /// </summary>
    public class ViewModelWS : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

        #region Vars
        /// <summary>
        /// Переменная для хранения точек и их подписей для отображения на схеме
        /// </summary>
        private Dictionary<string, Point> _namePointPairs = new Dictionary<string, Point>();
        #endregion

        #region Props

        /// <summary>
        /// Харнит экземпляр объекта схемы из главного окна
        /// </summary>
        public StationScheme WSScheme { get; set; }


        private string _stationName;
        /// <summary>
        /// Хранит наименование станции для отображения в TextBox
        /// </summary>
        public string StationName
        {
            get { return _stationName; }
            set { _stationName = value; OnPropertyChanged(); }
        }


        private string _startpointName;
        /// <summary>
        /// Хранит наименование выбранной начальной точки
        /// </summary>
        public string StartpointName
        {
            get { return _startpointName; }
            set { _startpointName = value; OnPropertyChanged(); }
        }


        private string _endpointName;
        /// <summary>
        /// Хранит наимнование выбранной конечной точки
        /// </summary>
        public string EndpointName
        {
            get { return _endpointName; }
            set { _endpointName = value; OnPropertyChanged(); }
        }


        private List<string> _cbPoints = new List<string>();
        /// <summary>
        /// Хранит список всех точек станции для выбора в combobox
        /// </summary>
        public List<string> CBPoints
        {
            get { return _cbPoints; }
            set { _cbPoints = value; OnPropertyChanged(); }
        }

        #endregion

        #region Commands
        private ICommand _pathFindCmd;
        /// <summary>
        /// Команда, связанная с кнопкой btn_Calculate, вызывающая метод вычисления кратчайшего пути
        /// </summary>
        public ICommand PathFindCmd => _pathFindCmd ?? (_pathFindCmd = new RelayCommand(PathFind, CanPathFind));

        #endregion

        #region Methods public

        /// <summary>
        /// Рисует схему станции
        /// </summary>
        public void SchemeCreate()
        {
            if (cv.Station != null)
            {
                StationName = cv.Station.Name;
                WSScheme.StationDrawing(cv.Station, true);
                SetPointsToComboBoxes();
            }
        }

        /// <summary>
        /// Подсвечивает выбранные начальную и конечную точки
        /// </summary>
        /// <param name="cbName"></param>
        public void PointsHighlighting(string cbName)
        {
            Brush brush;
            string pointName;
            if (cbName.ToLower().Contains("start"))
            {
                brush = Brushes.GreenYellow;
                pointName = _startpointName;
            }
            else
            {
                brush = Brushes.IndianRed;
                pointName = _endpointName;
            }
            if (pointName != null) WSScheme.PointHighlighting(cbName, _namePointPairs[pointName], brush);
        }

        /// <summary>
        /// Удаляет данные при сворачивании окна
        /// </summary>
        public void DataClear()
        {
            EndpointName = null;
            StartpointName = null;
            _namePointPairs.Clear();
            CBPoints.Clear();
            WSScheme.Clear();
        }

        #endregion

        #region Methods private

        /// <summary>
        /// Заполняет комбобоксы точками для выбора
        /// </summary>
        private void SetPointsToComboBoxes()
        {
            if (_namePointPairs.Count == 0)
            {
                string pointStr;
                for (int i = 0; i < cv.Station.StationPoints.Count(); i++)
                {
                    Point p = cv.Station.StationPoints[i];
                    pointStr = $"Т{i} ({p})";
                    CBPoints.Add(pointStr);
                    _namePointPairs.Add(pointStr, p);
                }
            }
        }

        /// <summary>
        /// Проверочный метод команды PathFindCmd
        /// </summary>
        /// <returns></returns>
        private bool CanPathFind() => cv.Station != null && _startpointName != default && _endpointName != default;

        /// <summary>
        /// Метод вызывающий алгоритм поиска пути в отдельном потоке
        /// </summary>
        private void PathFind()
        {
            List<Segment> sPathSegments = null;
            Task t = Task.Factory.StartNew(() =>
            {
                StationTest st = (StationTest)cv.Station;
                sPathSegments = st.GetShortestPath(_namePointPairs[_startpointName], _namePointPairs[_endpointName]);
            });
            t.Wait();
            t.Dispose();

            if (sPathSegments != null) WSScheme.SPathDrawing(sPathSegments);
        }

        #endregion

    }
}
