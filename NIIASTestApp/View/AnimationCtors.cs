using System;
using System.Windows.Media.Animation;

namespace NIIASTestApp.View
{
    /// <summary>
    /// Содержит методы, возвращающие готовые объекты анимаций 
    /// для сокращения кода при настройке анимаций
    /// </summary>
    public class AnimationCtors
    {
        /// <summary>
        /// Формируют объекты DoubleAnimation для Translate-параметров и Layout-параметров
        /// </summary>
        /// <param name="key">Определяет направление анимации: если true: from → scale  или false: scale→from </param>
        /// <param name="from">начальное значение параметра анимации, если = 1 → используется как максимальное значение параметра scale</param>
        /// <param name="scale">требуемый уровень масштабирования scale</param>
        /// <param name="speedRatio"> скорость анимации </param>
        /// <param name="autoReverse"> определяет, нужен ли автореверс анимации</param>
        /// <param name="beginTimeSpan">задержка начала выполнения анимации</param>
        /// <returns></returns>
        public DoubleAnimation DblAnimationScale(bool key, double from, double scale, double speedRatio = 1, bool autoReverse = false, double beginTimeSpan = 0)
        {
            double _from;
            double _to;

            /*если в качестве начального значения анимируемого паметра = 1, 
             * значит элемент управления будет применяться Transform - 
             * анимация | в противном случае анимация Layout - параметров*/
            if (from == 1)
            {
                _from = key ? from : scale;
                _to = key ? scale : from;
            }
            else
            {
                /*Определяет начальное значение анимируемого параметра на основании ключа Key 
                 * (прямое или обратное действие) и требуемого масштабирования scale*/
                _from = (scale == 0) ? (key ? from : 0) : from;

                /*Опередяет конечное значение анимируемого параметра по значениям key и scale*/
                _to = (scale == 0) ? (key ? 0 : from) : (key ? from * scale : from / scale);

            }

            DoubleAnimation da = new DoubleAnimation
            {
                From = _from,
                To = _to,
                SpeedRatio = speedRatio,
                AutoReverse = autoReverse,
                BeginTime = TimeSpan.FromSeconds(beginTimeSpan)
            };
            return da;
        }

        /// <summary>
        /// Формируют объекты DoubleAnimation для Layout-параметров.
        /// </summary>
        /// <param name="from"> начальное значение анимируемого параметра </param>
        /// <param name="to"> конечное значение анимируемого параметра</param>
        /// <returns></returns>
        public DoubleAnimation DblAnimationFromTo(bool key, double from, double to, double speedRatio, bool autoReverse, double beginTimeSpan = 0)
        {
            double _from = key ? from : to;
            double _to = key ? to : from;

            DoubleAnimation da = new DoubleAnimation
            {
                From = _from,
                To = _to,
                SpeedRatio = speedRatio,
                AutoReverse = autoReverse,
                BeginTime = TimeSpan.FromSeconds(beginTimeSpan)
            };
            return da;
        }

    }
}
