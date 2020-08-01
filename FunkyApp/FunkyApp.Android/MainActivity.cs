using Android;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.AppCompat.App;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.IO;
using Android.Util;
using Android.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;
using System.Linq;
using Android.App;

namespace FunkyApp.Droid
{
    [Android.App.Activity(Label = "FunkyApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : AppCompatActivity, CameraFragment.ICameraFragmentListener
    {
/*        MainActivityListener mainActivityListener;
        public interface MainActivityListener
        {
            void OnResponse(string response);
        }
        public void OnResponse(string response)
        {
            mainActivityListener.OnResponse(response);
        }*/

        private const string TAG = "MainActivity";

        // Interface Call
        //CameraFragment.CameraFragmentListener onImageSetListener;

        public int CameraPermissionRequestCode = 1337;
        private Bundle instanceState;

        //private readonly string key = "a2a972dd8bcd4525828ad98b324e1ef4";
        //private readonly string endpoint = "https://computervisiondetectionservice.cognitiveservices.azure.com/";

        public string OCRPredictionContentString;
        public string ObjectPredictionContentString;
        public byte[] BitmapByteArray;
        

        protected override void OnCreate(Bundle savedInstanceState)
        {
            //TabLayoutResource = Resource.Layout.Tabbar;
            //ToolbarResource = Resource.Layout.Toolbar;

            instanceState = savedInstanceState;
            base.OnCreate(savedInstanceState);

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == (int) Permission.Granted)
            {
                StartProgram();
            }
            else // Camera Permission not granted
            {
                RequestPermissions(new[] {Manifest.Permission.Camera}, CameraPermissionRequestCode);
            }

            var fragment = new CameraFragment();
            SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.container, fragment)
                .Commit();

            //Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            //global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            //LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (requestCode == CameraPermissionRequestCode)
            {
                if ((grantResults.Length == 1) && (grantResults[0] == Permission.Granted))
                {
                    StartProgram();
                }
                else
                {
                    Toast.MakeText(this, "Camera permission has not been granted; the app will not function.", ToastLength.Short)
                        .Show();
                }
            }
        }

        private void StartProgram()
        {
            SetContentView(Resource.Layout.activity_camera);
            if (instanceState == null)
            {
                base.SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.container, CameraFragment.NewInstance())
                    .Commit();
            }
        }

        private async Task MakePredictionRequest(byte[] imageByteArray)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Prediction-key", "c468b269e71d4cefaf20efcffbd36bfa");

            string url = "https://uksouth.api.cognitive.microsoft.com/customvision/v3.0/Prediction/3bad1034-6c58-4a6c-a005-a0b622612392/detect/iterations/BookObjectDetection_v1/image";

            HttpResponseMessage response;

            using var content = new ByteArrayContent(imageByteArray);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response = await client.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();
            ObjectPredictionContentString = responseString;
            Log.Debug(TAG, "Prediction Request Made");
        }

        private async Task MakeOCRRequest(byte[] imageByteArray)
        {

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "a2a972dd8bcd4525828ad98b324e1ef4");

            string url = "https://computervisiondetectionservice.cognitiveservices.azure.com/vision/v2.1/ocr?language=unk&detectOrientation=true";

            HttpResponseMessage response;

            using ByteArrayContent content = new ByteArrayContent(imageByteArray);
            content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");
            response = await client.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();
            OCRPredictionContentString = responseString;
        }

        private byte[] CompressBitmapToBytes(Bitmap bitmap)
        {
            var quality = 100;

            MemoryStream bytes = new MemoryStream();
            bitmap.Compress(Bitmap.CompressFormat.Jpeg, quality, bytes);

            // Resize and compress image to fit Custom Vision guidlines
            while (bytes.ToArray().Length >= 4194304)
            {
                quality -= 10;
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, quality, bytes);
            }
            return bytes.ToArray();
        }

        public async void OnImageSet(Bitmap image)
        {
            
            var loadingDialog = new LoadingDialog(this);
            loadingDialog.StartLoadingDialog();

            //loadingDialog.Text = "Getting Image";
            byte[] imageByteArray = CompressBitmapToBytes(image);
            //loadingDialog.Text = "Detecting Books";
            await MakePredictionRequest(imageByteArray);
            //loadingDialog.Text = "Processing Image";
            var croppedImageByteArray = ProcessImage(imageByteArray);
            //loadingDialog.Text = "Detecting Text";
            await MakeOCRRequest(croppedImageByteArray);
            Log.Debug(TAG, OCRPredictionContentString);
            //loadingDialog.Text = "Creating Fragment";
            ImageDisplayFragment fragment = new ImageDisplayFragment();
            Bundle args = new Bundle();
            args.PutByteArray("outputImage", croppedImageByteArray);
            args.PutString("predictionContent", ObjectPredictionContentString);
            args.PutString("OCRContent", OCRPredictionContentString);
            fragment.Arguments = args;

            SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.container, fragment)
                .AddToBackStack(null)
                .Commit();
            
            loadingDialog.DismissDialog();
        }

