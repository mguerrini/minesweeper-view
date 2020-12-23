using Minesweeper.Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Minesweeper.Converters
{
    public class EnumToStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            if (!(value is GameStatusType))
                return null;

            GameStatusType d = (GameStatusType)value;

            switch (d)
            {
                case GameStatusType.GameStatus_Created:
                    return "Created";
                case GameStatusType.GameStatus_Playing:
                    return "Playing";
                case GameStatusType.GameStatus_Won:
                    return "Won";
                case GameStatusType.GameStatus_Lost:
                    return "Lost";
                default:
                    return "Created";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
