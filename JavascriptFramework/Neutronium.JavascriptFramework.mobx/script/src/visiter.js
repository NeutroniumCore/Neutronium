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
module.exports = { visitObject }