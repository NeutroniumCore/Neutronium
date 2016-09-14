(function(){
	"use strict";

	if (!!window.vueDebug) return;

	const vueNodeName ="_debug_Vue_Node_", vueRootNodeName ="_debug_Vue_Root_Node_";
	let open =true, frame, component;
		
	window.vueDebug = function vueDebug(){
		var component = document.getElementById(vueRootNodeName);
		component.style.visibility = open ? "hidden" : "visible";
		open = !open;	
	};

	function addcss(css){
		var s = document.createElement('style');
		s.setAttribute('type', 'text/css');
		s.appendChild(document.createTextNode(css));
		var head = document.getElementsByTagName('head')[0];
		head.appendChild(s);
	}

	const css = " .expander { position: absolute; align-content: stretch; overflow: hidden; min-height: 15px; display: flex; flex-flow: row wrap; font-weight: bold; z-index: 98;} .test{ background: red; height: 100%; } .expander > * { flex: 1 100%; } .footer { background-color: #efefef; height: 10px; flex: 3 10px; text-align: center; order: 4; align-self: flex-end; cursor: ns-resize; } .fullexpander{ flex: 0 10px; height: 10px; order: 5; background: #efefef; align-self: flex-end; justify-content: flex-end; cursor: nwse-resize; } .header { background-color: #efefef; height: 10px; } .main { order: 2; flex: 3 auto; flex-grow: 1; } .right { flex: 0 10px; order: 3; width: 10px; background-color: #efefef; cursor: ew-resize; } .left { flex: 0 10px; order: 1; width: 10px; background-color: #efefef; cursor: ew-resize; } ";
	addcss(css);

	function convert(value){
		return Number(value);
	}

	var exp = Vue.component('expander', {
		template: "<div class=\"expander\" :style=\"{ top: py + 'px', left : px + 'px'}\"> <header class=\"header\" @mousedown=\"dragMoveStart\" @dblclick=\"reduce\"></header> <div class=\"left\" @mousedown=\"dragExpandXMoveStart\"></div> <div class=\"main\" :style=\"{height: height, width: width }\">  <slot class=\"center\"></slot> </div> <div class=\"right\" @mousedown=\"dragExpandXStart\"></div> <footer class=\"footer\" @mousedown=\"dragExpandYStart\"></footer>  <footer class=\"fullexpander\" @mousedown=\"dragExpandXYStart\"></footer>  </div>",
		data: function data() {
			    return {
			      	dragging: false,
			      	reduced: false,
			      	px: convert(this.right),
			      	py: convert(this.top),
			      	height : this.inicialheight,
			      	width: this.inicialwidth
			    };
			},
		props:{
			top:{
	      		default: 0
			},
			right:{
	      		default: 0
			},
	    	inicialheight: {
	      		type :String,
	      		default: 'auto'
	    	},
	    	inicialwidth: {
	      		type :String,
	      		default: 'auto'
	    		}
	  		},			
		methods: {
		    reduce: function reduce() {
		      var current = this.height;
		      if (this.reduced) this.height = this.oldHeigth;else this.height = "0px";
		      this.oldHeigth = current;
		      this.reduced = !this.reduced;
		    },
		    dragExpandXYStart: function dragExpandXYStart(e) {
		      this.dragStart(e, 'expandXYMove');
		    },
		    dragExpandXMoveStart: function dragExpandXMoveStart(e) {
		      this.dragStart(e, 'expandXMove');
		    },
		    dragExpandXStart: function dragExpandXStart(e) {
		      this.dragStart(e, 'expandX');
		    },
		    dragExpandYStart: function dragExpandYStart(e) {
		      this.dragStart(e, 'expandY');
		    },
		    dragMoveStart: function dragMoveStart(e) {
		      this.dragStart(e, 'move');
		    },
		    dragStart: function dragStart(e, drag) {
		      this.dragging = true;
		      this.draggingStyle = drag;
		      this.startX = e.pageX;
		      this.startY = e.pageY;
		      this.originalX = this.px;
		      this.originalY = this.py;
		      this.originalHeight = this.$el.offsetHeight - 15;
		      this.originalWidth = this.$el.offsetWidth - 10;
		      window.addEventListener("mousemove", this.dragMove);
		      window.addEventListener("mouseup", this.dragEnd);
		      window.addEventListener("mouseleave", this.dragEnd);
		    },
		    dragMove: function dragMove(e) {
		      if (this.dragging) {
		      	if (e.buttons===0){
		      		this.dragEnd();
		      		return;
		      	}
		        var dx = e.pageX - this.startX;
		        var dy = e.pageY - this.startY;
		        switch (this.draggingStyle) {
		          case "move":
		            this.px = this.originalX + dx;
		            this.py = this.originalY + dy;
		            break;
		
		          case "expandY":
		            this.height = this.originalHeight + dy + 'px';
		            break;
		
		          case "expandXMove":
		            this.px = this.originalX + dx;
		            var value = this.originalWidth - dx + 'px';
		            this.width = value;
		            break;
		
		          case "expandXYMove":
		            this.height = this.originalHeight + dy + 'px';
		
		          case "expandX":
		            this.width = this.originalWidth + dx + 'px';
		            break;
		        }
		      }
		    },
		    dragEnd: function dragEnd() {
		      	window.removeEventListener("mousemove", this.dragMove);
		      	window.removeEventListener("mouseup", this.dragEnd);
		      	window.removeEventListener("mouseleave", this.dragEnd);
		      	this.dragging = false;
		    }
		}
	});

	var windowW = window.innerWidth,
	    windowH = window.innerHeight;

	component = document.createElement('expander');
	component.id = vueRootNodeName;
	component.setAttribute("inicialheight", (windowH/2) +"px");
	component.setAttribute("inicialwidth", (windowW/3)+ "px");
	component.setAttribute("top",10);
	component.setAttribute("right",(2 *windowW/3 -60) );		
	frame = document.createElement('iframe');
	frame.id = vueNodeName;
	
	frame.style.height = "100%";
	frame.style.width = "100%";

	component.appendChild(frame);
	var v = new exp({ el: component });
	v.$appendTo(document.body);

	function dispacthToWindow(frame, e, evtName){
		var boundingClientRect = frame.getBoundingClientRect();

		var evt = new MouseEvent(evtName, {
		    view: window,
		    bubbles: true,
		    cancelable: false,
		    detail: e.detail,
		    screenX: e.screenX,
		    screenY: e.screenY, 
		    clientX: e.clientX + boundingClientRect.left, 
		    clientY: e.clientY + boundingClientRect.top, 
            ctrlKey: e.ctrlKey, 
		    altKey: e.altKey,
		    shiftKey: e.shiftKey, 
		    metaKey: e.metaKey,
		    button: e.button, 
		    buttons: e.buttons,
		    pageY : e.pageY,
		    pageX : e.pageX
		});
		frame.dispatchEvent(evt);
	}

	setTimeout( () =>{

		var newComponent= document.getElementById(vueRootNodeName);
		newComponent.style.zIndex = '99999';

		var newFrame = document.getElementById(vueNodeName);
		newFrame.sandbox = "allow-scripts allow-same-origin allow-modals";		
		newFrame.srcdoc ='<!doctype html><html style="height:100%;"><body style="height:100%;"><div id="app"></div> <script src="build/devtools.js"></script></body></html>';

		newFrame.onload  = () => {
			var contentWindow =  newFrame.contentWindow;
			["mousemove", "mouseup", "mouseleave"].forEach((evt) => contentWindow["on"+evt] = (ev) => dispacthToWindow(newFrame, ev, evt));
		};
		
	});

	vueDebug();
})();
