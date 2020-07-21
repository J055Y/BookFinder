using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace FunkyApp
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        //private Image j055yLogo;
        //private Label textLabel;
        //private Button clickyButton;

        public MainPage()
        {
            BackgroundColor = Color.AliceBlue;

            //BindingContext = new MainPageViewModel();

            //j055yLogo = new Image
            //{
            //    Source = "j055y.png"
            //};

            //clickyButton = new Button
            //{
            //    Text = "Click Me",
            //    TextColor = Color.White,
            //    BackgroundColor = Color.SlateGray,
            //    Margin = new Thickness(10)
            //};
            //clickyButton.SetBinding(Button.CommandProperty, "GiveUserMsgCommand");

            //textLabel = new Label
            //{
            //    FontSize = 24,
            //    Margin = new Thickness(10),
            //    FontFamily = "IBM Plex Sans"
            //};

            //var grid = new Grid
            //{
            //    Margin = new Thickness(10),

            //    ColumnDefinitions =
            //    {
            //        new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star)}
            //    },
            //    RowDefinitions =
            //    {
            //        new RowDefinition {Height = new GridLength(.4, GridUnitType.Star)},
            //        new RowDefinition {Height = new GridLength(1, GridUnitType.Star)},
            //        new RowDefinition {Height = new GridLength(.2, GridUnitType.Star)}
            //    }
            //};

            //grid.Children.Add(j055yLogo, 0, 0);
            //grid.Children.Add(textLabel, 0,1);
            //grid.Children.Add(clickyButton, 0, 2);

            //Content = grid;

            InitializeComponent();
        }

        private void Slider_OnValueChanged(object sender, ValueChangedEventArgs e)
        {
            HelloLabel.Text = $"Value is {e.NewValue:F2}";
        }

        private void Button_OnClicked(object sender, EventArgs e)
        {
            var val = $"{ValueSlider.Value:F2}".Substring(2);
            if (val[0].ToString().Equals("0"))
            {
                val = val[1].ToString();
            }
            ClickedLabel.Text = $"You Selected {val}%";
        }

        private void CameraButton_OnClicked(object sender, EventArgs e)
        {
            //var photo = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(
            //    new Plugin.Media.Abstractions.StoreCameraMediaOptions() { });

            //if (photo != null)
            //{
            //    PhotoImage.Source = ImageSource.FromStream((() => { return photo.GetStream() }));
            //}
        }
    }
}
