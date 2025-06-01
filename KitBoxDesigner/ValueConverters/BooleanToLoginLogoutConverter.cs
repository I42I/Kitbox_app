// New file: KitBoxDesigner/ValueConverters/BooleanToLoginLogoutConverter.cs
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace KitBoxDesigner.ValueConverters
{
    public class BooleanToLoginLogoutConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isAdminLoggedIn)
            {
                return isAdminLoggedIn ? "Logout Admin" : "Login as Admin";
            }
            return "Login as Admin";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
