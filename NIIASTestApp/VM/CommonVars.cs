using NIIASTestApp.Model;

namespace NIIASTestApp.VM
{
    /// <summary>
    /// Поддерживающий статический класс для хранения разделяемых констант и переменных
    /// </summary>
    public static class CommonVars
    {
        #region Vars

        /*наименования цветов для заливки парков*/
        public const string parkColorGreen = "Зеленый";
        public const string parkColorRed = "Красный";
        public const string parkColorBlue = "Синий";
        public const string parkColorYellow = "Желтый";
        public const string parkColorOrange = "Оранжевый";
        public const string parkColorEmpty = "Нет заливки";
        public const string logFilePath = @"w:\Apps\log.TXT";
        #endregion

        #region Props
        /// <summary>
        /// Хранит экземпляр активной станции для использования окнами
        /// </summary>
        public static Station Station { get; set; }
        #endregion
    }
}
