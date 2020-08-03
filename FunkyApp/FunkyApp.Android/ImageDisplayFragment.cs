using Android.Graphics;
using Android.OS;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
namespace FunkyApp.Droid
{
    public class ImageDisplayFragment : Fragment
    {
        private const string TAG = "ImageDisplayFragment";

        private byte[] outputImageByteArray;
        private string objectPredictionContentString;
        private string OCRPredictionContentString;

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

            outputText.Text = OCRPredictionContentString;
        }
    }
}