using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Threading;

namespace TempoHub.ViewModels
{
    public class LoadingDisplayViewModel : PropertyChangedBase
    {
        private Timer LoadingTimer { get; set; }
        private int dotCount { get; set; } = 0;
        public int DotCount
        {
            get { return dotCount; }
            set
            {
                if(dotCount != value)
                {
                    dotCount = value;
                    OnPropertyChanged(nameof(DotCount));
                }
            }
        }
        private Visibility progressTextVisibility = Visibility.Hidden;
        public Visibility ProgressTextVisibility
        {
            get { return progressTextVisibility; }
            set
            {
                if(progressTextVisibility != value)
                {
                    progressTextVisibility = value;
                    OnPropertyChanged(nameof(ProgressTextVisibility));
                }
            }
        }
        private string loadingText = "";
        public string LoadingText
        {
            get { return loadingText; }
            set
            {
                if(loadingText != value)
                {
                    loadingText = value;
                    OnPropertyChanged(nameof(LoadingText));
                }
            }
        }
        private double currentValue = 0.0;
        public double CurrentValue
        {
            get { return currentValue; }
            set
            {
                if(currentValue != value)
                {
                    currentValue = value;
                    OnPropertyChanged(nameof(CurrentValue));
                }
            }
        }
        private double minValue = 0.0;
        public double MinValue
        {
            get { return minValue; }
            set
            {
                if(minValue != value)
                {
                    minValue = value;
                    OnPropertyChanged(nameof(MinValue));
                }
            }
        }
        private double maxValue = 0.0;
        public double MaxValue
        {
            get { return maxValue; }
            set
            {
                if(maxValue != value)
                {
                    maxValue = value;
                    OnPropertyChanged(nameof(MaxValue));
                }
            }
        }

        private void IncrementLoadingLbl(object source, ElapsedEventArgs e)
        {
            LoadingText = "Loading" + new string('.', ++DotCount);

            if(DotCount >= 3)
            {
                DotCount = 0;
            }
        }

        public void Start(double min, double max, double curr)
        {
            MinValue = min;
            MaxValue = max;
            CurrentValue = curr;
            DotCount = 0;
            ProgressTextVisibility = Visibility.Visible;

            LoadingTimer = new Timer();
            LoadingTimer.Elapsed += new ElapsedEventHandler(IncrementLoadingLbl);
            LoadingTimer.Interval = 1000;
            LoadingTimer.Start();
        }

        public void Start()
        {
            MinValue = 0;
            MaxValue = 100;
            CurrentValue = 100;
            DotCount = 0;
            ProgressTextVisibility = Visibility.Hidden;

            LoadingTimer = new Timer();
            LoadingTimer.Elapsed += new ElapsedEventHandler(IncrementLoadingLbl);
            LoadingTimer.Interval = 1000;
            LoadingTimer.Start();
        }

        public void Stop()
        {
            if(LoadingTimer != null)
            {
                LoadingTimer.Stop();
            }
        }
    }
}
