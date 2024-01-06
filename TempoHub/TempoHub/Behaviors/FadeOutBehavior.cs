using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace TempoHub.Behaviors
{
    public class FadeOutBehavior
    {
        public static readonly DependencyProperty StartFadeOutProperty =
            DependencyProperty.RegisterAttached("StartFadeOut", typeof(bool), typeof(FadeOutBehavior), new PropertyMetadata(false, OnStartFadeOutChanged));
        public static readonly DependencyProperty FadeOutDurationProperty =
            DependencyProperty.RegisterAttached("FadeOutDuration", typeof(Duration), typeof(FadeOutBehavior), new PropertyMetadata(new Duration(TimeSpan.FromSeconds(5))));
        public static readonly DependencyProperty ActionOnCompleteProperty =
            DependencyProperty.Register("ActionOnComplete", typeof(Action), typeof(FadeOutBehavior));

        public static bool GetStartFadeOut(DependencyObject obj)
        {
            return (bool) obj.GetValue(StartFadeOutProperty);
        }

        public static void SetStartFadeOut(DependencyObject obj, bool value)
        {
            obj.SetValue(StartFadeOutProperty, value);
        }

        public static Duration GetFadeOutDuration(DependencyObject obj)
        {
            return (Duration) obj.GetValue(FadeOutDurationProperty);
        }

        public static void SetFadeOutDuration(DependencyObject obj, Duration value)
        {
            obj.SetValue(FadeOutDurationProperty, value);
        }

        public static Action GetActionOnComplete(DependencyObject obj)
        {
            return (Action) obj.GetValue(ActionOnCompleteProperty);
        }

        public static void SetActionOnComplete(DependencyObject obj, Action value)
        {
            obj.SetValue(ActionOnCompleteProperty, value);
        }

        private static void OnStartFadeOutChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue is bool start && start)
            {
                if(obj is UIElement element)
                {
                    var fadeOutAnimation = new DoubleAnimation
                    {
                        From = 1.0,
                        To = 0.0,
                        Duration = GetFadeOutDuration(element)
                    };

                    var actionOnEnd = GetActionOnComplete(element);

                    if(actionOnEnd != null)
                    {
                        fadeOutAnimation.Completed += new EventHandler((sender, e) => actionOnEnd());
                    }

                    element.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
                }
            }
        }
    }
}
