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
using AndroidX.RecyclerView.Widget;
using Java.IO;
using Java.Net;

namespace FunkyApp.Droid
{
    class RecyclerAdapter : RecyclerView.Adapter
    {
        private const string TAG = "RecyclerAdapter";

        Context Context;
        private IList<Book> Books;

        public RecyclerAdapter(Context context, IList<Book> books)
        {
            Context = context;
            Books = books;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

/*        public View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;
            RecyclerAdapterViewHolder holder = null;

            if (view != null)
                holder = view.Tag as RecyclerAdapterViewHolder;

            if (holder == null)
            {
                holder = new RecyclerAdapterViewHolder(view);
                var inflater = Context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
                //replace with your item and your holder items
                //comment back in
                view = inflater.Inflate(Resource.Layout.book_preview, parent, false);
                //holder.Title = view.FindViewById<TextView>(Resource.Id.text);
                view.Tag = holder;
            }


            //fill in your items
            //holder.Title.Text = "new text here";

            return view;
        }*/

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = holder as RecyclerAdapterViewHolder;

            try
            {
                /*                var url = new URL(Books[position].VolumeInfo.Thumbnail);
                                var image = BitmapFactory.DecodeStream(url.OpenConnection().InputStream);*/

                //InputStream url = (InputStream)new URL(Books[position].VolumeInfo.Thumbnail).GetContent();

                //Bitmap image = Bitmap.CreateBitmap();

                new ImageLoadTask(Books[position].VolumeInfo.Thumbnail, viewHolder.BookImage).Execute();


                //viewHolder?.BookImage.SetImageBitmap(image);
            } catch (InvalidOperationException e)
            {
                Log.Debug(TAG, "Could not get thumnbail link. " + e.Message);
            }

            if (viewHolder != null) viewHolder.BookCaption.Text = Books[position].VolumeInfo.Title;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            Log.Debug(TAG, parent.Context.ToString());
            try
            {
                var bookPreview = LayoutInflater
                    .From(parent.Context)
                    .Inflate(Resource.Layout.book_preview, parent, false);

                var viewHolder = new RecyclerAdapterViewHolder(bookPreview);
                return viewHolder;
            } catch (Java.Lang.Exception)
            {
                Log.Debug(TAG, "Bad Things happened here...");
                return null;
            }
        }
        
        public override int ItemCount => Books.Count;
    }

    class RecyclerAdapterViewHolder : RecyclerView.ViewHolder
    {
        public ImageView BookImage { get; private set; }
        public TextView BookCaption { get; private set; }

        public RecyclerAdapterViewHolder (View view) : base (view)
        {
            try
            {
                BookImage = view.FindViewById<ImageView>(Resource.Id.bookPreviewImage);
                BookCaption = view.FindViewById<TextView>(Resource.Id.bookPreviewCaption);
            } catch (Java.Lang.Exception)
            {
                Log.Debug("RecyclerAdapterViewHolder", "Book Image or Book Caption are null.");
            }
        }
    }
}