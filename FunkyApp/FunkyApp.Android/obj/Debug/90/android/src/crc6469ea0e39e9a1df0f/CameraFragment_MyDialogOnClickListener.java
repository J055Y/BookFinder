package crc6469ea0e39e9a1df0f;


public class CameraFragment_MyDialogOnClickListener
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.content.DialogInterface.OnClickListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onClick:(Landroid/content/DialogInterface;I)V:GetOnClick_Landroid_content_DialogInterface_IHandler:Android.Content.IDialogInterfaceOnClickListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("FunkyApp.Droid.CameraFragment+MyDialogOnClickListener, FunkyApp.Android", CameraFragment_MyDialogOnClickListener.class, __md_methods);
	}


	public CameraFragment_MyDialogOnClickListener ()
	{
		super ();
		if (getClass () == CameraFragment_MyDialogOnClickListener.class)
			mono.android.TypeManager.Activate ("FunkyApp.Droid.CameraFragment+MyDialogOnClickListener, FunkyApp.Android", "", this, new java.lang.Object[] {  });
	}

	public CameraFragment_MyDialogOnClickListener (crc6469ea0e39e9a1df0f.CameraFragment_ErrorDialog p0)
	{
		super ();
		if (getClass () == CameraFragment_MyDialogOnClickListener.class)
			mono.android.TypeManager.Activate ("FunkyApp.Droid.CameraFragment+MyDialogOnClickListener, FunkyApp.Android", "FunkyApp.Droid.CameraFragment+ErrorDialog, FunkyApp.Android", this, new java.lang.Object[] { p0 });
	}


	public void onClick (android.content.DialogInterface p0, int p1)
	{
		n_onClick (p0, p1);
	}

	private native void n_onClick (android.content.DialogInterface p0, int p1);

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
