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
        var div = document.createElement("div");
        div.appendChild(innerComponent);
        father.appendChild(div);
        //setTimeout(function () {
        var v = new component({ el: div });
        //}, 0);
    }

    Vue.adapter = {
        addOnReady: addOnReady,
        dynamicAppend: dynamicAppend
    };
})();

