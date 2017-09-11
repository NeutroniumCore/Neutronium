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

    Vue.adapter = {
        addOnReady: addOnReady
    };
})();

