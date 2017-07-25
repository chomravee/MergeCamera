using Android.App;
using Android.Widget;
using Android.OS;
using Android.Gms.Vision;
using Android.Support.V4.App;
using Android.Support.V7.App;

using Android.Util;
using Android;
using Android.Support.Design.Widget;
using Android.Content;
using Android.Gms.Vision.Faces;
using Java.Lang;
using System;
using Android.Runtime;
using static Android.Gms.Vision.MultiProcessor;
using Android.Content.PM;
using Android.Gms.Common;
using LiveCam.Shared;
using System.Threading.Tasks;
using ServiceHelpers;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;

namespace LiveCam.Droid
{
    [Activity(Label = "LiveCam.Droid", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/Theme.AppCompat.NoActionBar", ScreenOrientation = ScreenOrientation.FullSensor)]
    public class MainActivity : AppCompatActivity//, IFactory
    {
        private static readonly string TAG = "FaceTracker";

        /*
        private CameraSource mCameraSource = null;

        private CameraSourcePreview mPreview;
        private GraphicOverlay mGraphicOverlay;


        public static string GreetingsText
        {
            get;
            set;
        }

        private static readonly int RC_HANDLE_GMS = 9001;
        // permission request codes need to be < 256
        private static readonly int RC_HANDLE_CAMERA_PERM = 2;
        */
        protected  override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.LogIn);
            Button btnLogin = FindViewById<Button>(Resource.Id.btnLogin);
            btnLogin.Click += BtnLogin_Action;
            /*
            mPreview = FindViewById<CameraSourcePreview>(Resource.Id.preview);
            mGraphicOverlay = FindViewById<GraphicOverlay>(Resource.Id.faceOverlay);
            //greetingsText = FindViewById<TextView>(Resource.Id.greetingsTextView);


            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == Permission.Granted)
            {
                CreateCameraSource();
                LiveCamHelper.Init();
                LiveCamHelper.GreetingsCallback = (s) => { RunOnUiThread(() => GreetingsText = s); };
                await LiveCamHelper.RegisterFaces();
            }
            else { RequestCameraPermission(); }

            */
        }

        private void BtnLogin_Action(object sender, EventArgs e)
        {
            DriverMonitor.Start(this);
        }

        protected override void OnResume()
        {
            base.OnResume();
           // StartCameraSource();



        }

        protected override void OnPause()
        {
            base.OnPause();
            // mPreview.Stop();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            /* if (mCameraSource != null)
             {
                 mCameraSource.Release();
             }
             */
        }
        /*
        private void RequestCameraPermission()
        {
            Log.Warn(TAG, "Camera permission is not granted. Requesting permission");

            var permissions = new string[] { Manifest.Permission.Camera };

            if (!ActivityCompat.ShouldShowRequestPermissionRationale(this,
                    Manifest.Permission.Camera))
            {
                ActivityCompat.RequestPermissions(this, permissions, RC_HANDLE_CAMERA_PERM);
                return;
            }

            Snackbar.Make(mGraphicOverlay, Resource.String.permission_camera_rationale,
                    Snackbar.LengthIndefinite)
                    .SetAction(Resource.String.ok, (o) => { ActivityCompat.RequestPermissions(this, permissions, RC_HANDLE_CAMERA_PERM); })
                    .Show();
        }
        */
        /**
 * Creates and starts the camera.  Note that this uses a higher resolution in comparison
 * to other detection examples to enable the barcode detector to detect small barcodes
 * at long distances.
 */
        /*
               private void CreateCameraSource()
               {

                   var context = Application.Context;
                   FaceDetector detector = new FaceDetector.Builder(context)
                           .SetClassificationType(ClassificationType.All)
                           .Build();

                   detector.SetProcessor(
                           new MultiProcessor.Builder(this)
                                   .Build());

                   if (!detector.IsOperational)
                   {
                       // Note: The first time that an app using face API is installed on a device, GMS will
                       // download a native library to the device in order to do detection.  Usually this
                       // completes before the app is run for the first time.  But if that download has not yet
                       // completed, then the above call will not detect any faces.
                       //
                       // isOperational() can be used to check if the required native library is currently
                       // available.  The detector will automatically become operational once the library
                       // download completes on device.
                       Log.Warn(TAG, "Face detector dependencies are not yet available.");
                   }

                   mCameraSource = new CameraSource.Builder(context, detector)
                           .SetRequestedPreviewSize(640, 480)
                                                   .SetFacing(CameraFacing.Back)
                           .SetRequestedFps(30.0f)
                           .Build();


               }
               */
        /**
         * Starts or restarts the camera source, if it exists.  If the camera source doesn't exist yet
         * (e.g., because onResume was called before the camera source was created), this will be called
         * again when the camera source is created.
         */
        /*
       private void StartCameraSource()
       {

           // check that the device has play services available.
           int code = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(
                   this.ApplicationContext);
           if (code != ConnectionResult.Success)
           {
               Dialog dlg =
                       GoogleApiAvailability.Instance.GetErrorDialog(this, code, RC_HANDLE_GMS);
               dlg.Show();
           }

           if (mCameraSource != null)
           {
               try
               {
                   mPreview.Start(mCameraSource, mGraphicOverlay);
               }
               catch (System.Exception e)
               {
                   Log.Error(TAG, "Unable to start camera source.", e);
                   mCameraSource.Release();
                   mCameraSource = null;
               }
           }
       }
       public Tracker Create(Java.Lang.Object item)
       {
           return new GraphicFaceTracker(mGraphicOverlay, mCameraSource);
       }


       public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
       {
           if (requestCode != RC_HANDLE_CAMERA_PERM)
           {
               Log.Debug(TAG, "Got unexpected permission result: " + requestCode);
               base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

               return;
           }

           if (grantResults.Length != 0 && grantResults[0] == Permission.Granted)
           {
               Log.Debug(TAG, "Camera permission granted - initialize the camera source");
               // we have permission, so create the camerasource
               CreateCameraSource();
               return;
           }

           Log.Error(TAG, "Permission not granted: results len = " + grantResults.Length +
                   " Result code = " + (grantResults.Length > 0 ? grantResults[0].ToString() : "(empty)"));


           var builder = new Android.Support.V7.App.AlertDialog.Builder(this);
           builder.SetTitle("LiveCam")
                   .SetMessage(Resource.String.no_camera_permission)
                   .SetPositiveButton(Resource.String.ok, (o, e) => Finish())
                   .Show();

       }
   }
   */
        /*
        class GraphicFaceTracker : Tracker, CameraSource.IPictureCallback
        {
            private GraphicOverlay mOverlay;
            private FaceGraphic mFaceGraphic;
            private CameraSource mCameraSource = null;
            private bool isProcessing = false;

            public GraphicFaceTracker(GraphicOverlay overlay, CameraSource cameraSource =null)
            {
                mOverlay = overlay;
                mFaceGraphic = new FaceGraphic(overlay);
                mCameraSource = cameraSource;
            }

            public override void OnNewItem(int id, Java.Lang.Object item)
            {
                mFaceGraphic.SetId(id);
                if (mCameraSource != null && !isProcessing)
                    mCameraSource.TakePicture(null, this);
            }

            public override void OnUpdate(Detector.Detections detections, Java.Lang.Object item)
            {
                var face = item as Face;
                mOverlay.Add(mFaceGraphic);
                mFaceGraphic.UpdateFace(face);

            }

            public override void OnMissing(Detector.Detections detections)
            {
                mOverlay.Remove(mFaceGraphic);

            }

            public override void OnDone()
            {
                mOverlay.Remove(mFaceGraphic);

            }

            public void OnPictureTaken(byte[] data)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        isProcessing = true;

                        Console.WriteLine("face detected: ");

                        var imageAnalyzer = new ImageAnalyzer(data);
                        imageAnalyzer.FaceDetectionCompleted += ImageAnalyzer_FaceDetecyionCompleted; 
                        await LiveCamHelper.ProcessCameraCapture(imageAnalyzer);

                    }

                    finally
                    {
                        isProcessing = false;


                    }

                });
            }

            async private void ImageAnalyzer_FaceDetecyionCompleted(object sender, EventArgs e)
            {
                if (sender != null)
                {
                    ImageAnalyzer imags = (ImageAnalyzer)sender;

                    if (imags.DetectedFaces.Count() > 0)
                    {
                       var face =  imags.DetectedFaces.FirstOrDefault();
                       await SendPostRequest(face.FaceAttributes.Age.ToString(), face.FaceAttributes.HeadPose.Pitch.ToString());
                    }

                }



                return;
            }


            async Task<string> SendPostRequest(string name, string email)
            {
                // Remove this line
                //throw new NotImplementedException();

                string url = "";
                string contentType = "application/json"; // or application/xml

                // Create JObject instance as 'jsonObject', then Add( "key" , value);
                JObject jsonObject = new JObject();
                jsonObject.Add("uid", name);
                jsonObject.Add("Values", email);

                StringContent content = new StringContent(jsonObject.ToString(), Encoding.UTF8, contentType);


                // Create StringContent object with json, Encoding.UTF8 format, and content type


                var client = new HttpClient();

                var response = await client.PostAsync(url, content);

                // PostAsync to 'url' with 'content'


                // Read Content as JSON String
                //var json = response.Content.ReadAsStringAsync().Result;

                // Convert JSON String to our MessageModel object, then return
                //var messageModel = JsonConvert.DeserializeObject<MessageModel>(json);

                return string.Empty;
            }
        }

       */
    }

}


