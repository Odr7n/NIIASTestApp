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
    /// Класс назван WindowSlave, а не SlaveWindow, 
    /// что бы при добавлении новых окон в случае масштабирования приложения,
    /// наименования всех доп. окон начинались с WindowXXXX
    /// </summary>
    public partial class WindowSlave : Window
    {
        #region Vars
        /// <summary>
        /// Ключ для предотвращения слишком частого нажатия на кнопку btn_Calculate
        /// Предотвращает наложение эффектов анимации.
        /// </summary>
        private bool _canCalcBtnClick = true;
        #endregion

        #region Props

        /// <summary>
        /// хранит ключ состояния окна (свернуто или развернуто),
        /// используется для предотвращения повторного открытия окна через
        /// кнопку главного окна btn_Table
        /// </summary>
        public bool VisibleKey { get; set; } = false;

        /// <summary>
        /// Хранит ключ для определения возможности нажать кнопку открытия 
        /// дополнительного окна btn_WindowSlaveOpen на главном окне.
        /// Если WindowSlave открыто, то нажать кнопку его открытия нельзя.
        /// </summary>
        public bool CanOpenKey { get; set; } = true;

        /// <summary>
        /// Создает и хранит экземпляр VM
        /// </summary>
        public ViewModelWS VM { get; set; } = new ViewModelWS();

        #endregion

        public WindowSlave()
        {
            InitializeComponent();
            VM.WSScheme = stationScheme;
        }

        #region Methods & Events Handlers

        /// <summary>
        /// Включает возможность перетаскивания окна с помощью мыши
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

        /// <summary>
        /// Анимация кнопки btn_WindowSlaveOpen, анимация шрифта кнопки + анимация ширины границы эллипса (нижний слой кнопки) 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Calculate_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_canCalcBtnClick)
            {
                _canCalcBtnClick = false;

                Button btn = (Button)sender;
                AnimationCtors ac = new AnimationCtors();

                DoubleAnimation daFontSize = ac.DblAnimationFromTo(true, btn.FontSize, btn.FontSize - 2, 10, true);
                DoubleAnimation daEllipseStroke = ac.DblAnimationFromTo(true, btn_CalculateEllipse.StrokeThickness, btn_CalculateEllipse.StrokeThickness + 1, 10, true);

                daEllipseStroke.Completed += (o, a) => _canCalcBtnClick = true;

                btn.BeginAnimation(Button.FontSizeProperty, daFontSize);
                btn_CalculateEllipse.BeginAnimation(Ellipse.StrokeThicknessProperty, daEllipseStroke);

            }
        }

        /// <summary>
        /// Включает анимацию сворачивания окна, переключает ключи видимости и возможности открытия
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Hide_Click(object sender, RoutedEventArgs e)
        {
            _canCalcBtnClick = false;
            WSOpenCloseAnimation(false);

            /*Переключает ключ состояния окна 
             * для возможности повторного открытия 
             * (открытие осуществляется нажатием кнопки на главном экране) */
            VisibleKey = false;

            /*Переключает ключ для возможности повторного открытия этого окна WindowSlave*/
            CanOpenKey = true;
        }

        /// <summary>
        /// Анимация открытия и закрытия окна
        /// </summary>
        /// <param name="key"> ключ, отвечающий за направления анимации открытие / закрытие окна</param>
        public void WSOpenCloseAnimation(bool key)
        {
            /*В текущем приложении анимация точно такая же, 
             * как и у основного окна. 
             * Метод не вынесен в отдельный поддерживающий класс,
             * так как, при желании, анимацию можно изменить для данного окна*/

            AnimationCtors ac = new AnimationCtors();

            /* создается объект анимации */
            DoubleAnimation da = ac.DblAnimationScale(key, Height, 0);

            da.EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut, Exponent = 5 };

            /* после окончания анимации скрыть окно */
            if (!key) da.Completed += WindowHideCompleted;

            if (key) da.Completed += (o, a) => VM.SchemeCreate();

            /* выполнение анимации */
            TranslateTransform tt = new TranslateTransform();
            win_Slave.RenderTransform = tt;
            tt.BeginAnimation(TranslateTransform.YProperty, da);

        }

        /// <summary>
        /// Обработчик события завершения анимации сворачивания окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowHideCompleted(object sender, EventArgs e)
        {
            Hide();
            stationScheme.Clear();
            VM.DataClear();
        }

        #endregion

        /// <summary>
        /// Обработчик события выбора стартовой и конечной точек.
        /// Подключен к событиям обоих combobox (cb_Startpoint и cb_Endpoint)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CB_DropDownClosed(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            VM.PointsHighlighting(cb.Name);
        }

        private void CB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            VM.PointsHighlighting(cb.Name);
        }

    }
}
