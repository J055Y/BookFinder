using Android.Graphics;
using Android.OS;
using Android.Text.Method;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;
using Java.Util;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using static AndroidX.RecyclerView.Widget.RecyclerView;

namespace FunkyApp.Droid
{
    public class ImageDisplayFragment : Fragment
    {
        private const string TAG = "ImageDisplayFragment";

        private byte[] outputImageByteArray;
        private string objectPredictionContentString;
        private string OCRPredictionContentString;
        private string bookResultContentString;
        private IList<string> gBookResultContentStringArray;

        private ImageView outputImage;
        private TextView outputText;

        public static ImageDisplayFragment NewInstance()
        {
            var fragment = new ImageDisplayFragment {RetainInstance = true};
            return fragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (Arguments == null) return inflater.Inflate(Resource.Layout.fragment_display_image, container, false);
            outputImageByteArray = Arguments.GetByteArray("outputImage");
            objectPredictionContentString = Arguments.GetString("predictionContent");
            OCRPredictionContentString = Arguments.GetString("OCRContent");
            bookResultContentString = Arguments.GetString("bookContent");
            gBookResultContentStringArray = Arguments.GetStringArrayList("gBookStringArray");

            return inflater.Inflate(Resource.Layout.fragment_display_image, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            outputImage = (ImageView)view.FindViewById(Resource.Id.outputImage);
            outputText = (TextView)view.FindViewById(Resource.Id.outputText);
            outputText.MovementMethod = new ScrollingMovementMethod();

            DisplayOutput();
        }

        private static Bitmap ByteArrayToBitmap(byte[] byteArray)
        {
            var bitmap = BitmapFactory.DecodeByteArray(byteArray, 0, byteArray.Length);
            return bitmap;
        }

        private void DisplayOutput()
        {
            var bitmap = ByteArrayToBitmap(outputImageByteArray);
            outputImage.SetImageBitmap(bitmap);

            if (OCRPredictionContentString != null)
            {
                outputText.Text = bookResultContentString + "\n";

                try
                {
                    var books = gBookResultContentStringArray
                        .Select(JsonConvert.DeserializeObject<Book>)
                        .ToList();

                    var recyclerAdapter = new RecyclerAdapter(Context, books);
                    var recyclerView = View.FindViewById<RecyclerView>(Resource.Id.resultList);
                    recyclerView.SetAdapter(recyclerAdapter);

                    var layoutManager = new LinearLayoutManager(Context, LinearLayoutManager.Horizontal, false);
                    recyclerView.SetLayoutManager(layoutManager);
                } catch(Java.Lang.Exception)
                {
                    Log.Debug(TAG, "Bad things happened...");
                }
            }
            else
            {
                outputText.Text = "Failed to find text";
            }
        }
    }
}