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
using Goodreads;

namespace FunkyApp.Droid
{
    [Android.App.Activity(Label = "FunkyApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : AppCompatActivity, CameraFragment.ICameraFragmentListener
    {
        private const string TAG = "MainActivity";
        private const int CameraPermissionRequestCode = 1337;
        private Bundle instanceState;

        private string OCRPredictionContentString;
        private string objectPredictionContentString;
        private string bookResultContentString;
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
                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.container, CameraFragment.NewInstance())
                    .Commit();
            }
        }

        private async Task MakePredictionRequest(byte[] imageByteArray)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Prediction-key", "c468b269e71d4cefaf20efcffbd36bfa");

            const string url = "https://uksouth.api.cognitive.microsoft.com/customvision/v3.0/Prediction/3bad1034-6c58-4a6c-a005-a0b622612392/detect/iterations/BookModel_v2/image";

            using var content = new ByteArrayContent(imageByteArray);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            var response = await client.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();
            objectPredictionContentString = responseString;
            Log.Debug(TAG, "Prediction Request Made");
        }

        private async Task MakeOCRRequest(byte[] imageByteArray)
        {

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "a2a972dd8bcd4525828ad98b324e1ef4");

            const string url = "https://computervisiondetectionservice.cognitiveservices.azure.com/vision/v2.1/ocr?language=en&detectOrientation=true";

            using var content = new ByteArrayContent(imageByteArray);
            content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");
            var response = await client.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();
            OCRPredictionContentString = responseString;
        }

        private async Task MakeBookRequest(string bookString)
        {
            // const string key = "AIzaSyCzGOmxWYQzWDLRQEIzv1IRDQ9sGz7U44c";
            // const string url = "https://www.googleapis.com/books/v1/volumes?q=";

            const string apiKey = "ZI5z6LNTbugq3mYjL1WGww";
            const string secret = "e0W9GHLfPEklbFpWFp4XWXX0WslRkQ0YV7zACcDkpxQ";

            var client = GoodreadsClient.Create(apiKey, secret);
            var books = await client.Books.Search(bookString);
            if (books.List == null)
            {
                Log.Error(TAG, "Books is null");
                bookResultContentString = "Could not find book";
            }
            else
            {
                Log.Debug(TAG, "Book String: " + bookString);
                Log.Debug(TAG, "Book Response Object: " + books);
                Log.Debug(TAG, "Book Response Pagination: " + books.Pagination);
                Log.Debug(TAG, "Book Response Pagination Total Items: " + books.Pagination.TotalItems);
                Log.Debug(TAG, "Book Response Pagination Start: " + books.Pagination.Start);
                Log.Debug(TAG, "Book Response Pagination End: " + books.Pagination.End);
                Log.Debug(TAG, "Book Response List Count: " + books.List.Count);

                var book = books.List.First().BestBook;
                foreach (var bookWork in books.List)
                {
                    Log.Debug(TAG, bookWork.ToString());
                    Log.Debug(TAG, "Book Work Id: " + bookWork.Id);
                    Log.Debug(TAG, "Best Book: " + bookWork.BestBook.Title);
                }

                bookResultContentString = book.AuthorName + " - " + book.Title;

                // var oneBook = await client.Books.GetByTitle("After The Quake", "Haruki Murakami");
                // if (oneBook != null)
                // {
                //     Log.Debug(TAG, "One Book Title: " + oneBook.Title);
                // }
            }

            // try
            // {
            //     foreach (var book in books.List)
            //     {
            //         bookResultContentString += book.OriginalTitle;
            //     }
            // }
            // catch (NullReferenceException e)
            // {
            //     Log.Error(TAG, "Book list is empty (null). " + e.Message);
            // }
        }

        private static byte[] CompressBitmapToBytes(Bitmap bitmap)
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
            //var textView = (TextView)loadingDialog.FindViewById(Resource.Id.loadingText);
            loadingDialog.StartLoadingDialog();

            loadingDialog.Text= "Getting Image...";
            var imageByteArray = CompressBitmapToBytes(image);
            loadingDialog.Text = "Detecting Books...";
            try
            {
                await MakePredictionRequest(imageByteArray);
            }
            catch (Javax.Net.Ssl.SSLException e)
            {
                loadingDialog.DismissDialog();
                var errorDialog = new ErrorDialog(this, e.Message);
                errorDialog.StartDialog();
            }
            loadingDialog.Text = "Processing Image...";
            
            
            var croppedImageByteArray = ProcessImage(imageByteArray);
            if (croppedImageByteArray != null)
            {
                loadingDialog.Text = "Detecting Text...";
                await MakeOCRRequest(croppedImageByteArray);
                Log.Debug(TAG, OCRPredictionContentString);
                
                var bookString = ProcessOCRJson(OCRPredictionContentString);
                if (bookString != null)
                {
                    Log.Debug(TAG, "Book String: " + bookString);
                    loadingDialog.Text = "Searching Books...";
                    await MakeBookRequest(bookString);
                
                    loadingDialog.Text = "Creating Fragment...";
                    var fragment = new ImageDisplayFragment();
                    var args = new Bundle();
                    args.PutByteArray("outputImage", croppedImageByteArray);
                    args.PutString("predictionContent", objectPredictionContentString);
                    args.PutString("OCRContent", bookString);
                    args.PutString("bookContent", bookResultContentString);
                
                    fragment.Arguments = args;

                    SupportFragmentManager.BeginTransaction()
                        .Replace(Resource.Id.container, fragment)
                        .AddToBackStack(null)
                        .Commit();
                    loadingDialog.DismissDialog();
                }
                else
                {
                    loadingDialog.DismissDialog();
                    var errorDialog = new ErrorDialog(this, "Book String is Null");
                    errorDialog.StartDialog();
                }
            }
            else
            {
                loadingDialog.DismissDialog();
                var failureDialog = new BookFailureDialog(this);
                failureDialog.StartDialog();
            }
            
        }

        private byte[] ProcessImage(byte[] imageByteArray)
        {
            var bitmap = ByteArrayToBitmap(imageByteArray);
            var bookObject = ProcessPredictionJson(objectPredictionContentString);
            if (bookObject == null) return null;
            var croppedImage = CropImage(bitmap, bookObject.boundingBox);
            var imageBytes = BitmapToByteArray(croppedImage);
            return imageBytes;
        }

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
            return highestProbability.probability < 0.6 ? null : highestProbability;
        }

        private static string ProcessOCRJson(string jsonString)
        {
            var jObject = JObject.Parse(jsonString);

            try
            {
                var lines = (JArray) jObject["regions"][0]["lines"];

                IList<JObject> wordContainers = lines.Select(c => (JObject) c).ToList();
                var wordObjectArrays = wordContainers.Select(wordContainer => wordContainer["words"]);
                IList<string> textCollection = 
                    (from wordObjectArray in wordObjectArrays from wordObject in wordObjectArray
                        .Select(wordObject => (JObject) wordObject) select wordObject["text"]
                        .ToString()).ToList();

                var outString = "";
                foreach (var word in textCollection)
                {
                    outString += word;
                    if (!word.Equals(textCollection.Last()))
                    {
                        outString += " ";
                    }
                }

                Log.Debug(TAG, "Book String: " + outString);
                return outString;
            }
            catch (NullReferenceException e)
            {
                Log.Error(TAG, "Could not find text object in OCR String. " + e.Message);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Log.Error(TAG, "Lines Array is empty. " + e.Message);
            }
            
            return null;
        }

    }
}