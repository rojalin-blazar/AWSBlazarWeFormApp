using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;


using Amazon.S3;
using Amazon.S3.Transfer;

using Amazon.S3.IO;
using Amazon.S3.Model;

namespace AWSBlazarWeFormApp
{
    public partial class Bucket : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnCreateBucket_Click(object sender, EventArgs e)
        {
            createBucket(txtCreateBucket.Text);
        }

        public void createBucket(string bucketName)
        {
            AmazonS3Client client = new AmazonS3Client("AKIAIYKAM62F6DZ2CEGA", "HmHyI439/ZdyOOxjnrpW3izOzOWcu3kS5qwpV1Kd", RegionEndpoint.USEast1);
            string BUCKET_NAME = bucketName;
            ListBucketsResponse response = client.ListBuckets();
            bool found = false;
            foreach (S3Bucket bucket in response.Buckets)
            {
                if (bucket.BucketName == BUCKET_NAME)
                {
                    found = true;
                    break;
                }
            }
            if (found == false)
            {
                // Create a client
                // AmazonS3Client client = new AmazonS3Client();

                // Construct request
                PutBucketRequest request = new PutBucketRequest
                {
                    BucketName = bucketName,
                    BucketRegion = S3Region.US,         // set region to EU
                    CannedACL = S3CannedACL.PublicRead  // make bucket publicly readable
                };

                // Issue call
                client.PutBucket(request);
            }
        }
    }
}