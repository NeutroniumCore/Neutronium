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
/******/ ([
/* 0 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';
	
	var _backend = __webpack_require__(1);
	
	var _bridge = __webpack_require__(101);
	
	var _bridge2 = _interopRequireDefault(_bridge);
	
	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }
	
	var bridge = new _bridge2.default({
	  listen: function listen(fn) {
	    var target = document.getElementById("_debug_Vue_Node_");
	    var targetWindow = target.contentWindow;
	    targetWindow.addEventListener('message', function (evt) {
	      return fn(evt.data);
	    });
	  },
	  send: function send(data) {
	    window.postMessage(data, '*');
	  }
	});
	
	(0, _backend.initBackend)(bridge);

/***/ },
/* 1 */
/***/ function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function(process) {'use strict';
	
	Object.defineProperty(exports, "__esModule", {
	  value: true
	});
	
	var _keys = __webpack_require__(3);
	
	var _keys2 = _interopRequireDefault(_keys);
	
	var _map = __webpack_require__(38);
	
	var _map2 = _interopRequireDefault(_map);
	
	exports.initBackend = initBackend;
	
	var _highlighter = __webpack_require__(77);
	
	var _vuex = __webpack_require__(100);
	
	var _util = __webpack_require__(78);
	
	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }
	
	// hook should have been injected before this executes.
	var hook = window.__VUE_DEVTOOLS_GLOBAL_HOOK__; // This is the backend that is injected into the page that a Vue app lives in
	// when the Vue Devtools panel is activated.
	
	var rootInstances = [];
	var propModes = ['default', 'sync', 'once'];
	
	var instanceMap = window.__VUE_DEVTOOLS_INSTANCE_MAP__ = new _map2.default();
	var currentInspectedId = void 0;
	var bridge = void 0;
	var filter = '';
	var captureCount = 0;
	
	function initBackend(_bridge) {
	  bridge = _bridge;
	  if (hook.Vue) {
	    connect();
	  } else {
	    hook.once('init', connect);
	  }
	  if (hook.store) {
	    (0, _vuex.initVuexBackend)(hook, bridge);
	  } else {
	    hook.once('vuex:init', function (store) {
	      (0, _vuex.initVuexBackend)(hook, bridge);
	    });
	  }
	}
	
	function connect() {
	  hook.currentTab = 'components';
	  bridge.on('switch-tab', function (tab) {
	    hook.currentTab = tab;
	    if (tab === 'components') {
	      flush();
	    }
	  });
	
	  // the backend may get injected to the same page multiple times
	  // if the user closes and reopens the devtools.
	  // make sure there's only one flush listener.
	  hook.off('flush');
	  hook.on('flush', function () {
	    if (hook.currentTab === 'components') {
	      flush();
	    }
	  });
	
	  bridge.on('select-instance', function (id) {
	    currentInspectedId = id;
	    var instance = instanceMap.get(id);
	    if (instance) {
	      scrollIntoView(instance);
	      (0, _highlighter.highlight)(instance);
	    }
	    bridge.send('instance-details', (0, _util.stringify)(getInstanceDetails(id)));
	  });
	
	  bridge.on('send-to-console', function (id) {
	    window.$vm = instanceMap.get(id);
	    console.log('[vue-devtools] <' + getInstanceName(window.$vm) + '> is now available as $vm.');
	  });
	
	  bridge.on('filter-instances', function (_filter) {
	    filter = _filter.toLowerCase();
	    flush();
	  });
	
	  bridge.on('refresh', scan);
	  bridge.on('enter-instance', function (id) {
	    return (0, _highlighter.highlight)(instanceMap.get(id));
	  });
	  bridge.on('leave-instance', _highlighter.unHighlight);
	
	  bridge.log('backend ready.');
	  bridge.send('ready', hook.Vue.version);
	  console.log('[vue-devtools] Ready. Detected Vue v' + hook.Vue.version);
	  scan();
	}
	
	/**
	 * Scan the page for root level Vue instances.
	 */
	
	function scan() {
	  rootInstances.length = 0;
	  var inFragment = false;
	  var currentFragment = null;
	  walk(document, function (node) {
	    if (node.id === "_debug_Vue_Root_Node_") {
	      return true;
	    }
	    if (inFragment) {
	      if (node === currentFragment._fragmentEnd) {
	        inFragment = false;
	        currentFragment = null;
	      }
	      return true;
	    }
	    var instance = node.__vue__;
	    if (instance) {
	      if (instance._isFragment) {
	        inFragment = true;
	        currentFragment = instance;
	      }
	      rootInstances.push(instance);
	      return true;
	    }
	  });
	  flush();
	}
	
	/**
	 * DOM walk helper
	 *
	 * @param {NodeList} nodes
	 * @param {Function} fn
	 */
	
	function walk(node, fn) {
	  if (node.childNodes) {
	    Array.prototype.forEach.call(node.childNodes, function (node) {
	      var stop = fn(node);
	      if (!stop) {
	        walk(node, fn);
	      }
	    });
	  }
	}
	
	/**
	 * Called on every Vue.js batcher flush cycle.
	 * Capture current component tree structure and the state
	 * of the current inspected instance (if present) and
	 * send it to the devtools.
	 */
	
	function flush() {
	  var start = void 0;
	  if (process.env.NODE_ENV !== 'production') {
	    captureCount = 0;
	    start = window.performance.now();
	  }
	  var payload = (0, _util.stringify)({
	    inspectedInstance: getInstanceDetails(currentInspectedId),
	    instances: findQualifiedChildrenFromList(rootInstances)
	  });
	  bridge.send('flush', payload);
	}
	
	/**
	 * Iterate through an array of instances and flatten it into
	 * an array of qualified instances. This is a depth-first
	 * traversal - e.g. if an instance is not matched, we will
	 * recursively go deeper until a qualified child is found.
	 *
	 * @param {Array} instances
	 * @return {Array}
	 */
	
	function findQualifiedChildrenFromList(instances) {
	  instances = instances.filter(function (child) {
	    return !child._isBeingDestroyed;
	  });
	  return !filter ? instances.map(capture) : Array.prototype.concat.apply([], instances.map(findQualifiedChildren));
	}
	
	/**
	 * Find qualified children from a single instance.
	 * If the instance itself is qualified, just return itself.
	 * This is ok because [].concat works in both cases.
	 *
	 * @param {Vue} instance
	 * @return {Vue|Array}
	 */
	
	function findQualifiedChildren(instance) {
	  return isQualified(instance) ? capture(instance) : findQualifiedChildrenFromList(instance.$children);
	}
	
	/**
	 * Check if an instance is qualified.
	 *
	 * @param {Vue} instance
	 * @return {Boolean}
	 */
	
	function isQualified(instance) {
	  var name = getInstanceName(instance).toLowerCase();
	  return name.indexOf(filter) > -1;
	}
	
	/**
	 * Capture the meta information of an instance. (recursive)
	 *
	 * @param {Vue} instance
	 * @return {Object}
	 */
	
	function capture(instance, _, list) {
	  if (process.env.NODE_ENV !== 'production') {
	    captureCount++;
	  }
	  mark(instance);
	  var ret = {
	    id: instance._uid,
	    name: getInstanceName(instance),
	    inactive: !!instance._inactive,
	    isFragment: !!instance._isFragment,
	    children: instance.$children.filter(function (child) {
	      return !child._isBeingDestroyed;
	    }).map(capture)
	  };
	  // record screen position to ensure correct ordering
	  if ((!list || list.length > 1) && !instance._inactive) {
	    var rect = (0, _highlighter.getInstanceRect)(instance);
	    ret.top = rect ? rect.top : Infinity;
	  } else {
	    ret.top = Infinity;
	  }
	  // check router view
	  var isRouterView2 = instance.$vnode && instance.$vnode.data.routerView;
	  if (instance._routerView || isRouterView2) {
	    ret.isRouterView = true;
	    if (!instance._inactive) {
	      var matched = instance.$route.matched;
	      var depth = isRouterView2 ? instance.$vnode.data.routerViewDepth : instance._routerView.depth;
	      ret.matchedRouteSegment = matched && matched[depth] && (isRouterView2 ? matched[depth].path : matched[depth].handler.path);
	    }
	  }
	  return ret;
	}
	
	/**
	 * Mark an instance as captured and store it in the instance map.
	 *
	 * @param {Vue} instance
	 */
	
	function mark(instance) {
	  if (!instanceMap.has(instance._uid)) {
	    instanceMap.set(instance._uid, instance);
	    instance.$on('hook:beforeDestroy', function () {
	      instanceMap.delete(instance._uid);
	    });
	  }
	}
	
	/**
	 * Get the detailed information of an inspected instance.
	 *
	 * @param {Number} id
	 */
	
	function getInstanceDetails(id) {
	  var instance = instanceMap.get(id);
	  if (!instance) {
	    return {};
	  } else {
	    return {
	      id: id,
	      name: getInstanceName(instance),
	      state: processProps(instance).concat(processState(instance), processComputed(instance), processRouteContext(instance), processVuexGetters(instance), processFirebaseBindings(instance))
	    };
	  }
	}
	
	/**
	 * Get the appropriate display name for an instance.
	 *
	 * @param {Vue} instance
	 * @return {String}
	 */
	
	function getInstanceName(instance) {
	  var name = instance.$options.name || instance.$options._componentTag;
	  return name ? (0, _util.classify)(name) : instance.$root === instance ? 'Root' : 'Anonymous Component';
	}
	
	/**
	 * Process the props of an instance.
	 * Make sure return a plain object because window.postMessage()
	 * will throw an Error if the passed object contains Functions.
	 *
	 * @param {Vue} instance
	 * @return {Array}
	 */
	
	function processProps(instance) {
	  var props = void 0;
	  if (props = instance._props) {
	    // 1.x
	    return (0, _keys2.default)(props).map(function (key) {
	      var prop = props[key];
	      var options = prop.options;
	      return {
	        type: 'prop',
	        key: prop.path,
	        value: instance[prop.path],
	        meta: {
	          'type': options.type ? getPropType(options.type) : 'any',
	          required: !!options.required,
	          'binding mode': propModes[prop.mode]
	        }
	      };
	    });
	  } else if (props = instance.$options.props) {
	    // 2.0
	    return (0, _keys2.default)(props).map(function (key) {
	      var prop = props[key];
	      key = (0, _util.camelize)(key);
	      return {
	        type: 'prop',
	        key: key,
	        value: instance[key],
	        meta: {
	          type: prop.type ? getPropType(prop.type) : 'any',
	          required: !!prop.required
	        }
	      };
	    });
	  } else {
	    return [];
	  }
	}
	
	/**
	 * Convert prop type constructor to string.
	 *
	 * @param {Function} fn
	 */
	
	var fnTypeRE = /^function (\w+)\(/;
	function getPropType(type) {
	  return typeof type === 'function' ? type.toString().match(fnTypeRE)[1] : 'any';
	}
	
	/**
	 * Process state, filtering out props and "clean" the result
	 * with a JSON dance. This removes functions which can cause
	 * errors during structured clone used by window.postMessage.
	 *
	 * @param {Vue} instance
	 * @return {Array}
	 */
	
	function processState(instance) {
	  var props = instance._props || instance.$options.props;
	  var getters = instance.$options.vuex && instance.$options.vuex.getters;
	  return (0, _keys2.default)(instance._data).filter(function (key) {
	    return !(props && key in props) && !(getters && key in getters);
	  }).map(function (key) {
	    return {
	      key: key,
	      value: instance[key]
	    };
	  });
	}
	
	/**
	 * Process the computed properties of an instance.
	 *
	 * @param {Vue} instance
	 * @return {Array}
	 */
	
	function processComputed(instance) {
	  return (0, _keys2.default)(instance.$options.computed || {}).map(function (key) {
	    return {
	      type: 'computed',
	      key: key,
	      value: instance[key]
	    };
	  });
	}
	
	/**
	 * Process possible vue-router $route context
	 *
	 * @param {Vue} instance
	 * @return {Array}
	 */
	
	function processRouteContext(instance) {
	  var route = instance.$route;
	  if (route) {
	    var path = route.path;
	    var query = route.query;
	    var params = route.params;
	
	    var value = { path: path, query: query, params: params };
	    if (route.fullPath) value.fullPath = route.fullPath;
	    return [{
	      key: '$route',
	      value: value
	    }];
	  } else {
	    return [];
	  }
	}
	
	/**
	 * Process Vuex getters.
	 *
	 * @param {Vue} instance
	 * @return {Array}
	 */
	
	function processVuexGetters(instance) {
	  var getters = instance.$options.vuex && instance.$options.vuex.getters;
	  if (getters) {
	    return (0, _keys2.default)(getters).map(function (key) {
	      return {
	        type: 'vuex getter',
	        key: key,
	        value: instance[key]
	      };
	    });
	  } else {
	    return [];
	  }
	}
	
	/**
	 * Process Firebase bindings.
	 *
	 * @param {Vue} instance
	 * @return {Array}
	 */
	
	function processFirebaseBindings(instance) {
	  var refs = instance.$firebaseRefs;
	  if (refs) {
	    return (0, _keys2.default)(refs).map(function (key) {
	      return {
	        type: 'firebase binding',
	        key: key,
	        value: instance[key]
	      };
	    });
	  } else {
	    return [];
	  }
	}
	
	/**
	 * Sroll a node into view.
	 *
	 * @param {Vue} instance
	 */
	
	function scrollIntoView(instance) {
	  var rect = (0, _highlighter.getInstanceRect)(instance);
	  if (rect) {
	    window.scrollBy(0, rect.top);
	  }
	}
	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(2)))

