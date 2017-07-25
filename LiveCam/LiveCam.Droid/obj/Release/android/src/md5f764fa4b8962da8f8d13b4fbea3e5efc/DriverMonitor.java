package md5f764fa4b8962da8f8d13b4fbea3e5efc;


public class DriverMonitor
	extends android.support.v7.app.AppCompatActivity
	implements
		mono.android.IGCUserPeer,
		com.google.android.gms.vision.MultiProcessor.Factory
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"n_onResume:()V:GetOnResumeHandler\n" +
			"n_onPause:()V:GetOnPauseHandler\n" +
			"n_onDestroy:()V:GetOnDestroyHandler\n" +
			"n_onRequestPermissionsResult:(I[Ljava/lang/String;[I)V:GetOnRequestPermissionsResult_IarrayLjava_lang_String_arrayIHandler\n" +
			"n_create:(Ljava/lang/Object;)Lcom/google/android/gms/vision/Tracker;:GetCreate_Ljava_lang_Object_Handler:Android.Gms.Vision.MultiProcessor/IFactoryInvoker, Xamarin.GooglePlayServices.Vision\n" +
			"";
		mono.android.Runtime.register ("LiveCam.Droid.DriverMonitor, LiveCam.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", DriverMonitor.class, __md_methods);
	}


	public DriverMonitor () throws java.lang.Throwable
	{
		super ();
		if (getClass () == DriverMonitor.class)
			mono.android.TypeManager.Activate ("LiveCam.Droid.DriverMonitor, LiveCam.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);


	public void onResume ()
	{
		n_onResume ();
	}

	private native void n_onResume ();


	public void onPause ()
	{
		n_onPause ();
	}

	private native void n_onPause ();


	public void onDestroy ()
	{
		n_onDestroy ();
	}

	private native void n_onDestroy ();


	public void onRequestPermissionsResult (int p0, java.lang.String[] p1, int[] p2)
	{
		n_onRequestPermissionsResult (p0, p1, p2);
	}

	private native void n_onRequestPermissionsResult (int p0, java.lang.String[] p1, int[] p2);


	public com.google.android.gms.vision.Tracker create (java.lang.Object p0)
	{
		return n_create (p0);
	}

	private native com.google.android.gms.vision.Tracker n_create (java.lang.Object p0);

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
