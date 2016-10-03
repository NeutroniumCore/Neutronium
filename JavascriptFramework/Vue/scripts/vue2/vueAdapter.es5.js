"use strict";

(function () {
    if (!!Vue.adapter) return;

    function addOnReady(element, readyCallBack) {
        element.mounted = function () {
            this.$nextTick(function () {
                readyCallBack.call(this);
            });
        };
        return element;
    }

    function dynamicAppend(component, innerComponent, father) {
        father.appendChild(innerComponent);
        var v = new Vue({ el: innerComponent });
    }

    Vue.adapter = {
        addOnReady: addOnReady,
        dynamicAppend: dynamicAppend
    };
})();