/***/ },
/* 2 */
/***/ function(module, exports) {

	// shim for using process in browser
	
	var process = module.exports = {};
	
	// cached from whatever global is present so that test runners that stub it
	// don't break things.  But we need to wrap it in a try catch in case it is
	// wrapped in strict mode code which doesn't define any globals.  It's inside a
	// function because try/catches deoptimize in certain engines.
	
	var cachedSetTimeout;
	var cachedClearTimeout;
	
	(function () {
	  try {
	    cachedSetTimeout = setTimeout;
	  } catch (e) {
	    cachedSetTimeout = function () {
	      throw new Error('setTimeout is not defined');
	    }
	  }
	  try {
	    cachedClearTimeout = clearTimeout;
	  } catch (e) {
	    cachedClearTimeout = function () {
	      throw new Error('clearTimeout is not defined');
	    }
	  }
	} ())
	var queue = [];
	var draining = false;
	var currentQueue;
	var queueIndex = -1;
	
	function cleanUpNextTick() {
	    if (!draining || !currentQueue) {
	        return;
	    }
	    draining = false;
	    if (currentQueue.length) {
	        queue = currentQueue.concat(queue);
	    } else {
	        queueIndex = -1;
	    }
	    if (queue.length) {
	        drainQueue();
	    }
	}
	
	function drainQueue() {
	    if (draining) {
	        return;
	    }
	    var timeout = cachedSetTimeout.call(null, cleanUpNextTick);
	    draining = true;
	
	    var len = queue.length;
	    while(len) {
	        currentQueue = queue;
	        queue = [];
	        while (++queueIndex < len) {
	            if (currentQueue) {
	                currentQueue[queueIndex].run();
	            }
	        }
	        queueIndex = -1;
	        len = queue.length;
	    }
	    currentQueue = null;
	    draining = false;
	    cachedClearTimeout.call(null, timeout);
	}
	
	process.nextTick = function (fun) {
	    var args = new Array(arguments.length - 1);
	    if (arguments.length > 1) {
	        for (var i = 1; i < arguments.length; i++) {
	            args[i - 1] = arguments[i];
	        }
	    }
	    queue.push(new Item(fun, args));
	    if (queue.length === 1 && !draining) {
	        cachedSetTimeout.call(null, drainQueue, 0);
	    }
	};
	
	// v8 likes predictible objects
	function Item(fun, array) {
	    this.fun = fun;
	    this.array = array;
	}
	Item.prototype.run = function () {
	    this.fun.apply(null, this.array);
	};
	process.title = 'browser';
	process.browser = true;
	process.env = {};
	process.argv = [];
	process.version = ''; // empty string to avoid regexp issues
	process.versions = {};
	
	function noop() {}
	
	process.on = noop;
	process.addListener = noop;
	process.once = noop;
	process.off = noop;
	process.removeListener = noop;
	process.removeAllListeners = noop;
	process.emit = noop;
	
	process.binding = function (name) {
	    throw new Error('process.binding is not supported');
	};
	
	process.cwd = function () { return '/' };
	process.chdir = function (dir) {
	    throw new Error('process.chdir is not supported');
	};
	process.umask = function() { return 0; };


/***/ },
/* 3 */
/***/ function(module, exports, __webpack_require__) {

	module.exports = { "default": __webpack_require__(4), __esModule: true };

/***/ },
/* 4 */
/***/ function(module, exports, __webpack_require__) {

	__webpack_require__(5);
	module.exports = __webpack_require__(25).Object.keys;

/***/ },
/* 5 */
/***/ function(module, exports, __webpack_require__) {

	// 19.1.2.14 Object.keys(O)
	var toObject = __webpack_require__(6)
	  , $keys    = __webpack_require__(8);
	
	__webpack_require__(23)('keys', function(){
	  return function keys(it){
	    return $keys(toObject(it));
	  };
	});

/***/ },
/* 6 */
/***/ function(module, exports, __webpack_require__) {

	// 7.1.13 ToObject(argument)
	var defined = __webpack_require__(7);
	module.exports = function(it){
	  return Object(defined(it));
	};

/***/ },
/* 7 */
/***/ function(module, exports) {

	// 7.2.1 RequireObjectCoercible(argument)
	module.exports = function(it){
	  if(it == undefined)throw TypeError("Can't call method on  " + it);
	  return it;
	};

/***/ },
/* 8 */
/***/ function(module, exports, __webpack_require__) {

	// 19.1.2.14 / 15.2.3.14 Object.keys(O)
	var $keys       = __webpack_require__(9)
	  , enumBugKeys = __webpack_require__(22);
	
	module.exports = Object.keys || function keys(O){
	  return $keys(O, enumBugKeys);
	};

/***/ },
/* 9 */
/***/ function(module, exports, __webpack_require__) {

	var has          = __webpack_require__(10)
	  , toIObject    = __webpack_require__(11)
	  , arrayIndexOf = __webpack_require__(14)(false)
	  , IE_PROTO     = __webpack_require__(18)('IE_PROTO');
	
	module.exports = function(object, names){
	  var O      = toIObject(object)
	    , i      = 0
	    , result = []
	    , key;
	  for(key in O)if(key != IE_PROTO)has(O, key) && result.push(key);
	  // Don't enum bug & hidden keys
	  while(names.length > i)if(has(O, key = names[i++])){
	    ~arrayIndexOf(result, key) || result.push(key);
	  }
	  return result;
	};

/***/ },
/* 10 */
/***/ function(module, exports) {

	var hasOwnProperty = {}.hasOwnProperty;
	module.exports = function(it, key){
	  return hasOwnProperty.call(it, key);
	};

/***/ },
/* 11 */
/***/ function(module, exports, __webpack_require__) {

	// to indexed object, toObject with fallback for non-array-like ES3 strings
	var IObject = __webpack_require__(12)
	  , defined = __webpack_require__(7);
	module.exports = function(it){
	  return IObject(defined(it));
	};

/***/ },
/* 12 */
/***/ function(module, exports, __webpack_require__) {

	// fallback for non-array-like ES3 and non-enumerable old V8 strings
	var cof = __webpack_require__(13);
	module.exports = Object('z').propertyIsEnumerable(0) ? Object : function(it){
	  return cof(it) == 'String' ? it.split('') : Object(it);
	};

/***/ },
/* 13 */
/***/ function(module, exports) {

	var toString = {}.toString;
	
	module.exports = function(it){
	  return toString.call(it).slice(8, -1);
	};

/***/ },
/* 14 */
/***/ function(module, exports, __webpack_require__) {

	// false -> Array#indexOf
	// true  -> Array#includes
	var toIObject = __webpack_require__(11)
	  , toLength  = __webpack_require__(15)
	  , toIndex   = __webpack_require__(17);
	module.exports = function(IS_INCLUDES){
	  return function($this, el, fromIndex){
	    var O      = toIObject($this)
	      , length = toLength(O.length)
	      , index  = toIndex(fromIndex, length)
	      , value;
	    // Array#includes uses SameValueZero equality algorithm
	    if(IS_INCLUDES && el != el)while(length > index){
	      value = O[index++];
	      if(value != value)return true;
	    // Array#toIndex ignores holes, Array#includes - not
	    } else for(;length > index; index++)if(IS_INCLUDES || index in O){
	      if(O[index] === el)return IS_INCLUDES || index || 0;
	    } return !IS_INCLUDES && -1;
	  };
	};

/***/ },
/* 15 */
/***/ function(module, exports, __webpack_require__) {

	// 7.1.15 ToLength
	var toInteger = __webpack_require__(16)
	  , min       = Math.min;
	module.exports = function(it){
	  return it > 0 ? min(toInteger(it), 0x1fffffffffffff) : 0; // pow(2, 53) - 1 == 9007199254740991
	};

/***/ },
/* 16 */
/***/ function(module, exports) {

	// 7.1.4 ToInteger
	var ceil  = Math.ceil
	  , floor = Math.floor;
	module.exports = function(it){
	  return isNaN(it = +it) ? 0 : (it > 0 ? floor : ceil)(it);
	};

/***/ },
/* 17 */
/***/ function(module, exports, __webpack_require__) {

	var toInteger = __webpack_require__(16)
	  , max       = Math.max
	  , min       = Math.min;
	module.exports = function(index, length){
	  index = toInteger(index);
	  return index < 0 ? max(index + length, 0) : min(index, length);
	};

/***/ },
/* 18 */
/***/ function(module, exports, __webpack_require__) {

	var shared = __webpack_require__(19)('keys')
	  , uid    = __webpack_require__(21);
	module.exports = function(key){
	  return shared[key] || (shared[key] = uid(key));
	};

/***/ },
/* 19 */
/***/ function(module, exports, __webpack_require__) {

	var global = __webpack_require__(20)
	  , SHARED = '__core-js_shared__'
	  , store  = global[SHARED] || (global[SHARED] = {});
	module.exports = function(key){
	  return store[key] || (store[key] = {});
	};

/***/ },
/* 20 */
/***/ function(module, exports) {

	// https://github.com/zloirock/core-js/issues/86#issuecomment-115759028
	var global = module.exports = typeof window != 'undefined' && window.Math == Math
	  ? window : typeof self != 'undefined' && self.Math == Math ? self : Function('return this')();
	if(typeof __g == 'number')__g = global; // eslint-disable-line no-undef

/***/ },
/* 21 */
/***/ function(module, exports) {

	var id = 0
	  , px = Math.random();
	module.exports = function(key){
	  return 'Symbol('.concat(key === undefined ? '' : key, ')_', (++id + px).toString(36));
	};

/***/ },
/* 22 */
/***/ function(module, exports) {

	// IE 8- don't enum bug keys
	module.exports = (
	  'constructor,hasOwnProperty,isPrototypeOf,propertyIsEnumerable,toLocaleString,toString,valueOf'
	).split(',');

/***/ },
/* 23 */
/***/ function(module, exports, __webpack_require__) {

	// most Object methods by ES6 should accept primitives
	var $export = __webpack_require__(24)
	  , core    = __webpack_require__(25)
	  , fails   = __webpack_require__(34);
	module.exports = function(KEY, exec){
	  var fn  = (core.Object || {})[KEY] || Object[KEY]
	    , exp = {};
	  exp[KEY] = exec(fn);
	  $export($export.S + $export.F * fails(function(){ fn(1); }), 'Object', exp);
	};

/***/ },
/* 24 */
/***/ function(module, exports, __webpack_require__) {

	var global    = __webpack_require__(20)
	  , core      = __webpack_require__(25)
	  , ctx       = __webpack_require__(26)
	  , hide      = __webpack_require__(28)
	  , PROTOTYPE = 'prototype';
	
	var $export = function(type, name, source){
	  var IS_FORCED = type & $export.F
	    , IS_GLOBAL = type & $export.G
	    , IS_STATIC = type & $export.S
	    , IS_PROTO  = type & $export.P
	    , IS_BIND   = type & $export.B
	    , IS_WRAP   = type & $export.W
	    , exports   = IS_GLOBAL ? core : core[name] || (core[name] = {})
	    , expProto  = exports[PROTOTYPE]
	    , target    = IS_GLOBAL ? global : IS_STATIC ? global[name] : (global[name] || {})[PROTOTYPE]
	    , key, own, out;
	  if(IS_GLOBAL)source = name;
	  for(key in source){
	    // contains in native
	    own = !IS_FORCED && target && target[key] !== undefined;
	    if(own && key in exports)continue;
	    // export native or passed
	    out = own ? target[key] : source[key];
	    // prevent global pollution for namespaces
	    exports[key] = IS_GLOBAL && typeof target[key] != 'function' ? source[key]
	    // bind timers to global for call from export context
	    : IS_BIND && own ? ctx(out, global)
	    // wrap global constructors for prevent change them in library
	    : IS_WRAP && target[key] == out ? (function(C){
	      var F = function(a, b, c){
	        if(this instanceof C){
	          switch(arguments.length){
	            case 0: return new C;
	            case 1: return new C(a);
	            case 2: return new C(a, b);
	          } return new C(a, b, c);
	        } return C.apply(this, arguments);
	      };
	      F[PROTOTYPE] = C[PROTOTYPE];
	      return F;
	    // make static versions for prototype methods
	    })(out) : IS_PROTO && typeof out == 'function' ? ctx(Function.call, out) : out;
	    // export proto methods to core.%CONSTRUCTOR%.methods.%NAME%
	    if(IS_PROTO){
	      (exports.virtual || (exports.virtual = {}))[key] = out;
	      // export proto methods to core.%CONSTRUCTOR%.prototype.%NAME%
	      if(type & $export.R && expProto && !expProto[key])hide(expProto, key, out);
	    }
	  }
	};
	// type bitmap
	$export.F = 1;   // forced
	$export.G = 2;   // global
	$export.S = 4;   // static
	$export.P = 8;   // proto
	$export.B = 16;  // bind
	$export.W = 32;  // wrap
	$export.U = 64;  // safe
	$export.R = 128; // real proto method for `library` 
	module.exports = $export;

/***/ },
/* 25 */
/***/ function(module, exports) {

	var core = module.exports = {version: '2.4.0'};
	if(typeof __e == 'number')__e = core; // eslint-disable-line no-undef

/***/ },
/* 26 */
/***/ function(module, exports, __webpack_require__) {

	// optional / simple context binding
	var aFunction = __webpack_require__(27);
	module.exports = function(fn, that, length){
	  aFunction(fn);
	  if(that === undefined)return fn;
	  switch(length){
	    case 1: return function(a){
	      return fn.call(that, a);
	    };
	    case 2: return function(a, b){
	      return fn.call(that, a, b);
	    };
	    case 3: return function(a, b, c){
	      return fn.call(that, a, b, c);
	    };
	  }
	  return function(/* ...args */){
	    return fn.apply(that, arguments);
	  };
	};

/***/ },
/* 27 */
/***/ function(module, exports) {

	module.exports = function(it){
	  if(typeof it != 'function')throw TypeError(it + ' is not a function!');
	  return it;
	};

/***/ },
/* 28 */
/***/ function(module, exports, __webpack_require__) {

	var dP         = __webpack_require__(29)
	  , createDesc = __webpack_require__(37);
	module.exports = __webpack_require__(33) ? function(object, key, value){
	  return dP.f(object, key, createDesc(1, value));
	} : function(object, key, value){
	  object[key] = value;
	  return object;
	};

/***/ },
/* 29 */
/***/ function(module, exports, __webpack_require__) {

	var anObject       = __webpack_require__(30)
	  , IE8_DOM_DEFINE = __webpack_require__(32)
	  , toPrimitive    = __webpack_require__(36)
	  , dP             = Object.defineProperty;
	
	exports.f = __webpack_require__(33) ? Object.defineProperty : function defineProperty(O, P, Attributes){
	  anObject(O);
	  P = toPrimitive(P, true);
	  anObject(Attributes);
	  if(IE8_DOM_DEFINE)try {
	    return dP(O, P, Attributes);
	  } catch(e){ /* empty */ }
	  if('get' in Attributes || 'set' in Attributes)throw TypeError('Accessors not supported!');
	  if('value' in Attributes)O[P] = Attributes.value;
	  return O;
	};

/***/ },
/* 30 */
/***/ function(module, exports, __webpack_require__) {

	var isObject = __webpack_require__(31);
	module.exports = function(it){
	  if(!isObject(it))throw TypeError(it + ' is not an object!');
	  return it;
	};

/***/ },
/* 31 */
/***/ function(module, exports) {

	module.exports = function(it){
	  return typeof it === 'object' ? it !== null : typeof it === 'function';
	};

/***/ },
/* 32 */
/***/ function(module, exports, __webpack_require__) {

	module.exports = !__webpack_require__(33) && !__webpack_require__(34)(function(){
	  return Object.defineProperty(__webpack_require__(35)('div'), 'a', {get: function(){ return 7; }}).a != 7;
	});

/***/ },
/* 33 */
/***/ function(module, exports, __webpack_require__) {

	// Thank's IE8 for his funny defineProperty
	module.exports = !__webpack_require__(34)(function(){
	  return Object.defineProperty({}, 'a', {get: function(){ return 7; }}).a != 7;
	});

/***/ },
/* 34 */
/***/ function(module, exports) {

	module.exports = function(exec){
	  try {
	    return !!exec();
	  } catch(e){
	    return true;
	  }
	};

/***/ },
/* 35 */
/***/ function(module, exports, __webpack_require__) {

	var isObject = __webpack_require__(31)
	  , document = __webpack_require__(20).document
	  // in old IE typeof document.createElement is 'object'
	  , is = isObject(document) && isObject(document.createElement);
	module.exports = function(it){
	  return is ? document.createElement(it) : {};
	};

/***/ },
/* 36 */
/***/ function(module, exports, __webpack_require__) {

	// 7.1.1 ToPrimitive(input [, PreferredType])
	var isObject = __webpack_require__(31);
	// instead of the ES6 spec version, we didn't implement @@toPrimitive case
	// and the second argument - flag - preferred type is a string
	module.exports = function(it, S){
	  if(!isObject(it))return it;
	  var fn, val;
	  if(S && typeof (fn = it.toString) == 'function' && !isObject(val = fn.call(it)))return val;
	  if(typeof (fn = it.valueOf) == 'function' && !isObject(val = fn.call(it)))return val;
	  if(!S && typeof (fn = it.toString) == 'function' && !isObject(val = fn.call(it)))return val;
	  throw TypeError("Can't convert object to primitive value");
	};

/***/ },
/* 37 */
/***/ function(module, exports) {

	module.exports = function(bitmap, value){
	  return {
	    enumerable  : !(bitmap & 1),
	    configurable: !(bitmap & 2),
	    writable    : !(bitmap & 4),
	    value       : value
	  };
	};

/***/ },
/* 38 */
/***/ function(module, exports, __webpack_require__) {

	module.exports = { "default": __webpack_require__(39), __esModule: true };

/***/ },
/* 39 */
/***/ function(module, exports, __webpack_require__) {

	__webpack_require__(40);
	__webpack_require__(41);
	__webpack_require__(54);
	__webpack_require__(58);
	__webpack_require__(74);
	module.exports = __webpack_require__(25).Map;

/***/ },
/* 40 */
/***/ function(module, exports) {



/***/ },
/* 41 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';
	var $at  = __webpack_require__(42)(true);
	
	// 21.1.3.27 String.prototype[@@iterator]()
	__webpack_require__(43)(String, 'String', function(iterated){
	  this._t = String(iterated); // target
	  this._i = 0;                // next index
	// 21.1.5.2.1 %StringIteratorPrototype%.next()
	}, function(){
	  var O     = this._t
	    , index = this._i
	    , point;
	  if(index >= O.length)return {value: undefined, done: true};
	  point = $at(O, index);
	  this._i += point.length;
	  return {value: point, done: false};
	});

/***/ },
/* 42 */
/***/ function(module, exports, __webpack_require__) {

	var toInteger = __webpack_require__(16)
	  , defined   = __webpack_require__(7);
	// true  -> String#at
	// false -> String#codePointAt
	module.exports = function(TO_STRING){
	  return function(that, pos){
	    var s = String(defined(that))
	      , i = toInteger(pos)
	      , l = s.length
	      , a, b;
	    if(i < 0 || i >= l)return TO_STRING ? '' : undefined;
	    a = s.charCodeAt(i);
	    return a < 0xd800 || a > 0xdbff || i + 1 === l || (b = s.charCodeAt(i + 1)) < 0xdc00 || b > 0xdfff
	      ? TO_STRING ? s.charAt(i) : a
	      : TO_STRING ? s.slice(i, i + 2) : (a - 0xd800 << 10) + (b - 0xdc00) + 0x10000;
	  };
	};

/***/ },
/* 43 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';
	var LIBRARY        = __webpack_require__(44)
	  , $export        = __webpack_require__(24)
	  , redefine       = __webpack_require__(45)
	  , hide           = __webpack_require__(28)
	  , has            = __webpack_require__(10)
	  , Iterators      = __webpack_require__(46)
	  , $iterCreate    = __webpack_require__(47)
	  , setToStringTag = __webpack_require__(51)
	  , getPrototypeOf = __webpack_require__(53)
	  , ITERATOR       = __webpack_require__(52)('iterator')
	  , BUGGY          = !([].keys && 'next' in [].keys()) // Safari has buggy iterators w/o `next`
	  , FF_ITERATOR    = '@@iterator'
	  , KEYS           = 'keys'
	  , VALUES         = 'values';
	
	var returnThis = function(){ return this; };
	
	module.exports = function(Base, NAME, Constructor, next, DEFAULT, IS_SET, FORCED){
	  $iterCreate(Constructor, NAME, next);
	  var getMethod = function(kind){
	    if(!BUGGY && kind in proto)return proto[kind];
	    switch(kind){
	      case KEYS: return function keys(){ return new Constructor(this, kind); };
	      case VALUES: return function values(){ return new Constructor(this, kind); };
	    } return function entries(){ return new Constructor(this, kind); };
	  };
	  var TAG        = NAME + ' Iterator'
	    , DEF_VALUES = DEFAULT == VALUES
	    , VALUES_BUG = false
	    , proto      = Base.prototype
	    , $native    = proto[ITERATOR] || proto[FF_ITERATOR] || DEFAULT && proto[DEFAULT]
	    , $default   = $native || getMethod(DEFAULT)
	    , $entries   = DEFAULT ? !DEF_VALUES ? $default : getMethod('entries') : undefined
	    , $anyNative = NAME == 'Array' ? proto.entries || $native : $native
	    , methods, key, IteratorPrototype;
	  // Fix native
	  if($anyNative){
	    IteratorPrototype = getPrototypeOf($anyNative.call(new Base));
	    if(IteratorPrototype !== Object.prototype){
	      // Set @@toStringTag to native iterators
	      setToStringTag(IteratorPrototype, TAG, true);
	      // fix for some old engines
	      if(!LIBRARY && !has(IteratorPrototype, ITERATOR))hide(IteratorPrototype, ITERATOR, returnThis);
	    }
	  }
	  // fix Array#{values, @@iterator}.name in V8 / FF
	  if(DEF_VALUES && $native && $native.name !== VALUES){
	    VALUES_BUG = true;
	    $default = function values(){ return $native.call(this); };
	  }
	  // Define iterator
	  if((!LIBRARY || FORCED) && (BUGGY || VALUES_BUG || !proto[ITERATOR])){
	    hide(proto, ITERATOR, $default);
	  }
	  // Plug for library
	  Iterators[NAME] = $default;
	  Iterators[TAG]  = returnThis;
	  if(DEFAULT){
	    methods = {
	      values:  DEF_VALUES ? $default : getMethod(VALUES),
	      keys:    IS_SET     ? $default : getMethod(KEYS),
	      entries: $entries
	    };
	    if(FORCED)for(key in methods){
	      if(!(key in proto))redefine(proto, key, methods[key]);
	    } else $export($export.P + $export.F * (BUGGY || VALUES_BUG), NAME, methods);
	  }
	  return methods;
	};

/***/ },
/* 44 */
/***/ function(module, exports) {

	module.exports = true;

/***/ },
/* 45 */
/***/ function(module, exports, __webpack_require__) {

	module.exports = __webpack_require__(28);

/***/ },
/* 46 */
/***/ function(module, exports) {

	module.exports = {};

/***/ },
/* 47 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';
	var create         = __webpack_require__(48)
	  , descriptor     = __webpack_require__(37)
	  , setToStringTag = __webpack_require__(51)
	  , IteratorPrototype = {};
	
	// 25.1.2.1.1 %IteratorPrototype%[@@iterator]()
	__webpack_require__(28)(IteratorPrototype, __webpack_require__(52)('iterator'), function(){ return this; });
	
	module.exports = function(Constructor, NAME, next){
	  Constructor.prototype = create(IteratorPrototype, {next: descriptor(1, next)});
	  setToStringTag(Constructor, NAME + ' Iterator');
	};

/***/ },
/* 48 */
/***/ function(module, exports, __webpack_require__) {

	// 19.1.2.2 / 15.2.3.5 Object.create(O [, Properties])
	var anObject    = __webpack_require__(30)
	  , dPs         = __webpack_require__(49)
	  , enumBugKeys = __webpack_require__(22)
	  , IE_PROTO    = __webpack_require__(18)('IE_PROTO')
	  , Empty       = function(){ /* empty */ }
	  , PROTOTYPE   = 'prototype';
	
	// Create object with fake `null` prototype: use iframe Object with cleared prototype
	var createDict = function(){
	  // Thrash, waste and sodomy: IE GC bug
	  var iframe = __webpack_require__(35)('iframe')
	    , i      = enumBugKeys.length
	    , lt     = '<'
	    , gt     = '>'
	    , iframeDocument;
	  iframe.style.display = 'none';
	  __webpack_require__(50).appendChild(iframe);
	  iframe.src = 'javascript:'; // eslint-disable-line no-script-url
	  // createDict = iframe.contentWindow.Object;
	  // html.removeChild(iframe);
	  iframeDocument = iframe.contentWindow.document;
	  iframeDocument.open();
	  iframeDocument.write(lt + 'script' + gt + 'document.F=Object' + lt + '/script' + gt);
	  iframeDocument.close();
	  createDict = iframeDocument.F;
	  while(i--)delete createDict[PROTOTYPE][enumBugKeys[i]];
	  return createDict();
	};
	
	module.exports = Object.create || function create(O, Properties){
	  var result;
	  if(O !== null){
	    Empty[PROTOTYPE] = anObject(O);
	    result = new Empty;
	    Empty[PROTOTYPE] = null;
	    // add "__proto__" for Object.getPrototypeOf polyfill
	    result[IE_PROTO] = O;
	  } else result = createDict();
	  return Properties === undefined ? result : dPs(result, Properties);
	};


/***/ },
/* 49 */
/***/ function(module, exports, __webpack_require__) {

	var dP       = __webpack_require__(29)
	  , anObject = __webpack_require__(30)
	  , getKeys  = __webpack_require__(8);
	
	module.exports = __webpack_require__(33) ? Object.defineProperties : function defineProperties(O, Properties){
	  anObject(O);
	  var keys   = getKeys(Properties)
	    , length = keys.length
	    , i = 0
	    , P;
	  while(length > i)dP.f(O, P = keys[i++], Properties[P]);
	  return O;
	};

/***/ },
/* 50 */
/***/ function(module, exports, __webpack_require__) {

	module.exports = __webpack_require__(20).document && document.documentElement;

/***/ },
/* 51 */
/***/ function(module, exports, __webpack_require__) {

	var def = __webpack_require__(29).f
	  , has = __webpack_require__(10)
	  , TAG = __webpack_require__(52)('toStringTag');
	
	module.exports = function(it, tag, stat){
	  if(it && !has(it = stat ? it : it.prototype, TAG))def(it, TAG, {configurable: true, value: tag});
	};

/***/ },
/* 52 */
/***/ function(module, exports, __webpack_require__) {

	var store      = __webpack_require__(19)('wks')
	  , uid        = __webpack_require__(21)
	  , Symbol     = __webpack_require__(20).Symbol
	  , USE_SYMBOL = typeof Symbol == 'function';
	
	var $exports = module.exports = function(name){
	  return store[name] || (store[name] =
	    USE_SYMBOL && Symbol[name] || (USE_SYMBOL ? Symbol : uid)('Symbol.' + name));
	};
	
	$exports.store = store;

/***/ },
/* 53 */
/***/ function(module, exports, __webpack_require__) {

	// 19.1.2.9 / 15.2.3.2 Object.getPrototypeOf(O)
	var has         = __webpack_require__(10)
	  , toObject    = __webpack_require__(6)
	  , IE_PROTO    = __webpack_require__(18)('IE_PROTO')
	  , ObjectProto = Object.prototype;
	
	module.exports = Object.getPrototypeOf || function(O){
	  O = toObject(O);
	  if(has(O, IE_PROTO))return O[IE_PROTO];
	  if(typeof O.constructor == 'function' && O instanceof O.constructor){
	    return O.constructor.prototype;
	  } return O instanceof Object ? ObjectProto : null;
	};

/***/ },
/* 54 */
/***/ function(module, exports, __webpack_require__) {

	__webpack_require__(55);
	var global        = __webpack_require__(20)
	  , hide          = __webpack_require__(28)
	  , Iterators     = __webpack_require__(46)
	  , TO_STRING_TAG = __webpack_require__(52)('toStringTag');
	
	for(var collections = ['NodeList', 'DOMTokenList', 'MediaList', 'StyleSheetList', 'CSSRuleList'], i = 0; i < 5; i++){
	  var NAME       = collections[i]
	    , Collection = global[NAME]
	    , proto      = Collection && Collection.prototype;
	  if(proto && !proto[TO_STRING_TAG])hide(proto, TO_STRING_TAG, NAME);
	  Iterators[NAME] = Iterators.Array;
	}

/***/ },
/* 55 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';
	var addToUnscopables = __webpack_require__(56)
	  , step             = __webpack_require__(57)
	  , Iterators        = __webpack_require__(46)
	  , toIObject        = __webpack_require__(11);
	
	// 22.1.3.4 Array.prototype.entries()
	// 22.1.3.13 Array.prototype.keys()
	// 22.1.3.29 Array.prototype.values()
	// 22.1.3.30 Array.prototype[@@iterator]()
	module.exports = __webpack_require__(43)(Array, 'Array', function(iterated, kind){
	  this._t = toIObject(iterated); // target
	  this._i = 0;                   // next index
	  this._k = kind;                // kind
	// 22.1.5.2.1 %ArrayIteratorPrototype%.next()
	}, function(){
	  var O     = this._t
	    , kind  = this._k
	    , index = this._i++;
	  if(!O || index >= O.length){
	    this._t = undefined;
	    return step(1);
	  }
	  if(kind == 'keys'  )return step(0, index);
	  if(kind == 'values')return step(0, O[index]);
	  return step(0, [index, O[index]]);
	}, 'values');
	
	// argumentsList[@@iterator] is %ArrayProto_values% (9.4.4.6, 9.4.4.7)
	Iterators.Arguments = Iterators.Array;
	
	addToUnscopables('keys');
	addToUnscopables('values');
	addToUnscopables('entries');

/***/ },
/* 56 */
/***/ function(module, exports) {

	module.exports = function(){ /* empty */ };

/***/ },
/* 57 */
/***/ function(module, exports) {

	module.exports = function(done, value){
	  return {value: value, done: !!done};
	};

/***/ },
/* 58 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';
	var strong = __webpack_require__(59);
	
	// 23.1 Map Objects
	module.exports = __webpack_require__(69)('Map', function(get){
	  return function Map(){ return get(this, arguments.length > 0 ? arguments[0] : undefined); };
	}, {
	  // 23.1.3.6 Map.prototype.get(key)
	  get: function get(key){
	    var entry = strong.getEntry(this, key);
	    return entry && entry.v;
	  },
	  // 23.1.3.9 Map.prototype.set(key, value)
	  set: function set(key, value){
	    return strong.def(this, key === 0 ? 0 : key, value);
	  }
	}, strong, true);

/***/ },
/* 59 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';
	var dP          = __webpack_require__(29).f
	  , create      = __webpack_require__(48)
	  , redefineAll = __webpack_require__(60)
	  , ctx         = __webpack_require__(26)
	  , anInstance  = __webpack_require__(61)
	  , defined     = __webpack_require__(7)
	  , forOf       = __webpack_require__(62)
	  , $iterDefine = __webpack_require__(43)
	  , step        = __webpack_require__(57)
	  , setSpecies  = __webpack_require__(67)
	  , DESCRIPTORS = __webpack_require__(33)
	  , fastKey     = __webpack_require__(68).fastKey
	  , SIZE        = DESCRIPTORS ? '_s' : 'size';
	
	var getEntry = function(that, key){
	  // fast case
	  var index = fastKey(key), entry;
	  if(index !== 'F')return that._i[index];
	  // frozen object case
	  for(entry = that._f; entry; entry = entry.n){
	    if(entry.k == key)return entry;
	  }
	};
	
	module.exports = {
	  getConstructor: function(wrapper, NAME, IS_MAP, ADDER){
	    var C = wrapper(function(that, iterable){
	      anInstance(that, C, NAME, '_i');
	      that._i = create(null); // index
	      that._f = undefined;    // first entry
	      that._l = undefined;    // last entry
	      that[SIZE] = 0;         // size
	      if(iterable != undefined)forOf(iterable, IS_MAP, that[ADDER], that);
	    });
	    redefineAll(C.prototype, {
	      // 23.1.3.1 Map.prototype.clear()
	      // 23.2.3.2 Set.prototype.clear()
	      clear: function clear(){
	        for(var that = this, data = that._i, entry = that._f; entry; entry = entry.n){
	          entry.r = true;
	          if(entry.p)entry.p = entry.p.n = undefined;
	          delete data[entry.i];
	        }
	        that._f = that._l = undefined;
	        that[SIZE] = 0;
	      },
	      // 23.1.3.3 Map.prototype.delete(key)
	      // 23.2.3.4 Set.prototype.delete(value)
	      'delete': function(key){
	        var that  = this
	          , entry = getEntry(that, key);
	        if(entry){
	          var next = entry.n
	            , prev = entry.p;
	          delete that._i[entry.i];
	          entry.r = true;
	          if(prev)prev.n = next;
	          if(next)next.p = prev;
	          if(that._f == entry)that._f = next;
	          if(that._l == entry)that._l = prev;
	          that[SIZE]--;
	        } return !!entry;
	      },
	      // 23.2.3.6 Set.prototype.forEach(callbackfn, thisArg = undefined)
	      // 23.1.3.5 Map.prototype.forEach(callbackfn, thisArg = undefined)
	      forEach: function forEach(callbackfn /*, that = undefined */){
	        anInstance(this, C, 'forEach');
	        var f = ctx(callbackfn, arguments.length > 1 ? arguments[1] : undefined, 3)
	          , entry;
	        while(entry = entry ? entry.n : this._f){
	          f(entry.v, entry.k, this);
	          // revert to the last existing entry
	          while(entry && entry.r)entry = entry.p;
	        }
	      },
	      // 23.1.3.7 Map.prototype.has(key)
	      // 23.2.3.7 Set.prototype.has(value)
	      has: function has(key){
	        return !!getEntry(this, key);
	      }
	    });
	    if(DESCRIPTORS)dP(C.prototype, 'size', {
	      get: function(){
	        return defined(this[SIZE]);
	      }
	    });
	    return C;
	  },
	  def: function(that, key, value){
	    var entry = getEntry(that, key)
	      , prev, index;
	    // change existing entry
	    if(entry){
	      entry.v = value;
	    // create new entry
	    } else {
	      that._l = entry = {
	        i: index = fastKey(key, true), // <- index
	        k: key,                        // <- key
	        v: value,                      // <- value
	        p: prev = that._l,             // <- previous entry
	        n: undefined,                  // <- next entry
	        r: false                       // <- removed
	      };
	      if(!that._f)that._f = entry;
	      if(prev)prev.n = entry;
	      that[SIZE]++;
	      // add to index
	      if(index !== 'F')that._i[index] = entry;
	    } return that;
	  },
	  getEntry: getEntry,
	  setStrong: function(C, NAME, IS_MAP){
	    // add .keys, .values, .entries, [@@iterator]
	    // 23.1.3.4, 23.1.3.8, 23.1.3.11, 23.1.3.12, 23.2.3.5, 23.2.3.8, 23.2.3.10, 23.2.3.11
	    $iterDefine(C, NAME, function(iterated, kind){
	      this._t = iterated;  // target
	      this._k = kind;      // kind
	      this._l = undefined; // previous
	    }, function(){
	      var that  = this
	        , kind  = that._k
	        , entry = that._l;
	      // revert to the last existing entry
	      while(entry && entry.r)entry = entry.p;
	      // get next entry
	      if(!that._t || !(that._l = entry = entry ? entry.n : that._t._f)){
	        // or finish the iteration
	        that._t = undefined;
	        return step(1);
	      }
	      // return step by kind
	      if(kind == 'keys'  )return step(0, entry.k);
	      if(kind == 'values')return step(0, entry.v);
	      return step(0, [entry.k, entry.v]);
	    }, IS_MAP ? 'entries' : 'values' , !IS_MAP, true);
	
	    // add [@@species], 23.1.2.2, 23.2.2.2
	    setSpecies(NAME);
	  }
	};

