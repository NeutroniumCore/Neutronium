import rawVm from '../data/vm'
import { createVM } from 'neutronium-vm-loader'
import { extendObservable } from 'mobx';

const vm = createVM(rawVm);
console.log(JSON.stringify(vm, null, 2))
extendObservable(vm, vm)

var fulfillOnReady;
var ready = new Promise(function (fulfill) {
    fulfillOnReady = fulfill;
});

const updaters = []

function onVmInjected(updater) {
    updaters.push(updater)
}

function updateVm() {
    updaters.forEach(up => up(vm));
    fulfillOnReady(vm)
}

window.setTimeout(updateVm, 0);

export {
    onVmInjected,
    ready,
}