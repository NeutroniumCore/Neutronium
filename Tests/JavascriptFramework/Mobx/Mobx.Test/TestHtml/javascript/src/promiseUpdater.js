/*jshint esversion: 6 */
"use strict";
(function () {

    function vmUpdater(vm) {
        //console.log(JSON.stringify(vm, null, 2))
        //if (!vm.Factory)
        //    return;

        //const command = vm.Factory.CreateObject;
        //command.run = function() {
        //    command.Execute(vm).then(function(res) {
        //            alert(res.LastName());
        //            vm.factoryresult(res);
        //        },
        //        function(err) {
        //            vm.error(err);
        //            console.log(err);
        //        });
        //}
    }

    mobxManager.onVmInjected(vmUpdater);
})();