import React, { Component } from 'react';
import { observer } from 'mobx-react';
import { extendObservable, isObservable, computed, observe } from 'mobx';

@observer
export default class CommandButton extends Component {
  constructor(props) {
    super(props);
    this.handleClick = this.handleClick.bind(this);
    console.log(JSON.stringify(this.props.command, null, 2))

    extendObservable(this.props.command, {
      canExecute: computed(() => (this.props.command != null) && (this.props.command.CanExecuteValue))
    })
    const args = this.props.args;
    if (this.props.command != null && isObservable(this.props.command.CanExecuteCount)){
      observe(this.props.command, 'CanExecuteCount', () => this.props.command.CanExecute(args));
    }
  }

  handleClick(event) {
    const command = this.props.command;
    const args = this.props.args;
    if (command.canExecute) {
      command.Execute(args);
    }
  }

  render() {
    return (
      <button type="button" className="btn btn-default" onClick={this.handleClick} disabled={!this.props.command.canExecute}>{this.props.name}</button>
    );
  }
}
