using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Xamarin.Forms.Platform.Android;

namespace FunkyApp.Droid
{
    public class ErrorDialog
    {
        private readonly Activity activity;
        private AlertDialog dialog;
        private Button button;
        private readonly string errorString;

        public ErrorDialog(Context context, string e)
        {
            activity = context.GetActivity();
            errorString = e;
        }

        public void StartDialog()
        {
            var builder = new AlertDialog.Builder(activity);
            var inflater = activity.LayoutInflater;
            var view = inflater.Inflate(Resource.Layout.error_dialog, null);

            button = (Button)view.FindViewById(Resource.Id.errorButton);
            button.Click += (sender, e) =>
            {
                dialog.Dismiss();
            };

            var errorText = (TextView) view.FindViewById(Resource.Id.errorText);
            errorText.Text += errorString;
            
            builder.SetView(view);
            builder.SetCancelable(false);

            dialog = builder.Create();
            dialog.Show();
        }
    }
}