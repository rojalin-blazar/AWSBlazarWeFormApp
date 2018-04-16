using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Drawing;

using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;


using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon;
using Amazon.S3.IO;
using Amazon.S3.Model;

using Amazon.Rekognition;
using Amazon.Rekognition.Model;


namespace AWSBlazarWeFormApp
{
    public partial class ImageRecognition : System.Web.UI.Page
    {
        IAmazonRekognition rekoClient = new AmazonRekognitionClient(Amazon.RegionEndpoint.USEast1);
        protected void Page_Load(object sender, EventArgs e)
        {

            lblMessage.Text = "";
            

           // AWSConfigs.AWSRegion = "us-east-1";
          //  AWSConfigs.Logging = LoggingOptions.Log4Net;

            /*  create face collection
            var response = rekoClient.CreateCollection(new CreateCollectionRequest
            {
                CollectionId = "myphotos"
            });

            string collectionArn = response.CollectionArn;
            int statusCode = response.StatusCode;
            */

           


          //  List<string> collectionIds = response.CollectionIds;
 


        }
        protected void btnUpload_Click(object sender, EventArgs e)
        {
            /*
             *  First upload image in bucket
             *  detect face while uploading image into bucket
             *  compare face in face collection
             *  get list of image from bucket
             *  store uploaded image in face collection
             *  
             * compare face in face collection while uploading
             * 
             * 
             * */
            try
            {
                image.Visible = false;
                lblMessage.Text = "";
                string imagefile = string.Empty;
                if (fuImage.HasFile)
                {
                    string imagename = System.IO.Path.GetFullPath(fuImage.PostedFile.FileName);
                    //Label1.Text = imagename;
                    string ext = System.IO.Path.GetExtension(fuImage.FileName);
                    // Label2.Text = ext;
                    // imagefile = Server.MapPath("~/Images/" + imagename);
                    imagefile = imagename;
                    if (ext == ".jpg" | ext == ".png")
                    {
                        //fuImage.SaveAs(imagefile);

                        Stream st = fuImage.PostedFile.InputStream;
                        IAmazonRekognition rekoClient = new AmazonRekognitionClient(Amazon.RegionEndpoint.USEast1);
                        string name = Path.GetFileName(fuImage.PostedFile.FileName);
                        name = @"C:\Blazar\CompareImage\" + fuImage.FileName;
                        string myBucketName = "blazarstorage"; //your s3 bucket name goes here  
                        string s3DirectoryName = "";
                        string s3FileName = @name;
                        string fileName = fuImage.FileName;
                        /*
                         * StoreFaceInCollection(@name, rekoClient, myBucketName);
            
                         ImageRecognition imageRecog = new ImageRecognition();
                         IdentifyFaces(name, fuImage.PostedFile.FileName.ToString(), myBucketName);
                          * */
                        //STEP----1
                        //validate the image is a face or not--step 1
                        //bool isFaceImage = DetectFaces(fileName);
                        //bool isFaceImage = DetectFaces(name);
                       bool isFaceImage = true;
                        if (isFaceImage == true)
                        {
                            /*
                             we can compare image if those are inside buckets.
                             * For comparing images , we have to follow below steps 
                             *   store it in bucket
                             *   add to it face collection
                             *   compare images
                
                             */


                            //
                            bool isUpload;
                            //upload image into bucket
                            isUpload = sendMyFileToS3(st, myBucketName, s3DirectoryName, s3FileName, fileName);
                            if (isUpload == true)
                            {
                                //store image in a face collection
                                StoreFaceInCollection(@name, rekoClient, myBucketName);

                                //  AmazonUploader myUploader = new AmazonUploader();
                                //  bool b = myUploader.IsFilExist("AKIAIPXX2OIGXNKENL6Q", "oQQn3l4ll5/J/OY2RoZG4EV4RZtv8EsD114MrRnR");
                                //validate the existance of the image in the bucket
                                //for that we have to get all the face from face collection
                                try
                                {
                                    AmazonS3Client s3Client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1);
                                    ListObjectsV2Request reqGetObjFromBucket = new ListObjectsV2Request
                                    {
                                        BucketName = myBucketName,
                                        MaxKeys = 1000
                                    };
                                    ListObjectsV2Response resGetObjFromBucket;
                                    do
                                    {
                                        resGetObjFromBucket = s3Client.ListObjectsV2(reqGetObjFromBucket);
                                        foreach (Amazon.S3.Model.S3Object entry in resGetObjFromBucket.S3Objects)
                                        {
                                            //if (DetectFaces(entry.Key))//validat the image content
                                            //{
                                            //  if (s3FileName != entry.Key)//input image should not compare
                                            if (fileName != entry.Key)
                                            {
                                                var response = rekoClient.CompareFaces(new CompareFacesRequest
                                                {
                                                    SimilarityThreshold = 90,
                                                    SourceImage = new Amazon.Rekognition.Model.Image
                                                    {
                                                        S3Object = new Amazon.Rekognition.Model.S3Object
                                                        {
                                                            Bucket = myBucketName,
                                                            // Name=""
                                                            // Name = s3FileName
                                                            Name = fileName
                                                        }
                                                    },
                                                    TargetImage = new Amazon.Rekognition.Model.Image
                                                    {
                                                        S3Object = new Amazon.Rekognition.Model.S3Object
                                                        {
                                                            Bucket = myBucketName,
                                                            // Name=""
                                                            Name = entry.Key
                                                        }
                                                    }
                                                });
                                                if (response.FaceMatches.Count > 0)
                                                {
                                                    image.Visible = true;
                                                    dupImage.Src = name;
                                                    existingImage.Src = "https://s3.amazonaws.com/blazarstorage/" + entry.Key; ;
                                                    lblMessage.Text = "You are trying to upload the  image " + s3FileName + "  which  is  matching with  " + entry.Key;
                                                    lblMessage.ForeColor = System.Drawing.Color.Green;
                                                    IAmazonS3 s3 = new AmazonS3Client(Amazon.RegionEndpoint.USEast1);
                                                    s3.DeleteObject(myBucketName, fileName);
                                                    return;
                                                }
                                            }
                                            // }

                                        }

                                    } while (resGetObjFromBucket.IsTruncated == true);
                                }
                                catch(Exception ex)
                                {
                                    IAmazonS3 s3 = new AmazonS3Client(Amazon.RegionEndpoint.USEast1);
                                    s3.DeleteObject(myBucketName, fileName);
                                    lblMessage.Text = ex.Message.ToString();
                                    lblMessage.ForeColor = System.Drawing.Color.Red;

                                }
                                // List< Face > lstOfFaces= GetListOfFaceInFaceCollection();
                                //if (lstOfFaces.Count >= 1)
                                //{
                                //   for(int i=0;i<lstOfFaces.Count;i++)
                                //   {
                                //        GetObjectRequest objReq = new GetObjectRequest();
                                //        GetObjectResponse res = new GetObjectResponse();


                                //        if (response.FaceMatches.Count > 0)
                                //        {
                                //            IAmazonS3 s3 = new AmazonS3Client();
                                //            s3.DeleteObject(myBucketName, s3FileName);

                                //        }
                                //      }



                                //}
                            }
                            else
                                lblMessage.Text = "Please upload again!";
                            //else
                            //{
                            //    //upload image into bucket--step 3 
                            //    isUpload = sendMyFileToS3(st, myBucketName, s3DirectoryName, s3FileName);
                            //    if (isUpload == true)
                            //    {
                            //        //store image in a face collection
                            //        StoreFaceInCollection(@name, rekoClient, myBucketName);
                            //        lblMessage.Text = "successfully uploaded";
                            //        Response.Write("successfully uploaded");

                            //    }
                            //    else
                            //        lblMessage.Text = "error";
                            //}
                        }
                        else
                            lblMessage.Text = "Please upload a valid image!!!";
                    }
                }
            }
            catch (Exception ex)
            {
                
                lblMessage.Text = ex.Message.ToString(); }
            
        }
    
