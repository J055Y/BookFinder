using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Hardware.Camera2;
using Android.Hardware.Camera2.Params;

using Java.Lang;
using Java.Util;
using Java.Util.Concurrent;
using IOException = Java.IO.IOException;
using Math = Java.Lang.Math;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using DialogFragment = AndroidX.Fragment.App.DialogFragment;
using Fragment = AndroidX.Fragment.App.Fragment;
using Android.Support.Design.Widget;

namespace FunkyApp.Droid
{
    public class CameraFragment : Fragment, View.IOnClickListener
	{
		private ICameraFragmentListener cameraFragmentListener;

		public interface ICameraFragmentListener
		{
			public void OnImageSet(Bitmap image);
		}

		private const string TAG = "CameraFragment";
		
		private readonly SparseIntArray ORIENTATIONS = new SparseIntArray();
		
		//Button to capture frame
		private Button buttonCapture;

		// TextureView for Camera Preview
		public TextureView TextureView;
		
		public CameraDevice CameraDevice;
		public CameraCaptureSession PreviewSession;

		public readonly Semaphore CameraOpenCloseLock = new Semaphore(1);

		// Called when the CameraDevice changes state
		private readonly CameraStateCallback stateListener;
		// Handles several lifecycle events of a TextureView
		private readonly SurfaceTextureListener surfaceTextureListener;
		
		private CaptureRequest.Builder previewBuilder;

		private Size previewSize;

		private HandlerThread backgroundThread;
		private Handler backgroundHandler;

		public CameraFragment()
		{
			ORIENTATIONS.Append((int)SurfaceOrientation.Rotation0, 90);
			ORIENTATIONS.Append((int)SurfaceOrientation.Rotation90, 0);
			ORIENTATIONS.Append((int)SurfaceOrientation.Rotation180, 270);
			ORIENTATIONS.Append((int)SurfaceOrientation.Rotation270, 180);
			surfaceTextureListener = new SurfaceTextureListener(this);
			stateListener = new CameraStateCallback(this);
		}

		public static CameraFragment NewInstance()
		{
			var fragment = new CameraFragment();
			fragment.RetainInstance = true;
			return fragment;
		}

		private static int GreatestCommonDivisor(int width, int height)
		{
			while (true)
			{
				if (height == 0)
					return width;

				var width1 = width;
				width = height;
				height = width1 % height;
			}
		}

