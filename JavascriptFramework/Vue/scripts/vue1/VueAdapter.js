(function () {
    if (!!Vue.adapter)
        return;

    function addOnReady(element, readyCallBack) {
        element.ready = readyCallBack;
        return element;
    }

    Vue.adapter = {
        addOnReady: addOnReady
    };
})();