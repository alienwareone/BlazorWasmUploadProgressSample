using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorWasmUploadProgressSample.Client
{
    public class ProgressableStreamContent : HttpContent
    {
        private readonly HttpContent _content;
        private readonly int _bufferSize;
        private readonly Action<long, long> _progress;

        public ProgressableStreamContent(HttpContent content, int bufferSize, Action<long, long> progress)
        {
            _content = content;
            _bufferSize = bufferSize;
            _progress = progress;

            foreach (var h in content.Headers)
            {
                Headers.Add(h.Key, h.Value);
            }
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext? context)
        {
            var buffer = new byte[_bufferSize];
            TryComputeLength(out var size);
            var uploaded = 0;

            using (var inputStream = await _content.ReadAsStreamAsync())
            {
                while (true)
                {
                    var length = await inputStream.ReadAsync(buffer, 0, buffer.Length);
                    if (length <= 0) break;

                    uploaded += length;
                    _progress?.Invoke(uploaded, size);

                    // https://stackoverflow.com/questions/60584294/blazor-webassembly-how-to-get-ui-to-update-during-long-running-non-async-proce
                    await Task.Delay(1);

                    await stream.WriteAsync(buffer, 0, length);
                    await stream.FlushAsync();
                }
            }

            //await stream.FlushAsync();
        }

        protected override bool TryComputeLength(out long length)
        {
            length = _content.Headers.ContentLength.GetValueOrDefault();
            return false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _content.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}