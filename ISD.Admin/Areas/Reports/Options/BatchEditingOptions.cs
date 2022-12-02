using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reports.Options
{
    public class BatchEditingOptions
    {
        public BatchEditingOptions()
        {
            PagerMode = GridViewPagerMode.EndlessPaging;
            EditMode = GridViewBatchEditMode.Cell;
            StartEditAction = GridViewBatchStartEditAction.FocusedCellClick;
            HighlightDeletedRows = true;
            KeepChangesOnCallbacks = true;
        }
        public GridViewPagerMode PagerMode { get; set; }
        public GridViewBatchEditMode EditMode { get; set; }
        public GridViewBatchStartEditAction StartEditAction { get; set; }
        public bool HighlightDeletedRows { get; set; }
        public bool KeepChangesOnCallbacks { get; set; }

        public void Assign(BatchEditingOptions source)
        {
            EditMode = source.EditMode;
            StartEditAction = source.StartEditAction;
            HighlightDeletedRows = source.HighlightDeletedRows;
            PagerMode = source.PagerMode;
        }
    }
}