using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NLite.Net;

namespace NLite.Domain
{
    

    [Serializable]
    public class HttpPostedFileVO
    {
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public byte[] Content { get; set; }
    }

    public class HttpPostedFileWrapper:IHttpPostedFile
    {

        public static IHttpPostedFile From(HttpPostedFileVO vo)
        {
            if (vo == null || vo.Content == null || vo.Content.Length == null)
                return null;
            var wrapper = new HttpPostedFileWrapper();
            wrapper.FileName = vo.FileName;
            wrapper.ContentType = vo.ContentType;
            wrapper.ContentLength = vo.Content.Length;
            wrapper.InputStream = new MemoryStream(vo.Content);
            return wrapper;
        }

        public static HttpPostedFileVO ToVO(IHttpPostedFile file)
        {
            if (file == null || file.ContentLength == 0)
                return null;

            var vo = new HttpPostedFileVO();
            vo.FileName = file.FileName;
            vo.ContentType = file.ContentType;
            vo.Content = new byte[file.InputStream.Length];
            file.InputStream.Read(vo.Content, 0, vo.Content.Length);
            return vo;
        }

        public int ContentLength { get; private set; }

        public string ContentType { get; private set; }

        public string FileName { get; private set; }

        public Stream InputStream { get; private set; }

        public void SaveAs(string filename)
        {
            if (ContentLength == 0)
                return;

            try
            {
                var bytes = new byte[InputStream.Length];
                InputStream.Read(bytes, 0, bytes.Length);
                if (!File.Exists(filename))
                    File.WriteAllBytes(filename, bytes);
            }
            catch
            {
            }
        }
    }
}
