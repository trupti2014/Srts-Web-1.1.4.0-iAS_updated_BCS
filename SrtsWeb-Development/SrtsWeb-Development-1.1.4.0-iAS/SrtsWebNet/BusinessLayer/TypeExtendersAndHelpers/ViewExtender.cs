using System;
using System.Reflection;

namespace SrtsWeb.BusinessLayer.TypeExtendersAndHelpers.Extenders
{
    public static partial class SrtsExtender
    {
        public static T ResetViewProperties<T>(this T tIn)
        {
            if (tIn == null) return default(T);

            foreach (PropertyInfo pi in tIn.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (pi.GetSetMethod() != null &&
                    !pi.Name.ToLower().Equals("mysession") &&
                    !pi.Name.ToLower().Equals("lookupcache"))
                {
                    if (pi.GetMethod != null && pi.GetMethod.IsFinal)
                        try { pi.SetValue(tIn, null, null); }
                        catch { }
                }
            }

            return tIn;
        }

        public static T SetControlStateTbDdlRbl<T>(this T collectionIn, Boolean isEnabled)
            where T : System.Web.UI.ControlCollection
        {
            foreach (System.Web.UI.Control c in collectionIn)
            {
                if (c.HasControls()) c.Controls.SetControlStateTbDdlRbl(isEnabled);

                if (c is System.Web.UI.WebControls.TextBox)
                    ((System.Web.UI.WebControls.TextBox)c).Enabled = isEnabled;
                else if (c is System.Web.UI.WebControls.DropDownList)
                    ((System.Web.UI.WebControls.DropDownList)c).Enabled = isEnabled;
                else if (c is System.Web.UI.WebControls.RadioButtonList)
                    ((System.Web.UI.WebControls.RadioButtonList)c).Enabled = isEnabled;
            }

            return collectionIn;
        }

        public static T SetControlStateBtn<T>(this T collectionIn, Boolean isEnabled)
            where T : System.Web.UI.ControlCollection
        {
            foreach (System.Web.UI.Control c in collectionIn)
            {
                if (c.HasControls()) c.Controls.SetControlStateBtn(isEnabled);

                if (c is System.Web.UI.WebControls.Button)
                    ((System.Web.UI.WebControls.Button)c).Enabled = isEnabled;
            }

            return collectionIn;
        }
    }
}