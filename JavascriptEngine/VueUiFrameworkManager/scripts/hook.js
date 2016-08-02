/******/ (function(modules) { // webpackBootstrap
/******/ 	// The module cache
/******/ 	var installedModules = {};
/******/
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/
/******/ 		// Check if module is in cache
/******/ 		if(installedModules[moduleId])
/******/ 			return installedModules[moduleId].exports;
/******/
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = installedModules[moduleId] = {
/******/ 			exports: {},
/******/ 			id: moduleId,
/******/ 			loaded: false
/******/ 		};
/******/
/******/ 		// Execute the module function
/******/ 		modules[moduleId].call(module.exports, module, module.exports, __webpack_require__);
/******/
/******/ 		// Flag the module as loaded
/******/ 		module.loaded = true;
/******/
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/
/******/
/******/ 	// expose the modules object (__webpack_modules__)
/******/ 	__webpack_require__.m = modules;
/******/
/******/ 	// expose the module cache
/******/ 	__webpack_require__.c = installedModules;
/******/
/******/ 	// __webpack_public_path__
/******/ 	__webpack_require__.p = "/build/";
/******/
/******/ 	// Load entry module and return exports
/******/ 	return __webpack_require__(0);
/******/ })
/************************************************************************/
/******/ ({

/***/ 0:
/***/ function(module, exports, __webpack_require__) {

	'use strict';
	
	var _hook = __webpack_require__(195);
	
	(0, _hook.installHook)(window);

/***/ },

/***/ 195:
/***/ function(module, exports) {

	'use strict';
	
	Object.defineProperty(exports, "__esModule", {
	  value: true
	});
	exports.installHook = installHook;
	// this script is injected into every page.
	
	/**
	 * Install the hook on window, which is an event emitter.
	 * Note because Chrome content scripts cannot directly modify the window object,
	 * we are evaling this function by inserting a script tag. That's why we have
	 * to inline the whole event emitter implementation here.
	 *
	 * @param {Window} window
	 */
	
	function installHook(window) {
	  var listeners = {};
	
	  var hook = {
	    Vue: null,
	
	    on: function on(event, fn) {
	      event = '$' + event;(listeners[event] || (listeners[event] = [])).push(fn);
	    },
	    once: function once(event, fn) {
	      event = '$' + event;
	      function on() {
	        this.off(event, on);
	        fn.apply(this, arguments);
	      }
	      ;(listeners[event] || (listeners[event] = [])).push(on);
	    },
	    off: function off(event, fn) {
	      event = '$' + event;
	      if (!arguments.length) {
	        listeners = {};
	      } else {
	        var cbs = listeners[event];
	        if (cbs) {
	          if (!fn) {
	            listeners[event] = null;
	          } else {
	            for (var i = 0, l = cbs.length; i < l; i++) {
	              var cb = cbs[i];
	              if (cb === fn || cb.fn === fn) {
	                cbs.splice(i, 1);
	                break;
	              }
	            }
	          }
	        }
	      }
	    },
	    emit: function emit(event) {
	      event = '$' + event;
	      var cbs = listeners[event];
	      if (cbs) {
	        var args = [].slice.call(arguments, 1);
	        cbs = cbs.slice();
	        for (var i = 0, l = cbs.length; i < l; i++) {
	          cbs[i].apply(this, args);
	        }
	      }
	    }
	  };
	
	  hook.once('init', function (Vue) {
	    hook.Vue = Vue;
	  });
	
	  hook.once('vuex:init', function (store) {
	    hook.store = store;
	  });
	
	  Object.defineProperty(window, '__VUE_DEVTOOLS_GLOBAL_HOOK__', {
	    get: function get() {
	      return hook;
	    }
	  });
	}

/***/ }

/******/ });