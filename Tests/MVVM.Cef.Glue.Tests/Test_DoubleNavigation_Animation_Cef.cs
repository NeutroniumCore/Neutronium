using System;
using System.Threading;
using System.Windows.Controls;

using FluentAssertions;
using Xunit;

using MVVM.ViewModel;
using MVVM.HTML.Core;
using HTML_WPF.Component;
using MVVM.HTML.Core.Navigation;
using KnockoutUIFramework;
using IntegratedTest;
using Integrated.WPFInfra;
using IntegratedTest.WPF;
using MVVM.Cef.Glue.Test.Infra;

namespace MVVM.Cef.Glue.Test
{
    public class Test_DoubleNavigation_Animation_Cef : Test_DoubleNavigation_Animation
    {
        protected override WindowTestEnvironment GetEnvironment()
        {
            return CefTestHelper.GetEnvironment();
        }    
    }
}
