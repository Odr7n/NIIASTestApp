using NIIASTestApp.View;
using NIIASTestApp.VM;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;


namespace NIIASTestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Props

        /// <summary>
        /// Создает и хранит экземпляр объекта дополнительно окна
        /// </summary>
        public WindowSlave WS { get; set; } = new WindowSlave();

        /// <summary>
        /// Cоздает и хранит экземпляр VM
        /// </summary>
        public ViewModelMW VM { get; set; } = new ViewModelMW();

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            /*создать станцию (хардкод)*/
            VM.StationCreate();

            /*добавить в cb_Color список доступных цветов для заливки*/
            VM.SetCBColors();

            /*добавить ссылку на текущий объект custom control в свойство VM*/
            VM.MWScheme = stationScheme;
        }

        #region Events Handlers

        /// <summary>
        /// Анимация открытия окна при загрузке
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WinMain_Loaded(object sender, RoutedEventArgs e) => MWOpenCloseAnimation(true);

        /// <summary>
        /// Включает возможность перемещения окна с помощью мыши
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridMain_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

        /// <summary>
        /// Анимация закрытия окон при нажатии кнопки btn_Off и завершение приложения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Off_Click(object sender, RoutedEventArgs e)
        {
            if (WS.VisibleKey) WS.WSOpenCloseAnimation(false);
            MWOpenCloseAnimation(false);
        }

        /// <summary>
        /// Анимация кнопки btn_WindowSlaveOpen, анимация шрифта кнопки + анимация ширины границы эллипса (нижний слой кнопки) 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_WindowSlaveOpen_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            #region Анимация кнопки

            /* если предыдущая анимация нажатия завершилась, 
             * ключ переходит в true и можно открыть окно заново */
            if (WS.CanOpenKey)
            {
                WS.CanOpenKey = false;

                Button btn = (Button)sender;
                AnimationCtors ac = new AnimationCtors();

                /* создание объектов анимации */
                DoubleAnimation daFontSize = ac.DblAnimationFromTo(true, btn.FontSize, btn.FontSize - 2, 10, true);
                DoubleAnimation daEllipseStroke = ac.DblAnimationFromTo(true, btn_WindowSlaveOpenEllipse.StrokeThickness, btn_WindowSlaveOpenEllipse.StrokeThickness + 1, 10, true);


                /* после окончания анимации 
                 * позволяет нажать кнопку еще раз */
                daEllipseStroke.Completed += SlaveWindowOpen;

                btn.BeginAnimation(Button.FontSizeProperty, daFontSize);
                btn_WindowSlaveOpenEllipse.BeginAnimation(Ellipse.StrokeThicknessProperty, daEllipseStroke);
            }

            #endregion

            #region Открытие дополнительного окна WindowSlave

            void SlaveWindowOpen(object o, EventArgs a)
            {
                /* Если WindowSlave скрыто → открыть, изменить значение ключа */
                if (!WS.VisibleKey)
                {
                    /* Запустить анимацию открытия WindowSlave*/
                    WS.WSOpenCloseAnimation(true);
                    WS.Show();

                    /*Устатановить ключ видимости WindowSlave*/
                    WS.VisibleKey = true;
                }

                /* Устанавливает себя 
                 * в качестве владельца доп.окна */
                if (WS.Owner != this) WS.Owner = this;
            }

            #endregion
        }

        /// <summary>
        /// Событие при смене вбранной станции
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CB_Station_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VM.DataClear();
            VM.GetStation();
            VM.SchemeCreate();
            if (WS.IsVisible)
            {
                WS.VM.DataClear();
                WS.VM.SchemeCreate();
            }
        }

        /// <summary>
        /// Обработчик события смены цвета.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CB_Color_DropDownClosed(object sender, EventArgs e) => VM.ParkHighlighting();

        #endregion

        #region Methods

        /// <summary>
        /// Анимация открытия и закрытия окна
        /// </summary>
        /// <param name="key"> ключ, отвечающий за направления анимации открытие / закрытие окна</param>
        private void MWOpenCloseAnimation(bool key)
        {
            AnimationCtors ac = new AnimationCtors();

            /* создается объект анимации */
            DoubleAnimation da = ac.DblAnimationScale(key, Height, 0);

            da.EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut, Exponent = 5 };

            /* после окончания анимации завершить приложение */
            if (!key) da.Completed += (o, a) => Application.Current.Shutdown();

            /* выполнение анимации */
            TranslateTransform tt = new TranslateTransform();
            win_Main.RenderTransform = tt;
            tt.BeginAnimation(TranslateTransform.YProperty, da);
        }

        #endregion
    }
}