/***/ },
/* 60 */
/***/ function(module, exports, __webpack_require__) {

	var hide = __webpack_require__(28);
	module.exports = function(target, src, safe){
	  for(var key in src){
	    if(safe && target[key])target[key] = src[key];
	    else hide(target, key, src[key]);
	  } return target;
	};

/***/ },
/* 61 */
/***/ function(module, exports) {

	module.exports = function(it, Constructor, name, forbiddenField){
	  if(!(it instanceof Constructor) || (forbiddenField !== undefined && forbiddenField in it)){
	    throw TypeError(name + ': incorrect invocation!');
	  } return it;
	};

/***/ },
/* 62 */
/***/ function(module, exports, __webpack_require__) {

	var ctx         = __webpack_require__(26)
	  , call        = __webpack_require__(63)
	  , isArrayIter = __webpack_require__(64)
	  , anObject    = __webpack_require__(30)
	  , toLength    = __webpack_require__(15)
	  , getIterFn   = __webpack_require__(65)
	  , BREAK       = {}
	  , RETURN      = {};
	var exports = module.exports = function(iterable, entries, fn, that, ITERATOR){
	  var iterFn = ITERATOR ? function(){ return iterable; } : getIterFn(iterable)
	    , f      = ctx(fn, that, entries ? 2 : 1)
	    , index  = 0
	    , length, step, iterator, result;
	  if(typeof iterFn != 'function')throw TypeError(iterable + ' is not iterable!');
	  // fast case for arrays with default iterator
	  if(isArrayIter(iterFn))for(length = toLength(iterable.length); length > index; index++){
	    result = entries ? f(anObject(step = iterable[index])[0], step[1]) : f(iterable[index]);
	    if(result === BREAK || result === RETURN)return result;
	  } else for(iterator = iterFn.call(iterable); !(step = iterator.next()).done; ){
	    result = call(iterator, f, step.value, entries);
	    if(result === BREAK || result === RETURN)return result;
	  }
	};
	exports.BREAK  = BREAK;
	exports.RETURN = RETURN;

/***/ },
/* 63 */
/***/ function(module, exports, __webpack_require__) {

	// call something on iterator step with safe closing on error
	var anObject = __webpack_require__(30);
	module.exports = function(iterator, fn, value, entries){
	  try {
	    return entries ? fn(anObject(value)[0], value[1]) : fn(value);
	  // 7.4.6 IteratorClose(iterator, completion)
	  } catch(e){
	    var ret = iterator['return'];
	    if(ret !== undefined)anObject(ret.call(iterator));
	    throw e;
	  }
	};

/***/ },
/* 64 */
/***/ function(module, exports, __webpack_require__) {

	// check on default Array iterator
	var Iterators  = __webpack_require__(46)
	  , ITERATOR   = __webpack_require__(52)('iterator')
	  , ArrayProto = Array.prototype;
	
	module.exports = function(it){
	  return it !== undefined && (Iterators.Array === it || ArrayProto[ITERATOR] === it);
	};

/***/ },
/* 65 */
/***/ function(module, exports, __webpack_require__) {

	var classof   = __webpack_require__(66)
	  , ITERATOR  = __webpack_require__(52)('iterator')
	  , Iterators = __webpack_require__(46);
	module.exports = __webpack_require__(25).getIteratorMethod = function(it){
	  if(it != undefined)return it[ITERATOR]
	    || it['@@iterator']
	    || Iterators[classof(it)];
	};

/***/ },
/* 66 */
/***/ function(module, exports, __webpack_require__) {

	// getting tag from 19.1.3.6 Object.prototype.toString()
	var cof = __webpack_require__(13)
	  , TAG = __webpack_require__(52)('toStringTag')
	  // ES3 wrong here
	  , ARG = cof(function(){ return arguments; }()) == 'Arguments';
	
	// fallback for IE11 Script Access Denied error
	var tryGet = function(it, key){
	  try {
	    return it[key];
	  } catch(e){ /* empty */ }
	};
	
	module.exports = function(it){
	  var O, T, B;
	  return it === undefined ? 'Undefined' : it === null ? 'Null'
	    // @@toStringTag case
	    : typeof (T = tryGet(O = Object(it), TAG)) == 'string' ? T
	    // builtinTag case
	    : ARG ? cof(O)
	    // ES3 arguments fallback
	    : (B = cof(O)) == 'Object' && typeof O.callee == 'function' ? 'Arguments' : B;
	};

/***/ },
/* 67 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';
	var global      = __webpack_require__(20)
	  , core        = __webpack_require__(25)
	  , dP          = __webpack_require__(29)
	  , DESCRIPTORS = __webpack_require__(33)
	  , SPECIES     = __webpack_require__(52)('species');
	
	module.exports = function(KEY){
	  var C = typeof core[KEY] == 'function' ? core[KEY] : global[KEY];
	  if(DESCRIPTORS && C && !C[SPECIES])dP.f(C, SPECIES, {
	    configurable: true,
	    get: function(){ return this; }
	  });
	};

/***/ },
/* 68 */
/***/ function(module, exports, __webpack_require__) {

	var META     = __webpack_require__(21)('meta')
	  , isObject = __webpack_require__(31)
	  , has      = __webpack_require__(10)
	  , setDesc  = __webpack_require__(29).f
	  , id       = 0;
	var isExtensible = Object.isExtensible || function(){
	  return true;
	};
	var FREEZE = !__webpack_require__(34)(function(){
	  return isExtensible(Object.preventExtensions({}));
	});
	var setMeta = function(it){
	  setDesc(it, META, {value: {
	    i: 'O' + ++id, // object ID
	    w: {}          // weak collections IDs
	  }});
	};
	var fastKey = function(it, create){
	  // return primitive with prefix
	  if(!isObject(it))return typeof it == 'symbol' ? it : (typeof it == 'string' ? 'S' : 'P') + it;
	  if(!has(it, META)){
	    // can't set metadata to uncaught frozen object
	    if(!isExtensible(it))return 'F';
	    // not necessary to add metadata
	    if(!create)return 'E';
	    // add missing metadata
	    setMeta(it);
	  // return object ID
	  } return it[META].i;
	};
	var getWeak = function(it, create){
	  if(!has(it, META)){
	    // can't set metadata to uncaught frozen object
	    if(!isExtensible(it))return true;
	    // not necessary to add metadata
	    if(!create)return false;
	    // add missing metadata
	    setMeta(it);
	  // return hash weak collections IDs
	  } return it[META].w;
	};
	// add metadata on freeze-family methods calling
	var onFreeze = function(it){
	  if(FREEZE && meta.NEED && isExtensible(it) && !has(it, META))setMeta(it);
	  return it;
	};
	var meta = module.exports = {
	  KEY:      META,
	  NEED:     false,
	  fastKey:  fastKey,
	  getWeak:  getWeak,
	  onFreeze: onFreeze
	};

/***/ },
/* 69 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';
	var global         = __webpack_require__(20)
	  , $export        = __webpack_require__(24)
	  , meta           = __webpack_require__(68)
	  , fails          = __webpack_require__(34)
	  , hide           = __webpack_require__(28)
	  , redefineAll    = __webpack_require__(60)
	  , forOf          = __webpack_require__(62)
	  , anInstance     = __webpack_require__(61)
	  , isObject       = __webpack_require__(31)
	  , setToStringTag = __webpack_require__(51)
	  , dP             = __webpack_require__(29).f
	  , each           = __webpack_require__(70)(0)
	  , DESCRIPTORS    = __webpack_require__(33);
	
	module.exports = function(NAME, wrapper, methods, common, IS_MAP, IS_WEAK){
	  var Base  = global[NAME]
	    , C     = Base
	    , ADDER = IS_MAP ? 'set' : 'add'
	    , proto = C && C.prototype
	    , O     = {};
	  if(!DESCRIPTORS || typeof C != 'function' || !(IS_WEAK || proto.forEach && !fails(function(){
	    new C().entries().next();
	  }))){
	    // create collection constructor
	    C = common.getConstructor(wrapper, NAME, IS_MAP, ADDER);
	    redefineAll(C.prototype, methods);
	    meta.NEED = true;
	  } else {
	    C = wrapper(function(target, iterable){
	      anInstance(target, C, NAME, '_c');
	      target._c = new Base;
	      if(iterable != undefined)forOf(iterable, IS_MAP, target[ADDER], target);
	    });
	    each('add,clear,delete,forEach,get,has,set,keys,values,entries,toJSON'.split(','),function(KEY){
	      var IS_ADDER = KEY == 'add' || KEY == 'set';
	      if(KEY in proto && !(IS_WEAK && KEY == 'clear'))hide(C.prototype, KEY, function(a, b){
	        anInstance(this, C, KEY);
	        if(!IS_ADDER && IS_WEAK && !isObject(a))return KEY == 'get' ? undefined : false;
	        var result = this._c[KEY](a === 0 ? 0 : a, b);
	        return IS_ADDER ? this : result;
	      });
	    });
	    if('size' in proto)dP(C.prototype, 'size', {
	      get: function(){
	        return this._c.size;
	      }
	    });
	  }
	
	  setToStringTag(C, NAME);
	
	  O[NAME] = C;
	  $export($export.G + $export.W + $export.F, O);
	
	  if(!IS_WEAK)common.setStrong(C, NAME, IS_MAP);
	
	  return C;
	};

/***/ },
/* 70 */
/***/ function(module, exports, __webpack_require__) {

	// 0 -> Array#forEach
	// 1 -> Array#map
	// 2 -> Array#filter
	// 3 -> Array#some
	// 4 -> Array#every
	// 5 -> Array#find
	// 6 -> Array#findIndex
	var ctx      = __webpack_require__(26)
	  , IObject  = __webpack_require__(12)
	  , toObject = __webpack_require__(6)
	  , toLength = __webpack_require__(15)
	  , asc      = __webpack_require__(71);
	module.exports = function(TYPE, $create){
	  var IS_MAP        = TYPE == 1
	    , IS_FILTER     = TYPE == 2
	    , IS_SOME       = TYPE == 3
	    , IS_EVERY      = TYPE == 4
	    , IS_FIND_INDEX = TYPE == 6
	    , NO_HOLES      = TYPE == 5 || IS_FIND_INDEX
	    , create        = $create || asc;
	  return function($this, callbackfn, that){
	    var O      = toObject($this)
	      , self   = IObject(O)
	      , f      = ctx(callbackfn, that, 3)
	      , length = toLength(self.length)
	      , index  = 0
	      , result = IS_MAP ? create($this, length) : IS_FILTER ? create($this, 0) : undefined
	      , val, res;
	    for(;length > index; index++)if(NO_HOLES || index in self){
	      val = self[index];
	      res = f(val, index, O);
	      if(TYPE){
	        if(IS_MAP)result[index] = res;            // map
	        else if(res)switch(TYPE){
	          case 3: return true;                    // some
	          case 5: return val;                     // find
	          case 6: return index;                   // findIndex
	          case 2: result.push(val);               // filter
	        } else if(IS_EVERY)return false;          // every
	      }
	    }
	    return IS_FIND_INDEX ? -1 : IS_SOME || IS_EVERY ? IS_EVERY : result;
	  };
	};

/***/ },
/* 71 */
/***/ function(module, exports, __webpack_require__) {

	// 9.4.2.3 ArraySpeciesCreate(originalArray, length)
	var speciesConstructor = __webpack_require__(72);
	
	module.exports = function(original, length){
	  return new (speciesConstructor(original))(length);
	};

/***/ },
/* 72 */
/***/ function(module, exports, __webpack_require__) {

	var isObject = __webpack_require__(31)
	  , isArray  = __webpack_require__(73)
	  , SPECIES  = __webpack_require__(52)('species');
	
	module.exports = function(original){
	  var C;
	  if(isArray(original)){
	    C = original.constructor;
	    // cross-realm fallback
	    if(typeof C == 'function' && (C === Array || isArray(C.prototype)))C = undefined;
	    if(isObject(C)){
	      C = C[SPECIES];
	      if(C === null)C = undefined;
	    }
	  } return C === undefined ? Array : C;
	};

/***/ },
/* 73 */
/***/ function(module, exports, __webpack_require__) {

	// 7.2.2 IsArray(argument)
	var cof = __webpack_require__(13);
	module.exports = Array.isArray || function isArray(arg){
	  return cof(arg) == 'Array';
	};

/***/ },
/* 74 */
/***/ function(module, exports, __webpack_require__) {

	// https://github.com/DavidBruant/Map-Set.prototype.toJSON
	var $export  = __webpack_require__(24);
	
	$export($export.P + $export.R, 'Map', {toJSON: __webpack_require__(75)('Map')});

/***/ },
/* 75 */
/***/ function(module, exports, __webpack_require__) {

	// https://github.com/DavidBruant/Map-Set.prototype.toJSON
	var classof = __webpack_require__(66)
	  , from    = __webpack_require__(76);
	module.exports = function(NAME){
	  return function toJSON(){
	    if(classof(this) != NAME)throw TypeError(NAME + "#toJSON isn't generic");
	    return from(this);
	  };
	};

/***/ },
/* 76 */
/***/ function(module, exports, __webpack_require__) {

	var forOf = __webpack_require__(62);
	
	module.exports = function(iter, ITERATOR){
	  var result = [];
	  forOf(iter, false, result.push, result, ITERATOR);
	  return result;
	};


/***/ },
/* 77 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';
	
	Object.defineProperty(exports, "__esModule", {
	  value: true
	});
	exports.highlight = highlight;
	exports.unHighlight = unHighlight;
	exports.getInstanceRect = getInstanceRect;
	
	var _util = __webpack_require__(78);
	
	var overlay = document.createElement('div');
	overlay.style.backgroundColor = 'rgba(104, 182, 255, 0.35)';
	overlay.style.position = 'fixed';
	overlay.style.zIndex = '99998';
	overlay.style.pointerEvents = 'none';
	
	/**
	 * Highlight an instance.
	 *
	 * @param {Vue} instance
	 */
	
	function highlight(instance) {
	  if (!instance) return;
	  var rect = getInstanceRect(instance);
	  if (rect) {
	    showOverlay(rect);
	  }
	}
	
	/**
	 * Remove highlight overlay.
	 */
	
	function unHighlight() {
	  if (overlay.parentNode) {
	    document.body.removeChild(overlay);
	  }
	}
	
	/**
	 * Get the client rect for an instance.
	 *
	 * @param {Vue} instance
	 * @return {Object}
	 */
	
	function getInstanceRect(instance) {
	  if (!(0, _util.inDoc)(instance.$el)) {
	    return;
	  }
	  if (instance._isFragment) {
	    return getFragmentRect(instance);
	  } else {
	    return instance.$el.getBoundingClientRect();
	  }
	}
	
	/**
	 * Highlight a fragment instance.
	 * Loop over its node range and determine its bounding box.
	 *
	 * @param {Vue} instance
	 * @return {Object}
	 */
	
	function getFragmentRect(_ref) {
	  var _fragmentStart = _ref._fragmentStart;
	  var _fragmentEnd = _ref._fragmentEnd;
	
	  var top = void 0,
	      bottom = void 0,
	      left = void 0,
	      right = void 0;
	  util().mapNodeRange(_fragmentStart, _fragmentEnd, function (node) {
	    var rect = void 0;
	    if (node.nodeType === 1 || node.getBoundingClientRect) {
	      rect = node.getBoundingClientRect();
	    } else if (node.nodeType === 3 && node.data.trim()) {
	      rect = getTextRect(node);
	    }
	    if (rect) {
	      if (!top || rect.top < top) {
	        top = rect.top;
	      }
	      if (!bottom || rect.bottom > bottom) {
	        bottom = rect.bottom;
	      }
	      if (!left || rect.left < left) {
	        left = rect.left;
	      }
	      if (!right || rect.right > right) {
	        right = rect.right;
	      }
	    }
	  });
	  return {
	    top: top,
	    left: left,
	    width: right - left,
	    height: bottom - top
	  };
	}
	
	/**
	 * Get the bounding rect for a text node using a Range.
	 *
	 * @param {Text} node
	 * @return {Rect}
	 */
	
	var range = document.createRange();
	function getTextRect(node) {
	  range.selectNode(node);
	  return range.getBoundingClientRect();
	}
	
	/**
	 * Display the overlay with given rect.
	 *
	 * @param {Rect}
	 */
	
	function showOverlay(_ref2) {
	  var _ref2$width = _ref2.width;
	  var width = _ref2$width === undefined ? 0 : _ref2$width;
	  var _ref2$height = _ref2.height;
	  var height = _ref2$height === undefined ? 0 : _ref2$height;
	  var _ref2$top = _ref2.top;
	  var top = _ref2$top === undefined ? 0 : _ref2$top;
	  var _ref2$left = _ref2.left;
	  var left = _ref2$left === undefined ? 0 : _ref2$left;
	
	  overlay.style.width = ~~width + 'px';
	  overlay.style.height = ~~height + 'px';
	  overlay.style.top = ~~top + 'px';
	  overlay.style.left = ~~left + 'px';
	  document.body.appendChild(overlay);
	}
	
	/**
	 * Get Vue's util
	 */
	
	function util() {
	  return window.__VUE_DEVTOOLS_GLOBAL_HOOK__.Vue.util;
	}

