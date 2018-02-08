import { observable, computed, observe, isObservable } from "mobx";

class Command {
    constructor(command, args) {
        this.command = command;
        this.args = args;
        if (command !== null && isObservable(command.CanExecuteCount)) {
            observe(command, 'CanExecuteCount', () => this.command.CanExecute(this.args));
        }
    }

    @observable command;
    @observable args;

    @computed get canExecute() {
        return (this.command != null) && ((this.command.CanExecuteValue === undefined) || (this.command.CanExecuteValue))
    }

    execute() {
        const command = this.command;
        const args = this.args;
        if (this.canExecute) {
            command.Execute(args);
        }
    }
}

export {
    Command
}