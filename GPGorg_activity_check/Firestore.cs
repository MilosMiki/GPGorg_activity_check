using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebaseAdmin;
using Google.Cloud.Firestore;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using DotNetEnv;

namespace GPGorg_activity_check
{
    public partial class Firestore : Component
    {
        public static void UploadToFirestore(string json)
        {

            // Create a unique UID for the document
            string customUid = "total";

            // Upload the entire JSON as a single document
            DocumentReference docRef = Form1.Db.Collection("warnings").Document(customUid);
            docRef.SetAsync(new { Data = json }).Wait();
        }

        public static string GetFirebaseCredentialsJson()
        {
            // Load .env file
            Env.Load();

            // Load the private key from the environment variable
            string privateKey = Environment.GetEnvironmentVariable("FIREBASE_PRIVATE_KEY");

            // Don't escape newlines in the private key
            // privateKey = privateKey.Replace("\n", "\\n");

            // Create an anonymous object to represent the credentials
            var credentials = new
            {
                type = Environment.GetEnvironmentVariable("FIREBASE_TYPE"),
                project_id = Environment.GetEnvironmentVariable("FIREBASE_PROJECT_ID"),
                private_key_id = Environment.GetEnvironmentVariable("FIREBASE_PRIVATE_KEY_ID"),
                private_key = privateKey,
                client_email = Environment.GetEnvironmentVariable("FIREBASE_CLIENT_EMAIL"),
                client_id = Environment.GetEnvironmentVariable("FIREBASE_CLIENT_ID"),
                auth_uri = Environment.GetEnvironmentVariable("FIREBASE_AUTH_URI"),
                token_uri = Environment.GetEnvironmentVariable("FIREBASE_TOKEN_URI"),
                auth_provider_x509_cert_url = Environment.GetEnvironmentVariable("FIREBASE_AUTH_PROVIDER_X509_CERT_URL"),
                client_x509_cert_url = Environment.GetEnvironmentVariable("FIREBASE_CLIENT_X509_CERT_URL"),
                universe_domain = Environment.GetEnvironmentVariable("FIREBASE_UNIVERSE_DOMAIN")
            };

            // Serialize the object to JSON
            return JsonConvert.SerializeObject(credentials);
        }
    }
}
