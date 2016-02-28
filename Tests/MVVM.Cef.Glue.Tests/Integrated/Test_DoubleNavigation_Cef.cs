using System;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using System.Windows.Input;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Xunit;
using FluentAssertions;
using NSubstitute;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core;
using MVVM.ViewModel.Infra;
using MVVM.ViewModel;
using HTML_WPF.Component;
using MVVM.HTML.Core.Exceptions;
using MVVM.HTML.Core.Navigation;
using KnockoutUIFramework;
using IntegratedTest;
using Integrated.WPF;
using MVVM.Cef.Glue.Test.Infra;

namespace MVVM.Cef.Glue.Test
{
    public class Test_DoubleNavigation_Cef : Test_DoubleNavigation
    {
        protected override WindowTestEnvironment GetEnvironment()
        {
            return CefTestHelper.GetWindowEnvironment();
        }
    }
}
