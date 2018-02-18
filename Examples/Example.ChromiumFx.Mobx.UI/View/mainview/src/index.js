import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import App from './App';
import { ready } from './mobxManager';
import 'bootstrap/dist/css/bootstrap.css';

ready.then(res => {
    ReactDOM.render(<App viewModel={res.vm.ViewModel}/>, document.getElementById('root'), res.ready);
})
