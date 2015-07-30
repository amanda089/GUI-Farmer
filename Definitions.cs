using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FarmMonkey_GUI
{
    
    public class Housing
    {
        public string name;
        public uint uniqHousingId;
    }

    public class RowItem : INotifyPropertyChanged
    {

        private Housing FarmProperty;

        private string PlantProperty;

        private TimeSpan TimerProperty;

        private uint UniqIdProperty;

        public event PropertyChangedEventHandler PropertyChanged;

        public Housing Farm
        {
            get { return FarmProperty; }
            set
            {
                if (FarmProperty != value)
                {
                    FarmProperty = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string FarmName
        {
            get { if (string.IsNullOrEmpty(FarmProperty.name)) return "Unknown"; else return "[" + FarmProperty.uniqHousingId + "] " + FarmProperty.name; }
        }

        public string MoreInfo
        {
            get
            {
                string s = string.Empty;
                if (Timer.Days > 0) s += "{0:0} Days, ";
                if (Timer.Hours >= 0) s += "{1:00} Hours, ";
                if (Timer.Minutes >= 0) s += "{2:00} Minutes, ";
                if (Timer.Seconds >= 0) s += "{3:00} Seconds";
                return string.Format("Harvest-able in: " + s, Timer.Days, Timer.Hours, Timer.Minutes, Timer.Seconds);
            }
            private set { }
        }

        public string Plant
        {
            get { return PlantProperty; }
            set
            {
                if (value != PlantProperty)
                {
                    this.PlantProperty = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public TimeSpan Timer
        {
            get { return TimerProperty; }
            set
            {
                if (value != TimerProperty)
                {
                    this.TimerProperty = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("MoreInfo");
                    NotifyPropertyChanged("TimerString");
                }
            }
        }

        public string TimerString
        {
            get
            {
                string s = string.Empty;
                if (Timer.Days > 0) s += "{0:0}:";
                if (Timer.Hours >= 0) s += "{1:00}:";
                if (Timer.Minutes >= 0) s += "{2:00}:";
                if (Timer.Seconds >= 0) s += "{3:00}";
                return string.Format(s, Timer.Days, Timer.Hours, Timer.Minutes, Timer.Seconds);
            }
        }

        public uint UniqId
        {
            get { return UniqIdProperty; }
            set { UniqIdProperty = value; NotifyPropertyChanged(); }
        }

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
