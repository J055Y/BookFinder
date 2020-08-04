using Android.App;
using Android.Content;
using Android.Widget;
using Xamarin.Forms.Platform.Android;

namespace FunkyApp.Droid
{
    public class LoadingDialog
    {
        private readonly Activity activity;
        private AlertDialog dialog;
        private TextView text;

        public LoadingDialog(Context context)
        {
            activity = context.GetActivity();
        }

        public void StartLoadingDialog()
        {
            var builder = new AlertDialog.Builder(activity);
            var inflater = activity.LayoutInflater;
            var view = inflater.Inflate(Resource.Layout.loading_dialog, null);

            text = (TextView)view.FindViewById(Resource.Id.loadingText);
            
            builder.SetView(view);
            builder.SetCancelable(false);

            dialog = builder.Create();
            dialog.Show();
        }

        public void DismissDialog()
        {
            dialog.Dismiss();
            dialog.Dispose();
        }

        public string Text
        {
            get => text.Text;
            set => text.Text = value;
        }
    }
}