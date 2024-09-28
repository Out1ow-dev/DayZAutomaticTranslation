using AutomaticTranslationDayZ.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticTranslationDayZ.Model
{
    public class TranslationResponse
    {
        public ResponseData responseData { get; set; }
        public string responseDetails { get; set; }
        public int responseStatus { get; set; }
    }

}
