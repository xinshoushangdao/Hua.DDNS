using System.Net;
using System.Net.Http.Json;
using Newtonsoft.Json;

namespace Hua.DDNS.Common.Http
{
    public class HttpHelper: IHttpHelper
    {
        private static ILogger<HttpHelper> _logger;
        private static HttpClientHandler _handler;

        public HttpHelper(ILogger<HttpHelper> logger)
        {
            _logger = logger;
            _handler = new HttpClientHandler();
        }

        public HttpClient GetHttpClient()
        {
            return new HttpClient(_handler){};
        }

        /// <summary>
        /// PostAsync
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="url"></param>
        /// <param name="input"></param>
        /// <param name="timeOut">超时时间</param>
        /// <returns></returns>
        public async Task<TOut?> PostAsync<TIn, TOut>(string url, TIn input, int timeOut = 10)
        {   
            try
            {
                var client = GetHttpClient();
                client.Timeout = new TimeSpan(0, 10, timeOut);
                _logger.LogDebug($"Post:{url}\n[{JsonConvert.SerializeObject(input)}]");
                var result = await client.PostAsync(url, JsonContent.Create(input));
                var strResult = await result.Content.ReadAsStringAsync();
                if (result.StatusCode != HttpStatusCode.OK)
                {
                    _logger.LogDebug($"Error[{result.StatusCode}]:{url}\t{strResult}");
                }
                return await result.Content.ReadFromJsonAsync<TOut>();
            }
            catch (Exception e)
            {
                _logger.LogError(url);
                _logger.LogError(e.Message);
                throw;
            }
        }

        /// <summary>
        /// PostAsync
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="url"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public async Task<TOut?> GetAsync<TOut>(string url,int timeOut = 10)
        {
            try
            {
                var client = GetHttpClient();
                client.Timeout = new TimeSpan(0, 10, timeOut);
                _logger.LogDebug($"Get:{url}");
                var result = await client.GetAsync(url);
                var strResult = await result.Content.ReadAsStringAsync();
                if (result.StatusCode != HttpStatusCode.OK)
                {
                    _logger.LogDebug($"Error[{result.StatusCode}]:{url}\t{strResult}");
                }
                return await result.Content.ReadFromJsonAsync<TOut>();
            }
            catch (Exception e)
            {
                _logger.LogError(url);
                _logger.LogError(e.Message);
                throw;
            }
        }

        #region 下载文件

        /// <summary>
        /// http下载文件 (仅支持小文件)
        /// </summary>
        /// <param name="url">下载文件地址</param>
        /// <param name="localPath">文件存放地址，包含文件名</param>
        /// <returns></returns>
        public bool DownloadFile(string url, string localPath)
        {
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;

            var request = WebRequest.Create(url) as HttpWebRequest;
            Stream stream = new FileStream(localPath, FileMode.CreateNew);
            try
            {
                // 设置参数
                //发送请求并获取相应回应数据
                var response = request?.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                var responseStream = response?.GetResponseStream();
                //创建本地文件写入流
                stream.Close();
                responseStream?.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion


        /// <summary>
        /// 获得当前机器的公网 Ip
        /// </summary>
        public async Task<string> GetCurrentPublicIpv4()
        {
            using var client = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Get, "http://175.24.175.136:8008/WebUtil/GetIp");
            using var response = await client.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }
    }


    public interface IHttpHelper
    {
        public Task<string> GetCurrentPublicIpv4();
        public Task<TOut?> PostAsync<TIn, TOut>(string url, TIn input, int timeOut = 10);
        public Task<TOut?> GetAsync<TOut>(string url, int timeOut = 10);
        public bool DownloadFile(string url, string fileFullName);
        public HttpClient GetHttpClient();
    }
}
