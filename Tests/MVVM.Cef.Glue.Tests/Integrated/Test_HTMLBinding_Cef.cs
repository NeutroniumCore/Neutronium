using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Xunit;

using NSubstitute;
using FluentAssertions;
using Newtonsoft.Json;
using MVVM.ViewModel.Example;
using MVVM.ViewModel;
using MVVM.ViewModel.Infra;
using MVVM.Component;
using MVVM.HTML.Core;
using MVVM.HTML.Core.Exceptions;
using MVVM.HTML.Core.Binding.GlueObject;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

using MVVM.Cef.Glue.Test.Core;

using IntegratedTest;
using IntegratedTest.TestData;
using IntegratedTest.Windowless;
using MVVM.Cef.Glue.Test.Infra;

namespace MVVM.Cef.Glue.Test
{
    public class Test_HTMLBinding_Cef : Test_HTMLBinding
    {
        public Test_HTMLBinding_Cef(): base(CefTestHelper.GetWindowlessEnvironment())
        {
        }
    }
}

