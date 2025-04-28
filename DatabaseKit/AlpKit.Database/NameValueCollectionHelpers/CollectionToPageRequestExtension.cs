using AlpKit.Database.DynamicFilter.Models;
using System.Collections.Specialized;

namespace AlpKit.Database.NameValueCollectionHelpers;

public static class CollectionToPageRequestExtension
{
    public const string PageQueryStringName = "Page";

    public const string PageSizeQueryStringName = "PageSize";

    public static PageRequest ToPageRequest(this NameValueCollection nvc)
    {
        PageRequest pageRequest = new PageRequest
        {
            PageIndex = 1,
            PageSize = 10,
        };

        if (int.TryParse(nvc[PageQueryStringName], out int page))
        {
            if (page <= 0)
            {
                page = 1;
            }
            pageRequest.PageIndex = page;
        }

        if (int.TryParse(nvc[PageSizeQueryStringName], out int pageSize))
        {
            pageRequest.PageSize = pageSize;
        }

        return pageRequest;

    }
}
