namespace Hua.DDNS.Common.Http
{
    public class HttpResult<T>
    {
            public virtual T Data { get; set; }

            public string DataDescription { get; set; }

            public int Result { get; set; }

            public string Message { get; set; }

    }
}