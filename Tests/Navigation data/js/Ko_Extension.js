
function Enum(Type, intValue, name, displayName) {
    this.intValue = intValue;
    this.displayName = displayName;
    this.name = name;
    this.type = Type;
}

//to bypass awesomium limitations
function Null_reference() {
}

(function () {

    function PropertyListener(object, propertyname, listener) {
        return function (newvalue) {
            listener.TrackChanges(object, propertyname, newvalue);
        };
    }

    function CollectionListener(object, listener) {
        return function (changes) {
            var arg_value = [], arg_status = [], arg_index = [];
            var length= changes.length;
            for (var i = 0; i < length; i++) {
                arg_value.push(changes[i].value);
                arg_status.push(changes[i].status);
                arg_index.push(changes[i].index);
            }
            listener.TrackCollectionChanges(object, arg_value, arg_status, arg_index);
        };
    }

     function createSubsription(observable, tracker,res,att) {
         if (tracker.TrackChanges) {
             listener = PropertyListener(res, att, tracker);
             observable.listener = listener;
             observable.subscriber = observable.subscribe(listener);
             observable.silent = function (v) {
                 observable.subscriber.dispose();
                 observable((v instanceof Null_reference) ? null : v);
                 observable.subscriber = observable.subscribe(observable.listener);
             };
         }
         else
             observable.silent = observable;
     }

     function createCollectionSubsription(observable, tracker, res, att) {
         if (tracker.TrackCollectionChanges) {
             var collectionlistener = CollectionListener(res[att], tracker);
             observable.listener = collectionlistener;
             observable.subscriber = observable.subscribe(collectionlistener, null, 'arrayChange');
             observable.silent = function (fn) {
                 return function () {
                     observable.subscriber.dispose();
                     fn.apply(observable, arguments);
                     observable.subscriber = observable.subscribe(collectionlistener, null, 'arrayChange');
                 };
             };
         }
         else
             observable.silent = function (fn) {
                 return function () {
                     fn.apply(observable, arguments);
                 };
             };

         observable.silentsplice = observable.silent(observable.splice);
         observable.silentremoveAll = observable.silent(observable.removeAll);
     }


    function MapToObservable(or, context, Mapper, Listener) {

        if ((typeof or !== 'object') || (or instanceof Date) || (or instanceof Enum)) return or;

        if (!MapToObservable.Cache) {
            MapToObservable.Cache = {};
            MapToObservable._MappedId = 0;
        }

        if (!Mapper) Mapper = {};
        if (!Listener) Listener = {};

        if (or instanceof Null_reference) {
            if (context === null) {
                if (Mapper.Register) Mapper.Register(or);
                if (Mapper.End) Mapper.End(or);
                return or;
            }

            return null;
        }
        //Look in cache
        //not very clean implementation, but must handle "read-only" object with predefined _MappedId
        if (or._MappedId !== undefined) {
            var tentative = MapToObservable.Cache[or._MappedId];
            if (tentative) {
                if ((context === null) && (Mapper.End)) Mapper.End(tentative);
                return tentative;
            }
        }
        else {
            while (MapToObservable.Cache[MapToObservable._MappedId]) { MapToObservable._MappedId++; }
            or._MappedId = MapToObservable._MappedId;
        }

        var res = {};
        MapToObservable.Cache[or._MappedId] = res;
        if (Mapper.Register){
            if (context===null) 
                Mapper.Register(res);
            else if (context.index === undefined)
                Mapper.Register(res, context.object, context.attribute);
            else
                Mapper.Register(res, context.object, context.attribute, context.index);
        }


        for (var att in or) {
            if ((att !== "_MappedId") && (or.hasOwnProperty(att))) {
                var value = or[att];
                if ((value !== null) && (typeof value === 'object')) {
                    if (!Array.isArray(value)) {
                        var comp = MapToObservable(value, {
                            object: res,
                            attribute: att
                        }, Mapper, Listener);
                        res[att] = ko.observable(comp);
                        createSubsription(res[att], Listener, res, att);
                    } else {
                        var nar = [];
                        for (var i = 0; i < value.length; ++i) {
                            nar.push(MapToObservable(value[i], {
                                object: res,
                                attribute: att,
                                index: i
                            }, Mapper, Listener));
                        }

                        res[att] = ko.observableArray(nar);
                        if (Mapper.Register) Mapper.Register(res[att], res, att);
                        createCollectionSubsription(res[att], Listener, res, att);
                    }
                } else {
                    res[att] = ko.observable(value);
                    createSubsription(res[att], Listener, res, att);
                }
            }
        }

        if ((context === null) && (Mapper.End)) Mapper.End(res);

        return res;
    }

    ko.isDate = function (o) {
        return o instanceof Date;
    };

    //global ko
    ko.MapToObservable = function (o, mapper, listener) {
        return MapToObservable(o, null, mapper, listener);
    };

    ko.toJSONreplacer = function () {
        var keys = [];
        var valuesforkeys = [];

        function visit(localroot, pn, found) {
            found = found || [];

            if (found.indexOf(localroot) !== -1) {
                var existingIndex = keys.indexOf(localroot);
                if (existingIndex >= 0) valuesforkeys[existingIndex].push(pn);
                else {
                    keys.push(localroot);
                    valuesforkeys.push([pn]);
                }
                return;
            }

            var index = found.push(localroot);

            for (var att in localroot) {
                if (localroot.hasOwnProperty(att)) {
                    var value = localroot[att];
                    if ((value !== null) && (typeof value === 'object')) {
                        if (Array.isArray(value)) {
                            for (var i = 0; i < value.length; ++i) {
                                visit(value[i], att, found);
                            }
                        } else {
                            visit(value, att, found);
                        }
                    }
                }
            }

            found.splice(index - 1, 1);
        }

        var first = true;


        return function replacer(key, value) {

            if (first) {
                first = false;
                visit(value);
            }

            var existingIndex = keys.indexOf(value);
            if ((existingIndex >= 0) && (valuesforkeys[existingIndex].indexOf(key) >= 0)) return "[Circular]";

            return value;
        };
    };

    ko.bindingHandlers.command = {
        preprocess: function (value, name, addBinding) {
            addBinding('commandOnEvent', '{ "command": "' + value + '", "event": "click"}');
            return value;
        }
    };

    ko.bindingHandlers.commandOnEvent = {
        preprocess: function (compvalue, name, addBinding) {
            var value = JSON.parse(compvalue.replace(/'/g, '"'));
            addBinding('enable', value.command + '()!==null && ' + value.command + '().CanExecute($data)===undefined &&' + value.command + '().CanExecuteCount() &&' + value.command + '().CanExecuteValue()');
            addBinding('event', '{' + value.event + ': function(){ if (' + value.command + '()!==null) {' + value.command + '().Execute($data);}}}');
            return compvalue;
        }
    };

    ko.bindingHandlers.execute = {
        preprocess: function (value, name, addBinding) {
            addBinding('executeOnEvent', '{ "command": "' + value + '", "event": "click"}');
            return value;
        }
    };

    ko.bindingHandlers.executeOnEvent = {
        preprocess: function (compvalue, name, addBinding) {
            var value = JSON.parse(compvalue.replace(/'/g, '"'));
            addBinding('event', '{' + value.event + ': function(){ if (' + value.command + '()!==null) {' + value.command + '().Execute($data);}}}');
            return compvalue;
        }
    };

    ko.bindingHandlers.numberInput = {
        init: function (element, valueAccessor, allBindingsAccessor) {
            var value = valueAccessor();
            element.addEventListener('change', function () {
                value(Number(element.value));
            }, false);
        },

        update: function (element, valueAccessor, allBindingsAccessor) {
            var value = valueAccessor();
            element.value = value();
        }
    };
     
    ko.getimage = function (Enumvalue) {
        if ((!Enumvalue instanceof Enum) || (!ko.Enumimages))
            return null;

        var ec = ko.Enumimages[Enumvalue.type];
        return ec ? ec[Enumvalue.name] : null;
    };

    ko.images = function (enumtype) {
        return (!!ko.Enumimages) ? ko.Enumimages[enumtype] : null;
    };

    ko.bindingHandlers.enumimage = {
        update: function (element, valueAccessor) {
            var v = ko.utils.unwrapObservable(valueAccessor());
            var imagepath = ko.getimage(v);
            if (imagepath) element.src=imagepath;
        }
    };

    ko.bindingHandlers.enumimagewithfallback = {
        update: function (element, valueAccessor) {
            var v = ko.utils.unwrapObservable(valueAccessor());
            var imagepath = ko.getimage(v.image) || v.fallback;
            element.src = imagepath;
        }
    };

    ko.bindingHandlers.onclose = {
        preprocess: function (value) {
            return '{when: $root.__window__().State, do: ' + value + '}';
        },

        init: function (element, valueAccessor,allBindings,viewModel,bindingContext) {
            bindingContext.$root.__window__().IsListeningClose(true);
        },

        update: function (element, valueAccessor,allBindings,viewModel,bindingContext) {
            var v = ko.utils.unwrapObservable(valueAccessor());
            if (v.when().name !== 'Closing')
                return;

            v.do(function () { bindingContext.$root.__window__().CloseReady().Execute(); }, element);
        }
    };

    ko.bindingHandlers.onopened= {
        preprocess: function (value) {
            return '{when:  $root.__window__().State, do: ' + value + '}';
        },

        init: function (element, valueAccessor,allBindings,viewModel,bindingContext) {
            bindingContext.$root.__window__().IsLiteningOpen(true);
        },

        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var v = ko.utils.unwrapObservable(valueAccessor());
            if (v.when().name !== 'Opened')
                return;

            v.do(element, function () { bindingContext.$root.__window__().EndOpen().Execute(); });
        }
    };

    //improve knockout binding debug
    //allow parcial binding even if somebinding are KO
    var existing = ko.bindingProvider.instance;

        ko.bindingProvider.instance = {
            nodeHasBindings: existing.nodeHasBindings,
            getBindings: function(node, bindingContext) {
                var bindings;
                try {
                   bindings = existing.getBindings(node, bindingContext);
                }
                catch (ex) {
                   if (window.console && console.log) {
                       console.log("binding error : "+ ex.message, node, bindingContext);
                   }

                   if (ko.log)
                       ko.log("ko binding error: '" + ex.message + "'", "node HTLM: " + node.outerHTML, "context:" + ko.toJSON(bindingContext.$data, ko.toJSONreplacer()));
                }

                return bindings;
            }
        };


}());
