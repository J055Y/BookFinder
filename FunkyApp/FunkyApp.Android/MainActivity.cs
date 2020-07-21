using Android;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.AppCompat.App;

namespace FunkyApp.Droid
{
    [Android.App.Activity(Label = "FunkyApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : AppCompatActivity
    {
        public int CameraPermissionRequestCode = 1337;
        private Bundle instanceState;

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
                    .Replace(Resource.Id.container, CameraFragment.newInstance())
                    .Commit();
            }
        }

        //public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
            //{
            //    Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            //    base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            //}
        }
}