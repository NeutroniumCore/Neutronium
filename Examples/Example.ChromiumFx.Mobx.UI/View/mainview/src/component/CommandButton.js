import React, { Component } from 'react';
import { observer } from 'mobx-react';
import { Command } from './Command';

@observer
export default class CommandButton extends Component {
  constructor(props) {
    super(props);
    this.handleClick = this.handleClick.bind(this);
    this.command = new Command(this.props.command);
  }

  handleClick(event) {
    this.command.execute();
  }

  render() {
    return (
      <button type="button" className="btn btn-default" onClick={this.handleClick} disabled={!this.command.canExecute}>{this.props.name}</button>
    );
  }
}
