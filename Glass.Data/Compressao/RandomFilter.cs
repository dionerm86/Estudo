using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Linq;

namespace Glass.Data.Compressao
{
    public class RandomFilter : Stream
    {
        #region Construtores

        private HttpContext _context;
        private Stream _responseStream;

        public RandomFilter(HttpContext context, Stream responseStream)
        {
            _context = context;
            _responseStream = responseStream;
        }

        #endregion

        #region Propriedades

        public override bool CanRead
        {
            get { return _responseStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return _responseStream.CanSeek; ; }
        }

        public override bool CanTimeout
        {
            get { return _responseStream.CanTimeout; }
        }

        public override bool CanWrite
        {
            get { return _responseStream.CanWrite; }
        }

        public override void Flush()
        {
            _responseStream.Flush();
        }

        public override long Length
        {
            get { return _responseStream.Length; }
        }

        public override long Position
        {
            get { return _responseStream.Position; }
            set { _responseStream.Position = value; }
        }

        #endregion

        #region Métodos

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _responseStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _responseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _responseStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            string random = "?random=" + new Random().Next().ToString();
            
            string[] regex = new string[] {
                "((?<html>(href|src)\\s*=\\s*\")|(?<css>url\\())(?<url>.*?)(?(html)\"|\\))",
                "(src=(?<link>.*js[\'\"]))"
            };

            string[] extensoesValidas = new string[] {
                "js",
                "css"
            };

            int difCount = buffer.Length - offset - count;

            Regex objRegex = new Regex("(" + String.Join("|", regex) + ")");
            string html = System.Text.Encoding.UTF8.GetString(buffer);

            MatchCollection objCol = objRegex.Matches(html);
            foreach (Match m in objCol)
            {
                if (m.Value.Split('.', '?', '/', '\"', '\'').Intersect(extensoesValidas).Count() == 0)
                    continue;

                char final = m.Value[m.Value.Length - 1];
                string newJsValue = m.Value.TrimEnd(final) + random + final.ToString();
                html = html.Replace(m.Value, newJsValue);
            }

            buffer = System.Text.Encoding.UTF8.GetBytes(html);
            _responseStream.Write(buffer, offset, buffer.Length - difCount);
        }

        #endregion
    }
}