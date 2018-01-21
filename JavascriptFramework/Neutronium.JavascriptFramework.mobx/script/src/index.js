import { extendObservable, observe } from 'mobx';
import { visitObject, getMapped } from './visiter'

console.log('mobx adapter loaded');

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
    var listener = observe(array, changeListener);
    array.silentSplice = function () {
        listener();
        var res = array.splice.apply(array, arguments);
        listener = observe(array, changeListener);
        return res;
    };
}

function collectionListener(object) {
    return function (changes) {
        const neutroniumChanges = getChanges(changes);
        var arg_value = [], arg_status = [], arg_index = [];
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
            const deleted = changes.removed.map(d => ({ index: index++, value: d, status: 'deleted' }));
            const added = changes.added.map(d => ({ index: index++, value: d, status: 'added' }));
            return deleted.concat(added);

        case "update":
            var index = changes.index;
            return [{ index, value: changes.oldValue, status: 'deleted' },
                    { index, value: changes.newValue, status: 'added' }];
    }
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