        #region "private methods"
        public void getImagesFromBucket(string BucketName, IAmazonS3 _amazonS3Client)
        {
            var request = new ListObjectsRequest { BucketName = BucketName };

            ListObjectsResponse response = _amazonS3Client.ListObjects(request);
            foreach (Amazon.S3.Model.S3Object o in response.S3Objects)
            {
                var objRequest = new GetObjectRequest
                {
                    BucketName = BucketName,
                    Key = o.Key

                };
                GetObjectResponse objResponse = _amazonS3Client.GetObject(objRequest);
                
                //objResponse.WriteResponseStreamToFile(ConfigurationManager.AppSettings["TargetLocation"] + o.Key);
            }
        }
        //get list of faces from face collection
        public List<Face> GetListOfFaceInFaceCollection()
        {
            ListFacesResponse listFaceResponse = null;
            
           string pageToken=string.Empty;
           if (listFaceResponse != null)
               pageToken = listFaceResponse.NextToken;
            //var getresponse = rekoClient.ListCollections(new ListCollectionsRequest
            //{
            //});
            listFaceResponse = rekoClient.ListFaces(new ListFacesRequest
            {
                CollectionId = "myphotos",
                MaxResults=1,
                NextToken=pageToken
            });
            //pageToken = listFaceResponse.NextToken;
            List<Face> lstFace = listFaceResponse.Faces;
            //List<string> collectionIds = getresponse.CollectionIds;
            return lstFace;

        }
        public void StoreFaceInCollection(string imageName,IAmazonRekognition client,string bucketName)
        {
            try
            {
                var response = client.IndexFaces(new IndexFacesRequest
                {
                    CollectionId = "myphotos",
                    DetectionAttributes = new List<string>
                    {

                    },
                    // ExternalImageId = "myBlazarPhotoID",
                    ExternalImageId = imageName,
                    Image = new Amazon.Rekognition.Model.Image
                    {
                        S3Object = new Amazon.Rekognition.Model.S3Object
                        {
                            Bucket = bucketName,
                            Name = imageName
                        }
                    }
                });

                if (response.FaceRecords.Count > 0)
                {
                    existingImage.Src = "https://s3.amazonaws.com/blazarstorage/" + imageName;
                    lblMessage.Text = imageName + "  uploaded succesfully!!!";
                    lblMessage.ForeColor = System.Drawing.Color.Green;
                }
            }
            catch(Exception ex)
            {
                lblMessage.Text = ex.Message.ToString();
            }

        }
        /// <summary>
        /// This method will detect  the input image is face or not
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool DetectFaces(string filename)
        {
            IAmazonRekognition rekoClient = new AmazonRekognitionClient(Amazon.RegionEndpoint.USEast1);
            DetectFacesRequest dfr = new DetectFacesRequest();
            Amazon.Rekognition.Model.Image img = new Amazon.Rekognition.Model.Image();




            // Request needs image butes, so read and add to request



            byte[] data = null;

            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {

                data = new byte[fs.Length];

                fs.Read(data, 0, (int)fs.Length);

            }

            img.Bytes = new MemoryStream(data);

            dfr.Image = img;

            var outcome = rekoClient.DetectFaces(dfr);
            if (outcome.FaceDetails.Count > 0)
                return true;
            else
                return false;

        }
        public void IdentifyFaces(string filename, string path, string bucketName)
        {
            // Using USWest2, not the default region
            IAmazonRekognition rekoClient = new AmazonRekognitionClient("AKIAIYKAM62F6DZ2CEGA", "HmHyI439/ZdyOOxjnrpW3izOzOWcu3kS5qwpV1Kd", Amazon.RegionEndpoint.USEast1);
            Amazon.Rekognition.Model.Image img;
            var response = rekoClient.DetectFaces(new DetectFacesRequest
            {

                Image = new Amazon.Rekognition.Model.Image
                {
                    S3Object = new Amazon.Rekognition.Model.S3Object
                    {

                        Bucket = bucketName,
                        Name = path,

                    }
                }
            });
            int k = response.FaceDetails.Count;
            //List<facedetail> faceDetails = response.FaceDetails;
            string orientationCorrection = response.OrientationCorrection;


            //</facedetail>
            DetectFacesRequest dfr = new DetectFacesRequest();

            // Request needs image butes, so read and add to request



            byte[] data = null;
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                data = new byte[fs.Length];
                fs.Read(data, 0, (int)fs.Length);
            }
            //img.Bytes = new MemoryStream(data);

