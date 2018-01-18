var globalListener;

var fufillOnReady;
var ready = new Promise(function (fullfill) {
    fufillOnReady = fullfill;
});

const helper = {
    register(vm, listener) {
        globalListener = listener;
        console.log(vm, listener)
        fufillOnReady(null)
    },
    ready
}

module.exports = helper