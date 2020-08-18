using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rossvyaz2
{
    class Lists
    {
        public List<string> Selected = new List<string>();
        public List<string> NoSelect = new List<string>();
        public void Select(string Item)
        {
            NoSelect.Remove(Item);
            Selected.Add(Item);
        }
        public void UnSelect(string Item)
        {
            Selected.Remove(Item);
            NoSelect.Add(Item);
        }
    }
}
