using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticTranslationDayZ.Model
{
    public class Languages : ObservableObject
    {
        private bool _isChecked;
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string DayZName { get; set; }

        public bool IsChecked
        {
            get => _isChecked;
            set => SetProperty(ref _isChecked, value);
        }
    }

}
