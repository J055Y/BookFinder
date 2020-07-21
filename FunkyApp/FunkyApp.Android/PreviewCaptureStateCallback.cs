using System;
using Android.Hardware.Camera2;
using Android.Widget;

namespace FunkyApp.Droid
{
    public class PreviewCaptureStateCallback : CameraCaptureSession.StateCallback
    {
        CameraFragment fragment;
        public PreviewCaptureStateCallback(CameraFragment frag)
        {
            fragment = frag;
        }
        public override void OnConfigured(CameraCaptureSession session)
        {
            fragment.previewSession = session;
            fragment.updatePreview();

        }

        public override void OnConfigureFailed(CameraCaptureSession session)
        {
            if (null != fragment.Activity)
                Toast.MakeText(fragment.Activity, "Failed", ToastLength.Short).Show();
        }
    }
}