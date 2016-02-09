using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections;

namespace MVVM.HTML.Core.HTMLBinding
{
    public abstract class GlueBase
    {
        protected abstract void ComputeString(StringBuilder sb, HashSet<IJSCSGlue> alreadyComputed);

        public void BuilString(StringBuilder sb, HashSet<IJSCSGlue> alreadyComputed)
        {
            if (!alreadyComputed.Add(this as IJSCSGlue))
                return;

            ComputeString(sb, alreadyComputed);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            BuilString(sb, new HashSet<IJSCSGlue>());
            return sb.ToString();
        }

        //public void ApplyOnListenable(IJSCSGlueListenableVisitor ivisitor)
        //{
        //    var jsGlue = (IJSCSGlue)this;
        //    foreach (var child in jsGlue.GetAllChildren(true).Distinct())
        //    {
        //        var c_childvalue = child.CValue;
        //        var NotifyCollectionChanged = c_childvalue as INotifyCollectionChanged;
        //        if (NotifyCollectionChanged != null)
        //        {
        //            ivisitor.OnCollection(NotifyCollectionChanged);
        //            continue;
        //        }

        //        var NotifyPropertyChanged = c_childvalue as INotifyPropertyChanged;
        //        if ((NotifyPropertyChanged != null) && !(child is IEnumerable))
        //            ivisitor.OnObject(NotifyPropertyChanged);

        //        if (child.Type == JSCSGlueType.Command)
        //            ivisitor.OnCommand(child as JSCommand);
        //    }
        //}
    }
}
