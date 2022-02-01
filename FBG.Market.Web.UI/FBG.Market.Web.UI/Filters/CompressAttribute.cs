using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace FBG.Market.Web.Identity.Filters
{
    /// <summary>
    /// Creates and empty filter for HttpResponse.Filter (i.e no proessing is done on the text before writting to the stream)
    /// </summary>
    class EmptyFilter : MemoryStream
    {
        private string source = string.Empty;
        private readonly Stream filter;


        public EmptyFilter(HttpResponse httpResponseBase)
        {
            this.filter = httpResponseBase.Filter;
        }


        public override void Write(byte[] buffer, int offset, int count)
        {
            this.source = Encoding.UTF8.GetString(buffer);

            this.filter.Write(Encoding.UTF8.GetBytes(this.source), offset, Encoding.UTF8.GetByteCount(this.source));
        }
    }
    public class CompressAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            HttpRequestBase request = filterContext.HttpContext.Request;

            string acceptEncoding = request.Headers["Accept-Encoding"];

            if (string.IsNullOrEmpty(acceptEncoding)) return;

            acceptEncoding = acceptEncoding.ToUpperInvariant();

            HttpResponseBase response = filterContext.HttpContext.Response;

            if (acceptEncoding.Contains("GZIP"))
            {
                response.AppendHeader("Content-encoding", "gzip");
                
                response.Filter = new EmptyFilter(HttpContext.Current.Response);
                response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
            }
            else if (acceptEncoding.Contains("DEFLATE"))
            {
                response.AppendHeader("Content-encoding", "deflate");
                response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
            }
        }
    }
} 