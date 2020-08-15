using Android.Util;
using System;
using System.Collections.Generic;

namespace FunkyApp.Droid
{
    public class VolumeInfo
    {
        public string Title { get; set; }
        public IList<string> Authors { get; set; }
        public string Publisher { get; set; }

/*        private DateTime dPublishedDate;
        public string PublishedDate
        {
            get => dPublishedDate.Date.ToString("R");
            set
            {

                if (DateTime.TryParse(value, out dPublishedDate)) dPublishedDate = DateTime.Parse(value);
                else
                {
                    try
                    {
                        dPublishedDate = DateTime.Parse("01-01-" + value);
                    }
                    catch (Exception)
                    {
                        Log.Debug("VolumeInfo DateTime Format", "Could not parse date");
                    }
                }
            }
        }*/

        public string PublishedDate { get; set; }

        public string Description { get; set; }

        private string isbn;
        public string ISBN
        {
            get => isbn;
            set { isbn = value.Length == 13 ? value : "Invalid ISBN"; }
        }
        public IList<string> Categories { get; set; }
        public double AverageRating { get; set; }
        public int RatingsCount { get; set; }
        public string Thumbnail { get; set; }

        public override string ToString()
        {
            return
                "Title: " + Title + "\n" +
                "Authors: " + string.Join(",", Authors) + "\n" +
                "Publisher: " + Publisher + "\n" +
                "Publish Date: " + PublishedDate + "\n" +
                "Description: " + Description + "\n" +
                "ISBN: " + ISBN + "\n" +
                "Categories: " + string.Join(",", Categories) + "\n" +
                "Average Rating: " + AverageRating + "\n" +
                "Ratings Count: " + RatingsCount + "\n" +
                "Thumbnail: " + Thumbnail + "\n";
        }
    }
}