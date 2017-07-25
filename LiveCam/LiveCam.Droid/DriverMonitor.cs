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
    [Activity(Label = "DriverMonitor")]
    public class DriverMonitor : AppCompatActivity, IFactory
    {
        private static readonly string TAG = "FaceTracker";

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


        public static void Start(Activity activity)
        {
            var intent = new Intent(activity, typeof(DriverMonitor));
            activity.StartActivity(intent);
        }

        protected async override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            mPreview = FindViewById<CameraSourcePreview>(Resource.Id.preview);
            mGraphicOverlay = FindViewById<GraphicOverlay>(Resource.Id.faceOverlay);
            //greetingsText = FindViewById<TextView>(Resource.Id.greetingsTextView);


            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == Permission.Granted)
            {
                CreateCameraSource();
                LiveCamHelper.Init();
                LiveCamHelper.GreetingsCallback = (s) => { RunOnUiThread(() => GreetingsText = s); };
              //  await LiveCamHelper.RegisterFaces();
            }
            else { RequestCameraPermission(); }


        }

        protected override void OnResume()
        {
            base.OnResume();
            StartCameraSource();



        }

        protected override void OnPause()
        {
            base.OnPause();
            mPreview.Stop();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (mCameraSource != null)
            {
                mCameraSource.Release();
            }
        }

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

        /**
 * Creates and starts the camera.  Note that this uses a higher resolution in comparison
 * to other detection examples to enable the barcode detector to detect small barcodes
 * at long distances.
 */
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

        /**
         * Starts or restarts the camera source, if it exists.  If the camera source doesn't exist yet
         * (e.g., because onResume was called before the camera source was created), this will be called
         * again when the camera source is created.
         */
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


    class GraphicFaceTracker : Tracker, CameraSource.IPictureCallback
    {
        private GraphicOverlay mOverlay;
        private FaceGraphic mFaceGraphic;
        private CameraSource mCameraSource = null;
        private bool isProcessing = false;
        private double headPoseDeviation;
        private double eyeAperture;
        private double mouthAperture;

        public GraphicFaceTracker(GraphicOverlay overlay, CameraSource cameraSource = null)
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

                    var face = imags.DetectedFaces.FirstOrDefault();

                    Tb_Trans_Driver_Logs logs = new Tb_Trans_Driver_Logs();
                    this.ProcessHeadPose(face);
                    this.ProcessMouth(face);
                    this.ProcessEyes(face);
                    SetTransDriverlog(logs, face);

                    await SendPostRequest(logs);

                }



            }
            return;

        }


        private void ProcessHeadPose(Microsoft.ProjectOxford.Face.Contract.Face f)
        {
            headPoseDeviation = Java.Lang.Math.Abs(f.FaceAttributes.HeadPose.Yaw);

          //  this.headPoseIndicator.Margin = new Thickness((-f.FaceAttributes.HeadPose.Yaw / 90) * headPoseIndicatorHost.ActualWidth / 2, 0, 0, 0);

          
        }

        private void ProcessMouth(Microsoft.ProjectOxford.Face.Contract.Face f)
        {
            double mouthWidth = Java.Lang.Math.Abs(f.FaceLandmarks.MouthRight.X - f.FaceLandmarks.MouthLeft.X);
            double mouthHeight = Java.Lang.Math.Abs(f.FaceLandmarks.UpperLipBottom.Y - f.FaceLandmarks.UnderLipTop.Y);
            mouthAperture = mouthHeight / mouthWidth;
            
        }

        private void ProcessEyes(Microsoft.ProjectOxford.Face.Contract.Face f)
        {
            double leftEyeWidth = Java.Lang.Math.Abs(f.FaceLandmarks.EyeLeftInner.X - f.FaceLandmarks.EyeLeftOuter.X);
            double leftEyeHeight = Java.Lang.Math.Abs(f.FaceLandmarks.EyeLeftBottom.Y - f.FaceLandmarks.EyeLeftTop.Y);

         

            double rightEyeWidth = Java.Lang.Math.Abs(f.FaceLandmarks.EyeRightInner.X - f.FaceLandmarks.EyeRightOuter.X);
            double rightEyeHeight = Java.Lang.Math.Abs(f.FaceLandmarks.EyeRightBottom.Y - f.FaceLandmarks.EyeRightTop.Y);

            eyeAperture = Java.Lang.Math.Max(leftEyeHeight / leftEyeWidth, rightEyeHeight / rightEyeWidth);
         
        }

        public void SetTransDriverlog(Tb_Trans_Driver_Logs t, Microsoft.ProjectOxford.Face.Contract.Face f)
        {
          
            t.EyeAperture = eyeAperture;
            t.EyeLeftBottom_X = f.FaceLandmarks.EyeLeftBottom.X;
            t.EyeLeftBottom_Y = f.FaceLandmarks.EyeLeftBottom.Y;
            t.EyeLeftInner_X = f.FaceLandmarks.EyeLeftInner.X;
            t.EyeLeftInner_Y = f.FaceLandmarks.EyeLeftInner.Y;
            t.EyeLeftOuter_X = f.FaceLandmarks.EyeLeftOuter.X;
            t.EyeLeftOuter_Y = f.FaceLandmarks.EyeLeftOuter.Y;
            t.EyeLeftTop_X = f.FaceLandmarks.EyeLeftTop.X;
            t.EyeLeftTop_Y = f.FaceLandmarks.EyeLeftTop.Y;
            t.EyeRightBottom_X = f.FaceLandmarks.EyeRightBottom.X;
            t.EyeRightBottom_Y = f.FaceLandmarks.EyeRightBottom.Y;
            t.EyeRightInner_X = f.FaceLandmarks.EyeRightInner.X;
            t.EyeRightInner_Y = f.FaceLandmarks.EyeRightInner.Y;
            t.EyeRightOuter_X = f.FaceLandmarks.EyeRightOuter.X;
            t.EyeRightOuter_Y = f.FaceLandmarks.EyeRightOuter.Y;
            t.EyeRightTop_X = f.FaceLandmarks.EyeRightTop.X;
            t.EyeRightTop_Y = f.FaceLandmarks.EyeRightTop.Y;
            t.HeadPost_Pitch = f.FaceAttributes.HeadPose.Pitch;
            t.HeadPost_PoseDeviation = headPoseDeviation;
            t.HeadPost_Roll = f.FaceAttributes.HeadPose.Roll;
            t.HeadPost_Yaw = f.FaceAttributes.HeadPose.Yaw;
            t.MouthAperture = mouthAperture;
            t.MouthLeft_X = f.FaceLandmarks.MouthLeft.X;
            t.MouthLeft_Y = f.FaceLandmarks.MouthLeft.Y;
            t.MouthRight_X = f.FaceLandmarks.MouthRight.X;
            t.MouthRight_Y = f.FaceLandmarks.MouthRight.Y;
            t.RefRegisterID = 1;
            t.TransID = 1;
            t.TimeStamp = DateTime.Now;
            t.UnderLipBottom_X = f.FaceLandmarks.UnderLipBottom.X;
            t.UnderLipBottom_Y = f.FaceLandmarks.UnderLipBottom.Y;

            t.UnderLipTop_X = f.FaceLandmarks.UnderLipTop.X;
            t.UnderLipTop_Y = f.FaceLandmarks.UnderLipTop.Y;
            t.UpperLipBottom_X = f.FaceLandmarks.UpperLipBottom.X;
            t.UpperLipBottom_Y = f.FaceLandmarks.UpperLipBottom.Y;
            t.UpperLipTop_X = f.FaceLandmarks.UpperLipTop.X;
            t.UpperLipTop_Y = f.FaceLandmarks.UpperLipTop.Y;
        }





        async Task<string> SendPostRequest(Tb_Trans_Driver_Logs t)

        {

            // Remove this line

            //throw new NotImplementedException();


            // Create JObject instance as 'jsonObject', then Add( "key" , value);
            string url = "http://my-first-web-cutto.azurewebsites.net/api/DriverLogs";
           // string contentType = "application/json"; // or application/xml
            JObject jsonObject = new JObject();
            // Create JObject instance as 'jsonObject', then Add( "key" , value);


            string jsonObjectA = JsonConvert.SerializeObject(t);

            //wrap it around in object container notation
            //var  jSoNToPost = string.Concat("{", jsonObjectA, "}");
            //convert it to JSON acceptible content
            HttpContent content = new StringContent(jsonObjectA, Encoding.UTF8, "application/json");
            var client = new HttpClient();
            var response = await client.PostAsync(url, content);


            // Create StringContent object with json, Encoding.UTF8 format, and content type



            return string.Empty;

        }
    }
}