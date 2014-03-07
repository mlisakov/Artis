using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Artis.ArtisDataFiller.Controls
{
    /// <summary>
    /// Interaction logic for RatingItem.xaml
    /// </summary>
    public partial class RatingItem
    {
        /// <summary>
        /// Notifies when <see cref="State"/> has been changed.
        /// </summary>
        public event EventHandler<StarStateChangedEventArgs> StateChanged;


        public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
            "State", typeof (StarState), typeof (RatingItem), new PropertyMetadata(StarState.Off, OnStateChanged));

        public StarState State
        {
            get { return (StarState) GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        public static readonly DependencyProperty OnColorProperty = DependencyProperty.Register(
            "OnColor", typeof (Brush), typeof (RatingItem), new PropertyMetadata(Brushes.LightCoral, OnStarOnColorChanged ));

        public Brush OnColor
        {
            get { return (Brush) GetValue(OnColorProperty); }
            set { SetValue(OnColorProperty, value); }
        }

        public static readonly DependencyProperty OffColorProperty = DependencyProperty.Register(
            "OffColor", typeof(Brush), typeof(RatingItem), new PropertyMetadata(Brushes.White, OnStarOffColorChanged, CoerceOnStarOffColor));

        public Brush OffColor
        {
            get { return (Brush) GetValue(OffColorProperty); }
            set { SetValue(OffColorProperty, value); }
        }

        /// <summary>
        /// Gets whether or not <see cref="State"/> is <see cref="StarState.On"/>.
        /// </summary>
        public bool IsOn
        {
            get { return (State == StarState.On); }
        }

        /// <summary>
        /// Gets or sets the star fill brush.
        /// </summary>
        private Brush StarFill
        {
            get { return PathFill.Fill; }
            set { PathFill.Fill = value; }
        }

        public RatingItem()
        {
            InitializeComponent();
        }

         private static void OnStateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var star = obj as RatingItem;

            if (star != null)
            {
                var newState = (StarState)e.NewValue;

                star.StarFill = (newState == StarState.On) ? star.OnColor : star.OffColor;
                star.OnStateChanged(new StarStateChangedEventArgs(star.State));
            }
        }

         private static void OnStarOnColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var star = obj as RatingItem;

            //// if star is on, set fill color to on color
            if (star != null && star.IsOn)
            {
                star.StarFill = star.OnColor;
            }
        }

         private static void OnStarOffColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
         {
             var star = obj as RatingItem;

             if (star != null && !star.IsOn)
             {
                 star.StarFill = star.OffColor;
             }
         }

         private static object CoerceOnStarOffColor(DependencyObject obj, object value)
         {
             var star = obj as RatingItem;

             if (star != null)
             {
                 var brush = (Brush)value;

                 var colorBrush = brush as SolidColorBrush;
                 if (colorBrush != null && colorBrush.Color == Colors.Transparent)
                 {
                     return Brushes.White;
                 }

                 return brush;
             }

             return Brushes.White;
         }

         private void OnGridMouseEnter(object sender, MouseEventArgs e)
         {
             if (!IsOn)
             {
                 StarFill = OnColor;
             }
         }

         private void OnGridMouseLeave(object sender, MouseEventArgs e)
         {
             if (!IsOn)
             {
                 StarFill = OffColor;
             }
         }

         private void OnGridMouseUp(object sender, MouseButtonEventArgs e)
         {
             //// change state if left mouse button was released
             if (e.ChangedButton == MouseButton.Left)
             {
                 State = (State == StarState.On) ? StarState.Off : StarState.On;
             }
         }

         private void OnStateChanged(StarStateChangedEventArgs e)
         {
             if (StateChanged != null)
             {
                 StateChanged(this, e);
             }
         }
    }

    /// <summary>
    /// Event args for star state change.
    /// </summary>
    public class StarStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the state before change.
        /// </summary>
        public StarState OldState { get; private set; }

        /// <summary>
        /// GEts the state after change. Same as current state.
        /// </summary>
        public StarState NewState { get; private set; }

        public StarStateChangedEventArgs(StarState current)
        {
            if (current == StarState.On)
            {
                OldState = StarState.Off;
                NewState = StarState.On;
            }
            else
            {
                OldState = StarState.On;
                NewState = StarState.Off;
            }
        }
    }

    public enum StarState
    {
        /// <summary>
        /// Star is off
        /// </summary>
        Off = 0,

        /// <summary>
        /// Star is on
        /// </summary>
        On = 1
    }
}
