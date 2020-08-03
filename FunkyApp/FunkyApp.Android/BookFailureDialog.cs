using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Platform.Android;

namespace FunkyApp.Droid
{
    public class BookFailureDialog
    {
        private readonly Activity activity;
        private AlertDialog dialog;
        private Button button;

        public BookFailureDialog(Context context)
        {
            activity = context.GetActivity();
        }

        public void StartDialog()
        {
            var builder = new AlertDialog.Builder(activity);
            var inflater = activity.LayoutInflater;
            var view = inflater.Inflate(Resource.Layout.book_failure_dialog, null);

            button = (Button)view.FindViewById(Resource.Id.book_failure_button);
            button.Click += (sender, e) =>
            {
                dialog.Dismiss();
            };
            
            builder.SetView(view);
            builder.SetCancelable(false);

            dialog = builder.Create();
            dialog.Show();
        }
    }
}