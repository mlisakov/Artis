using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Artis.ArtisDataFiller.Controls
{
    /// <summary>
    /// Interaction logic for Rating.xaml
    /// </summary>
    public partial class Rating
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof (int), typeof (Rating), new PropertyMetadata(default(int), OnValueChanged, CoerceValueValue));

        public int Value
        {
            get { return (int) GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
            "Maximum", typeof (int), typeof (Rating), new PropertyMetadata(5));

        public int Maximum
        {
            get { return (int) GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
            "Minimum", typeof (int), typeof (Rating), new PropertyMetadata(0));

        public int Minimum
        {
            get { return (int) GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty StarOnColorProperty = DependencyProperty.Register(
            "StarOnColor", typeof(Brush), typeof(Rating), new PropertyMetadata(Brushes.LightCoral, OnStarOnColorChanged));

        public Brush StarOnColor
        {
            get { return (Brush) GetValue(StarOnColorProperty); }
            set { SetValue(StarOnColorProperty, value); }
        }

        public static readonly DependencyProperty StarOffColorProperty = DependencyProperty.Register(
            "StarOffColor", typeof (Brush), typeof (Rating),
            new PropertyMetadata(Brushes.White, OnStarOffColorChanged));

        public Brush StarOffColor
        {
            get { return (Brush) GetValue(StarOffColorProperty); }
            set { SetValue(StarOffColorProperty, value); }
        }

                /// <summary>
        /// Notifies when rating has been changed.
        /// </summary>
        public event EventHandler<RatingChangedEventArgs> RatingChanged;






        public Rating()
        {
            InitializeComponent();
        }

                private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var rating = obj as Rating;

            if (rating != null)
            {
                rating.OnRatingChanged();
            }
        }

        private void Rating_OnLoaded(object sender, RoutedEventArgs e)
        {
                    InitializeStars();
        }

        private void InitializeStars()
        {
            StarsStackPanel.Children.Clear();

            int value = 1;

            for (int i = 0; i < Maximum; i++)
            {
                var star = new RatingItem {OnColor = StarOnColor, OffColor = StarOffColor, Tag = value};
                star.StateChanged += star_StateChanged;
                star.MouseEnter += star_MouseEnter;
                star.MouseLeave += star_MouseLeave;

                value++;

                
                StarsStackPanel.Children.Insert(i, star);
            }
        }

        private static object CoerceValueValue(DependencyObject obj, object value)
        {
            var rating = (Rating) obj;

            var current = (int) value;

            if (current < rating.Minimum)
            {
                current = rating.Minimum;
            }

            if (current > rating.Maximum)
            {
                current = rating.Maximum;
            }

            return current;
        }

                private static void OnStarOffColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var rating = obj as Rating;

            if (rating != null)
            {
                var offColor = (Brush)e.NewValue;

                foreach (RatingItem star in rating.StarsStackPanel.Children)
                {
                    star.OffColor = offColor;
                }
            }
        }

        private static void OnStarOnColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var rating = obj as Rating;

            if (rating != null)
            {
                var onColor = (Brush)e.NewValue;

                foreach (RatingItem star in rating.StarsStackPanel.Children)
                {
                    star.OnColor = onColor;
                }
            }
        }

        
        private void star_MouseLeave(object sender, MouseEventArgs e)
        {
            var star = (RatingItem)sender;

            if (!star.IsOn)
            {
                var current = (int)star.Tag;

                foreach (RatingItem str in StarsStackPanel.Children)
                {
                    DisableStateChange(str);

                    var value = (int)str.Tag;

                    if (value < current && value > Value)
                    {
                        str.State = StarState.Off;
                    }

                    EnableStateChange(str);
                }
            }
        }

        private void star_MouseEnter(object sender, MouseEventArgs e)
        {
            var star = (RatingItem)sender;

            if (!star.IsOn)
            {
                var current = (int)star.Tag;

                foreach (RatingItem str in StarsStackPanel.Children)
                {
                    DisableStateChange(str);

                    var value = (int)str.Tag;

                    if (value < current)
                    {
                        str.State = StarState.On;
                    }

                    EnableStateChange(str);
                }
            }
        }

                private void EnableStateChange(RatingItem str)
        {
            str.StateChanged += star_StateChanged;
        }

                private void DisableStateChange(RatingItem str)
        {
            str.StateChanged -= star_StateChanged;
        }

        private void OnRatingChanged()
        {
            if (RatingChanged != null)
            {
                RatingChanged(this, new RatingChangedEventArgs(Value));
            }
        }

        
        private void star_StateChanged(object sender, StarStateChangedEventArgs e)
        {
            var star = (RatingItem)sender;

            var current = (int)star.Tag;

            bool reset = (current < Value);

            foreach (RatingItem str in StarsStackPanel.Children)
            {
                var value = (int)str.Tag;

                DisableStateChange(str);

                if (value < current)
                {
                    str.State = StarState.On;
                }
                else if (value > current)
                {
                    str.State = StarState.Off;
                }
                else if (value == current && reset)
                {
                    str.State = StarState.On;
                }

                EnableStateChange(str);
            }

            Value = current;
        }
    }

    /// <summary>
    /// Event arguments for the rating change.
    /// </summary>
    public class RatingChangedEventArgs
    {
        /// <summary>
        /// Gets the value of the rating.
        /// </summary>
        public int Value { get; private set; }

        public RatingChangedEventArgs(int value)
        {
            Value = value;
        }
    }
}
