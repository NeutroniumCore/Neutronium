/*jshint esversion: 6 */
"use strict";
(function () {

    function vmUpdater(vm) {
        mobx.extendObservable(vm, {
            completeName: mobx.computed(function() {
                return this.Name + " " + this.LastName;
            })
        });
        window._vm = vm;
    }

    mobxManager.onVmInjected(vmUpdater);
})();