/***/ },
/* 78 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';
	
	Object.defineProperty(exports, "__esModule", {
	  value: true
	});
	exports.camelize = exports.classify = undefined;
	
	var _typeof2 = __webpack_require__(79);
	
	var _typeof3 = _interopRequireDefault(_typeof2);
	
	var _create = __webpack_require__(96);
	
	var _create2 = _interopRequireDefault(_create);
	
	exports.inDoc = inDoc;
	exports.stringify = stringify;
	
	var _circularJsonEs = __webpack_require__(99);
	
	var _circularJsonEs2 = _interopRequireDefault(_circularJsonEs);
	
	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }
	
	function cached(fn) {
	  var cache = (0, _create2.default)(null);
	  return function cachedFn(str) {
	    var hit = cache[str];
	    return hit || (cache[str] = fn(str));
	  };
	}
	
	var classifyRE = /(?:^|[-_\/])(\w)/g;
	var classify = exports.classify = cached(function (str) {
	  return str.replace(classifyRE, toUpper);
	});
	
	var camelizeRE = /-(\w)/g;
	var camelize = exports.camelize = cached(function (str) {
	  return str.replace(camelizeRE, toUpper);
	});
	
	function toUpper(_, c) {
	  return c ? c.toUpperCase() : '';
	}
	
	function inDoc(node) {
	  if (!node) return false;
	  var doc = node.ownerDocument.documentElement;
	  var parent = node.parentNode;
	  return doc === node || doc === parent || !!(parent && parent.nodeType === 1 && doc.contains(parent));
	}
	
	/**
	 * Stringify data using CircularJSON.
	 *
	 * @param {*} data
	 * @return {String}
	 */
	
	function stringify(data) {
	  return _circularJsonEs2.default.stringify(data, function (key, val) {
	    return sanitize(val);
	  });
	}
	
	/**
	 * Sanitize data to be posted to the other side.
	 * Since the message posted is sent with structured clone,
	 * we need to filter out any types that might cause an error.
	 *
	 * @param {*} data
	 * @return {*}
	 */
	
	function sanitize(data) {
	  if (!isPrimitive(data) && !Array.isArray(data) && !isPlainObject(data)) {
	    // handle types that will probably cause issues in
	    // the structured clone
	    return Object.prototype.toString.call(data);
	  } else {
	    return data;
	  }
	}
	
	function isPlainObject(obj) {
	  return Object.prototype.toString.call(obj) === '[object Object]';
	}
	
	function isPrimitive(data) {
	  if (data == null) {
	    return true;
	  }
	  var type = typeof data === 'undefined' ? 'undefined' : (0, _typeof3.default)(data);
	  return type === 'string' || type === 'number' || type === 'boolean' || data instanceof RegExp;
	}

