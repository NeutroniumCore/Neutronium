( function (){

if (ko.dodebug !== undefined)
  return;

var toogle=true,head,koViewStyle,koViewContainer,first=true;

ko.dodebug = function()
{
  

    (function (ko) {

    "use strict";

    if (first){
        document.addEventListener( 'click', generalClick, false );
        first=false;
    }

  

  var trackmove=false,oldp = null;

  var show=true;

  var TYPE_STRING = 1,
  TYPE_NUMBER = 2,
  TYPE_OBJECT = 3,
  TYPE_ARRAY  = 4,
  TYPE_BOOL   = 5,
  TYPE_NULL   = 6,

  baseSpan = document.createElement('span'),
  baseDiv  = document.createElement('div'),
  baseText = document.createTextNode(""),

  templatesObj = {
    t_kvov: getSpanClass('kvov'),
    t_exp: getSpanClass('e'),
    t_key: getSpanClass('k'),
    t_string: getSpanClass('s'),
    t_number: getSpanClass('n'),
    
    t_null: getSpanBoth('null', 'nl'),
    t_true: getSpanBoth('true','bl'),
    t_false: getSpanBoth('false','bl'),
    
    t_oBrace: getSpanBoth('{','b'),
    t_cBrace: getSpanBoth('}','b'),
    t_oBracket: getSpanBoth('[','b'),
    t_cBracket: getSpanBoth(']','b'),
    
    t_ellipsis: getSpanClass('ell'),
    t_blockInner: getSpanClass('blockInner'),
    
    t_colonAndSpace: document.createTextNode(':\u00A0'),
    t_commaText: document.createTextNode(','),
    t_dblqText: document.createTextNode('"')
  },

  mac = navigator.platform.indexOf('Mac') !== -1,
  modKey,
  context = ko.contextFor(document.body),
  lastKvovIdGiven = 0;

  if (mac) {
    modKey = function (ev) {
      return ev.metaKey;
    };
  } else {
    modKey = function (ev) {
      return ev.ctrlKey;
    };
  }

  toogle=!toogle;

  if (toogle===true){
      closeView(true);
      return;
  }

  if (context && !document.getElementById("ko-view-container")) {

     var showText          = "Show",
      hideText          = "Hide",
      fragment          = document.createDocumentFragment(),
      styleSheetText = '@charset "UTF-8";#ko-view-container{-webkit-font-smoothing:antialiased;-webkit-user-select: text;user-select: text;position:fixed;z-index:99999;left:10px;top:20px}#ko-view-container>.ko-view-menu{width:82px;height:25px;line-height:25px;padding:0 10px;font-family:Arial;position:absolute;left:0px;top:0px;font-size:13px;background:#fff;border:1px solid #eee;border-radius:3px;box-shadow:7px 7px 20px rgba(0,0,0,0.3);-webkit-touch-callout:none;-webkit-user-select:none;-khtml-user-select:none;-moz-user-select:none;-ms-user-select:none;user-select:none}#ko-view-container>.ko-view-menu:hover .ko-view-move,#ko-view-container>.ko-view-menu:hover .ko-view-close{opacity:1}#ko-view-container>.ko-view-menu .ko-view-move{cursor:move;opacity:0;-webkit-transition:opacity .15s ease;-moz-transition:opacity .15s ease;transition:opacity .15s ease;position:relative;left:0;top:0;padding-left:12px;-webkit-touch-callout:none;-webkit-user-select:none;-khtml-user-select:none;-moz-user-select:none;-ms-user-select:none;user-select:none}#ko-view-container>.ko-view-menu .ko-view-move:hover:before{border-color:#5fa3ff}#ko-view-container>.ko-view-menu .ko-view-move:before{content:"";position:absolute;top:2px;left:0;width:13px;height:2px;-webkit-transition:border .15s ease;-moz-transition:border .15s ease;transition:border .15s ease;border-top:6px double #aaa;border-bottom:2px solid #aaa}#ko-view-container>.ko-view-content{height:450px;width:550px;overflow:auto;font-family:"Menlo", "Consolas", "Lucida Console", Monaco, monospace;line-height:14px;margin:0;background:#fff;border:1px solid #eee;box-shadow:7px 7px 20px rgba(0,0,0,0.3);padding:4px;color:#666;background:#fff;resize:both;position:absolute;left:0px;top:40px}#ko-view-container .ko-view-toggle{display:inline-block;width:40px;text-align:center;padding:0 10px;font-weight:bold;color:#666;background:#fff;cursor:pointer;-webkit-touch-callout:none;-webkit-user-select:none;-khtml-user-select:none;-moz-user-select:none;-ms-user-select:none;user-select:none}#ko-view-container .ko-view-close{opacity:0;-webkit-transition:opacity .15s ease, color .15s ease;-moz-transition:opacity .15s ease, color .15s ease;transition:opacity .15s ease, color .15s ease;color:#aaa;font-weight:bold;width:8px;cursor:pointer;position:relative;top:-1px;-webkit-touch-callout:none;-webkit-user-select:none;-khtml-user-select:none;-moz-user-select:none;-ms-user-select:none;user-select:none}#ko-view-container .ko-view-close:hover{color:#a23e25}#ko-view-container #formattedJson{padding-left:20px;padding-top:6px}#ko-view-container .kvov{display:block;padding-left:15px;margin-left:-15px;position:relative}#ko-view-container .collapsed{white-space:nowrap}#ko-view-container .collapsed>.blockInner{display:none}#ko-view-container .collapsed>.ell:after{content:"…";font-weight:bold}#ko-view-container .collapsed>.ell{margin:0 4px;color:#888}#ko-view-container .collapsed .kvov{display:inline}#ko-view-container .e{width:20px;height:18px;display:block;position:absolute;left:-2px;top:1px;z-index:5;background-image:url("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAgAAAAICAYAAADED76LAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAD1JREFUeNpiYGBgOADE%2F3Hgw0DM4IRHgSsDFOzFInmMAQnY49ONzZRjDFiADT7dMLALiE8y4AGW6LoBAgwAuIkf%2F%2FB7O9sAAAAASUVORK5CYII%3D");background-repeat:no-repeat;background-position:center center;display:block;opacity:0.15}#ko-view-container .collapsed>.e{-webkit-transform:rotate(-90deg);-moz-transform:rotate(-90deg);-ms-transform:rotate(-90deg);-o-transform:rotate(-90deg);transform:rotate(-90deg);width:18px;height:20px;left:0px;top:0px}#ko-view-container .e:hover{opacity:0.35}#ko-view-container .e:active{opacity:0.5}#ko-view-container .collapsed .kvov .e{display:none}#ko-view-container .blockInner{display:block;padding-left:24px;border-left:1px dotted #bbb;margin-left:2px}#ko-view-container #formattedJson,#ko-view-container #jsonpOpener,#ko-view-container #jsonpCloser{color:#333;font:13px/18px monospace}#ko-view-container #formattedJson{color:#444}#ko-view-container .b{font-weight:bold}#ko-view-container .s{color:#718c00;word-wrap:break-word}#ko-view-container a:link,#ko-view-container a:visited{text-decoration:none;color:inherit}#ko-view-container a:hover,#ko-view-container a:active{text-decoration:underline;color:#050}#ko-view-container .bl,#ko-view-container .nl,#ko-view-container .n{font-weight:bold;color:#f5871f}#ko-view-container .k{color:#718c00}#ko-view-container span{white-space:pre-wrap}',
      styleTextNode     = baseText.cloneNode(false),      
     
      koViewMenu        = getDivClass('ko-view-menu'),
      koViewContent     = getDivClass('ko-view-content'),
      koViewMove        = getSpanClass('ko-view-move'),
      koViewToggle      = getSpanClass('ko-view-toggle'),
      koViewToggleText  = baseText.cloneNode(false),
      koViewClose       = getSpanBoth('x', 'ko-view-close');

    koViewContainer   = getDivId('ko-view-container');
     head              = document.getElementsByTagName('head')[0];
    koViewStyle       = document.createElement("style");
    koViewStyle.type      = "text/css";
    koViewStyle.id        = "ko-debug-style";
    styleTextNode     .nodeValue = styleSheetText;
    koViewStyle.appendChild(styleTextNode     );
    head.appendChild(koViewStyle);

    koViewToggleText.nodeValue = hideText;
    koViewToggle.appendChild(koViewToggleText);

    koViewMove.setAttribute('draggable', 'true');
    koViewClose.setAttribute('data-bind', 'click: closeView');
    koViewContent.setAttribute("data-bind", "html: format(ko.toJS(data))");


    koViewMenu.appendChild(koViewMove);
    koViewMenu.appendChild(koViewToggle);
    koViewMenu.appendChild(koViewClose);

    koViewContainer.appendChild(koViewMenu);
    koViewContainer.appendChild(koViewContent);

    fragment.appendChild(koViewContainer);
    document.body.appendChild(fragment);
    
    ko.applyBindings({ data: context.$root, closeView: closeView, format: jsonObjToHtml }, koViewContainer);

    

    koViewToggle.onclick = function() {
      koViewContent.style.display = koViewContent.style.display === "none" ? "" : "none";
      koViewToggleText.nodeValue  = koViewContent.style.display === "none" ? showText : hideText;
    };


    document.addEventListener('mousedown',start,false);
    document.addEventListener('mouseup',stop,false);
  }

  function start(e)
  {
    if (e.toElement===koViewMove){
      //moveHandler = evt.attach('mousemove', document, move, true);
      document.addEventListener('mousemove',move,true);
      trackmove = true;
      oldp = { x: e.x, y: e.y };
    }
  }

  function move(e)
  {
      if (trackmove) {
       
       var style = window.getComputedStyle(koViewContainer, null),
       left = parseInt(style.getPropertyValue("left"),10),
       top = parseInt(style.getPropertyValue("top"),10),
       newp = { x: e.x, y: e.y };

       koViewContainer.style.left = left + newp.x - oldp.x + 'px'
       koViewContainer.style.top = top + newp.y - oldp.y + 'px';
       oldp = newp;
      if (e.which!==1) stop();
     }
     else stop();
  }


  function stop()
  {
    if (trackmove){
      document.removeEventListener('mousemove',move);
      //evt.detach('mousemove', document, moveHandler);
      //moveHandler=null;
      trackmove=false;
    }  
  }

  function closeView(ontogle) {

      if (ontogle != true)
          toogle = !toogle;

    // Stop listening
    //document.removeEventListener( 'click', generalClick );

    // Remove the stylesheet and element
    ko.removeNode(koViewContainer);
    head.removeChild(koViewStyle);
    show=false;
  }

  function getDivId(id) {
    var div = baseDiv.cloneNode(false);
    div.id = id;
    return div;
  }

  function getDivClass(className) {
    var div = baseDiv.cloneNode(false);
    div.className = className;
    return div;
  }

  function getSpanBoth(text,className) {
    var span     = baseSpan.cloneNode(false),
        textNode = baseText.cloneNode(false);
    span.className = className;
    textNode.nodeValue = text;
    span.appendChild(textNode);
    return span;
  }
  
  function getSpanClass(className) {
    var span = baseSpan.cloneNode(false);
    span.className = className;
    return span;
  }

  function exists(object) {
    return (object !== null && typeof object !== 'undefined');
  }

  function getKvovDOM(value, keyName, parentKeyName, found,circular) {
    var type,
    kvov,
    nonZeroSize,
    templates = templatesObj, // bring into scope for tiny speed boost
    objKey,
    keySpan,
    valueElement;
    found = found || [];

    circular = (circular === undefined) ? false : circular;

    // Establish value type
      if (typeof value === 'string') {
        type = TYPE_STRING;
      }
      else if (typeof value === 'number') {
        type = TYPE_NUMBER;
      }
      else if (value === false || value === true ) {
        type = TYPE_BOOL;
      }
      else if (value === null) {
        type = TYPE_NULL;
      }
      else if (value instanceof Array) {
        type = TYPE_ARRAY;
      }
      else {
        type = TYPE_OBJECT;
      }

      var index, ncircular =(type===TYPE_OBJECT) && (found.indexOf(value) !== -1);
     
      if ((type === TYPE_OBJECT) && (!ncircular))
          index = found.push(value);
   
    // Root node for this kvov
      kvov = templates.t_kvov.cloneNode(false);
    
    // Add an 'expander' first (if this is object/array with non-zero size)
      if (type === TYPE_OBJECT || type === TYPE_ARRAY) {
        nonZeroSize = false;
        for (objKey in value) {
          if (value.hasOwnProperty(objKey)) {
            nonZeroSize = true;
            break; // no need to keep counting; only need one
          }
        }
        if (nonZeroSize) {
          kvov.appendChild(  templates.t_exp.cloneNode(false) );
        }
      }
      
    // If there's a key, add that before the value
      if (keyName !== false) { // NB: "" is a legal keyname in JSON
        // This kvov must be an object property
          kvov.classList.add('objProp');
        // Create a span for the key name
          keySpan = templates.t_key.cloneNode(false);
          keySpan.textContent = JSON.stringify(keyName).slice(1,-1); // remove quotes
          // Add it to kvov, with quote marks
          kvov.appendChild(templates.t_dblqText.cloneNode(false));
          kvov.appendChild(keySpan);
          kvov.appendChild(templates.t_dblqText.cloneNode(false));
        // Also add ":&nbsp;" (colon and non-breaking space)
          kvov.appendChild( templates.t_colonAndSpace.cloneNode(false) );
      }
      else {
        // This is an array element instead
          kvov.classList.add('arrElem');
      }
    
    // Generate DOM for this value
      var blockInner, childKvov;
      switch (type) {
        case TYPE_STRING:
          // If string is a URL, get a link, otherwise get a span
            var innerStringEl = baseSpan.cloneNode(false),
                escapedString = JSON.stringify(value);

            escapedString = escapedString.substring(1, escapedString.length-1); // remove quotes
            if (value[0] === 'h' && value.substring(0, 4) === 'http') { // crude but fast - some false positives, but rare, and UX doesn't suffer terribly from them.
              var innerStringA = document.createElement('A');
              innerStringA.href = value;
              innerStringA.innerText = escapedString;
              innerStringEl.appendChild(innerStringA);
            }
            else {
              innerStringEl.innerText = escapedString;
            }
            valueElement = templates.t_string.cloneNode(false);
            if(!circular)
                valueElement.appendChild(templates.t_dblqText.cloneNode(false));
            valueElement.appendChild(innerStringEl);
            if (!circular)
                valueElement.appendChild(templates.t_dblqText.cloneNode(false));
            kvov.appendChild(valueElement);
          break;
        
        case TYPE_NUMBER:
          // Simply add a number element (span.n)
            valueElement = templates.t_number.cloneNode(false);
            valueElement.innerText = value;
            kvov.appendChild(valueElement);
          break;
        
        case TYPE_OBJECT:
          // Add opening brace
            kvov.appendChild( templates.t_oBrace.cloneNode(true) );
          // If any properties, add a blockInner containing k/v pair(s)
            if (nonZeroSize) {
              // Add ellipsis (empty, but will be made to do something when kvov is collapsed)
                kvov.appendChild( templates.t_ellipsis.cloneNode(false) );
              // Create blockInner, which indents (don't attach yet)
                blockInner = templates.t_blockInner.cloneNode(false);
              // For each key/value pair, add as a kvov to blockInner
                var count = 0, k, comma;
                for (k in value) {
                  if (value.hasOwnProperty(k)) {
                      count++;
                      var valuek = value[k],stopped=false;
                      if ((ncircular === true) && (valuek != null) && (typeof (valuek) === "object")){
                          valuek = "{...}";
                          stopped = true;
                      }
                      childKvov = getKvovDOM(valuek, k, keyName + "0", found, stopped);
                    // Add comma
                      comma = templates.t_commaText.cloneNode();
                      childKvov.appendChild(comma);
                    blockInner.appendChild( childKvov );
                  }
                }
              // Now remove the last comma
                childKvov.removeChild(comma);
              // Add blockInner
                kvov.appendChild( blockInner );
            }
          
          // Add closing brace
            kvov.appendChild( templates.t_cBrace.cloneNode(true) );
          break;

        case TYPE_ARRAY:
          // Add opening bracket
            kvov.appendChild( templates.t_oBracket.cloneNode(true) );
          // If non-zero length array, add blockInner containing inner vals
            if (nonZeroSize) {
              // Add ellipsis
                kvov.appendChild( templates.t_ellipsis.cloneNode(false) );
              // Create blockInner (which indents) (don't attach yet)
                blockInner = templates.t_blockInner.cloneNode(false);
              // For each key/value pair, add the markup
                for (var i=0, length=value.length, lastIndex=length-1; i<length; i++) {
                    // Make a new kvov, with no key

                    var valuei = value[i], stopped = false;;
                    if ((ncircular === true) && (valuei != null) && (typeof (valuei) === "object")) {
                        valuei = "{...}";
                        stopped = true;
                    }
                        

                    childKvov = getKvovDOM(valuei, false, keyName + "0", found, stopped);

                  // Add comma if not last one
                    if (i < lastIndex) {
                      childKvov.appendChild( templates.t_commaText.cloneNode() );
                    }
                  // Append the child kvov
                    blockInner.appendChild( childKvov );
                }
              // Add blockInner
                kvov.appendChild( blockInner );
            }
          // Add closing bracket
            kvov.appendChild( templates.t_cBracket.cloneNode(true) );
          break;

        case TYPE_BOOL:
          if (value) {
            kvov.appendChild( templates.t_true.cloneNode(true) );
          }
          else {
            kvov.appendChild( templates.t_false.cloneNode(true) );
          }
          break;

        case TYPE_NULL:
          kvov.appendChild( templates.t_null.cloneNode(true) );
          break;
      }

      if ((type === TYPE_OBJECT) && (!ncircular))
        found.splice(index - 1, 1);

    return kvov;
  }

  function collapse(elements) {
    // console.log('elements', elements);

    var el, i, blockInner, count;

    for (i = elements.length - 1; i >= 0; i--) {
      el = elements[i];
      el.classList.add('collapsed');

      // (CSS hides the contents and shows an ellipsis.)

      // Add a count of the number of child properties/items (if not already done for this item)
        if (!el.id) {
          el.id = 'kvov' + (++lastKvovIdGiven);

          // Find the blockInner
            blockInner = el.firstElementChild;
            while ( blockInner && !blockInner.classList.contains('blockInner') ) {
              blockInner = blockInner.nextElementSibling;
            }
            if (!blockInner) {
              continue;
            }

          // See how many children in the blockInner
            count = blockInner.children.length;

          // Generate comment text eg "4 items"
            var comment = count + (count===1 ? ' item' : ' items');
          // Add CSS that targets it
            koViewStyle.insertAdjacentHTML(
              'beforeend',
              '\n#kvov'+lastKvovIdGiven+'.collapsed:after{color: #aaa; content:" // '+comment+'"}'
            );
        }
    }
  }

  function expand(elements) {
    for (var i = elements.length - 1; i >= 0; i--) {
      elements[i].classList.remove('collapsed');
    }
  }

  function generalClick(ev) {
    // console.log('click', ev);
    if (ev.which === 1) {
      var elem = ev.target;
      
      if (elem.className === 'e') {
        // It's a click on an expander.

        ev.preventDefault();

        var parent = elem.parentNode;
        
        // Expand or collapse
          if (parent.classList.contains('collapsed')) {
            // EXPAND
              if (modKey(ev)) {
                expand(parent.parentNode.children);
              }
              else {
                expand([parent]);
              }
          }
          else {
            // COLLAPSE
              if (modKey(ev)) {
                collapse(parent.parentNode.children);
              }
              else {
                collapse([parent]);
              }
          }

        return;
      }
    }
  }

  function jsonObjToHtml(obj) {

    // Format object (using recursive kvov builder)
    var rootKvov = getKvovDOM(obj, false);

    // The whole DOM is now built.

    // Set class on root node to identify it
    rootKvov.classList.add('rootKvov');
      
    // Make div#formattedJson and append the root kvov
    var divFormattedJson = document.createElement('div');
    divFormattedJson.id = 'formattedJson';
    divFormattedJson.appendChild( rootKvov );
    
    // Get the HTML
    var returnHTML = divFormattedJson.outerHTML;

    // Return the HTML
    return returnHTML;
  }

})( window.ko );
};
})();


