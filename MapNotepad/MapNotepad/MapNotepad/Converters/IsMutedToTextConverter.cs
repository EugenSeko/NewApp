﻿using System;
using System.Globalization;
using Xamarin.Forms;

namespace MapNotepad.Converters
{
    internal class IsMutedToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isMuted)
            {
                var result = isMuted ? "MOVE TO UNMUTE" : "MOVE TO MUTE";
                return result;
            }

            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}