/***/ },
/* 79 */
/***/ function(module, exports, __webpack_require__) {

	"use strict";
	
	exports.__esModule = true;
	
	var _iterator = __webpack_require__(80);
	
	var _iterator2 = _interopRequireDefault(_iterator);
	
	var _symbol = __webpack_require__(83);
	
	var _symbol2 = _interopRequireDefault(_symbol);
	
	var _typeof = typeof _symbol2.default === "function" && typeof _iterator2.default === "symbol" ? function (obj) { return typeof obj; } : function (obj) { return obj && typeof _symbol2.default === "function" && obj.constructor === _symbol2.default ? "symbol" : typeof obj; };
	
	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }
	
	exports.default = typeof _symbol2.default === "function" && _typeof(_iterator2.default) === "symbol" ? function (obj) {
	  return typeof obj === "undefined" ? "undefined" : _typeof(obj);
	} : function (obj) {
	  return obj && typeof _symbol2.default === "function" && obj.constructor === _symbol2.default ? "symbol" : typeof obj === "undefined" ? "undefined" : _typeof(obj);
	};

/***/ },
/* 80 */
/***/ function(module, exports, __webpack_require__) {

	module.exports = { "default": __webpack_require__(81), __esModule: true };

/***/ },
/* 81 */
/***/ function(module, exports, __webpack_require__) {

	__webpack_require__(41);
	__webpack_require__(54);
	module.exports = __webpack_require__(82).f('iterator');

/***/ },
/* 82 */
/***/ function(module, exports, __webpack_require__) {

	exports.f = __webpack_require__(52);

/***/ },
/* 83 */
/***/ function(module, exports, __webpack_require__) {

	module.exports = { "default": __webpack_require__(84), __esModule: true };

/***/ },
/* 84 */
/***/ function(module, exports, __webpack_require__) {

	__webpack_require__(85);
	__webpack_require__(40);
	__webpack_require__(94);
	__webpack_require__(95);
	module.exports = __webpack_require__(25).Symbol;

/***/ },
/* 85 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';
	// ECMAScript 6 symbols shim
	var global         = __webpack_require__(20)
	  , has            = __webpack_require__(10)
	  , DESCRIPTORS    = __webpack_require__(33)
	  , $export        = __webpack_require__(24)
	  , redefine       = __webpack_require__(45)
	  , META           = __webpack_require__(68).KEY
	  , $fails         = __webpack_require__(34)
	  , shared         = __webpack_require__(19)
	  , setToStringTag = __webpack_require__(51)
	  , uid            = __webpack_require__(21)
	  , wks            = __webpack_require__(52)
	  , wksExt         = __webpack_require__(82)
	  , wksDefine      = __webpack_require__(86)
	  , keyOf          = __webpack_require__(87)
	  , enumKeys       = __webpack_require__(88)
	  , isArray        = __webpack_require__(73)
	  , anObject       = __webpack_require__(30)
	  , toIObject      = __webpack_require__(11)
	  , toPrimitive    = __webpack_require__(36)
	  , createDesc     = __webpack_require__(37)
	  , _create        = __webpack_require__(48)
	  , gOPNExt        = __webpack_require__(91)
	  , $GOPD          = __webpack_require__(93)
	  , $DP            = __webpack_require__(29)
	  , $keys          = __webpack_require__(8)
	  , gOPD           = $GOPD.f
	  , dP             = $DP.f
	  , gOPN           = gOPNExt.f
	  , $Symbol        = global.Symbol
	  , $JSON          = global.JSON
	  , _stringify     = $JSON && $JSON.stringify
	  , PROTOTYPE      = 'prototype'
	  , HIDDEN         = wks('_hidden')
	  , TO_PRIMITIVE   = wks('toPrimitive')
	  , isEnum         = {}.propertyIsEnumerable
	  , SymbolRegistry = shared('symbol-registry')
	  , AllSymbols     = shared('symbols')
	  , OPSymbols      = shared('op-symbols')
	  , ObjectProto    = Object[PROTOTYPE]
	  , USE_NATIVE     = typeof $Symbol == 'function'
	  , QObject        = global.QObject;
	// Don't use setters in Qt Script, https://github.com/zloirock/core-js/issues/173
	var setter = !QObject || !QObject[PROTOTYPE] || !QObject[PROTOTYPE].findChild;
	
	// fallback for old Android, https://code.google.com/p/v8/issues/detail?id=687
	var setSymbolDesc = DESCRIPTORS && $fails(function(){
	  return _create(dP({}, 'a', {
	    get: function(){ return dP(this, 'a', {value: 7}).a; }
	  })).a != 7;
	}) ? function(it, key, D){
	  var protoDesc = gOPD(ObjectProto, key);
	  if(protoDesc)delete ObjectProto[key];
	  dP(it, key, D);
	  if(protoDesc && it !== ObjectProto)dP(ObjectProto, key, protoDesc);
	} : dP;
	
	var wrap = function(tag){
	  var sym = AllSymbols[tag] = _create($Symbol[PROTOTYPE]);
	  sym._k = tag;
	  return sym;
	};
	
	var isSymbol = USE_NATIVE && typeof $Symbol.iterator == 'symbol' ? function(it){
	  return typeof it == 'symbol';
	} : function(it){
	  return it instanceof $Symbol;
	};
	
	var $defineProperty = function defineProperty(it, key, D){
	  if(it === ObjectProto)$defineProperty(OPSymbols, key, D);
	  anObject(it);
	  key = toPrimitive(key, true);
	  anObject(D);
	  if(has(AllSymbols, key)){
	    if(!D.enumerable){
	      if(!has(it, HIDDEN))dP(it, HIDDEN, createDesc(1, {}));
	      it[HIDDEN][key] = true;
	    } else {
	      if(has(it, HIDDEN) && it[HIDDEN][key])it[HIDDEN][key] = false;
	      D = _create(D, {enumerable: createDesc(0, false)});
	    } return setSymbolDesc(it, key, D);
	  } return dP(it, key, D);
	};
	var $defineProperties = function defineProperties(it, P){
	  anObject(it);
	  var keys = enumKeys(P = toIObject(P))
	    , i    = 0
	    , l = keys.length
	    , key;
	  while(l > i)$defineProperty(it, key = keys[i++], P[key]);
	  return it;
	};
	var $create = function create(it, P){
	  return P === undefined ? _create(it) : $defineProperties(_create(it), P);
	};
	var $propertyIsEnumerable = function propertyIsEnumerable(key){
	  var E = isEnum.call(this, key = toPrimitive(key, true));
	  if(this === ObjectProto && has(AllSymbols, key) && !has(OPSymbols, key))return false;
	  return E || !has(this, key) || !has(AllSymbols, key) || has(this, HIDDEN) && this[HIDDEN][key] ? E : true;
	};
	var $getOwnPropertyDescriptor = function getOwnPropertyDescriptor(it, key){
	  it  = toIObject(it);
	  key = toPrimitive(key, true);
	  if(it === ObjectProto && has(AllSymbols, key) && !has(OPSymbols, key))return;
	  var D = gOPD(it, key);
	  if(D && has(AllSymbols, key) && !(has(it, HIDDEN) && it[HIDDEN][key]))D.enumerable = true;
	  return D;
	};
	var $getOwnPropertyNames = function getOwnPropertyNames(it){
	  var names  = gOPN(toIObject(it))
	    , result = []
	    , i      = 0
	    , key;
	  while(names.length > i){
	    if(!has(AllSymbols, key = names[i++]) && key != HIDDEN && key != META)result.push(key);
	  } return result;
	};
	var $getOwnPropertySymbols = function getOwnPropertySymbols(it){
	  var IS_OP  = it === ObjectProto
	    , names  = gOPN(IS_OP ? OPSymbols : toIObject(it))
	    , result = []
	    , i      = 0
	    , key;
	  while(names.length > i){
	    if(has(AllSymbols, key = names[i++]) && (IS_OP ? has(ObjectProto, key) : true))result.push(AllSymbols[key]);
	  } return result;
	};
	
	// 19.4.1.1 Symbol([description])
	if(!USE_NATIVE){
	  $Symbol = function Symbol(){
	    if(this instanceof $Symbol)throw TypeError('Symbol is not a constructor!');
	    var tag = uid(arguments.length > 0 ? arguments[0] : undefined);
	    var $set = function(value){
	      if(this === ObjectProto)$set.call(OPSymbols, value);
	      if(has(this, HIDDEN) && has(this[HIDDEN], tag))this[HIDDEN][tag] = false;
	      setSymbolDesc(this, tag, createDesc(1, value));
	    };
	    if(DESCRIPTORS && setter)setSymbolDesc(ObjectProto, tag, {configurable: true, set: $set});
	    return wrap(tag);
	  };
	  redefine($Symbol[PROTOTYPE], 'toString', function toString(){
	    return this._k;
	  });
	
	  $GOPD.f = $getOwnPropertyDescriptor;
	  $DP.f   = $defineProperty;
	  __webpack_require__(92).f = gOPNExt.f = $getOwnPropertyNames;
	  __webpack_require__(90).f  = $propertyIsEnumerable;
	  __webpack_require__(89).f = $getOwnPropertySymbols;
	
	  if(DESCRIPTORS && !__webpack_require__(44)){
	    redefine(ObjectProto, 'propertyIsEnumerable', $propertyIsEnumerable, true);
	  }
	
	  wksExt.f = function(name){
	    return wrap(wks(name));
	  }
	}
	
	$export($export.G + $export.W + $export.F * !USE_NATIVE, {Symbol: $Symbol});
	
	for(var symbols = (
	  // 19.4.2.2, 19.4.2.3, 19.4.2.4, 19.4.2.6, 19.4.2.8, 19.4.2.9, 19.4.2.10, 19.4.2.11, 19.4.2.12, 19.4.2.13, 19.4.2.14
	  'hasInstance,isConcatSpreadable,iterator,match,replace,search,species,split,toPrimitive,toStringTag,unscopables'
	).split(','), i = 0; symbols.length > i; )wks(symbols[i++]);
	
	for(var symbols = $keys(wks.store), i = 0; symbols.length > i; )wksDefine(symbols[i++]);
	
	$export($export.S + $export.F * !USE_NATIVE, 'Symbol', {
	  // 19.4.2.1 Symbol.for(key)
	  'for': function(key){
	    return has(SymbolRegistry, key += '')
	      ? SymbolRegistry[key]
	      : SymbolRegistry[key] = $Symbol(key);
	  },
	  // 19.4.2.5 Symbol.keyFor(sym)
	  keyFor: function keyFor(key){
	    if(isSymbol(key))return keyOf(SymbolRegistry, key);
	    throw TypeError(key + ' is not a symbol!');
	  },
	  useSetter: function(){ setter = true; },
	  useSimple: function(){ setter = false; }
	});
	
	$export($export.S + $export.F * !USE_NATIVE, 'Object', {
	  // 19.1.2.2 Object.create(O [, Properties])
	  create: $create,
	  // 19.1.2.4 Object.defineProperty(O, P, Attributes)
	  defineProperty: $defineProperty,
	  // 19.1.2.3 Object.defineProperties(O, Properties)
	  defineProperties: $defineProperties,
	  // 19.1.2.6 Object.getOwnPropertyDescriptor(O, P)
	  getOwnPropertyDescriptor: $getOwnPropertyDescriptor,
	  // 19.1.2.7 Object.getOwnPropertyNames(O)
	  getOwnPropertyNames: $getOwnPropertyNames,
	  // 19.1.2.8 Object.getOwnPropertySymbols(O)
	  getOwnPropertySymbols: $getOwnPropertySymbols
	});
	
	// 24.3.2 JSON.stringify(value [, replacer [, space]])
	$JSON && $export($export.S + $export.F * (!USE_NATIVE || $fails(function(){
	  var S = $Symbol();
	  // MS Edge converts symbol values to JSON as {}
	  // WebKit converts symbol values to JSON as null
	  // V8 throws on boxed symbols
	  return _stringify([S]) != '[null]' || _stringify({a: S}) != '{}' || _stringify(Object(S)) != '{}';
	})), 'JSON', {
	  stringify: function stringify(it){
	    if(it === undefined || isSymbol(it))return; // IE8 returns string on undefined
	    var args = [it]
	      , i    = 1
	      , replacer, $replacer;
	    while(arguments.length > i)args.push(arguments[i++]);
	    replacer = args[1];
	    if(typeof replacer == 'function')$replacer = replacer;
	    if($replacer || !isArray(replacer))replacer = function(key, value){
	      if($replacer)value = $replacer.call(this, key, value);
	      if(!isSymbol(value))return value;
	    };
	    args[1] = replacer;
	    return _stringify.apply($JSON, args);
	  }
	});
	
	// 19.4.3.4 Symbol.prototype[@@toPrimitive](hint)
	$Symbol[PROTOTYPE][TO_PRIMITIVE] || __webpack_require__(28)($Symbol[PROTOTYPE], TO_PRIMITIVE, $Symbol[PROTOTYPE].valueOf);
	// 19.4.3.5 Symbol.prototype[@@toStringTag]
	setToStringTag($Symbol, 'Symbol');
	// 20.2.1.9 Math[@@toStringTag]
	setToStringTag(Math, 'Math', true);
	// 24.3.3 JSON[@@toStringTag]
	setToStringTag(global.JSON, 'JSON', true);

/***/ },
/* 86 */
/***/ function(module, exports, __webpack_require__) {

	var global         = __webpack_require__(20)
	  , core           = __webpack_require__(25)
	  , LIBRARY        = __webpack_require__(44)
	  , wksExt         = __webpack_require__(82)
	  , defineProperty = __webpack_require__(29).f;
	module.exports = function(name){
	  var $Symbol = core.Symbol || (core.Symbol = LIBRARY ? {} : global.Symbol || {});
	  if(name.charAt(0) != '_' && !(name in $Symbol))defineProperty($Symbol, name, {value: wksExt.f(name)});
	};

/***/ },
/* 87 */
/***/ function(module, exports, __webpack_require__) {

	var getKeys   = __webpack_require__(8)
	  , toIObject = __webpack_require__(11);
	module.exports = function(object, el){
	  var O      = toIObject(object)
	    , keys   = getKeys(O)
	    , length = keys.length
	    , index  = 0
	    , key;
	  while(length > index)if(O[key = keys[index++]] === el)return key;
	};

/***/ },
/* 88 */
/***/ function(module, exports, __webpack_require__) {

	// all enumerable object keys, includes symbols
	var getKeys = __webpack_require__(8)
	  , gOPS    = __webpack_require__(89)
	  , pIE     = __webpack_require__(90);
	module.exports = function(it){
	  var result     = getKeys(it)
	    , getSymbols = gOPS.f;
	  if(getSymbols){
	    var symbols = getSymbols(it)
	      , isEnum  = pIE.f
	      , i       = 0
	      , key;
	    while(symbols.length > i)if(isEnum.call(it, key = symbols[i++]))result.push(key);
	  } return result;
	};

/***/ },
/* 89 */
/***/ function(module, exports) {

	exports.f = Object.getOwnPropertySymbols;

/***/ },
/* 90 */
/***/ function(module, exports) {

	exports.f = {}.propertyIsEnumerable;

/***/ },
/* 91 */
/***/ function(module, exports, __webpack_require__) {

	// fallback for IE11 buggy Object.getOwnPropertyNames with iframe and window
	var toIObject = __webpack_require__(11)
	  , gOPN      = __webpack_require__(92).f
	  , toString  = {}.toString;
	
	var windowNames = typeof window == 'object' && window && Object.getOwnPropertyNames
	  ? Object.getOwnPropertyNames(window) : [];
	
	var getWindowNames = function(it){
	  try {
	    return gOPN(it);
	  } catch(e){
	    return windowNames.slice();
	  }
	};
	
	module.exports.f = function getOwnPropertyNames(it){
	  return windowNames && toString.call(it) == '[object Window]' ? getWindowNames(it) : gOPN(toIObject(it));
	};


/***/ },
/* 92 */
/***/ function(module, exports, __webpack_require__) {

	// 19.1.2.7 / 15.2.3.4 Object.getOwnPropertyNames(O)
	var $keys      = __webpack_require__(9)
	  , hiddenKeys = __webpack_require__(22).concat('length', 'prototype');
	
	exports.f = Object.getOwnPropertyNames || function getOwnPropertyNames(O){
	  return $keys(O, hiddenKeys);
	};

/***/ },
/* 93 */
/***/ function(module, exports, __webpack_require__) {

	var pIE            = __webpack_require__(90)
	  , createDesc     = __webpack_require__(37)
	  , toIObject      = __webpack_require__(11)
	  , toPrimitive    = __webpack_require__(36)
	  , has            = __webpack_require__(10)
	  , IE8_DOM_DEFINE = __webpack_require__(32)
	  , gOPD           = Object.getOwnPropertyDescriptor;
	
	exports.f = __webpack_require__(33) ? gOPD : function getOwnPropertyDescriptor(O, P){
	  O = toIObject(O);
	  P = toPrimitive(P, true);
	  if(IE8_DOM_DEFINE)try {
	    return gOPD(O, P);
	  } catch(e){ /* empty */ }
	  if(has(O, P))return createDesc(!pIE.f.call(O, P), O[P]);
	};

/***/ },
/* 94 */
/***/ function(module, exports, __webpack_require__) {

	__webpack_require__(86)('asyncIterator');

/***/ },
/* 95 */
/***/ function(module, exports, __webpack_require__) {

	__webpack_require__(86)('observable');

/***/ },
/* 96 */
/***/ function(module, exports, __webpack_require__) {

	module.exports = { "default": __webpack_require__(97), __esModule: true };

/***/ },
/* 97 */
/***/ function(module, exports, __webpack_require__) {

	__webpack_require__(98);
	var $Object = __webpack_require__(25).Object;
	module.exports = function create(P, D){
	  return $Object.create(P, D);
	};

/***/ },
/* 98 */
/***/ function(module, exports, __webpack_require__) {

	var $export = __webpack_require__(24)
	// 19.1.2.2 / 15.2.3.5 Object.create(O [, Properties])
	$export($export.S, 'Object', {create: __webpack_require__(48)});

/***/ },
/* 99 */
/***/ function(module, exports) {

	function encode (data, replacer, list, seen) {
	  var stored, key, value, i, l
	  var seenIndex = seen.get(data)
	  if (seenIndex != null) {
	    return seenIndex
	  }
	  var index = list.length
	  if (isPlainObject(data)) {
	    stored = {}
	    seen.set(data, index)
	    list.push(stored)
	    var keys = Object.keys(data)
	    for (i = 0, l = keys.length; i < l; i++) {
	      key = keys[i]
	      value = data[key]
	      if (replacer) {
	        value = replacer.call(data, key, value)
	      }
	      stored[key] = encode(value, replacer, list, seen)
	    }
	  } else if (Array.isArray(data)) {
	    stored = []
	    seen.set(data, index)
	    list.push(stored)
	    for (i = 0, l = data.length; i < l; i++) {
	      value = data[i]
	      if (replacer) {
	       value = replacer.call(data, i, value)
	      }
	      stored[i] = encode(value, replacer, list, seen)
	    }
	    seen.set(data, list.length)
	  } else {
	    index = list.length
	    list.push(data)
	  }
	  return index
	}
	
	function decode (list, reviver) {
	  var i = list.length
	  var j, k, data, key, value
	  while (i--) {
	    var data = list[i]
	    if (isPlainObject(data)) {
	      var keys = Object.keys(data)
	      for (j = 0, k = keys.length; j < k; j++) {
	        key = keys[j]
	        value = list[data[key]]
	        if (reviver) value = reviver.call(data, key, value)
	        data[key] = value
	      }
	    } else if (Array.isArray(data)) {
	      for (j = 0, k = data.length; j < k; j++) {
	        value = list[data[j]]
	        if (reviver) value = reviver.call(data, j, value)
	        data[j] = value
	      }
	    }
	  }
	}
	
	function isPlainObject (obj) {
	  return Object.prototype.toString.call(obj) === '[object Object]'
	}
	
	exports.stringify = function stringify (data, replacer, space) {
	  try {
	    return arguments.length === 1
	      ? JSON.stringify(data)
	      : JSON.stringify(data, replacer, space)
	  } catch (e) {
	    return exports.stringifyStrict(data, replacer, space)
	  }
	}
	
	exports.parse = function parse (data, reviver) {
	  var hasCircular = /^\s/.test(data)
	  if (!hasCircular) {
	    return arguments.length === 1
	      ? JSON.parse(data)
	      : JSON.parse(data, reviver)
	  } else {
	    var list = JSON.parse(data)
	    decode(list, reviver)
	    return list[0]
	  }
	}
	
	exports.stringifyStrict = function (data, replacer, space) {
	  var list = []
	  encode(data, replacer, list, new Map())
	  return space
	    ? ' ' + JSON.stringify(list, null, space)
	    : ' ' + JSON.stringify(list)
	}


/***/ },
/* 100 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';
	
	Object.defineProperty(exports, "__esModule", {
	  value: true
	});
	exports.initVuexBackend = initVuexBackend;
	
	var _circularJsonEs = __webpack_require__(99);
	
	var _circularJsonEs2 = _interopRequireDefault(_circularJsonEs);
	
	var _util = __webpack_require__(78);
	
	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }
	
	function initVuexBackend(hook, bridge) {
	  var store = hook.store;
	  bridge.send('vuex:init', (0, _util.stringify)(store.state));
	
	  // deal with multiple backend injections
	  hook.off('vuex:mutation');
	
	  // application -> devtool
	  hook.on('vuex:mutation', function (mutation, state) {
	    bridge.send('vuex:mutation', {
	      mutation: {
	        type: mutation.type,
	        payload: (0, _util.stringify)(mutation.payload)
	      },
	      timestamp: Date.now(),
	      state: (0, _util.stringify)(state)
	    });
	  });
	
	  // devtool -> application
	  bridge.on('vuex:travel-to-state', function (state) {
	    hook.emit('vuex:travel-to-state', _circularJsonEs2.default.parse(state));
	  });
	}

/***/ },
/* 101 */
/***/ function(module, exports, __webpack_require__) {

	'use strict';
	
	Object.defineProperty(exports, "__esModule", {
	  value: true
	});
	
	var _getPrototypeOf = __webpack_require__(102);
	
	var _getPrototypeOf2 = _interopRequireDefault(_getPrototypeOf);
	
	var _classCallCheck2 = __webpack_require__(105);
	
	var _classCallCheck3 = _interopRequireDefault(_classCallCheck2);
	
	var _createClass2 = __webpack_require__(106);
	
	var _createClass3 = _interopRequireDefault(_createClass2);
	
	var _possibleConstructorReturn2 = __webpack_require__(110);
	
	var _possibleConstructorReturn3 = _interopRequireDefault(_possibleConstructorReturn2);
	
	var _inherits2 = __webpack_require__(111);
	
	var _inherits3 = _interopRequireDefault(_inherits2);
	
	var _events = __webpack_require__(116);
	
	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }
	
	var Bridge = function (_EventEmitter) {
	  (0, _inherits3.default)(Bridge, _EventEmitter);
	
	  function Bridge(wall) {
	    (0, _classCallCheck3.default)(this, Bridge);
	
	    var _this = (0, _possibleConstructorReturn3.default)(this, (0, _getPrototypeOf2.default)(Bridge).call(this));
	
	    _this.setMaxListeners(Infinity);
	    _this.wall = wall;
	    wall.listen(function (message) {
	      if (typeof message === 'string') {
	        _this.emit(message);
	      } else {
	        _this.emit(message.event, message.payload);
	      }
	    });
	    return _this;
	  }
	
	  /**
	   * Send an event.
	   *
	   * @param {String} event
	   * @param {*} payload
	   */
	
	  (0, _createClass3.default)(Bridge, [{
	    key: 'send',
	    value: function send(event, payload) {
	      this.wall.send({
	        event: event,
	        payload: payload
	      });
	    }
	
	    /**
	     * Log a message to the devtools background page.
	     *
	     * @param {String} message
	     */
	
	  }, {
	    key: 'log',
	    value: function log(message) {
	      this.send('log', message);
	    }
	  }]);
	  return Bridge;
	}(_events.EventEmitter);
	
	exports.default = Bridge;

