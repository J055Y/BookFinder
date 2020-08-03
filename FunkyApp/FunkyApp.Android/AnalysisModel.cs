using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace FunkyApp.Droid
{
    class AnalysisModel
    {
        public IList<Book> books { get; set; }
        public string requestId { get; set; }
        //public Desc description;
    }
}