using NIIASTestApp.CustomControls;
using NIIASTestApp.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Media;
using cv = NIIASTestApp.VM.CommonVars;

namespace NIIASTestApp.VM
{
    /// <summary>
    /// VM для основного окна
    /// </summary>
    public class ViewModelMW : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

        #region Props
        /// <summary>
        /// Харнит экземпляр объекта схемы из главного окна
        /// </summary>
        public StationScheme MWScheme { get; set; }


        private string _stationName;
        /// <summary>
        /// Хранит наименование выбранной станции
        /// </summary>
        public string StationName
        {
            get { return _stationName; }
            set { _stationName = value; OnPropertyChanged(); }
        }


        private List<string> _cbStations = new List<string>();
        /// <summary>
        /// Хранит список доступных станций
        /// </summary>
        public List<string> CBStations
        {
            get { return _cbStations; }
            set { _cbStations = value; OnPropertyChanged(); }
        }


        private List<Station> _stations = new List<Station>();
        /// <summary>
        /// Харнит список загруженных станций
        /// </summary>
        public List<Station> Stations => _stations;


        private string _parkName;
        /// <summary>
        /// Хранит наименование выбранного парка
        /// </summary>
        public string ParkName
        {
            get { return _parkName; }
            set { _parkName = value; OnPropertyChanged(); }
        }


        private List<string> _cbParks = new List<string>();
        /// <summary>
        /// Хранит список доступных парков
        /// </summary>
        public List<string> CBParks
        {
            get { return _cbParks; }
            set { _cbParks = value; OnPropertyChanged(); }
        }


        private string _colorName;
        /// <summary>
        /// Хранит наименование выбранного цвета для заливки парка
        /// </summary>
        public string ColorName
        {
            get { return _colorName; }
            set { _colorName = value; OnPropertyChanged(); }
        }


        private List<string> _cbColors = new List<string>();
        /// <summary>
        /// Хранит список доступных цветов для заливки парка
        /// </summary>
        public List<string> CBColors
        {
            get { return _cbColors; }
            set { _cbColors = value; OnPropertyChanged(); }
        }

        #endregion

        #region Methods public

        /// <summary>
        /// Создает объект станции
        /// </summary>
        public void StationCreate()
        {
            Task t = Task.Factory.StartNew(() =>
            {
                Logic l = new Logic();
                Station st = l.StationCreateByHardcode();

                _stationName = st.Name;
                _cbStations.Add(st.Name);
                _stations.Add(st);
            });
            t.Wait();
            t.Dispose();

            GetStation();
        }

        /// <summary>
        /// Находит в списке доступных станций выбранную,
        /// записывает ссылку на объект выбранной станции в переменную cv.Station,
        /// добавляет список парков в комбобокс cb_Park
        /// </summary>
        public void GetStation()
        {
            cv.Station = _stations.Find(s => s.Name == _stationName);
            List<string> parkNames = new List<string>();
            foreach (var name in cv.Station.NameParkPairs.Keys.ToList()) parkNames.Add(name);
            CBParks.AddRange(parkNames);
        }

        /// <summary>
        /// Создает схему на основании данных выбранной станции
        /// </summary>
        public void SchemeCreate()
        {
            cv.Station = _stations.Find(s => s.Name == _stationName);
            MWScheme.StationDrawing(cv.Station);
        }

        /// <summary>
        /// Осуществляет заливку выбранного в комбобоксе парка
        /// </summary>
        public void ParkHighlighting()
        {
            if (ColorName != default && ParkName != default)
            {
                MWScheme.ParkHighlighting(cv.Station, ParkName, ParkColorConverter(ColorName));
            }
        }

        /// <summary>
        /// Устанавливает список доступных цветов в CBColors
        /// </summary>
        public void SetCBColors()
        {
            CBColors.Add(cv.parkColorGreen);
            CBColors.Add(cv.parkColorBlue);
            CBColors.Add(cv.parkColorRed);
            CBColors.Add(cv.parkColorYellow);
            CBColors.Add(cv.parkColorOrange);
            CBColors.Add(cv.parkColorEmpty);
        }

        /// <summary>
        /// Удаляет предыдущие данные при смене станции
        /// </summary>
        public void DataClear()
        {
            ColorName = null;
            CBParks.Clear();
            ParkName = null;
            MWScheme.Clear();
        }

        #endregion

        #region Methods private

        /// <summary>
        /// возвращает объект brush для цветов из их текстовых наименований
        /// </summary>
        /// <param name="colorName"></param>
        /// <returns></returns>
        private Brush ParkColorConverter(string colorName)
        {
            switch (colorName)
            {
                case cv.parkColorRed: return Brushes.Red;
                case cv.parkColorOrange: return Brushes.Orange;
                case cv.parkColorYellow: return Brushes.Yellow;
                case cv.parkColorBlue: return Brushes.DodgerBlue;
                case cv.parkColorGreen: return Brushes.Green;
                case cv.parkColorEmpty: return Brushes.Transparent;
                default: return null;
            }
        }

        #endregion
    }
}
