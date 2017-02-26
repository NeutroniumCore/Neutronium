using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.MVVMComponents;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Neutronium.Core.Binding
{
    internal class CSharpToJavascriptConverter 
    {
        private readonly IJavascriptSessionCache _Cacher;
        private readonly IJSCommandFactory _CommandFactory;
        private readonly IWebSessionLogger _Logger;
        private readonly HTMLViewContext _Context;

        public CSharpToJavascriptConverter(HTMLViewContext context, IJavascriptSessionCache icacher, IJSCommandFactory commandFactory, IWebSessionLogger logger)
        {
            _CommandFactory = commandFactory;
            _Logger = logger;
            _Context = context;
            _Cacher = icacher;
        }

        public async Task<IJSCSGlue> Map(object from, object iadditional = null)
        {
            var res = await _Context.EvaluateOnUIContextAsync(() => InternalMap(from, iadditional));
            await _Context.RunOnJavascriptContextAsync(() =>
            {
                res.ComputeJavascriptValue(_Context.WebView.Factory, _Cacher);
            });
            return res;
        }

        public IJSCSGlue InternalMap(object from, object iadditional=null)
        {
            if (from == null)
                return new JSBasicObject(null);

            var res = _Cacher.GetCached(from);
            if (res != null)
                return res;

            var command = from as ICommand;
            if (command != null)
                return _CommandFactory.Build(command);

            var simpleCommand = from as ISimpleCommand;
            if (simpleCommand != null)
                return _CommandFactory.Build(simpleCommand);

            var resultCommand = from as IResultCommand;
            if (resultCommand != null)
                return _CommandFactory.Build(resultCommand);

            var type = from.GetType();
            if (_Context.WebView.Factory.IsTypeBasic(type))
                return new JSBasicObject(from);

            if (type.IsEnum)
            {
                var trueres = new JSBasicObject(from);
                _Cacher.Cache(from, trueres);
                return trueres;
            }

            var ienfro = from as IEnumerable;
            if (ienfro!=null)
                return  Convert(ienfro);

            var propertyInfos = GetPropertyInfos(from).Concat(GetPropertyInfos(iadditional)).ToList();

            var gres = new JsGenericObject(_Context, from, propertyInfos.Count);
            _Cacher.Cache(from, gres);

            MappNested(gres, propertyInfos);

            return gres;
        }

        private static IEnumerable<Tuple<PropertyInfo, object>> GetPropertyInfos(object from) 
        {
            return @from?.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead).Select(prop => Tuple.Create(prop, @from)) ?? Enumerable.Empty<Tuple<PropertyInfo, object>>();          
        }

        private void MappNested(JsGenericObject gres, IReadOnlyCollection<Tuple<PropertyInfo, object>> properties)
        {
            foreach (var property in properties)
            {
                var propertyInfo = property.Item1;
                var propertyName = propertyInfo.Name;
                var parentObject = property.Item2;
                object childvalue;
                try
                {
                    childvalue = propertyInfo.GetValue(parentObject, null); 
                }
                catch(TargetInvocationException e)
                {
                    _Logger.Info(()=> $"Unable to convert property {propertyName} from {parentObject} of type {parentObject.GetType().FullName} exception {e.InnerException}");
                    continue;
                }

                var childres = InternalMap(childvalue);          
                gres.UpdateCSharpProperty(propertyName, childres);
            }
        }

        private IJSCSGlue Convert(IEnumerable source)
        {
            var res = new JSArray(_Context, source.Cast<object>().Select(s => InternalMap(s)), source);
            _Cacher.Cache(source, res);
            return res;
        }
    }
}
