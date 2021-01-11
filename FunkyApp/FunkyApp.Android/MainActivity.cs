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
using Java.Lang;

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
        private IList<string> gBookResultContentStringArray;
        
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
            if (requestCode != CameraPermissionRequestCode) return;
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

        private void StartProgram()
        {
            SetContentView(Resource.Layout.activity_camera);
            if (instanceState == null)
            {
                try
                {
                    SupportFragmentManager.BeginTransaction()
                        .Replace(Resource.Id.container, CameraFragment.NewInstance())
                        .Commit();
                }
                catch (IllegalArgumentException e)
                {
                    Log.Debug(TAG, "Could not access container view. " + e.Message);
                    throw;
                }
            }
        }

        private async Task MakePredictionRequest(byte[] imageByteArray)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Prediction-key", Key.PredictionKey);

            using var content = new ByteArrayContent(imageByteArray);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            var response = await client.PostAsync(Key.ModelUrl, content);
            var responseString = await response.Content.ReadAsStringAsync();
            objectPredictionContentString = responseString;
            Log.Debug(TAG, "Prediction Request Made");
        }

        private async Task MakeOCRRequest(byte[] imageByteArray)
        {

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Key.OcpKey);

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
            var client = GoodreadsClient.Create(Key.ApiKey, Key.Secret);
            var goodreadsBooks = await client.Books.Search(bookString);
            if (goodreadsBooks.List == null)
            {
                Log.Error(TAG, "Books is null");
                bookResultContentString = "Could not find book";
            }
            else
            {
                var goodreadsBook = goodreadsBooks.List.First().BestBook;
                bookResultContentString = goodreadsBook.AuthorName + " - " + goodreadsBook.Title;

                var gClient = new HttpClient();
                var result = await gClient.GetAsync(Key.GUrl + bookString + "&key=" + Key.GKey);
                var resultString = await result.Content.ReadAsStringAsync();
                //Log.Debug(TAG, "GResultString: " + resultString);
                
                var resultObject = JsonConvert.DeserializeObject<JObject>(resultString);
                var bookObjects = resultObject.Value<JArray>("items").Take(5);
                foreach (var b in bookObjects)
                {
                    Log.Debug(TAG, "Book Object" + b);
                }
                var books = bookObjects.Select(CreateBook).ToList();
                var jsonBooks = books.Select(JsonConvert.SerializeObject).ToList();
                gBookResultContentStringArray = jsonBooks;
            }
        }
        
        private static Book CreateBook(JToken bookObject)
        {
            var book = new Book();
            
            // Google Books Reference
            book.Id = bookObject["id"]?.ToString();
            book.GUrl = bookObject["selfLink"]?.ToString();
            
            // Volume Info
            var volumeInfo = bookObject["volumeInfo"];
            var volume = new VolumeInfo();

            if (volumeInfo != null)
            {
                volume.Title = volumeInfo["title"]?.ToString() ?? "??";

                var authorObjects = volumeInfo.Value<JArray>("authors");
                if (authorObjects != null)
                {
                    var authors = authorObjects.Select(authorObject => authorObject.ToString()).ToList();
                    if (authors.Count > 0) volume.Authors = authors;
                }

                volume.Publisher = volumeInfo["publisher"]?.ToString();
                volume.PublishedDate = volumeInfo["publishedDate"]?.ToString();
                volume.Description = volumeInfo["description"]?.ToString();

                var identifierObjects = volumeInfo.Value<JArray>("industryIdentifiers");
                foreach (var identifierObject in identifierObjects)
                {
                    if (identifierObject["type"].ToString().Equals("ISBN_13"))
                    {
                        volume.ISBN = identifierObject["identifier"]?.ToString();
                    }
                }

                var categoryObjects = volumeInfo.Value<JArray>("categories");
                if (categoryObjects != null)
                {
                    var categories = categoryObjects.Select(categoryObject => categoryObject.ToString()).ToList();
                    if (categories.Count > 0) volume.Categories = categories;
                }
                
                volume.AverageRating = double.Parse(volumeInfo["averageRating"]?.ToString() ?? "-1");
                volume.RatingsCount = int.Parse(volumeInfo["ratingsCount"]?.ToString() ?? "-1");
                volume.Thumbnail = volumeInfo["imageLinks"]?["thumbnail"]?.ToString() ?? "https://j055y.com/images/j055y.png";
            }

            book.VolumeInfo = volume;

            // Sale Info
            var saleInfo = bookObject["saleInfo"];
            var sale = new SaleInfo();

            if (saleInfo != null)
            {
                sale.Country = saleInfo["country"]?.ToString();

                var listPrice = new BookPrice(
                    double.Parse(saleInfo["retailPrice"]?["amount"]?.ToString() ?? "-1"),
                    saleInfo["listPrice"]?["currencyCode"]?.ToString() ?? "UNK");
                sale.ListPrice = listPrice;
                var retailPrice = new BookPrice(
                    double.Parse(saleInfo["retailPrice"]?["amount"]?.ToString() ?? "-1"),
                    saleInfo["retailPrice"]?["currencyCode"]?.ToString() ?? "UNK");
                sale.RetailPrice = retailPrice;

                sale.PurchaseLink = saleInfo["buyLink"]?.ToString();
            }

            book.SaleInfo = sale;

            return book;
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
                    args.PutStringArrayList("gBookStringArray", gBookResultContentStringArray);

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
                var errorDialog = new ErrorDialog(this, "Unable to find books in image");
                errorDialog.StartDialog();
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

        private BookPrediction ProcessPredictionJson(string jsonString)
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
                var errorDialog = new ErrorDialog(this, "No bounding boxes returned");
                errorDialog.StartDialog();
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