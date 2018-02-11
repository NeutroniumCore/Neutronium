import React, { Component } from 'react';
import { observer } from 'mobx-react';
import CommandButton from './CommandButton';

@observer
export default class Skill extends Component {
  constructor(props) {
    super(props);
    this.handleInputChange = this.handleInputChange.bind(this);
  }

  handleInputChange(event) {
    const target = event.target;
    const value = target.value;
    const name = target.name;
    this.props.skill[name] = value;
  }

  render() {
    return (
        <div className="list-group-item">
            <div className="col-md-12">
                <label className="col-md-2" htmlFor={'name'+this.props.id}>Name</label>
                <input className="col-md-10 form-control" id={'name'+this.props.id} placeholder="Name" name="Name" value={this.props.skill.Name} onChange={this.handleInputChange}>
                </input>
            </div>
            <div className="col-md-12">
                <label className="col-md-2" htmlFor={'type'+this.props.id}>Type</label>
                <input className="col-md-10 form-control" id={'type'+this.props.id} placeholder="Type"  name="Type" value={this.props.skill.Type} onChange={this.handleInputChange} >
                </input>
            </div>
            <div className="col-md-2 panel-body">
                <CommandButton command={this.props.removeSkill} arg={this.props.skill} name="Remove Skill"></CommandButton>
            </div>
      </div>
    );
  }
}
