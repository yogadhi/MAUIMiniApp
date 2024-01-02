using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAP.Libs.Models
{
    public class RootItem
    {
        public IServiceProvider Provider { get; set; }
        public List<FlyoutPageItem> MenuItemList { get; set; }
        public int SelectedMenuIndex { get; set; } = -1;
    }
}
