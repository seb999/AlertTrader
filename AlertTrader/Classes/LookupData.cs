using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AlertTrader.Classes
{

    public static class LookupData
    {
        public class Information
        {
            public DateTime Date { get; set; }
            public string DisplayInfo { get; set; }
            public Brush DisplayColor { get; set; }
        }

        public static ObservableCollection<Information> MessageList { get; set; }
    }
}
