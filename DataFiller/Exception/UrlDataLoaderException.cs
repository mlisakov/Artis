using System;

namespace Artis.DataLoader
{
    public class UrlDataLoaderException : Exception
    {
        public UrlDataLoaderException(string message):base(message)
        {
        }
        public UrlDataLoaderException(string url, string message):base(message)
        {
        }
    }
}
