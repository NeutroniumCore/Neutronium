mobxManager =
/******/ (function(modules) { // webpackBootstrap
/******/ 	// The module cache
/******/ 	var installedModules = {};
/******/
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/
/******/ 		// Check if module is in cache
/******/ 		if(installedModules[moduleId]) {
/******/ 			return installedModules[moduleId].exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = installedModules[moduleId] = {
/******/ 			i: moduleId,
/******/ 			l: false,
/******/ 			exports: {}
/******/ 		};
/******/
/******/ 		// Execute the module function
/******/ 		modules[moduleId].call(module.exports, module, module.exports, __webpack_require__);
/******/
/******/ 		// Flag the module as loaded
/******/ 		module.l = true;
/******/
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/
/******/
/******/ 	// expose the modules object (__webpack_modules__)
/******/ 	__webpack_require__.m = modules;
/******/
/******/ 	// expose the module cache
/******/ 	__webpack_require__.c = installedModules;
/******/
/******/ 	// define getter function for harmony exports
/******/ 	__webpack_require__.d = function(exports, name, getter) {
/******/ 		if(!__webpack_require__.o(exports, name)) {
/******/ 			Object.defineProperty(exports, name, {
/******/ 				configurable: false,
/******/ 				enumerable: true,
/******/ 				get: getter
/******/ 			});
/******/ 		}
/******/ 	};
/******/
/******/ 	// getDefaultExport function for compatibility with non-harmony modules
/******/ 	__webpack_require__.n = function(module) {
/******/ 		var getter = module && module.__esModule ?
/******/ 			function getDefault() { return module['default']; } :
/******/ 			function getModuleExports() { return module; };
/******/ 		__webpack_require__.d(getter, 'a', getter);
/******/ 		return getter;
/******/ 	};
/******/
/******/ 	// Object.prototype.hasOwnProperty.call
/******/ 	__webpack_require__.o = function(object, property) { return Object.prototype.hasOwnProperty.call(object, property); };
/******/
/******/ 	// __webpack_public_path__
/******/ 	__webpack_require__.p = "";
/******/
/******/ 	// Load entry module and return exports
/******/ 	return __webpack_require__(__webpack_require__.s = 1);
/******/ })
/************************************************************************/
/******/ ([
/* 0 */
/***/ (function(module, exports) {

module.exports = mobx;

/***/ }),
/* 1 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


var _mobx = __webpack_require__(0);

var _visiter = __webpack_require__(2);

console.log('mobx adapter loaded');

var silenterProperty = '__silenter';
var globalListener;

var fulfillOnReady;
var ready = new Promise(function (fulfill) {
    fulfillOnReady = fulfill;
});

function silentChange(father, propertyName, value) {
    var silenter = father[silenterProperty];
    if (silenter) {
        silentChangeElement(silenter, propertyName, value);
        return;
    }
    (0, _mobx.runInAction)(function () {
        return father[propertyName] = value;
    });
}

function silentChangeUpdate(father, propertyName, value) {
    value = updateVm(value);
    var silenter = father[silenterProperty];
    if (silenter) {
        silentChangeElement(silenter, propertyName, value);
        return;
    }
    (0, _mobx.runInAction)(function () {
        return father[propertyName] = value;
    });
}

function silentChangeElement(element, propertyName, value) {
    var listener = element.listeners[propertyName];
    listener.watch();
    element.father[propertyName] = value;
    updateListenerElement(element, listener, propertyName);
}

function onPropertyChange(prop, father) {
    return function (newVal) {
        globalListener.TrackChanges(father, prop, newVal);
    };
}

function Silenter(father) {
    this.father = father;
    this.listeners = {};
}

function createListener(element, propertyName) {
    var silenter = element[silenterProperty];
    if (!silenter) {
        silenter = new Silenter(element);
        Object.defineProperty(element, silenterProperty, { value: silenter, configurable: true });
    }
    createElement(silenter, propertyName, onPropertyChange(propertyName, element));
}

function createElement(element, propertyName, callback) {
    var listener = { callback: callback };
    updateListenerElement(element, listener, propertyName);
    element.listeners[propertyName] = listener;
}

function updateListenerElement(element, listener, propertyName) {
    listener.watch = (0, _mobx.observe)(element.father, propertyName, function (change) {
        listener.callback(change.newValue);
    });
}

function updateArray(array) {
    var changeListener = collectionListener(array);
    var listener = (0, _mobx.observe)(array, changeListener);
    array.silentSplice = function () {
        var _arguments = arguments;

        listener();
        var res;
        (0, _mobx.runInAction)(function () {
            res = array.splice.apply(array, _arguments);
        });
        listener = (0, _mobx.observe)(array, changeListener);
        return res;
    };
}

function collectionListener(object) {
    return function (changes) {
        var neutroniumChanges = getChanges(changes);
        var arg_value = [],
            arg_status = [],
            arg_index = [];
        var length = neutroniumChanges.length;
        for (var i = 0; i < length; i++) {
            arg_value.push(neutroniumChanges[i].value);
            arg_status.push(neutroniumChanges[i].status);
            arg_index.push(neutroniumChanges[i].index);
        }
        globalListener.TrackCollectionChanges(object, arg_value, arg_status, arg_index);
    };
}

function getChanges(changes) {
    switch (changes.type) {
        case "splice":
            var index = changes.index;
            var deleted = changes.removed.map(function (d) {
                return { index: index++, value: d, status: 'deleted' };
            });
            var added = changes.added.map(function (d) {
                return { index: index++, value: d, status: 'added' };
            });
            return deleted.concat(added);

        case "update":
            var index = changes.index;
            return [{ index: index, value: changes.oldValue, status: 'deleted' }, { index: index, value: changes.newValue, status: 'added' }];
    }
}

function updateVm(vm) {
    return (0, _visiter.visitObject)(vm, createListener, updateArray);
}

function unListen(items) {
    var arrayCount = items.length;
    for (var i = 0; i < arrayCount; i++) {
        var father = items[i];
        var silenter = father[silenterProperty];
        if (!silenter) continue;

        disposeElement(silenter);
        delete father[silenterProperty];
    }
}

function disposeElement(element) {
    var listeners = element.listeners;
    for (var property in listeners) {
        var listener = listeners[property];
        listener.watch();
    }
    element.listeners = {};
}

function silentSplice() {
    var _arguments2 = Array.prototype.slice.call(arguments),
        array = _arguments2[0],
        args = _arguments2.slice(1);

    var mappedArray = (0, _visiter.getMapped)(array._MappedId);
    if (!mappedArray) return;

    mappedArray.silentSplice.apply(mappedArray, args);
}

function clearCollection(array) {
    var mappedArray = (0, _visiter.getMapped)(array._MappedId);
    if (!mappedArray) return;

    mappedArray.clear();
}

var updaters = [];

function onVmInjected(updater) {
    updaters.push(updater);
}

var helper = {
    silentChange: silentChange,
    silentChangeUpdate: silentChangeUpdate,
    silentSplice: silentSplice,
    register: function register(vm, listener) {
        globalListener = listener;
        updateVm(vm);
        updaters.forEach(function (up) {
            return up(vm);
        });
        fulfillOnReady(vm);
        window._vm = vm;
    },

    onVmInjected: onVmInjected,
    updateVm: updateVm,
    unListen: unListen,
    ready: ready,
    clearCollection: clearCollection
};

module.exports = helper;

/***/ }),
/* 2 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


var _mobx = __webpack_require__(0);

var visited = new Map();

function getMapped(id) {
    return visited.get(id);
}

function visitObject(vm, visit, visitArray) {
    "use strict";

    if (!vm) return vm;

    var currentId = vm._MappedId;
    if (!currentId) return vm;

    var cached = visited.get(currentId);
    if (cached) return cached;

    if (Array.isArray(vm)) {
        var updated = (0, _mobx.observable)([]);
        updated._MappedId = currentId;
        visited.set(currentId, updated);

        var updating = [];
        var arrayCount = vm.length;
        for (var i = 0; i < arrayCount; i++) {
            var _value = vm[i];
            var child = visitObject(_value, visit, visitArray);
            updating.push(child);
        }
        updated.replace(updating);
        visitArray(updated);
        return updated;
    }

    visited.set(currentId, vm);

    var needVisitSelf = !vm.__readonly__;
    for (var property in vm) {
        var value = vm[property];
        if (typeof value === "function") continue;

        var updater = {};
        updater[property] = visitObject(value, visit, visitArray);
        (0, _mobx.extendObservable)(vm, updater);

        if (needVisitSelf) {
            visit(vm, property);
        }
    }
    return vm;
}

module.exports = { visitObject: visitObject, getMapped: getMapped };

/***/ })
/******/ ]);