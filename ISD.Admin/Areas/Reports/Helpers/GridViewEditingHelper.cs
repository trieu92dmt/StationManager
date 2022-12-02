using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.Extensions
{
    public class GridViewEditingHelper
    {
        static List<GridViewEditingMode> availableEditModesList;
        public static List<GridViewEditingMode> AvailableEditModesList
        {
            get
            {
                if (availableEditModesList == null)
                    availableEditModesList = new List<GridViewEditingMode> {
                        GridViewEditingMode.Inline,
                        GridViewEditingMode.EditForm,
                        GridViewEditingMode.EditFormAndDisplayRow,
                        GridViewEditingMode.PopupEditForm
                    };
                return availableEditModesList;
            }
        }
    }
}
