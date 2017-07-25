package md5f764fa4b8962da8f8d13b4fbea3e5efc;


public class GraphicFaceTracker
	extends com.google.android.gms.vision.Tracker
	implements
		mono.android.IGCUserPeer,
		com.google.android.gms.vision.CameraSource.PictureCallback
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onNewItem:(ILjava/lang/Object;)V:GetOnNewItem_ILjava_lang_Object_Handler\n" +
			"n_onUpdate:(Lcom/google/android/gms/vision/Detector$Detections;Ljava/lang/Object;)V:GetOnUpdate_Lcom_google_android_gms_vision_Detector_Detections_Ljava_lang_Object_Handler\n" +
			"n_onMissing:(Lcom/google/android/gms/vision/Detector$Detections;)V:GetOnMissing_Lcom_google_android_gms_vision_Detector_Detections_Handler\n" +
			"n_onDone:()V:GetOnDoneHandler\n" +
			"n_onPictureTaken:([B)V:GetOnPictureTaken_arrayBHandler:Android.Gms.Vision.CameraSource/IPictureCallbackInvoker, Xamarin.GooglePlayServices.Vision\n" +
			"";
		mono.android.Runtime.register ("LiveCam.Droid.GraphicFaceTracker, LiveCam.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", GraphicFaceTracker.class, __md_methods);
	}


	public GraphicFaceTracker () throws java.lang.Throwable
	{
		super ();
		if (getClass () == GraphicFaceTracker.class)
			mono.android.TypeManager.Activate ("LiveCam.Droid.GraphicFaceTracker, LiveCam.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public GraphicFaceTracker (md5f764fa4b8962da8f8d13b4fbea3e5efc.GraphicOverlay p0, com.google.android.gms.vision.CameraSource p1) throws java.lang.Throwable
	{
		super ();
		if (getClass () == GraphicFaceTracker.class)
			mono.android.TypeManager.Activate ("LiveCam.Droid.GraphicFaceTracker, LiveCam.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "LiveCam.Droid.GraphicOverlay, LiveCam.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null:Android.Gms.Vision.CameraSource, Xamarin.GooglePlayServices.Vision, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", this, new java.lang.Object[] { p0, p1 });
	}


	public void onNewItem (int p0, java.lang.Object p1)
	{
		n_onNewItem (p0, p1);
	}

	private native void n_onNewItem (int p0, java.lang.Object p1);


	public void onUpdate (com.google.android.gms.vision.Detector.Detections p0, java.lang.Object p1)
	{
		n_onUpdate (p0, p1);
	}

	private native void n_onUpdate (com.google.android.gms.vision.Detector.Detections p0, java.lang.Object p1);


	public void onMissing (com.google.android.gms.vision.Detector.Detections p0)
	{
		n_onMissing (p0);
	}

	private native void n_onMissing (com.google.android.gms.vision.Detector.Detections p0);


	public void onDone ()
	{
		n_onDone ();
	}

	private native void n_onDone ();


	public void onPictureTaken (byte[] p0)
	{
		n_onPictureTaken (p0);
	}

	private native void n_onPictureTaken (byte[] p0);

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
