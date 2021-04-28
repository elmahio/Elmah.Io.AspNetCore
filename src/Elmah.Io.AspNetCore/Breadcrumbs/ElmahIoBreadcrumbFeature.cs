using System.Collections.Generic;
using System.Linq;

namespace Elmah.Io.AspNetCore.Breadcrumbs
{
    internal class ElmahIoBreadcrumbFeature
    {
        private const int MaximumCount = 10;
        private readonly ElmahIoOptions options;

        public List<Client.Models.Breadcrumb> Breadcrumbs { get; }

        public ElmahIoBreadcrumbFeature(ElmahIoOptions options)
        {
            this.options = options;
            Breadcrumbs = new List<Client.Models.Breadcrumb>();
        }

        public void Add(Client.Models.Breadcrumb breadcrumb)
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
