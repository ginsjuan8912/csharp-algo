using Azure.Storage.Blobs;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ginsjuan.Extensions
{
    public static class BlobContainerClientExtensions
    {
        /// <summary>
        /// Return a stream of a blob contents based by the blobName
        /// </summary>
        /// <param Name="client"></param>
        /// <param Name="blobName">The Name of the blob that needs to be downloaded as stream</param>
        /// <param Name="cancellationToken">A cancellation token to handle a cancelation signal if needed</param>
        /// <returns>Returns a stream with contents of the blob</returns>
        /// <exception cref="ArgumentException">Throws ArgumentException if the blobName is empty</exception>
        public async static Task<Stream> GetStreamByBlobNameAsync(this BlobContainerClient client, string blobName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(blobName)) throw new ArgumentException("blobName cannot be empty or null");

            MemoryStream stream = new MemoryStream();

            var blobClient = client.GetBlobClient(blobName);
            await blobClient.DownloadToAsync(stream, cancellationToken).ConfigureAwait(false);

            return stream;
        }

        /// <summary>
        /// Uploads a stream content passed by parameter using the blobName,
        /// if the process is completed successfuly returns true, otherwise, returns false
        /// </summary>
        /// <param Name="client">Extension reference</param>
        /// <param Name="contents">A stream with the contents to be uploaded</param>
        /// <param Name="blobName">The Name of the blob that is going to be written</param>
        /// <param Name="cancellationToken">A cancellation token to handle a cancelation signal if needed</param>
        /// <returns>True if the process was completed successfuly, otherwise, return false</returns>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if the contents to upload are null</exception>
        /// <exception cref="ArgumentException">Throws ArgumentException if the blobName is empty</exception>
        public async static Task<bool> UploadAsync(this BlobContainerClient client, Stream contents, string blobName, CancellationToken cancellationToken)
        {
            if (client == null) throw new ArgumentNullException(nameof(contents));
            if (string.IsNullOrEmpty(blobName)) throw new ArgumentException("blobName cannot be empty or null");

            try
            {
                contents.Seek(0, SeekOrigin.Begin);

                var blobClient = client.GetBlobClient(blobName);
                var resultTask = await blobClient.UploadAsync(contents, true, cancellationToken).ConfigureAwait(false);

                return resultTask.GetRawResponse() != null ?
                        resultTask.GetRawResponse().Status.ToString().StartsWith("20") : false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