/***/ },
/* 102 */
/***/ function(module, exports, __webpack_require__) {

	module.exports = { "default": __webpack_require__(103), __esModule: true };

/***/ },
/* 103 */
/***/ function(module, exports, __webpack_require__) {

	__webpack_require__(104);
	module.exports = __webpack_require__(25).Object.getPrototypeOf;

/***/ },
/* 104 */
/***/ function(module, exports, __webpack_require__) {

	// 19.1.2.9 Object.getPrototypeOf(O)
	var toObject        = __webpack_require__(6)
	  , $getPrototypeOf = __webpack_require__(53);
	
	__webpack_require__(23)('getPrototypeOf', function(){
	  return function getPrototypeOf(it){
	    return $getPrototypeOf(toObject(it));
	  };
	});

/***/ },
/* 105 */
/***/ function(module, exports) {

	"use strict";
	
	exports.__esModule = true;
	
	exports.default = function (instance, Constructor) {
	  if (!(instance instanceof Constructor)) {
	    throw new TypeError("Cannot call a class as a function");
	  }
	};

/***/ },
/* 106 */
/***/ function(module, exports, __webpack_require__) {

	"use strict";
	
	exports.__esModule = true;
	
	var _defineProperty = __webpack_require__(107);
	
	var _defineProperty2 = _interopRequireDefault(_defineProperty);
	
	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }
	
	exports.default = function () {
	  function defineProperties(target, props) {
	    for (var i = 0; i < props.length; i++) {
	      var descriptor = props[i];
	      descriptor.enumerable = descriptor.enumerable || false;
	      descriptor.configurable = true;
	      if ("value" in descriptor) descriptor.writable = true;
	      (0, _defineProperty2.default)(target, descriptor.key, descriptor);
	    }
	  }
	
	  return function (Constructor, protoProps, staticProps) {
	    if (protoProps) defineProperties(Constructor.prototype, protoProps);
	    if (staticProps) defineProperties(Constructor, staticProps);
	    return Constructor;
	  };
	}();

