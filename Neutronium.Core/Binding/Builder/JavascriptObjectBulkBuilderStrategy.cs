using Neutronium.Core.Binding.Builder.Packer;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System;
using System.Collections.Generic;
using System.Linq;
using Neutronium.Core.Extension;
using Neutronium.Core.Infra.Reflection;

namespace Neutronium.Core.Binding.Builder 
{
    internal class JavascriptObjectBulkBuilderStrategy : IJavascriptObjectBuilderStrategy, IBulkUpdater
    {
        public IJavascriptObject CommandConstructor => _Factory.Value.CommandConstructor;
        public IJavascriptObject ExecutableConstructor => _Factory.Value.ExecutableConstructor;

        private readonly IWebView _WebView;
        private readonly IJavascriptSessionCache _Cache;
        private readonly Lazy<BulkJsHelper> _Factory;
        private readonly bool _Mapping;
        private IJavascriptObject BulkObjectsUpdater => _Factory.Value.BulkObjectsUpdater;
        private IJavascriptObject BulkArraysUpdater => _Factory.Value.BulkArraysUpdater;

        public JavascriptObjectBulkBuilderStrategy(IWebView webView, IJavascriptSessionCache cache, bool mapping)
        {
            _Mapping = mapping;
            _WebView = webView;
            _Cache = cache;
            _Factory = new Lazy<BulkJsHelper>(FactoryBuilder);
        }

        public void UpdateJavascriptValue(IJsCsGlue root)
        {
            var builder = new JavascriptObjectBulkBuilder(_WebView.Factory, _Cache, this, root, _Mapping);
            builder.UpdateJavascriptValue();
        }

        private BulkJsHelper FactoryBuilder()
        {
            var script =
                 @"(function(){
                    function Command(id, canExecute){
                        Object.defineProperty(this, '{{NeutroniumConstants.ObjectId}}', {value: id});
                        Object.defineProperty(this, '{{NeutroniumConstants.ReadOnlyFlag}}', {value: {{ReadOnlyObservable}}});
                        this.CanExecuteCount = 1;
                        this.CanExecuteValue = canExecute;
                    }
                    Command.prototype.Execute = function() {
                        this.privateExecute(this.{{NeutroniumConstants.ObjectId}}, ...arguments)
                    }
                    Command.prototype.CanExecute = function() {
                        this.privateCanExecute(this.{{NeutroniumConstants.ObjectId}}, ...arguments)
                    }
                    function Executable(id){
                        Object.defineProperty(this, '{{NeutroniumConstants.ObjectId}}', {value: id});
                        Object.defineProperty(this, '{{NeutroniumConstants.ReadOnlyFlag}}', {value: {{ReadOnly}}});
                    }
                    Executable.prototype.Execute = function() {
                        this.privateExecute(this.{{NeutroniumConstants.ObjectId}}, ...arguments)
                    }
                    function bulkUpdateObjects(prop){
						const propsArrays = eval( prop )
						var objectCount = 1
						var propsArrayLength = propsArrays.length;
						var propIndex = 0
						
						while(propsArrayLength--){
							const propArray = propsArrays[propIndex++]
							var repetition = propArray.c
							var properties = propArray.a
							while(repetition--){
								const objectToUpdate = arguments[objectCount++]
								var propertiesLength = properties.length
								var index = 0
								while(propertiesLength--){
									objectToUpdate[properties[index++]] = arguments[objectCount++]
								}
							}
						}
                    }
                    function bulkUpdateArrays(prop){
						const propsArrays = eval( prop )
						var objectCount = 1
						var propsArrayLength = propsArrays.length;
						var propIndex = 0
						
						while(propsArrayLength--){
							const propArray = propsArrays[propIndex++]
							var repetition = propArray.c
                            var description = propArray.a
							var arrayStart = description.b
                            var arrayEnd = description.e
							while(repetition--){
                                const objectToUpdate = arguments[objectCount++]
                                for(var index = arrayStart; index< arrayEnd; index++){					
                                    objectToUpdate[index] = arguments[objectCount++]
                                }
							}
						}
                    }
                    return {
                        bulkUpdateArrays,
                        bulkUpdateObjects,
                        Command,
                        Executable
                    }
                })()";

            script = script.Replace(NeutroniumConstants.ObjectIdTemplate, NeutroniumConstants.ObjectId)
                            .Replace(NeutroniumConstants.ReadOnlyFlagTemplate, NeutroniumConstants.ReadOnlyFlag)
                            .Replace("{{ReadOnly}}", ((int)ObjectObservability.ReadOnly).ToString())
                            .Replace("{{ReadOnlyObservable}}", ((int)ObjectObservability.ReadOnlyObservable).ToString());
            _WebView.Eval(script, out var helper);
            return new BulkJsHelper(_Cache, _WebView, helper);
        }

        void IBulkUpdater.BulkUpdateProperty(IEnumerable<ObjectDescriptor> updates)
        {
            var orderedUpdates = updates.GroupBy(up => up.Father.CValue.GetType()).SelectMany(grouped => grouped);
            BulkUpdate(orderedUpdates);
        }

        void IBulkUpdater.BulkUpdateArray(IEnumerable<ArrayDescriptor> updates)
        {
            BulkUpdate(updates);
        }

        private void BulkUpdate(IEnumerable<ObjectDescriptor> updates)
        {
            var spliter = new EntityDescriptorSpliter { MaxCount = _WebView.GetMaxAcceptableArguments() - 1 };

            var packer = new ObjectChildrenDescriptionPacker();
            foreach (var entityDescriptor in spliter.SplitParameters(updates))
            {
                var arguments = GetUpdateParameters(entityDescriptor, packer);
                BulkObjectsUpdater.ExecuteFunctionNoResult(_WebView, null, arguments);
            }
        }

        private void BulkUpdate(IEnumerable<ArrayDescriptor> updates) 
        {
            var spliter = new EntityArraySpliter { MaxCount = _WebView.GetMaxAcceptableArguments() - 1 };

            var packer = new ArrayChildrenDescriptionPacker();
            foreach (var entityDescriptor in spliter.SplitParameters(updates)) 
            {
                var arguments = GetUpdateParameters(entityDescriptor, packer);
                BulkArraysUpdater.ExecuteFunctionNoResult(_WebView, null, arguments);
            }
        }

        private IJavascriptObject[] GetUpdateParameters(Parameters updates, ObjectChildrenDescriptionPacker packer)
        {
            var sizes = packer.Pack(updates.ObjectDescriptors);
            var res = new IJavascriptObject[updates.Count + 1];       
            res[0] = _WebView.Factory.CreateString(sizes);
            var count = 1;
            foreach (var father in updates.ObjectDescriptors)
            {
                res[count++] = father.Father.JsValue;
                foreach (var atribute in father.AttributeValues)
                {
                    res[count++] = atribute.JsValue;
                }
            }
            return res;
        }

        private IJavascriptObject[] GetUpdateParameters(List<ArrayDescriptor> updates, ArrayChildrenDescriptionPacker packer) 
        {
            var sizes = packer.Pack(updates);
            var res = new IJavascriptObject[updates.Select(up => up.OrdenedChildren.Count + 1).Sum() + 1];
            res[0] = _WebView.Factory.CreateString(sizes);
            var count = 1;
            foreach (var father in updates)
            {
                res[count++] = father.Father.JsValue;
                foreach (var atribute in father.OrdenedChildren)
                {
                    res[count++] = atribute.JsValue;
                }
            }
            return res;
        }

        public void Dispose()
        {
            if (!_Factory.IsValueCreated)
                return;

            _Factory.Value.Dispose();
        }
    }
}
