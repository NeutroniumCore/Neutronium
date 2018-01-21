import { extendObservable, observe, observable } from 'mobx';
import { subscribe } from './subscribeArray';
import { visitObject, getMapped } from './visiter'

console.log('mobx adapter loaded');

const observableArrayPrototype = Object.getPrototypeOf(observable([]))
Object.defineProperty(observableArrayPrototype, 'subscribe', {
    value: subscribe
});

const silenterProperty = '__silenter';
var globalListener;

var fufillOnReady;
var ready = new Promise(function (fullfill) {
    fufillOnReady = fullfill;
});

function silentChange(father, propertyName, value) {
    const silenter = father[silenterProperty];
    if (silenter) {
        silentChangeElement(silenter, propertyName, value);
        return;
    }
    father[propertyName] = value;
}

function silentChangeUpdate(father, propertyName, value) {
    value = updateVm(value)
    const silenter = father[silenterProperty];
    if (silenter) {
        silentChangeElement(silenter, propertyName, value);
        return;
    }
    father[propertyName] = value;
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
    listener.watch = observe(element.father, propertyName, (change) => {
        listener.callback(change.newValue)
    })
}

function updateArray(array) {
    var changeListener = collectionListener(array);
    var listener = array.subscribe(changeListener);
    array.silentSplice = function () {
        listener();
        var res = array.splice.apply(array, arguments);
        listener = array.subscribe(changeListener);
        return res;
    };
}

function collectionListener(object) {
    return function (changes) {
        var arg_value = [], arg_status = [], arg_index = [];
        var length = changes.length;
        for (var i = 0; i < length; i++) {
            arg_value.push(changes[i].value);
            arg_status.push(changes[i].status);
            arg_index.push(changes[i].index);
        }
        globalListener.TrackCollectionChanges(object, arg_value, arg_status, arg_index);
    };
}

function updateVm(vm) {
    return visitObject(vm, createListener, updateArray)
}

function unListen(items) {
    const arrayCount = items.length;
    for (var i = 0; i < arrayCount; i++) {
        const father = items[i];
        const silenter = father[silenterProperty];
        if (!silenter)
            continue;

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
    const [array, ...args] = arguments
    var mappedArray = getMapped(array._MappedId)
    if (!mappedArray)
        return

    mappedArray.silentSplice.apply(mappedArray, args)
}

function clearCollection(array) {
    var mappedArray = getMapped(array._MappedId)
    if (!mappedArray)
        return

    mappedArray.clear();
}

const helper = {
    silentChange,
    silentChangeUpdate,
    silentSplice,
    register(vm, listener) {
        globalListener = listener;

        updateVm(vm);

        window._vm = vm;
        fufillOnReady(null)
    },
    updateVm,
    unListen,
    ready,
    clearCollection
}

module.exports = helper