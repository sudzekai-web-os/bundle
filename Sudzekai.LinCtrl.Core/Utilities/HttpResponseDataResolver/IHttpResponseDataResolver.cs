
namespace Sudzekai.LinCtrl.Core 
{
    public interface IHttpResponseDataResolver
    {
        ContentType ContentType { get; }

        HttpResponse Resolve<T>(HttpResponse response, T data);
    }
}