/***/ },
/* 107 */
/***/ function(module, exports, __webpack_require__) {

	module.exports = { "default": __webpack_require__(108), __esModule: true };

/***/ },
/* 108 */
/***/ function(module, exports, __webpack_require__) {

	__webpack_require__(109);
	var $Object = __webpack_require__(25).Object;
	module.exports = function defineProperty(it, key, desc){
	  return $Object.defineProperty(it, key, desc);
	};

/***/ },
/* 109 */
/***/ function(module, exports, __webpack_require__) {

	var $export = __webpack_require__(24);
	// 19.1.2.4 / 15.2.3.6 Object.defineProperty(O, P, Attributes)
	$export($export.S + $export.F * !__webpack_require__(33), 'Object', {defineProperty: __webpack_require__(29).f});

/***/ },
/* 110 */
/***/ function(module, exports, __webpack_require__) {

	"use strict";
	
	exports.__esModule = true;
	
	var _typeof2 = __webpack_require__(79);
	
	var _typeof3 = _interopRequireDefault(_typeof2);
	
	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }
	
	exports.default = function (self, call) {
	  if (!self) {
	    throw new ReferenceError("this hasn't been initialised - super() hasn't been called");
	  }
	
	  return call && ((typeof call === "undefined" ? "undefined" : (0, _typeof3.default)(call)) === "object" || typeof call === "function") ? call : self;
	};

