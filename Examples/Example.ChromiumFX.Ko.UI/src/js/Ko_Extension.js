function executeAsPromise(vm,fnname,argument) {
    return new Promise(function (fullfill, reject) {
        var res = { fullfill: function (res) {fullfill(res); }, reject: function(err){reject(new Error(err));}};
        vm[fnname]().Execute( argument, res);
    });
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
             var listener = PropertyListener(res, att, tracker);
             observable.listener = listener;
             observable.subscriber = observable.subscribe(listener);
             observable.silent = function (v) {
                 observable.subscriber.dispose();
                 observable(v);
                 observable.subscriber = observable.subscribe(observable.listener);
             };
         }
         else
             observable.silent = observable;
     }

     function createCollectionSubsription(observable, tracker) {
         if (tracker.TrackCollectionChanges) {
             var collectionlistener = CollectionListener(observable, tracker);
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

        if ((or === null) || (or instanceof Null_reference)) {
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

        if (Array.isArray(or)) {
            var arrayres = ko.observableArray();
            arrayres._MappedId = or._MappedId;
            MapToObservable.Cache[or._MappedId] = arrayres;
            if ((Mapper.Register) && (context === null))
                Mapper.Register(arrayres);

            for (var i = 0; i < or.length; ++i) {
                arrayres.push(MapToObservable(or[i], {
                    object: arrayres,
                    attribute: null,
                    index: i
                }, Mapper, Listener));
            }

            createCollectionSubsription(arrayres, Listener);
            if ((context === null) && (Mapper.End)) Mapper.End(arrayres);
    
            return arrayres;
        }

        var res = {};
        res._MappedId = or._MappedId;
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
                        var collection = ko.observableArray(nar);
                        collection._MappedId = value._MappedId;
                        res[att] = ko.observable(collection);
                        if (Mapper.Register) Mapper.Register(collection, res, att);
                        createCollectionSubsription(collection, Listener);
                        createSubsription(res[att], Listener, res, att);
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

    function addClass(element,className) {
        element.className += ' '+className;
    }

    function removeClass(element, className) {
        var reg = new RegExp('(?:^|\s)' + className + '(?!\S)');
        element.className.replace(RegExp, '');
    }

    function toogle(element,tooglevalue,className) {
        if (!className)
            return;

        if (tooglevalue)
            addClass(element, className);
        else
            removeClass(element, className);
    }

    ko.bindingHandlers.command = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var option = allBindings.get('commandoption') || {},
               eventname = option.event || 'click', cb = option.callback,
               value = valueAccessor(),
                eventhandler = function handler(callback) {
                    var myHandler = value();
                    if (myHandler === null)
                        return;

                    if (!!callback)
                        callback();

                    myHandler.Execute(bindingContext.$data);
                };

            //create compute to create a dependency on observable values 
            ko.computed({
                read: function () {
                    var myHandler = value(), key="enablevalue";
                    var enable = (myHandler !== null) && (myHandler.CanExecute(bindingContext.$data) === undefined) &&
                              (myHandler.CanExecuteCount()) && (myHandler.CanExecuteValue());

                   var currentenable = ko.utils.domData.get(element, key);

                   if (currentenable === enable)
                        return;

                    ko.utils.domData.set(element, key, enable);

                    var cssOn = option.cssOn, ccsOff = option.ccsOff;

                    ko.bindingHandlers.enable.update(element, function () { return enable; }, allBindings);

                    toogle(element, enable, cssOn);
                    toogle(element, !enable, ccsOff);
                },
                disposeWhenNodeIsRemoved: element
            });

            element.addEventListener(eventname, function () { eventhandler(cb) }, false);
        }
    };

  
    ko.bindingHandlers.execute = {
        init: function (element, valueAccessor, allBindings,viewModel, bindingContext) {
            var option = allBindings.get('executeoption') || {},
                eventname = option.event || 'click', cb = option.callback,
                value = valueAccessor(),
                eventhandler = function handler(callback) {
                    var myHandler = value();
                    if (myHandler === null)
                        return;

                    if (!!callback)
                        callback();

                    myHandler.Execute(bindingContext.$data);
                };

            element.addEventListener(eventname, function () { eventhandler(cb) }, false);
        }
    };

    ko.bindingHandlers.parentCommand = {
        preprocess: function (value, name, addBinding) {
            return value;
        },

        init: function (element, valueAccessor, allBindings,viewModel,bindinContext) {
            var value = valueAccessor();
            var transformed = {};
            ko.utils.objectForEach(value, function (event, keyvalue) {
                var newlogic = {};
                transformed[event] = newlogic;
                ko.utils.objectForEach(keyvalue, function (logicname, logichandler) {
                    newlogic[logicname] = function (el) { logichandler().Execute(el); };
                });
            });

            console.log(transformed);

            ko.bindingHandlers.delegatedParentHandler.init(element, function () { return transformed; }, allBindings, viewModel, bindinContext);
        }
    };

    ko.bindingHandlers.executeResult = {

        init: function (element, valueAccessor, allBindings) {
            var promiseresult = allBindings.get('promiseoption'),
                then = typeof promiseresult == 'function' ? promiseresult : promiseresult.then,
                error = promiseresult.error || function () { },
                arg = promiseresult.arg,
                eventname = promiseresult.event || 'click',
                value = valueAccessor(),
                handlerevent = function handler(argv) {
                    return new Promise(function (fullfill, reject) { 
                        var res = { fullfill: function (res) { fullfill(res); }, reject: function (err) { reject(new Error(err)); } };
                        value().Execute(argv, res);
                    });
                };

            element.addEventListener(eventname, function () { handlerevent(ko.utils.unwrapObservable(allBindings.get('promiseoption').arg)).then(then).catch(error); }, false);
        }
    };

    ko.bindingHandlers.numberInput = {
        init: function (element, valueAccessor, allBindings) {
            var value = valueAccessor();

            var valueUpdate = allBindings.get('valueUpdate');
            var event = (valueUpdate === 'afterkeydown') ? 'input' : 'change';

            element.addEventListener(event, function () {
                var numb = Number(element.value);
                if (isNaN(numb)) {
                    element.value = 0;
                    numb = 0;
                }
                
                value(numb);
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
            return '{when: $root.Window().State, act: ' + value + '}';
        },

        init: function (element, valueAccessor,allBindings,viewModel,bindingContext) {
            bindingContext.$root.Window().IsListeningClose(true);
        },

        update: function (element, valueAccessor,allBindings,viewModel,bindingContext) {
            var v = ko.utils.unwrapObservable(valueAccessor());
            if (v.when().name !== 'Closing')
                return;

            v.act(function () { bindingContext.$root.Window().CloseReady().Execute(); }, element);
        }
    };

    ko.bindingHandlers.onopened= {
        preprocess: function (value) {
            return '{when:  $root.Window().State, act: ' + value + '}';
        },

        init: function (element, valueAccessor,allBindings,viewModel,bindingContext) {
            bindingContext.$root.Window().IsListeningOpen(true);
        },

        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var v = ko.utils.unwrapObservable(valueAccessor());
            if (v.when().name !== 'Opened')
                return;

            v.act(element, function () { bindingContext.$root.Window().EndOpen().Execute(); });
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
                       ko.log("ko binding error: '" + ex.message + "'", "node HTML: " + node.outerHTML, "context:" + ko.toJSON(bindingContext.$data, ko.toJSONreplacer()));
                }

                return bindings;
            }
        };
}());
