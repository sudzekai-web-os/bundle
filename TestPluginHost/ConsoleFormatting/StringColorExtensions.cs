namespace TestPluginHost.ConsoleFormatting
{
    /// <summary>
    /// Provides extension methods for applying ANSI colors to strings.
    /// </summary>
    public static class StringColorExtensions
    {
        private const string Reset = "\e[0m";

        /// <summary>
        /// Applies the specified foreground color to the string.
        /// </summary>
        /// <param name="value">
        /// The string to which the color is applied.
        /// </param>
        /// <param name="color">
        /// The foreground color to apply.
        /// </param>
        /// <returns>
        /// The string with the specified foreground color applied.
        /// </returns>
        public static string GetColoredString(this string value, FormatterColor color)
            => color switch
            {
                FormatterColor.Black => GetBlackString(value),
                FormatterColor.DarkRed => GetDarkRedString(value),
                FormatterColor.DarkGreen => GetDarkGreenString(value),
                FormatterColor.DarkYellow => GetDarkYellowString(value),
                FormatterColor.DarkBlue => GetDarkBlueString(value),
                FormatterColor.DarkMagenta => GetDarkMagentaString(value),
                FormatterColor.DarkCyan => GetDarkCyanString(value),
                FormatterColor.DarkGray => GetDarkGrayString(value),
                FormatterColor.Red => GetRedString(value),
                FormatterColor.Green => GetGreenString(value),
                FormatterColor.Yellow => GetYellowString(value),
                FormatterColor.Blue => GetBlueString(value),
                FormatterColor.Magenta => GetMagentaString(value),
                FormatterColor.Cyan => GetCyanString(value),
                FormatterColor.White => GetGrayString(value),
                _ => value
            };

        /// <summary>
        /// Applies the black foreground color to the string.
        /// </summary>
        /// <param name="value">The string to color.</param>
        /// <returns>The string with the black foreground color applied.</returns>
        public static string GetBlackString(this string value)
            => $"\e[30m{value}{Reset}";

        /// <summary>
        /// Applies the dark red foreground color to the string.
        /// </summary>
        /// <param name="value">The string to color.</param>
        /// <returns>The string with the dark red foreground color applied.</returns>
        public static string GetDarkRedString(this string value)
            => $"\e[31m{value}{Reset}";

        /// <summary>
        /// Applies the dark green foreground color to the string.
        /// </summary>
        /// <param name="value">The string to color.</param>
        /// <returns>The string with the dark green foreground color applied.</returns>
        public static string GetDarkGreenString(this string value)
            => $"\e[32m{value}{Reset}";

        /// <summary>
        /// Applies the dark yellow foreground color to the string.
        /// </summary>
        /// <param name="value">The string to color.</param>
        /// <returns>The string with the dark yellow foreground color applied.</returns>
        public static string GetDarkYellowString(this string value)
            => $"\e[33m{value}{Reset}";

        /// <summary>
        /// Applies the dark blue foreground color to the string.
        /// </summary>
        /// <param name="value">The string to color.</param>
        /// <returns>The string with the dark blue foreground color applied.</returns>
        public static string GetDarkBlueString(this string value)
            => $"\e[34m{value}{Reset}";

        /// <summary>
        /// Applies the dark magenta foreground color to the string.
        /// </summary>
        /// <param name="value">The string to color.</param>
        /// <returns>The string with the dark magenta foreground color applied.</returns>
        public static string GetDarkMagentaString(this string value)
            => $"\e[35m{value}{Reset}";

        /// <summary>
        /// Applies the dark cyan foreground color to the string.
        /// </summary>
        /// <param name="value">The string to color.</param>
        /// <returns>The string with the dark cyan foreground color applied.</returns>
        public static string GetDarkCyanString(this string value)
            => $"\e[36m{value}{Reset}";

        /// <summary>
        /// Applies the dark gray foreground color to the string.
        /// </summary>
        /// <param name="value">The string to color.</param>
        /// <returns>The string with the dark gray foreground color applied.</returns>
        public static string GetDarkGrayString(this string value)
            => $"\e[90m{value}{Reset}";

        /// <summary>
        /// Applies the red foreground color to the string.
        /// </summary>
        /// <param name="value">The string to color.</param>
        /// <returns>The string with the red foreground color applied.</returns>
        public static string GetRedString(this string value)
            => $"\e[91m{value}{Reset}";

        /// <summary>
        /// Applies the green foreground color to the string.
        /// </summary>
        /// <param name="value">The string to color.</param>
        /// <returns>The string with the green foreground color applied.</returns>
        public static string GetGreenString(this string value)
            => $"\e[92m{value}{Reset}";

        /// <summary>
        /// Applies the yellow foreground color to the string.
        /// </summary>
        /// <param name="value">The string to color.</param>
        /// <returns>The string with the yellow foreground color applied.</returns>
        public static string GetYellowString(this string value)
            => $"\e[93m{value}{Reset}";

        /// <summary>
        /// Applies the blue foreground color to the string.
        /// </summary>
        /// <param name="value">The string to color.</param>
        /// <returns>The string with the blue foreground color applied.</returns>
        public static string GetBlueString(this string value)
            => $"\e[94m{value}{Reset}";

        /// <summary>
        /// Applies the magenta foreground color to the string.
        /// </summary>
        /// <param name="value">The string to color.</param>
        /// <returns>The string with the magenta foreground color applied.</returns>
        public static string GetMagentaString(this string value)
            => $"\e[95m{value}{Reset}";

        /// <summary>
        /// Applies the cyan foreground color to the string.
        /// </summary>
        /// <param name="value">The string to color.</param>
        /// <returns>The string with the cyan foreground color applied.</returns>
        public static string GetCyanString(this string value)
            => $"\e[96m{value}{Reset}";

        /// <summary>
        /// Applies the gray foreground color to the string.
        /// </summary>
        /// <param name="value">The string to color.</param>
        /// <returns>The string with the gray foreground color applied.</returns>
        public static string GetGrayString(this string value)
            => $"\e[97m{value}{Reset}";

        /// <summary>
        /// Applies the specified background color to the string.
        /// </summary>
        /// <param name="value">
        /// The string to which the background color is applied.
        /// </param>
        /// <param name="color">
        /// The background color to apply.
        /// </param>
        /// <returns>
        /// The string with the specified background color applied.
        /// </returns>
        public static string GetColoredBackgroundString(
            this string value,
            FormatterColor color)
            => color switch
            {
                FormatterColor.Default => value,
                FormatterColor.Black => GetBlackBackgroundString(value),
                FormatterColor.DarkRed => GetDarkRedBackgroundString(value),
                FormatterColor.DarkGreen => GetDarkGreenBackgroundString(value),
                FormatterColor.DarkYellow => GetDarkYellowBackgroundString(value),
                FormatterColor.DarkBlue => GetDarkBlueBackgroundString(value),
                FormatterColor.DarkMagenta => GetDarkMagentaBackgroundString(value),
                FormatterColor.DarkCyan => GetDarkCyanBackgroundString(value),
                FormatterColor.DarkGray => GetDarkGrayBackgroundString(value),
                FormatterColor.Red => GetRedBackgroundString(value),
                FormatterColor.Green => GetGreenBackgroundString(value),
                FormatterColor.Yellow => GetYellowBackgroundString(value),
                FormatterColor.Blue => GetBlueBackgroundString(value),
                FormatterColor.Magenta => GetMagentaBackgroundString(value),
                FormatterColor.Cyan => GetCyanBackgroundString(value),
                FormatterColor.White => GetGrayBackgroundString(value),
                _ => value
            };

        /// <summary>
        /// Applies the black background color to the string.
        /// </summary>
        /// <param name="value">The string to color.</param>
        /// <returns>The string with the black background color applied.</returns>
        public static string GetBlackBackgroundString(this string value)
            => $"\e[40m{value}{Reset}";

        /// <summary>
        /// Applies the dark red background color to the string.
        /// </summary>
        /// <param name="value">The string to color.</param>
        /// <returns>The string with the dark red background color applied.</returns>
        public static string GetDarkRedBackgroundString(this string value)
            => $"\e[41m{value}{Reset}";

        /// <summary>
        /// Applies the dark green background color to the string.
        /// </summary>
        /// <param name="value">The string to color.</param>
        /// <returns>The string with the dark green background color applied.</returns>
        public static string GetDarkGreenBackgroundString(this string value)
            => $"\e[42m{value}{Reset}";

        /// <summary>
        /// Applies the dark yellow background color to the string.
        /// </summary>
        /// <param name="value">The string to color.</param>
        /// <returns>The string with the dark yellow background color applied.</returns>
        public static string GetDarkYellowBackgroundString(this string value)
            => $"\e[43m{value}{Reset}";

        /// <summary>
        /// Applies the dark blue background color to the string.
        /// </summary>
        /// <param name="value">The string to color.</param>
        /// <returns>The string with the dark blue background color applied.</returns>
        public static string GetDarkBlueBackgroundString(this string value)
            => $"\e[44m{value}{Reset}";

        /// <summary>
        /// Applies the dark magenta background color to the string.
        /// </summary>
        /// <param name="value">The string to color.</param>
        /// <returns>The string with the dark magenta background color applied.</returns>
        public static string GetDarkMagentaBackgroundString(this string value)
            => $"\e[45m{value}{Reset}";

        /// <summary>
        /// Applies the dark cyan background color to the string.
        /// </summary>
        /// <param name="value">The string to color.</param>
        /// <returns>The string with the dark cyan background color applied.</returns>
        public static string GetDarkCyanBackgroundString(this string value)
            => $"\e[46m{value}{Reset}";

        /// <summary>
        /// Applies the dark gray background color to the string.
        /// </summary>
        /// <param name="value">The string to color.</param>
        /// <returns>The string with the dark gray background color applied.</returns>
        public static string GetDarkGrayBackgroundString(this string value)
            => $"\e[100m{value}{Reset}";

        /// <summary>
        /// Applies the red background color to the string.
        /// </summary>
        /// <param name="value">The string to color.</param>
        /// <returns>The string with the red background color applied.</returns>
        public static string GetRedBackgroundString(this string value)
            => $"\e[101m{value}{Reset}";

        /// <summary>
        /// Applies the green background color to the string.
        /// </summary>
        /// <param name="value">The string to color.</param>
        /// <returns>The string with the green background color applied.</returns>
        public static string GetGreenBackgroundString(this string value)
            => $"\e[102m{value}{Reset}";

        /// <summary>
        /// Applies the yellow background color to the string.
        /// </summary>
        /// <param name="value">The string to color.</param>
        /// <returns>The string with the yellow background color applied.</returns>
        public static string GetYellowBackgroundString(this string value)
            => $"\e[103m{value}{Reset}";

        /// <summary>
        /// Applies the blue background color to the string.
        /// </summary>
        /// <param name="value">The string to color.</param>
        /// <returns>The string with the blue background color applied.</returns>
        public static string GetBlueBackgroundString(this string value)
            => $"\e[104m{value}{Reset}";

        /// <summary>
        /// Applies the magenta background color to the string.
        /// </summary>
        /// <param name="value">The string to color.</param>
        /// <returns>The string with the magenta background color applied.</returns>
        public static string GetMagentaBackgroundString(this string value)
            => $"\e[105m{value}{Reset}";

        /// <summary>
        /// Applies the cyan background color to the string.
        /// </summary>
        /// <param name="value">The string to color.</param>
        /// <returns>The string with the cyan background color applied.</returns>
        public static string GetCyanBackgroundString(this string value)
            => $"\e[106m{value}{Reset}";

        /// <summary>
        /// Applies the gray background color to the string.
        /// </summary>
        /// <param name="value">The string to color.</param>
        /// <returns>The string with the gray background color applied.</returns>
        public static string GetGrayBackgroundString(this string value)
            => $"\e[107m{value}{Reset}";
    }
}