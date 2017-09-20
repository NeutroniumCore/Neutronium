using Neutronium.Core.Binding.Builder.Packer;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System;
using System.Collections.Generic;
using System.Linq;
using Neutronium.Core.Extension;

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
        private IJavascriptObject BulkCreator => _Factory.Value.BulkCreator;
        
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
                        Object.defineProperty(this, '{{NeutroniumConstants.ReadOnlyFlag}}', {value: true});
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
                        Object.defineProperty(this, '{{NeutroniumConstants.ReadOnlyFlag}}', {value: true});
                    }
                    Executable.prototype.Execute = function() {
                        this.privateExecute(this.{{NeutroniumConstants.ObjectId}}, ...arguments)
                    }
                    function bulkCreate(prop){
                        const propss = eval('(' + prop + ')')
                        const count = propss.count
                        const elements = propss.elements
						
		                const args = Array.from(arguments)
		                const objs = args.slice(1, args.length + 1)
                        
                        var elementCount = 0
                        var innerCount = 0						
						var objectCount =0

                        var element = null
		                for(var i=0; i< count; i ++){
                            if (!element || innerCount > element.c) {
                                element = elements[elementCount++]
                                innerCount = 0;
                            }
                            var props = element.a
							var objectToUpdate = objs[objectCount++]
                            for (var j = 0, len = props.length; j < len; j++) {
                                objectToUpdate[props[j]] = objs[objectCount++]
                            }
                            innerCount++
		                }
                    }
                    return {
                        bulkCreate,
                        Command,
                        Executable
                    }
                })()";

            IJavascriptObject helper;
            script = script.Replace("{{NeutroniumConstants.ObjectId}}", NeutroniumConstants.ObjectId)
                            .Replace("{{NeutroniumConstants.ReadOnlyFlag}}", NeutroniumConstants.ReadOnlyFlag);
            _WebView.Eval(script, out helper);
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
                Execute(arguments);
            }
        }

        private void BulkUpdate(IEnumerable<ArrayDescriptor> updates) 
        {
            var spliter = new EntityArraySpliter { MaxCount = _WebView.GetMaxAcceptableArguments() - 1 };

            var packer = new ArrayChildrenDescriptionPacker();
            foreach (var entityDescriptor in spliter.SplitParameters(updates)) 
            {
                var arguments = GetUpdateParameters(entityDescriptor, packer);
                Execute(arguments);
            }
        }

        private IJavascriptObject[] GetUpdateParameters(Parameters updates, ObjectChildrenDescriptionPacker packer)
        {     
            var res = new IJavascriptObject[updates.Count + 1];
            var sizes = packer.Pack(updates.ObjectDescriptors);
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
            var res = new IJavascriptObject[updates.Count + updates.Select(up => up.OrdenedChildren.Count).Sum() + 1];
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

        private IJavascriptObject[] BuildArguments(string paramString, IEnumerable<IJsCsGlue> paramsObjects)
        {
            return new[] { _WebView.Factory.CreateString(paramString) }.Concat(paramsObjects.Select(glue => glue.JsValue)).ToArray();
        }

        private void Execute(params IJavascriptObject[] arguments)
        {
            BulkCreator.ExecuteFunctionNoResult(_WebView, null, arguments);
        }
    }
}
