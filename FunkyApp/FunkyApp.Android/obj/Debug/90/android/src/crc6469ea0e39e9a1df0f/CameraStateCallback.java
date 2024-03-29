package crc6469ea0e39e9a1df0f;


public class CameraStateCallback
	extends android.hardware.camera2.CameraDevice.StateCallback
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onOpened:(Landroid/hardware/camera2/CameraDevice;)V:GetOnOpened_Landroid_hardware_camera2_CameraDevice_Handler\n" +
			"n_onDisconnected:(Landroid/hardware/camera2/CameraDevice;)V:GetOnDisconnected_Landroid_hardware_camera2_CameraDevice_Handler\n" +
			"n_onError:(Landroid/hardware/camera2/CameraDevice;I)V:GetOnError_Landroid_hardware_camera2_CameraDevice_IHandler\n" +
			"";
		mono.android.Runtime.register ("FunkyApp.Droid.CameraStateCallback, FunkyApp.Android", CameraStateCallback.class, __md_methods);
	}


	public CameraStateCallback ()
	{
		super ();
		if (getClass () == CameraStateCallback.class)
			mono.android.TypeManager.Activate ("FunkyApp.Droid.CameraStateCallback, FunkyApp.Android", "", this, new java.lang.Object[] {  });
	}

	public CameraStateCallback (crc6469ea0e39e9a1df0f.CameraFragment p0)
	{
		super ();
		if (getClass () == CameraStateCallback.class)
			mono.android.TypeManager.Activate ("FunkyApp.Droid.CameraStateCallback, FunkyApp.Android", "FunkyApp.Droid.CameraFragment, FunkyApp.Android", this, new java.lang.Object[] { p0 });
	}


	public void onOpened (android.hardware.camera2.CameraDevice p0)
	{
		n_onOpened (p0);
	}

	private native void n_onOpened (android.hardware.camera2.CameraDevice p0);


	public void onDisconnected (android.hardware.camera2.CameraDevice p0)
	{
		n_onDisconnected (p0);
	}

	private native void n_onDisconnected (android.hardware.camera2.CameraDevice p0);


	public void onError (android.hardware.camera2.CameraDevice p0, int p1)
	{
		n_onError (p0, p1);
	}

	private native void n_onError (android.hardware.camera2.CameraDevice p0, int p1);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
