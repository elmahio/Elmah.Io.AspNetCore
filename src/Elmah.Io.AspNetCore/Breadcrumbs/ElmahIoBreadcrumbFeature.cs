using System.Collections.Generic;
using System.Linq;

namespace Elmah.Io.AspNetCore.Breadcrumbs
{
    internal class ElmahIoBreadcrumbFeature(ElmahIoOptions options)
    {
        private const int MaximumCount = 10;
        private readonly ElmahIoOptions options = options;

        public List<Client.Breadcrumb> Breadcrumbs { get; } = [];

        public void Add(Client.Breadcrumb breadcrumb)
        {
            if (options.OnFilterBreadcrumb != null && options.OnFilterBreadcrumb(breadcrumb)) return;
            Breadcrumbs.Add(breadcrumb);

            if (Breadcrumbs.Count > MaximumCount)
            {
                var oldest = Breadcrumbs.OrderBy(b => b.DateTime).First();
                Breadcrumbs.Remove(oldest);
            }
        }
    }
}
