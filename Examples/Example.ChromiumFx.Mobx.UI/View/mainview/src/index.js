import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import App from './App';
import registerServiceWorker from './registerServiceWorker';
import { ready } from './mobxManager';
import 'bootstrap/dist/css/bootstrap.css';


ready.then(res =>{
    ReactDOM.render(<App viewModel={res.ViewModel}/>, document.getElementById('root'));
    registerServiceWorker();
})
