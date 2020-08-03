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
using Newtonsoft.Json;

namespace FunkyApp.Droid
{
    public class BookPrediction
    {
        [JsonProperty("probability")]
        public double probability { get; set; }

        [JsonProperty("tagId")]
        public string tagId { get; set; }

        [JsonProperty("tagName")]
        public string tagName { get; set; }

        [JsonProperty("boundingBox")]
        public AzureBoundingBox boundingBox { get; set; }
    }
    public class AzureBoundingBox
    {
        [JsonProperty("left")]
        public double left { get; set; }

        [JsonProperty("top")]
        public double top { get; set; }

        [JsonProperty("width")]
        public double width { get; set; }

        [JsonProperty("height")]
        public double height { get; set; }
    }
    public class BoundingBox
    {
        public int x { get; set; }
        public int y { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }
}