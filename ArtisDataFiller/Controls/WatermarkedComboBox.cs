using System.Windows;
using System.Windows.Controls;

namespace Artis.ArtisDataFiller.Controls
{
    /// <summary>
    /// ComboBox c watermak 
    /// </summary>
    public class WatermarkedComboBox : ComboBox
    {
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register(
            "Watermark", typeof (string), typeof (WatermarkedComboBox), new PropertyMetadata(default(string)));

        public string Watermark
        {
            get { return (string) GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }
    }
}
