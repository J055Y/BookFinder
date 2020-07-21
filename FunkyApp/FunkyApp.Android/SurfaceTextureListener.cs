using Android.Views;
using Android.Graphics;

namespace FunkyApp.Droid
{
    public class SurfaceTextureListener : Java.Lang.Object, TextureView.ISurfaceTextureListener
    {
        CameraFragment fragment;
        public SurfaceTextureListener(CameraFragment frag)
        {
            fragment = frag;
        }

        public void OnSurfaceTextureAvailable(SurfaceTexture surface_texture, int width, int height)
        {
            fragment.OpenCamera(width, height);
        }

        public void OnSurfaceTextureSizeChanged(SurfaceTexture surface_texture, int width, int height)
        {
            fragment.configureTransform(width, height);
        }

        public bool OnSurfaceTextureDestroyed(SurfaceTexture surface_texture)
        {
            return true;
        }

        public void OnSurfaceTextureUpdated(SurfaceTexture surface_texture)
        {
        }

    }
}