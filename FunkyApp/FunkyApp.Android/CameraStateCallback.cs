using System;
using Android.Hardware.Camera2;
using Android.Widget;

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
            fragment.cameraDevice = camera;
            fragment.startPreview();
            fragment.cameraOpenCloseLock.Release();
            if (null != fragment.textureView)
                fragment.configureTransform(fragment.textureView.Width, fragment.textureView.Height);
        }

        public override void OnDisconnected(CameraDevice camera)
        {
            fragment.cameraOpenCloseLock.Release();
            camera.Close();
            fragment.cameraDevice = null;
        }

        public override void OnError(CameraDevice camera, CameraError error)
        {
            fragment.cameraOpenCloseLock.Release();
            camera.Close();
            fragment.cameraDevice = null;
            if (null != fragment.Activity)
                fragment.Activity.Finish();
        }


    }
}