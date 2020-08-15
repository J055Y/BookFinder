using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Java.Net;

namespace FunkyApp.Droid
{
    class ImageLoadTask : AsyncTask<Java.Lang.Void, Java.Lang.Void, Bitmap>
    {
        private const string TAG = "ImageLoadTask";

        private string link;
        private ImageView imageView;

        public ImageLoadTask (string linkToImage, ImageView mImageView)
        {
            link = linkToImage;
            imageView = mImageView;
        }

        protected override Bitmap RunInBackground(params Java.Lang.Void[] @params)
        {
            try
            {
                var url = new URL(link);
                HttpURLConnection connection = (HttpURLConnection)url.OpenConnection();
                connection.DoInput = true;
                connection.Connect();

                Stream input = connection.InputStream;
                var image = BitmapFactory.DecodeStream(input);
                return image;
            } catch (Java.Lang.Exception e)
            {
                e.PrintStackTrace();
            }
            return null;
        }

        protected override void OnPostExecute(Bitmap result)
        {
            base.OnPostExecute(result);
            imageView.SetImageBitmap(result);
        }
    }
}