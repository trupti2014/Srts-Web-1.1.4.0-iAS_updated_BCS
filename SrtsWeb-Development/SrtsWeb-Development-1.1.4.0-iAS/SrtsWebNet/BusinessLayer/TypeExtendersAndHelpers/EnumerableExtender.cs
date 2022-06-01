using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SrtsWeb.BusinessLayer.TypeExtendersAndHelpers.Extenders
{
    public static partial class SrtsExtender
    {
        public static ListBox Sort(this ListBox listIn){
            var l = new SortedList();
            listIn.Items.Cast<ListItem>().ToList().ForEach(x => l.Add(x.Text, x.Value));
            listIn.Items.Clear();
            foreach (DictionaryEntry li in l)
                listIn.Items.Add(new ListItem(li.Key.ToString(), li.Value.ToString()));
            return listIn;
        }
        public static List<ListItem> GetItemsList(this ListBox listIn)
        {
            if (listIn.Items.Count.Equals(0)) return new List<ListItem>() { new ListItem() };
            return listIn.Items.Cast<ListItem>().ToList();
        }
    }
}