/*        public static ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client =
              new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
              { Endpoint = endpoint };
            return client;
        }*/


        private byte[] ProcessImage(byte[] imageByteArray)
        {
            var bitmap = ByteArrayToBitmap(imageByteArray);
            var bookObject = ProcessPredictionJson(ObjectPredictionContentString);
            var croppedImage = CropImage(bitmap, bookObject.boundingBox);
            var imageBytes = BitmapToByteArray(croppedImage);

            return imageBytes;
        }

        /*        private void stuff()
                {
                    var bookObject = ProcessPredictionJson(ObjectPredictionContentString);
                    if (bookObject != null)
                    {
                        try
                        {
                            var croppedImage = CropImage(bitmap, bookObject.boundingBox);
                        }
                        catch (Exception e)
                        {
                            Log.Error(TAG, "Couldn't crop image. Error: " + e.Message);
                        }
                    }
                    else
                    {
                        Log.Error(TAG, "Book Object is null; unable to parse Json: " + ObjectPredictionContentString);
                    }
                }*/

        private static byte[] BitmapToByteArray(Bitmap bitmap)
        {
            using var stream = new MemoryStream();
            bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);
            return stream.ToArray();
        }

        private static Bitmap ByteArrayToBitmap(byte[] byteArray)
        {
            Bitmap bitmap = BitmapFactory.DecodeByteArray(byteArray, 0, byteArray.Length);
            return bitmap;
        }

        private static Bitmap CropImage(Bitmap image, AzureBoundingBox boundingBox)
        {
            var box = FormatBoundingBox(boundingBox, image);
            return Bitmap.CreateBitmap(image, box.x, box.y, box.width, box.height);
        }

        private static BoundingBox FormatBoundingBox(AzureBoundingBox box, Bitmap bitmap)
        {

            //double[] nums = new double[4];
            var formattedBox = new BoundingBox();

            // Calculate position of upper leftmost vertex
            formattedBox.x = (int)(box.left * bitmap.Width); // X
            formattedBox.y = (int)(box.top * bitmap.Height); // Y

            // Calculate width and height
            formattedBox.width = (int)(box.width * bitmap.Width); // Width
            formattedBox.height = (int)(box.height * bitmap.Height); // Height

            //int[] intNums = DoubleArrayToIntArray(nums);
            return formattedBox;
        }

        private static BookPrediction ProcessPredictionJson(string jsonString)
        {
            // Iterate through each prediction record
            // Deserialise Json as BookPrediction
            // Add BookPrediction object to List
            // Iterate through List of BookPredictions
            // return the one that has the highest probability

            var jsonResponse = JsonConvert.DeserializeObject<JObject>(jsonString);
            var jsonPredictions = jsonResponse.Value<JArray>("predictions");

            var bookPredictions = new List<BookPrediction>();

            try
            {
                bookPredictions.AddRange(from JObject prediction in jsonPredictions.Children() select prediction.ToObject<BookPrediction>());
            }
            catch (NullReferenceException)
            {
                return null;
            }

            var highestProbability = bookPredictions.OrderByDescending(item => item.probability).First();
            return highestProbability;
        }

    }
}