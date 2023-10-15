using Core.Utilities.Business.Abstract;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities.Business.Concrete
{
    public class UriManager : IUriService
    {
        private readonly string _baseUri;
        public UriManager(string baseUri)
        {
            _baseUri = baseUri;
        }
        public Uri GetPageUri(int pNumber, int pSize, string route)
        {
            var _enpointUri = new Uri(string.Concat(_baseUri, route));
            var modifiedUri = QueryHelpers.AddQueryString(_enpointUri.ToString(), "pageNumber", pNumber.ToString());
            modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "pageSize", pSize.ToString());
            return new Uri(modifiedUri);
        }
    }
}
