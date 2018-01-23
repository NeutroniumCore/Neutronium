/*jshint esversion: 6 */
"use strict";
(function () {

    function vmUpdater(vm) {
        const commands = [
            { command: vm.TestCommand, data: vm },
            { command: vm.AutoCommand, data: vm },
            { command: vm.CommandGeneric, data: 'data' },
            { command: vm.CommandWithoutParameters, data: null }];
        commands.forEach(commandUpdater);
    }

    function commandUpdater(commandBinder) {
        const command = commandBinder.command;
        const data = commandBinder.data;
        if (!command)
            return;

        if (command.CanExecute) {
            if (data) {
                command.CanExecute(data);
            }

            mobx.observe(command, 'CanExecuteCount', function () {
                command.CanExecute(data);
            });
        }
    }

    mobxManager.onVmInjected(vmUpdater);
})();