		private static Size ChooseOptimalSize(IReadOnlyList<Size> choices, int width, int height, TextureView textureView) // Need to scale preview size to device
        {
	        var aspectRatio = GreatestCommonDivisor(textureView.Width, textureView.Height);
            //var bigEnough = choices.Where(option => option.Height == option.Width * h / w && option.Width >= width && option.Height >= height).ToList();
            var goodResolutions = choices.Where(option => GreatestCommonDivisor(option.Width, option.Height) == aspectRatio).ToList();
            
            if (goodResolutions.Count > 0)
                return (Size)Collections.Min(goodResolutions, new CompareSizesByArea());
            else
            {
                Log.Error(TAG, "Couldn't find any suitable preview size");
                return choices[0];
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			return inflater.Inflate(Resource.Layout.fragment_camera2_video, container, false);
		}

		public override void OnViewCreated(View view, Bundle savedInstanceState)
		{
			TextureView = (TextureView)view.FindViewById(Resource.Id.texture);
			buttonCapture = (Button)view.FindViewById(Resource.Id.capture);
			buttonCapture.SetOnClickListener(this);
		}

		public override void OnResume()
		{
			base.OnResume();
			StartBackgroundThread();
			if (TextureView.IsAvailable)
				OpenCamera(TextureView.Width, TextureView.Height);
			else
				TextureView.SurfaceTextureListener = surfaceTextureListener;
		}

		public override void OnPause()
		{
			CloseCamera();
			StopBackgroundThread();
			base.OnPause();
		}

        private void StartBackgroundThread()
        {
            backgroundThread = new HandlerThread("CameraBackground");
            backgroundThread.Start();
            backgroundHandler = new Handler(backgroundThread.Looper);
        }

        private void StopBackgroundThread()
        {
            backgroundThread.QuitSafely();
            try
            {
                backgroundThread.Join();
                backgroundThread = null;
                backgroundHandler = null;
            }
            catch (InterruptedException e)
            {
                e.PrintStackTrace();
            }
        }

        public void OnClick(View view)
		{
            switch (view.Id)
            {
				case Resource.Id.capture:
					var bitmap = CaptureStillImage();

					if (bitmap != null)
					{
						Snackbar.Make(View, "Bitmap Captured", Snackbar.LengthShort)
							.Show();
					}
					else
					{
						Snackbar.Make(View, "Bitmap Not Captured", Snackbar.LengthShort)
							   .Show();
					}
					cameraFragmentListener?.OnImageSet(bitmap);
					break;
                default:
                    break;
            }
        }

        private Bitmap CaptureStillImage()
        {
			try
			{
				Log.Debug(TAG, "TextureView Width: " + TextureView.Width +
				               "\n | TextureView Height: " + TextureView.Height);
				var frame = Bitmap.CreateBitmap(TextureView.Width, TextureView.Height, Bitmap.Config.Argb8888);
				return(TextureView.GetBitmap(frame));
			}
			catch (IllegalArgumentException e)
			{
				e.PrintStackTrace();
				return null;
			}
			catch (IllegalStateException e)
            {
				e.PrintStackTrace();
				return null;
            }
		}

        //Tries to open a CameraDevice
        public void OpenCamera(int width, int height)
		{
			if (null == Activity || Activity.IsFinishing)
				return;

			var manager = (CameraManager)Activity.GetSystemService(Context.CameraService);
			try
			{
				if (!CameraOpenCloseLock.TryAcquire(2500, TimeUnit.Milliseconds))
                    throw new RuntimeException("Time out waiting to lock camera opening.");
                var cameraId = manager.GetCameraIdList()[0];
                var characteristics = manager.GetCameraCharacteristics(cameraId);
                var map = (StreamConfigurationMap)characteristics.Get(CameraCharacteristics.ScalerStreamConfigurationMap);
                // var largest = (Size)Collections.Max(Arrays.AsList(map.GetOutputSizes((int)ImageFormatType.Yuv420888)),
                //     new CompareSizesByArea());

				previewSize = ChooseOptimalSize(map.GetOutputSizes(Class.FromType(typeof(SurfaceTexture))), width, height, TextureView);
				Log.Debug(TAG, "Preview Width: " + previewSize.Width +
				               "\n | Preview Height: " + previewSize.Height);
				ConfigureTransform(width, height);
                manager.OpenCamera(cameraId, stateListener, null);
			}
			catch (CameraAccessException)
			{
				Toast.MakeText(Activity, "Cannot access the camera.", ToastLength.Short).Show();
				Activity.Finish();
			}
			catch (NullPointerException)
			{
				var dialog = new ErrorDialog();
				dialog.Show(Activity.SupportFragmentManager, "dialog");
			}
			catch (InterruptedException)
			{
				throw new RuntimeException("Interrupted while trying to lock camera opening.");
			}
		}
        
        private void CloseCamera()
        {
	        try
	        {
		        CameraOpenCloseLock.Acquire();
		        if (null == CameraDevice) return;
		        CameraDevice.Close();
		        CameraDevice = null;
	        }
	        catch (InterruptedException)
	        {
		        throw new RuntimeException("Interrupted while trying to lock camera closing.");
	        }
	        finally
	        {
		        CameraOpenCloseLock.Release();
	        }
        }

		//Start the camera preview
		public void StartPreview()
		{
			if (null == CameraDevice || !TextureView.IsAvailable || null == previewSize)
				return;

			try
			{
				SurfaceTexture texture = TextureView.SurfaceTexture;
				texture.SetDefaultBufferSize(previewSize.Width, previewSize.Height);

				previewBuilder = CameraDevice.CreateCaptureRequest(CameraTemplate.Record);

				var surfaces = new List<Surface>();
				var previewSurface = new Surface(texture);
				surfaces.Add(previewSurface);
				previewBuilder.AddTarget(previewSurface);

				CameraDevice.CreateCaptureSession(surfaces, new PreviewCaptureStateCallback(this), backgroundHandler);

			}
			catch (CameraAccessException e)
			{
				e.PrintStackTrace();
			}
			catch (IOException e)
			{
				e.PrintStackTrace();
			}
		}

		//Update the preview
		public void UpdatePreview()
		{
			if (null == CameraDevice)
				return;

			try
			{
				var thread = new HandlerThread("CameraPreview");
				thread.Start();
				PreviewSession.SetRepeatingRequest(previewBuilder.Build(), null, backgroundHandler);
			}
			catch (CameraAccessException e)
			{
				e.PrintStackTrace();
			}
		}

		//Configures the necessary matrix transformation to apply to the textureView
		public void ConfigureTransform(int viewWidth, int viewHeight)
		{
			if (null == Activity || null == previewSize || null == TextureView)
				return;

			var rotation = (int)Activity.WindowManager.DefaultDisplay.Rotation;
			var matrix = new Matrix();
			var viewRect = new RectF(0, 0, viewWidth, viewHeight);
			var bufferRect = new RectF(0, 0, previewSize.Height, previewSize.Width);
			var centerX = viewRect.CenterX();
			var centerY = viewRect.CenterY();
			switch (rotation)
			{
				case (int)SurfaceOrientation.Rotation90:
				case (int)SurfaceOrientation.Rotation270:
				{
					bufferRect.Offset((centerX - bufferRect.CenterX()), (centerY - bufferRect.CenterY()));
					matrix.SetRectToRect(viewRect, bufferRect, Matrix.ScaleToFit.Fill);
					var scale = System.Math.Max(
						(float)viewHeight / previewSize.Height,
						(float)viewHeight / previewSize.Width);
					matrix.PostScale(scale, scale, centerX, centerY);
					matrix.PostRotate(90 * (rotation - 2), centerX, centerY);
					break;
				}
				case (int)SurfaceOrientation.Rotation180:
					matrix.PostRotate(180, centerX, centerY);
					break;
				default:
				{
					bufferRect.Offset(centerX - bufferRect.CenterX(), centerY - bufferRect.CenterY());
					matrix.SetRectToRect(viewRect, bufferRect, Matrix.ScaleToFit.Fill);
					var scale = Math.Max(
						(float)viewHeight / previewSize.Height,
						(float)viewHeight / previewSize.Width);
					matrix.PostScale(scale, scale, centerX, centerY);
					matrix.PostRotate(0, centerX, centerY);
					break;
				}
			}
			TextureView.SetTransform(matrix);
		}

        private class ErrorDialog : DialogFragment
		{
			public override Dialog OnCreateDialog(Bundle savedInstanceState)
			{
				var alert = new AlertDialog.Builder(Activity);
				alert.SetMessage("This device doesn't support Camera2 API.");
				alert.SetPositiveButton(Android.Resource.String.Ok, new MyDialogOnClickListener(this));
				return alert.Show();

			}
		}

		private class MyDialogOnClickListener : Java.Lang.Object, IDialogInterfaceOnClickListener
		{
			private readonly ErrorDialog er;
			public MyDialogOnClickListener(ErrorDialog e)
			{
				er = e;
			}
			public void OnClick(IDialogInterface dialogInterface, int i)
			{
				er.Activity.Finish();
			}
		}

		// Compare two Sizes based on their areas
		private class CompareSizesByArea : Java.Lang.Object, IComparator
		{
			public int Compare(Java.Lang.Object lhs, Java.Lang.Object rhs)
			{
				// We cast here to ensure the multiplications won't overflow
				if (!(lhs is Size) || !(rhs is Size)) return 0;
				var right = (Size)rhs;
				var left = (Size)lhs;
				return Long.Signum((long)left.Width * left.Height -
				                   (long)right.Width * right.Height);
			}
		}

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
			if (context is ICameraFragmentListener listener)
            {
				cameraFragmentListener = listener;
            } else
            {
				throw new RuntimeException(context.ToString()
					+ " must implement OnImageSetListener");
            }
        }

        public override void OnDetach()
        {
            base.OnDetach();
			cameraFragmentListener = null;
        }
    }
}