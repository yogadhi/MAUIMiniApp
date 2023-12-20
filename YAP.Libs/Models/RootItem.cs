using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAP.Libs.Flyouts;

namespace YAP.Libs.Models
{
    public class RootItem
    {
        public IServiceProvider Provider { get; set; }
        public List<FlyoutPageItem> MenuItemList { get; set; }
    }
}
