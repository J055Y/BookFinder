using Android.Hardware.Camera2;

namespace FunkyApp.Droid
{
    public class CameraStateCallback : CameraDevice.StateCallback
    {
        CameraFragment fragment;
        public CameraStateCallback(CameraFragment frag)
        {
            fragment = frag;
        }
        public override void OnOpened(CameraDevice camera)
        {
            fragment.CameraDevice = camera;
            fragment.StartPreview();
            fragment.CameraOpenCloseLock.Release();
            if (null != fragment.TextureView)
                fragment.ConfigureTransform(fragment.TextureView.Width, fragment.TextureView.Height);
        }

        public override void OnDisconnected(CameraDevice camera)
        {
            fragment.CameraOpenCloseLock.Release();
            camera.Close();
            fragment.CameraDevice = null;
        }

        public override void OnError(CameraDevice camera, CameraError error)
        {
            fragment.CameraOpenCloseLock.Release();
            camera.Close();
            fragment.CameraDevice = null;
            if (null != fragment.Activity)
                fragment.Activity.Finish();
        }
    }
}