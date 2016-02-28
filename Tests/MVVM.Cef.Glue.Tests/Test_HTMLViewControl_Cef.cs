using System;
using System.Windows.Controls;
using System.Threading;
using System.Reflection;

using Xunit;
using FluentAssertions;

using MVVM.HTML.Core.Infra;
using MVVM.ViewModel.Example;
using MVVM.HTML.Core.Exceptions;
using System.IO;
using MVVM.HTML.Core;
using HTML_WPF.Component;
using System.Threading.Tasks;
using MVVM.HTML.Core.Navigation;
using IntegratedTest;
using Integrated.WPFInfra;
using IntegratedTest.WPF;
using MVVM.Cef.Glue.Test.Infra;

namespace MVVM.Cef.Glue.Test
{
    public class Test_HTMLViewControl_Cef : Test_HTMLViewControl
    {
        protected override WindowTestEnvironment GetEnvironment()
        {
            return CefTestHelper.GetEnvironment();
        }
    }
}
