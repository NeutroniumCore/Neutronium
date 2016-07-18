using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.HTML.Core.JavascriptUIFramework {
    public interface IJavascriptViewModelManager {

        /// <summary>
        /// Returns an IJavascriptSessionInjector 
        /// </summary>
        IJavascriptSessionInjector Injector { get; }

        /// <summary>
        /// Returns an IJavascriptViewModelUpdater 
        /// </summary>
        IJavascriptViewModelUpdater ViewModelUpdater { get; }
    }
}
