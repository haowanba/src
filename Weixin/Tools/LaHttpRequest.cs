using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Weixin.Tools
{
    public  class LaHttpRequest
    {
        public static string Get(string url)
        {
            HttpClientHandler handler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip };

            using (HttpClient http = new HttpClient(handler))
            {
                var response = http.GetAsync(url);
                //确保HTTP成功状态值
                response.Result.EnsureSuccessStatusCode();
                //await异步读取最后的JSON（注意此时gzip已经被自动解压缩了，因为上面的AutomaticDecompression = DecompressionMethods.GZip）
                Task<string> result = response.Result.Content.ReadAsStringAsync();
                string responseBodyAsText = result.Result;
                responseBodyAsText = responseBodyAsText.Replace("<br>", Environment.NewLine); // Insert new lines
                return responseBodyAsText;
            }
        }
    }
}
