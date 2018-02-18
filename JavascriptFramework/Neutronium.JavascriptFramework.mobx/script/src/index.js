import { observe, runInAction, extendObservable } from 'mobx';
import { visitObject, getMapped } from './visiter'

console.log('mobx adapter loaded');

const silenterProperty = '__silenter';
var globalListener;

function addProperty(father, propertyName, value) {
    const updater = {};
    updater[propertyName] = updateVm(value);
    extendObservable(father, updater)
}

function silentChange(father, propertyName, value) {
    const silenter = father[silenterProperty];
    if (silenter) {
        silentChangeElement(silenter, propertyName, value);
        return;
    }
    runInAction(() => father[propertyName] = value);
}

function silentChangeUpdate(father, propertyName, value) {
    value = updateVm(value)
    const silenter = father[silenterProperty];
    if (silenter) {
        silentChangeElement(silenter, propertyName, value);
        return;
    }
    runInAction(() => father[propertyName] = value);
}

function silentChangeElement(element, propertyName, value) {
    const listener = element.listener;
    listener.watch();
    runInAction(() => element.father[propertyName] = value);
    updateListenerElement(element, listener);
}

function onPropertyChange(father) {
    return function (propertyName, newVal) {
        globalListener.TrackChanges(father, propertyName, newVal);
    };
}

function Silenter(father) {
    this.father = father;
    this.listener = null;
}

function createListener(element) {
    const silenter = new Silenter(element);
    Object.defineProperty(element, silenterProperty, { value: silenter, configurable: true });
    createElement(silenter, onPropertyChange(element));
}

function createElement(element, callback) {
    var listener = { callback };
    updateListenerElement(element, listener);
    element.listener = listener;
}

function updateListenerElement(element, listener) {
    listener.watch = observe(element.father, (change) => {
        listener.callback(change.name, change.newValue)
    })
}

function updateArray(array) {
    var changeListener = collectionListener(array);
    var listener = observe(array, changeListener);
    array.silentSplice = function () {
        listener();
        var res;
        runInAction(() => { res = array.splice.apply(array, arguments) });
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

function unListen() {
    const [...items] = arguments
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

const updaters = []

function onVmInjected(updater) {
    updaters.push(updater)
}

var fulfillOnReady;
var ready = new Promise(function (fulfill) {
    fulfillOnReady = fulfill;
});

var fulfillDone;
var done = new Promise(function (fulfill) {
    fulfillDone = fulfill;
});

function Result(vm, callback) {
    this.vm = vm;
    this.callback = callback;
    this.isListened = false;
}

Result.prototype.checkDone = function () {
    this.isListened || this.callback();
}

Object.defineProperty(Result.prototype, "ready", {
    get() {
        this.isListened = true;
        return this.callback;
    },
    set() {
    }
});

const helper = {
    addProperty,
    done,
    silentChange,
    silentChangeUpdate,
    silentSplice,
    register(vm, listener) {
        globalListener = listener;
        updateVm(vm);
        updaters.forEach(up => up(vm));
        const res = new Result(vm, fulfillDone);
        fulfillOnReady(res);
        res.checkDone();
        window._vm = vm;
    },
    onVmInjected,
    updateVm,
    unListen,
    ready,
    clearCollection
}

module.exports = helper