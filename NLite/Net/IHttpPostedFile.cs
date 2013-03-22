using System;
using System.IO;
namespace NLite.Net
{
    public interface IHttpPostedFile
    {
        // Summary:
        //     When overridden in a derived class, gets the size of an uploaded file, in
        //     bytes.
        //
        // Returns:
        //     The length of the file, in bytes.
        //
        // Exceptions:
        //   System.NotImplementedException:
        //     Always.
        int ContentLength { get; }
        //
        // Summary:
        //     When overridden in a derived class, gets the MIME content type of an uploaded
        //     file.
        //
        // Returns:
        //     The MIME content type of the file.
        //
        // Exceptions:
        //   System.NotImplementedException:
        //     Always.
        string ContentType { get; }
        //
        // Summary:
        //     When overridden in a derived class, gets the fully qualified serviceDispatcherName of the
        //     file on the client.
        //
        // Returns:
        //     The serviceDispatcherName of the file on the client, which includes the directory path.
        //
        // Exceptions:
        //   System.NotImplementedException:
        //     Always.
        string FileName { get; }
        //
        // Summary:
        //     When overridden in a derived class, gets a System.IO.Stream object that points
        //     to an uploaded file to prepare for reading the contents of the file.
        //
        // Returns:
        //     An object for reading a file.
        //
        // Exceptions:
        //   System.NotImplementedException:
        //     Always.
        Stream InputStream { get; }

        // Summary:
        //     When overridden in a derived class, saves the contents of an uploaded file.
        //
        // Parameters:
        //   filename:
        //     The serviceDispatcherName of the file to save.
        //
        // Exceptions:
        //   System.NotImplementedException:
        //     Always.
        void SaveAs(string filename);
    }
}
