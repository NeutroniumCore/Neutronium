using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

using MVVM.HTML.Core.Infra.VM;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.Exceptions;
using MVVM.HTML.Core;
using MVVM.HTML.Core.Navigation;

using MVVM.Cef.Glue.CefSession;
using MVVM.Cef.Glue.Navigation;


namespace MVVM.Cef.Glue
{
    public partial class HTMLViewControl : HTMLControlBase, IWebViewLifeCycleManager, IDisposable
    {
    

        public static readonly DependencyProperty UriProperty = DependencyProperty.Register("Uri", typeof(Uri), typeof(HTMLViewControl));

        public Uri Uri
        {
            get { return (Uri)this.GetValue(UriProperty); }
            internal set { this.SetValue(UriProperty, value); }
        }

        public string RelativeSource
        {
            set 
            {
                string path = string.Format("{0}\\{1}", Assembly.GetExecutingAssembly().GetPath(), value);
                if (!File.Exists(path))
                    throw ExceptionHelper.Get(string.Format("Path not found {0}",path));
                Uri = new Uri(path); 

                if (DataContext!=null)
                    this.NavigateAsyncBase(DataContext, null, Mode);
            }          
        }


        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(JavascriptBindingMode), typeof(HTMLViewControl), new PropertyMetadata(JavascriptBindingMode.TwoWay));

        public JavascriptBindingMode Mode
        {
            get { return (JavascriptBindingMode)this.GetValue(ModeProperty); }
            set { this.SetValue(ModeProperty, value); }
        }

        private UrlSolver _UrlSolver;

        public HTMLViewControl():this(new UrlSolver())
        {
        }

        private HTMLViewControl(UrlSolver iIUrlSolver) : base(iIUrlSolver)
        {
            _UrlSolver= iIUrlSolver;
            _UrlSolver.Solver = this;
            this.DataContextChanged += HTMLViewControl_DataContextChanged;
        }

        private void HTMLViewControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Uri!=null)
                this.NavigateAsyncBase(DataContext, null, Mode);
        }

        private class UrlSolver : IUrlSolver
        {
            public HTMLViewControl Solver { get; set; }

            Uri IUrlSolver.Solve(object iViewModel, string Id)
            {
                return Solver.Uri;
            }
        }

    }
}