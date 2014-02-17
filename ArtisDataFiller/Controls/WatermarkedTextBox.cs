using System;
using System.Windows;
using System.Windows.Controls;

namespace Artis.ArtisDataFiller.Controls
{
    public class WatermarkedTextBox : TextBox
    {
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register(
            "Watermark", typeof (string), typeof (WatermarkedTextBox), new PropertyMetadata(default(string)));

        public string Watermark
        {
            get { return (string) GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        public WatermarkedTextBox()
        {
//            DefaultStyleKey = typeof (WatermarkedTextBox);
        }
    }
}