/***/ },
/* 111 */
/***/ function(module, exports, __webpack_require__) {

	"use strict";
	
	exports.__esModule = true;
	
	var _setPrototypeOf = __webpack_require__(112);
	
	var _setPrototypeOf2 = _interopRequireDefault(_setPrototypeOf);
	
	var _create = __webpack_require__(96);
	
	var _create2 = _interopRequireDefault(_create);
	
	var _typeof2 = __webpack_require__(79);
	
	var _typeof3 = _interopRequireDefault(_typeof2);
	
	function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }
	
	exports.default = function (subClass, superClass) {
	  if (typeof superClass !== "function" && superClass !== null) {
	    throw new TypeError("Super expression must either be null or a function, not " + (typeof superClass === "undefined" ? "undefined" : (0, _typeof3.default)(superClass)));
	  }
	
	  subClass.prototype = (0, _create2.default)(superClass && superClass.prototype, {
	    constructor: {
	      value: subClass,
	      enumerable: false,
	      writable: true,
	      configurable: true
	    }
	  });
	  if (superClass) _setPrototypeOf2.default ? (0, _setPrototypeOf2.default)(subClass, superClass) : subClass.__proto__ = superClass;
	};

/***/ },
/* 112 */
/***/ function(module, exports, __webpack_require__) {

	module.exports = { "default": __webpack_require__(113), __esModule: true };

/***/ },
/* 113 */
/***/ function(module, exports, __webpack_require__) {

	__webpack_require__(114);
	module.exports = __webpack_require__(25).Object.setPrototypeOf;

/***/ },
/* 114 */
/***/ function(module, exports, __webpack_require__) {

	// 19.1.3.19 Object.setPrototypeOf(O, proto)
	var $export = __webpack_require__(24);
	$export($export.S, 'Object', {setPrototypeOf: __webpack_require__(115).set});

/***/ },
/* 115 */
/***/ function(module, exports, __webpack_require__) {

	// Works with __proto__ only. Old v8 can't work with null proto objects.
	/* eslint-disable no-proto */
	var isObject = __webpack_require__(31)
	  , anObject = __webpack_require__(30);
	var check = function(O, proto){
	  anObject(O);
	  if(!isObject(proto) && proto !== null)throw TypeError(proto + ": can't set as prototype!");
	};
	module.exports = {
	  set: Object.setPrototypeOf || ('__proto__' in {} ? // eslint-disable-line
	    function(test, buggy, set){
	      try {
	        set = __webpack_require__(26)(Function.call, __webpack_require__(93).f(Object.prototype, '__proto__').set, 2);
	        set(test, []);
	        buggy = !(test instanceof Array);
	      } catch(e){ buggy = true; }
	      return function setPrototypeOf(O, proto){
	        check(O, proto);
	        if(buggy)O.__proto__ = proto;
	        else set(O, proto);
	        return O;
	      };
	    }({}, false) : undefined),
	  check: check
	};

/***/ },
/* 116 */
/***/ function(module, exports) {

	// Copyright Joyent, Inc. and other Node contributors.
	//
	// Permission is hereby granted, free of charge, to any person obtaining a
	// copy of this software and associated documentation files (the
	// "Software"), to deal in the Software without restriction, including
	// without limitation the rights to use, copy, modify, merge, publish,
	// distribute, sublicense, and/or sell copies of the Software, and to permit
	// persons to whom the Software is furnished to do so, subject to the
	// following conditions:
	//
	// The above copyright notice and this permission notice shall be included
	// in all copies or substantial portions of the Software.
	//
	// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
	// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
	// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
	// NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
	// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
	// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
	// USE OR OTHER DEALINGS IN THE SOFTWARE.
	
	function EventEmitter() {
	  this._events = this._events || {};
	  this._maxListeners = this._maxListeners || undefined;
	}
	module.exports = EventEmitter;
	
	// Backwards-compat with node 0.10.x
	EventEmitter.EventEmitter = EventEmitter;
	
	EventEmitter.prototype._events = undefined;
	EventEmitter.prototype._maxListeners = undefined;
	
	// By default EventEmitters will print a warning if more than 10 listeners are
	// added to it. This is a useful default which helps finding memory leaks.
	EventEmitter.defaultMaxListeners = 10;
	
	// Obviously not all Emitters should be limited to 10. This function allows
	// that to be increased. Set to zero for unlimited.
	EventEmitter.prototype.setMaxListeners = function(n) {
	  if (!isNumber(n) || n < 0 || isNaN(n))
	    throw TypeError('n must be a positive number');
	  this._maxListeners = n;
	  return this;
	};
	
	EventEmitter.prototype.emit = function(type) {
	  var er, handler, len, args, i, listeners;
	
	  if (!this._events)
	    this._events = {};
	
	  // If there is no 'error' event listener then throw.
	  if (type === 'error') {
	    if (!this._events.error ||
	        (isObject(this._events.error) && !this._events.error.length)) {
	      er = arguments[1];
	      if (er instanceof Error) {
	        throw er; // Unhandled 'error' event
	      } else {
	        // At least give some kind of context to the user
	        var err = new Error('Uncaught, unspecified "error" event. (' + er + ')');
	        err.context = er;
	        throw err;
	      }
	    }
	  }
	
	  handler = this._events[type];
	
	  if (isUndefined(handler))
	    return false;
	
	  if (isFunction(handler)) {
	    switch (arguments.length) {
	      // fast cases
	      case 1:
	        handler.call(this);
	        break;
	      case 2:
	        handler.call(this, arguments[1]);
	        break;
	      case 3:
	        handler.call(this, arguments[1], arguments[2]);
	        break;
	      // slower
	      default:
	        args = Array.prototype.slice.call(arguments, 1);
	        handler.apply(this, args);
	    }
	  } else if (isObject(handler)) {
	    args = Array.prototype.slice.call(arguments, 1);
	    listeners = handler.slice();
	    len = listeners.length;
	    for (i = 0; i < len; i++)
	      listeners[i].apply(this, args);
	  }
	
	  return true;
	};
	
	EventEmitter.prototype.addListener = function(type, listener) {
	  var m;
	
	  if (!isFunction(listener))
	    throw TypeError('listener must be a function');
	
	  if (!this._events)
	    this._events = {};
	
	  // To avoid recursion in the case that type === "newListener"! Before
	  // adding it to the listeners, first emit "newListener".
	  if (this._events.newListener)
	    this.emit('newListener', type,
	              isFunction(listener.listener) ?
	              listener.listener : listener);
	
	  if (!this._events[type])
	    // Optimize the case of one listener. Don't need the extra array object.
	    this._events[type] = listener;
	  else if (isObject(this._events[type]))
	    // If we've already got an array, just append.
	    this._events[type].push(listener);
	  else
	    // Adding the second element, need to change to array.
	    this._events[type] = [this._events[type], listener];
	
	  // Check for listener leak
	  if (isObject(this._events[type]) && !this._events[type].warned) {
	    if (!isUndefined(this._maxListeners)) {
	      m = this._maxListeners;
	    } else {
	      m = EventEmitter.defaultMaxListeners;
	    }
	
	    if (m && m > 0 && this._events[type].length > m) {
	      this._events[type].warned = true;
	      console.error('(node) warning: possible EventEmitter memory ' +
	                    'leak detected. %d listeners added. ' +
	                    'Use emitter.setMaxListeners() to increase limit.',
	                    this._events[type].length);
	      if (typeof console.trace === 'function') {
	        // not supported in IE 10
	        console.trace();
	      }
	    }
	  }
	
	  return this;
	};
	
	EventEmitter.prototype.on = EventEmitter.prototype.addListener;
	
	EventEmitter.prototype.once = function(type, listener) {
	  if (!isFunction(listener))
	    throw TypeError('listener must be a function');
	
	  var fired = false;
	
	  function g() {
	    this.removeListener(type, g);
	
	    if (!fired) {
	      fired = true;
	      listener.apply(this, arguments);
	    }
	  }
	
	  g.listener = listener;
	  this.on(type, g);
	
	  return this;
	};
	
	// emits a 'removeListener' event iff the listener was removed
	EventEmitter.prototype.removeListener = function(type, listener) {
	  var list, position, length, i;
	
	  if (!isFunction(listener))
	    throw TypeError('listener must be a function');
	
	  if (!this._events || !this._events[type])
	    return this;
	
	  list = this._events[type];
	  length = list.length;
	  position = -1;
	
	  if (list === listener ||
	      (isFunction(list.listener) && list.listener === listener)) {
	    delete this._events[type];
	    if (this._events.removeListener)
	      this.emit('removeListener', type, listener);
	
	  } else if (isObject(list)) {
	    for (i = length; i-- > 0;) {
	      if (list[i] === listener ||
	          (list[i].listener && list[i].listener === listener)) {
	        position = i;
	        break;
	      }
	    }
	
	    if (position < 0)
	      return this;
	
	    if (list.length === 1) {
	      list.length = 0;
	      delete this._events[type];
	    } else {
	      list.splice(position, 1);
	    }
	
	    if (this._events.removeListener)
	      this.emit('removeListener', type, listener);
	  }
	
	  return this;
	};
	
	EventEmitter.prototype.removeAllListeners = function(type) {
	  var key, listeners;
	
	  if (!this._events)
	    return this;
	
	  // not listening for removeListener, no need to emit
	  if (!this._events.removeListener) {
	    if (arguments.length === 0)
	      this._events = {};
	    else if (this._events[type])
	      delete this._events[type];
	    return this;
	  }
	
	  // emit removeListener for all listeners on all events
	  if (arguments.length === 0) {
	    for (key in this._events) {
	      if (key === 'removeListener') continue;
	      this.removeAllListeners(key);
	    }
	    this.removeAllListeners('removeListener');
	    this._events = {};
	    return this;
	  }
	
	  listeners = this._events[type];
	
	  if (isFunction(listeners)) {
	    this.removeListener(type, listeners);
	  } else if (listeners) {
	    // LIFO order
	    while (listeners.length)
	      this.removeListener(type, listeners[listeners.length - 1]);
	  }
	  delete this._events[type];
	
	  return this;
	};
	
	EventEmitter.prototype.listeners = function(type) {
	  var ret;
	  if (!this._events || !this._events[type])
	    ret = [];
	  else if (isFunction(this._events[type]))
	    ret = [this._events[type]];
	  else
	    ret = this._events[type].slice();
	  return ret;
	};
	
	EventEmitter.prototype.listenerCount = function(type) {
	  if (this._events) {
	    var evlistener = this._events[type];
	
	    if (isFunction(evlistener))
	      return 1;
	    else if (evlistener)
	      return evlistener.length;
	  }
	  return 0;
	};
	
	EventEmitter.listenerCount = function(emitter, type) {
	  return emitter.listenerCount(type);
	};
	
	function isFunction(arg) {
	  return typeof arg === 'function';
	}
	
	function isNumber(arg) {
	  return typeof arg === 'number';
	}
	
	function isObject(arg) {
	  return typeof arg === 'object' && arg !== null;
	}
	
	function isUndefined(arg) {
	  return arg === void 0;
	}


/***/ }
/******/ ]);
//# sourceMappingURL=backend.js.map