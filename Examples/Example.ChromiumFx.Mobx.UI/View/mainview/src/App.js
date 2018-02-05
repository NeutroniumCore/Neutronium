import React, { Component } from 'react';
import { observer } from 'mobx-react';
import logo from './logo.svg';
import './App.css';

@observer
export default class App extends Component {
  constructor(props) {
    super(props);
    this.handleInputChange = this.handleInputChange.bind(this);
    this.handleCityChange = this.handleCityChange.bind(this);
  }

  handleInputChange(event) {
    const target = event.target;
    const value = target.value;
    const name = target.name;
    this.props.viewModel[name] = value;
  }

  handleCityChange(event) {
    const value = event.target.value;
    this.props.viewModel.Local.City = value;
  }

  render() {
    const vm = this.props.viewModel;
    return (
      <div id="app" className="fluid container">
        <div className="jumbotron logo">
          <img src={logo} className="App-logo" alt="logo" />
          <h4>Welcome to Neutronium-Mobx-React</h4>
        </div>
        <form>
          <div className="form-group">
            <label htmlFor="name">Name</label>
            <input id="name" name="Name" placeholder="Name" className="form-control" value={vm.Name} onChange={this.handleInputChange} />
          </div>
          <div className="form-group">
            <label htmlFor="Last">Last Name</label>
            <input id="Last" name="LastName" placeholder="Last Name" className="form-control" value={vm.LastName} onChange={this.handleInputChange} />
          </div>
          <div className="form-group">
            <label htmlFor="City">City</label>
            <input id="City" placeholder="City" className="form-control" value={vm.Local.City} onChange={this.handleCityChange} />
          </div>
        </form>
        <div>
          {vm.Count}
        </div>

      </div>
    );
  }
}
