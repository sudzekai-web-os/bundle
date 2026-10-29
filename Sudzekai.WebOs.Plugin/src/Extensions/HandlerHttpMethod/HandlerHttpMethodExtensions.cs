using Sudzekai.WebOs.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sudzekai.WebOs.Plugin
{
    public static class HandlerHttpMethodExtensions
    {
        extension (HandlerHttpMethod method)
        {
            public string ToMethodString()
                => method switch
                {
                    HandlerHttpMethod.Get => "get",
                    HandlerHttpMethod.Post => "post",
                    HandlerHttpMethod.Put => "put",
                    HandlerHttpMethod.Patch => "patch",
                    HandlerHttpMethod.Delete => "delete",
                    _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
                };
        }
    }
}
