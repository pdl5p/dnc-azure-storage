using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Azure.KeyVault;
//using Microsoft.Azure.KeyVault;


[assembly: UserSecretsId("dnc-azure-storage")]

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            (new Program()).Run().Wait();
        }

        private async Task Run()
        {

            var builder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddEnvironmentVariables()
                    .AddUserSecrets();

            var config = builder.Build();
            var settings = new AppSettings();
            config.Bind(settings);

            var keyVault = new KeyVaultClient(async (string authority, string resource, string scope) =>
            {

                var authContext = new AuthenticationContext(authority);
                var credential = new ClientCredential(settings.TheChillerId, settings.TheChillerKey);
                var token = await authContext.AcquireTokenAsync(resource, credential);

                return token.AccessToken;
            });

            var storageConnection = keyVault.GetSecretAsync(settings.StorageConnectionVaultUrl).Result.Value;

            CloudStorageAccount csa = CloudStorageAccount.Parse(storageConnection);

            CloudTableClient ctc = csa.CreateCloudTableClient();

            CloudTable table = ctc.GetTableReference("crazytable");

            await table.CreateIfNotExistsAsync();

            var ce = new CustomerEntity("Pan", "Peter")
            {
                Email = "pete@pantry.com",
                PhoneNumber = "0409111222",
                Country = "Austria",
                Twitter = "@pp"
            };

            var bop = new TableBatchOperation();

            //var op = TableOperation.InsertOrMerge(ce);
            //var op = TableOperation.Insert(ce);
            bop.InsertOrReplace(ce);

            try
            {
                //table.ExecuteAsync(op).Wait();
                await table.ExecuteBatchAsync(bop);

            }
            catch (Exception ex)
            {

                var y = ex;
            }
            Console.WriteLine("Done!");
        }
    }
}
