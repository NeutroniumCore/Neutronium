"use strict";

(function () {
    if (!!Vue.adapter) return;

    function addOnReady(element, readyCallBack) {
        element.ready = readyCallBack;
        return element;
    }

    function dynamicAppend(component, innerComponent, father) {
        var v = new component({ el: innerComponent });
        v.$appendTo(father);
    }

    Vue.adapter = {
        addOnReady: addOnReady,
        dynamicAppend: dynamicAppend
    };
})();

