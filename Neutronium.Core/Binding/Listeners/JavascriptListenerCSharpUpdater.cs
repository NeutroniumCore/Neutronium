using System;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.Updater;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.Listeners
{
    internal class JavascriptListenerCSharpUpdater: IJavascriptChangesListener
    {
        private readonly ICSharpChangesListener _IcSharpChangesListener;
        private readonly IJsUpdateHelper _JsUpdateHelper;
        private readonly IJavascriptToCSharpConverter _JavascriptToCSharpConverter;
        private readonly IWebSessionLogger _Logger;

        public JavascriptListenerCSharpUpdater(ICSharpChangesListener icSharpChangesListener, IJsUpdateHelper jsUpdateHelper, IJavascriptToCSharpConverter javascriptToCSharpConverter, IWebSessionLogger logger)
        {
            _IcSharpChangesListener = icSharpChangesListener;
            _JsUpdateHelper = jsUpdateHelper;
            _JavascriptToCSharpConverter = javascriptToCSharpConverter;
            _Logger = logger;
        }

        public async void OnJavaScriptCollectionChanges(JavascriptCollectionChanges changes)
        {
            try
            {
                var jsArray = _JsUpdateHelper.GetCachedFromJsObject<JsArray>(changes.Collection);
                if (jsArray == null)
                    return;

                var collectionChanges = jsArray.GetChanger(changes, _JavascriptToCSharpConverter);
                var updater = await _JsUpdateHelper.EvaluateOnUiContextAsync(() =>
                    UpdateCollectionAfterJavascriptChanges(jsArray, jsArray.CValue, collectionChanges)
                );

                if (updater?.HasUpdatesOnJavascriptContext != true)
                    return;

                await _JsUpdateHelper.RunOnJavascriptContextAsync(() =>
                    _JsUpdateHelper.UpdateOnJavascriptContext(updater)
                );
            }
            catch (Exception exception)
            {
                LogJavascriptSetException(exception);
            }
        }

        private BridgeUpdater UpdateCollectionAfterJavascriptChanges(JsArray array, object collection, CollectionChanges.CollectionChanges change)
        {
            var updater = default(BridgeUpdater);
            try
            {
                using (_IcSharpChangesListener.GetCollectionSilenter(collection))
                {
                    updater = array.UpdateEventArgsFromJavascript(change);
                }
                _JsUpdateHelper.UpdateOnUiContext(updater, _IcSharpChangesListener.Off);
            }
            catch (Exception exception)
            {
                LogJavascriptSetException(exception);
            }
            return updater;
        }

        public async void OnJavaScriptObjectChanges(IJavascriptObject objectChanged, string propertyName, IJavascriptObject newValue)
        {
            try
            {
                var currentFather = _JsUpdateHelper.GetCachedFromJsObject<JsGenericObject>(objectChanged);
                if (currentFather == null)
                    return;

                var propertyUpdater = currentFather.GetPropertyUpdater(propertyName);
                if (!propertyUpdater.IsSettable)
                {
                    LogReadOnlyProperty(propertyName);
                    return;
                }

                var glue = _JavascriptToCSharpConverter.GetCachedOrCreateBasic(newValue, propertyUpdater.TargetType);
                var bridgeUpdater = await _JsUpdateHelper.EvaluateOnUiContextAsync(() => UpdateOnUiContextChangeFromJs(propertyUpdater, glue));

                if (bridgeUpdater?.HasUpdatesOnJavascriptContext != true)
                    return;

                await _JsUpdateHelper.RunOnJavascriptContextAsync(() => _JsUpdateHelper.UpdateOnJavascriptContext(bridgeUpdater));
            }
            catch (Exception exception)
            {
                LogJavascriptSetException(exception);
            }
        }

        private BridgeUpdater UpdateOnUiContextChangeFromJs(AttributeUpdater propertyUpdater, IJsCsGlue glue)
        {
            var currentFather = propertyUpdater.Father;
            using (_IcSharpChangesListener.GetPropertySilenter(currentFather.CValue, propertyUpdater.PropertyName))
            {
                var oldValue = propertyUpdater.GetCurrentChildValue();

                try
                {
                    propertyUpdater.Set(glue.CValue);
                }
                catch (Exception exception)
                {
                    LogSetError(propertyUpdater.PropertyName, propertyUpdater.TargetType, glue.CValue, exception);
                }

                var actualValue = propertyUpdater.GetCurrentChildValue();

                if (Equals(actualValue, glue.CValue))
                {
                    var bridgeUpdater = currentFather.GetUpdaterChangeOnJsContext(propertyUpdater, glue);
                    _JsUpdateHelper.UpdateOnUiContext(bridgeUpdater, _IcSharpChangesListener.Off);
                    return bridgeUpdater;
                }

                if (!Equals(oldValue, actualValue))
                {
                    _IcSharpChangesListener.ReportPropertyChanged(currentFather.CValue, propertyUpdater.PropertyName);
                }

                return null;
            }
        }

        private void LogSetError(string propertyName, Type targetType, object @object, Exception exception)
        {
            _Logger.Info($"Unable to set C# from javascript object: property: {propertyName} of {targetType}, javascript value {@object}. Exception {exception} was thrown.");
        }

        private void LogReadOnlyProperty(string propertyName)
        {
            _Logger.Info(() => $"Unable to set C# from javascript object: property: {propertyName} is readonly.");
        }

        private void LogJavascriptSetException(Exception exception)
        {
            _Logger.Error(() => $"Unable to update ViewModel from View, exception raised: {exception.Message}");
        }
    }
}
