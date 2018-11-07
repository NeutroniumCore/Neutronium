import { extendObservable, observable } from "mobx";

const visited = new Map();

function isReadOnly(vm) {
    return vm.__readonly__;
}

function getMapped(id) {
    return visited.get(id);
}

function visitObject(vm, visit, visitArray) {
    "use strict";
    if (!vm)
        return vm;

    const currentId = vm._MappedId
    if (!currentId)
        return vm;

    const cached = visited.get(currentId);
    if (cached)
        return cached;

    if (Array.isArray(vm)) {
        const updated = observable([]);
        updated._MappedId = currentId;
        visited.set(currentId, updated);

        const updating = []
        const arrayCount = vm.length;
        for (var i = 0; i < arrayCount; i++) {
            const value = vm[i];
            const child = visitObject(value, visit, visitArray);
            updating.push(child);
        }
        updated.replace(updating);
        visitArray(updated);
        return updated;
    }

    visited.set(currentId, vm);

    for (var property in vm) {
        var value = vm[property];
        if (typeof value === "function")
            continue;

        var updater = {}
        updater[property] = visitObject(value, visit, visitArray);
        extendObservable(vm, updater)
    }

    if (!isReadOnly(vm)) {
        visit(vm, property);
    }
    
    return vm;
}

module.exports = { visitObject, getMapped }