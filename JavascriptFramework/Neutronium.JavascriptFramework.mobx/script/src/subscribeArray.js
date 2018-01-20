function isFunction(obj) {
    return !!(obj && obj.constructor && obj.call && obj.apply);
}

var updateSubject = function (subject, callBack) {

    if ((!subject) || (!Array.isArray(subject)))
        throw new Error("subject should be an array");

    if ((!callBack) || (!isFunction(callBack)))
        throw new Error("callBack should be a function");

    if (!!subject.addCallBack) {
        return subject.addCallBack(callBack);
    }

    function overrideMethod(name, f) {
        Object.defineProperty(subject, name, {
            value: f
        });
    }

    overrideMethod("addCallBack", addCallBack);

    var listeners = [];
    var changes = [];
    var underUpdate = false;

    function remove(array, item) {
        var index = array.indexOf(item);
        if (index != -1)
            array.splice(index, 1);
    }

    function addCallBack(callBack) {
        listeners.push(callBack);
        return () => remove(listeners, callBack);
    }

    function callListeners() {
        listeners.forEach((sub) => sub(changes));
        changes = [];
        underUpdate = false;
    }

    function emit(event) {
        if (!Array.isArray(event)) {
            changes.push(event);
        }
        else {
            if (event.length === 0)
                return;
            changes = changes.concat(event);
        }

        if (underUpdate === true)
            return;

        underUpdate = true;
        setTimeout(callListeners);
    }

    function onListeners(changeFactory) {
        if (!listeners.length)
            return;
        var changes = changeFactory();
        emit(changes);
    }

    function arrayChange(index, value, status, moved) {
        this.index = index;
        this.value = value;
        this.status = status;
        if (moved !== undefined)
            this.moved = moved;
    }

    function deleteChange(index, value, moved) {
        return new arrayChange(index, value, 'deleted', moved);
    }

    function addChange(index, value, moved) {
        return new arrayChange(index, value, 'added', moved);
    }

    function appendMoveChange(oldIndex, newIndex, value, array) {
        array.push(deleteChange(oldIndex, value, newIndex));
        array.push(addChange(newIndex, value, oldIndex));
    }

    // We need to augment all the standard Array mutator methods to notify
    // all observers in case of a change.
    //
    // https://developer.mozilla.org/en/JavaScript/Reference/Global_Objects/Array#Mutator_methods
    // pop: Removes the last element from an array and returns that element.
    var oginalPop = subject.pop;
    overrideMethod('pop', function () {
        var deleted_item = oginalPop.apply(this, arguments);
        onListeners(() => deleteChange(this.length, deleted_item));
        return deleted_item;
    });

    // push: Adds one or more elements to the end of an array and returns
    // the new length of the array.
    var oginalPush = subject.push;
    overrideMethod('push', function () {
        var new_item = arguments[0];
        var new_length = oginalPush.apply(this, arguments);
        onListeners(() => addChange(new_length - 1, new_item));
        return new_length;
    });

    // shift: Removes the first element from an array and returns that
    // element.
    var oginalShift = subject.shift;
    overrideMethod('shift', function () {
        var deleted_item = oginalShift.apply(this, arguments);
        onListeners(() => deleteChange(0, deleted_item));
        return deleted_item;
    });

    function spliceChangeBuilder(deleted, insert, position) {
        var changes = [];
        var deletePodition = position;
        deleted.forEach((arg) => changes.push(deleteChange(deletePodition++, arg)));
        insert.forEach((arg) => changes.push(addChange(position++, arg)));
        return changes;
    }

    // splice: Adds and/or removes elements from an array.
    var oginalSplice = subject.splice;
    overrideMethod('splice', function (i /*, length , insert */) {
        var position = i < 0 ? this.length + i : i;
        if (position > this.length)
            position = this.length;
        var insert = Array.prototype.slice.call(arguments, 2);
        var deleted = oginalSplice.apply(this, arguments);
        onListeners(() => spliceChangeBuilder(deleted, insert, position));
        return deleted;
    });

    function unshiftArgumentBuilder(added) {
        var changes = [];
        added.forEach((arg, index) => changes.push(addChange(index, arg)));
        return changes;
    }

    // unshift: Adds one or more elements to the front of an array and
    // returns the new length of the array.
    var oginalUnshift = subject.unshift;
    overrideMethod('unshift', function () {
        var new_length = oginalUnshift.apply(this, arguments);
        onListeners(() => unshiftArgumentBuilder([].slice.call(arguments)));
        return new_length;
    });

    function reverseArgumentBuilder(array) {
        var changes = [], count = (array.length - 1) / 2;
        for (var i = 0; i < count; i++) {
            var index2 = array.length - 1 - i;
            var f = array[i], l = array[index2];
            if (f !== l) {
                appendMoveChange(i, index2, l, changes);
                appendMoveChange(index2, i, f, changes);
            }
        }
        return changes;
    }

    // reverse: Reverses the order of the elements of an array -- the first
    // becomes the last, and the last becomes the first.
    var oginalReverse = subject.reverse;
    overrideMethod('reverse', function () {
        var result = oginalReverse.apply(this, arguments);
        onListeners(() => reverseArgumentBuilder(this));
        return result;
    });

    function basicCompare(a, b) {
        var sa = String(a), sb = String(b);
        return sa.localeCompare(sb);
    }

    function sortArgumentBuild(transformed) {
        var changes = [];
        transformed.forEach((el, index) => {
            if (el.index !== index)
                appendMoveChange(el.index, index, el.el, changes);
        });
        return changes;
    }

    // sort: Sorts the elements of an array.
    overrideMethod('sort', function (compare) {
        var intermediate = this.map((el, index) => { return { el: el, index: index }; });
        var arg = (!compare) ? (a, b) => basicCompare(a.el, b.el) : (a, b) => compare(a.el, b.el);
        var result = Array.prototype.sort.call(intermediate, arg);
        var raw = result.map((el) => el.el);
        raw.unshift(0, this.length);
        oginalSplice.apply(this, raw);
        onListeners(() => sortArgumentBuild(result));
        return raw.splice(2);
    });

    return addCallBack(callBack);
};

function subscribe(listener){
    return updateSubject(this, listener);
}

module.exports = {
    subscribe
}