import { extendObservable, autorun } from 'mobx';

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
    listener.watch = autorun(() => listener.callback(element.father[propertyName]))
}

var visited = new Map();
visited.set(undefined, null);

function visitObject(vm, onNew, visit, visitArray) {
    "use strict";
    if (!vm || visited.has(vm._MappedId))
        return;

    visited.set(vm._MappedId, vm);

    if (Array.isArray(vm)) {
        visitArray(vm);
        const arrayCount = vm.length;
        for (var i = 0; i < arrayCount; i++) {
            const value = vm[i];
            visitObject(value, onNew, visit, visitArray);
        }
        return;
    }

    onNew(vm);

    const needVisitSelf = !vm.__readonly__;

    for (var property in vm) {
        var value = vm[property];
        if (typeof value === "function")
            continue;

        if (needVisitSelf) {
            visit(vm, property);
        }
        visitObject(value, onNew, visit, visitArray);
    }
}


function updateArray(array) {
}


const helper = {
    silentChange,
    register(vm, listener) {
        globalListener = listener;

        visitObject(vm, (obj) => extendObservable(obj, obj), createListener, updateArray);

        window._vm = vm;
        autorun(() => console.log(JSON.stringify(vm, null, 2)))
        fufillOnReady(null)
    },
    ready
}

module.exports = helper