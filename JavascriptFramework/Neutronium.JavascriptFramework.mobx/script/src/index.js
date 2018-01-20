import { extendObservable, autorun } from 'mobx';
import { subscribe } from './subscribeArray';
import { visitObject } from './visiter'

console.log('mobx adapter loaded');

Object.defineProperty(Array.prototype, 'subscribe', {
    value: subscribe
});

const silenterProperty = '__silenter';
var globalListener;

var fufillOnReady;
var ready = new Promise(function (fullfill) {
    fufillOnReady = fullfill;
});

function silentChange(father, propertyName, value) {
    updateVm(value)
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

function updateArray(array) {
    // var changelistener = collectionListener(array);
    // var listener = array.subscribe(changelistener);
    // array.silentSplice = function () {
    //     listener();
    //     var res = array.splice.apply(array, arguments);
    //     listener = array.subscribe(changelistener);
    //     return res;
    // };
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
    visitObject(vm, (obj) => extendObservable(obj, obj), createListener, updateArray)
}

const helper = {
    silentChange,
    register(vm, listener) {
        globalListener = listener;

        updateVm(vm);

        window._vm = vm;
        autorun(() => console.log(JSON.stringify(vm, null, 2)))
        fufillOnReady(null)
    },
    ready
}

module.exports = helper