            //if (img.S3Object != null)
            //{
            //    img.S3Object.Name = "blazarName";
            //    img.S3Object.Bucket = "blazarstorage";
            //}

            //dfr.Image = img;

            //var outcome = rekoClient.DetectFaces(dfr);

            if (response.FaceDetails.Count > 0)
            {
                // Load a bitmap to modify with face bounding box rectangles
                System.Drawing.Bitmap facesHighlighted = new System.Drawing.Bitmap(filename);
                Pen pen = new Pen(Color.Black, 3);

                // Create a graphics context
                using (var graphics = Graphics.FromImage(facesHighlighted))
                {
                    foreach (var fd in response.FaceDetails)
                    {
                        // Get the bounding box
                        BoundingBox bb = fd.BoundingBox;
                        Console.WriteLine("Bounding box = (" + bb.Left + ", " + bb.Top + ", " +
                            bb.Height + ", " + bb.Width + ")");
                        // Draw the rectangle using the bounding box values
                        // They are percentages so scale them to picture
                        graphics.DrawRectangle(pen, x: facesHighlighted.Width * bb.Left,
                            y: facesHighlighted.Height * bb.Top,
                            width: facesHighlighted.Width * bb.Width,
                            height: facesHighlighted.Height * bb.Height);
                    }
                }
                // Save the image with highlights as PNG
                string fileout = filename.Replace(Path.GetExtension(filename), "_faces.png");
                facesHighlighted.Save(fileout, System.Drawing.Imaging.ImageFormat.Png);
                Console.WriteLine(">>> " + response.FaceDetails.Count + " face(s) highlighted in file " + fileout);
            }
            else
                Console.WriteLine(">>> No faces found");
        }
        public bool sendMyFileToS3(System.IO.Stream localFilePath, string bucketName, string subDirectoryInBucket, string fileNameInS3,string fileName)
        {
            IAmazonS3 client = new AmazonS3Client(RegionEndpoint.USEast1);
            TransferUtility utility = new TransferUtility(client);
            TransferUtilityUploadRequest request = new TransferUtilityUploadRequest();

            if (subDirectoryInBucket == "" || subDirectoryInBucket == null)
            {
                request.BucketName = bucketName; //no subdirectory just bucket name  
            }
            else
            {   // subdirectory and bucket name  
                request.BucketName = bucketName + @"/" + subDirectoryInBucket;
            }
           // request.Key = fileNameInS3; //file name up in S3  
            request.Key = fileName;
            request.InputStream = localFilePath;
            utility.Upload(request); //commensing the transfer  

            return true; //indicate that the file was sent  
        }
        public void SearchFaces(string fileName,string bucketName)
        {

            IAmazonRekognition rekoClient = new AmazonRekognitionClient(Amazon.RegionEndpoint.USEast1);
            var response = rekoClient.SearchFacesByImage(new SearchFacesByImageRequest 
            {
                CollectionId = "myphotos",
                FaceMatchThreshold = 95,
                Image = new Amazon.Rekognition.Model.Image
                {
                    S3Object = new Amazon.Rekognition.Model.S3Object
                    {

                        Bucket = bucketName,
                        Name = fileName,

                    }
                },
                MaxFaces = 5
            });
 


        }

        protected void btnAddToCollection_Click(object sender, EventArgs e)
        {
            Stream st = fuImage.PostedFile.InputStream;
            IAmazonRekognition rekoClient = new AmazonRekognitionClient(Amazon.RegionEndpoint.USEast1);
            string name = Path.GetFileName(fuImage.PostedFile.FileName);
            string myBucketName = "blazarstorage"; //your s3 bucket name goes here  
           // string s3DirectoryName = "";
            string s3FileName = @name;
            StoreFaceInCollection(@name, rekoClient, myBucketName);
        }

        
        //public static AmazonS3 GetS3Client()
        //{
        //    NameValueCollection appConfig = ConfigurationManager.AppSettings;

        //    AmazonS3 s3Client = AWSClientFactory.CreateAmazonS3Client(
        //            appConfig["AWSAccessKey"],
        //            appConfig["AWSSecretKey"]
        //            );
        //    return s3Client;
        //}

      
        #endregion
    }
}