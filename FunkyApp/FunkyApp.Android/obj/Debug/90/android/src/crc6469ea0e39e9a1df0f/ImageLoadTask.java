package crc6469ea0e39e9a1df0f;


public class ImageLoadTask
	extends android.os.AsyncTask
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_doInBackground:([Ljava/lang/Object;)Ljava/lang/Object;:GetDoInBackground_arrayLjava_lang_Object_Handler\n" +
			"n_onPostExecute:(Ljava/lang/Object;)V:GetOnPostExecute_Ljava_lang_Object_Handler\n" +
			"";
		mono.android.Runtime.register ("FunkyApp.Droid.ImageLoadTask, FunkyApp.Android", ImageLoadTask.class, __md_methods);
	}


	public ImageLoadTask ()
	{
		super ();
		if (getClass () == ImageLoadTask.class)
			mono.android.TypeManager.Activate ("FunkyApp.Droid.ImageLoadTask, FunkyApp.Android", "", this, new java.lang.Object[] {  });
	}

	public ImageLoadTask (java.lang.String p0, android.widget.ImageView p1)
	{
		super ();
		if (getClass () == ImageLoadTask.class)
			mono.android.TypeManager.Activate ("FunkyApp.Droid.ImageLoadTask, FunkyApp.Android", "System.String, mscorlib:Android.Widget.ImageView, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}


	public java.lang.Object doInBackground (java.lang.Object[] p0)
	{
		return n_doInBackground (p0);
	}

	private native java.lang.Object n_doInBackground (java.lang.Object[] p0);


	public void onPostExecute (java.lang.Object p0)
	{
		n_onPostExecute (p0);
	}

	private native void n_onPostExecute (java.lang.Object p0);

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
