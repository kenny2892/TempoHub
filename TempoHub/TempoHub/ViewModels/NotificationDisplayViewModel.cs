using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Media;

namespace TempoHub.ViewModels
{
    public class NotificationDisplayViewModel : PropertyChangedBase
    {
        private Brush background = Brushes.Transparent;
        public Brush Background

        {
            get { return background; }
            set
            {
                if(background != value)
                {
                    background = value;
                    OnPropertyChanged(nameof(Background));
                }
            }
        }
        private Visibility gridVisibility = Visibility.Collapsed;
        public Visibility GridVisibility

        {
            get { return gridVisibility; }
            set
            {
                if(gridVisibility != value)
                {
                    gridVisibility = value;
                    OnPropertyChanged(nameof(GridVisibility));
                }
            }
        }
        private string text = "";
        public string Text

        {
            get { return text; }
            set
            {
                if(text != value)
                {
                    text = value;
                    OnPropertyChanged(nameof(Text));
                }
            }
        }
        private bool startFadeOut = false;
        public bool StartFadeOut

        {
            get { return startFadeOut; }
            set
            {
                if(startFadeOut != value)
                {
                    startFadeOut = value;
                    OnPropertyChanged(nameof(StartFadeOut));
                }
            }
        }
        private Duration fadeOutDuration = TimeSpan.FromSeconds(5);
        public Duration FadeOutDuration
        {
            get { return fadeOutDuration; }
            set
            {
                if(fadeOutDuration != value)
                {
                    fadeOutDuration = value;
                    OnPropertyChanged(nameof(FadeOutDuration));
                }
            }
        }
        private Action onAnimationComplete = null;
        public Action OnAnimationComplete
        {
            get { return onAnimationComplete; }
            set
            {
                if(onAnimationComplete != value)
                {
                    onAnimationComplete = value;
                    OnPropertyChanged(nameof(OnAnimationComplete));
                }
            }
        }

        public void DisplayNotification(string text, Brush backgroundBrush, int secondsStayStill, int secondsToFadeOut)
        {
            Text = text;
            Background = backgroundBrush;
            GridVisibility = Visibility.Visible;
            FadeOutDuration = TimeSpan.FromSeconds(secondsToFadeOut);

            OnAnimationComplete = () =>
            {
                Text = "";
                Background = Brushes.Transparent;
                GridVisibility = Visibility.Collapsed;
                StartFadeOut = false;
            };

            Thread startAnimation = new Thread(() =>
            {
                Thread.Sleep(secondsStayStill * 1000);
                StartFadeOut = true;
            });

            startAnimation.Start();
        }
    }
}
