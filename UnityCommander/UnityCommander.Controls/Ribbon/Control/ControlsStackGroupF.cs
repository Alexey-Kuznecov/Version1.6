using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace UnityCommander.Controls.Ribbon.Control
{
    partial class ControlsStackGroupF : Panel
    {
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            //bool etwTracingEnabled = IsScrolling && EventTrace.IsEnabled(EventTrace.Keyword.KeywordGeneral, EventTrace.Level.Info);
            //if (etwTracingEnabled)
            //{
            //    EventTrace.EventProvider.TraceEvent(EventTrace.Event.WClientStringBegin, EventTrace.Keyword.KeywordGeneral, EventTrace.Level.Info, "STACK:ArrangeOverride");
            //}
            //try
            //{
            //    // Call the arrange helper.
            //    StackArrangeHelper(this, _scrollData, arrangeSize);
            //}
            //finally
            //{
            //    if (etwTracingEnabled)
            //    {
            //        EventTrace.EventProvider.TraceEvent(EventTrace.Event.WClientStringEnd, EventTrace.Keyword.KeywordGeneral, EventTrace.Level.Info, "STACK:ArrangeOverride");
            //    }
            //}

            return arrangeSize;
        }
    }
}
