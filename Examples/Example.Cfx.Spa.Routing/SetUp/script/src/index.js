//This overlay is inspired from https://github.com/webpack/webpack-dev-server/blob/d725fd9f3b0c2ab92e6489c993f80602c0e47848/client/overlay.js
// Which is inspired (and mostly copied) from Create React App (https://github.com/facebookincubator/create-react-app)
// They, in turn, got inspired by webpack-hot-middleware (https://github.com/glenjamin/webpack-hot-middleware).

var ansiHTML = require("ansi-html");
var Entities = require("html-entities").AllHtmlEntities;
var entities = new Entities();

var colors = {
  reset: ["transparent", "transparent"],
  black: "181818",
  red: "E36049",
  green: "B3CB74",
  yellow: "FFD080",
  blue: "7CAFC2",
  magenta: "7FACCA",
  cyan: "C3C2EF",
  lightgrey: "EBE7E3",
  darkgrey: "6D7891"
};
ansiHTML.setColors(colors);

function createOverlayIframe(onIframeLoad) {
  var iframe = document.createElement("iframe");
  iframe.id = "neutronium-loading-overlay";
  iframe.src = "about:blank";
  iframe.style.position = "fixed";
  iframe.style.left = 0;
  iframe.style.top = 0;
  iframe.style.right = 0;
  iframe.style.bottom = 0;
  iframe.style.width = "100vw";
  iframe.style.height = "100vh";
  iframe.style.border = "none";
  iframe.style.zIndex = 9999999999;
  iframe.onload = onIframeLoad;
  return iframe;
}

function addOverlayDivTo(iframe) {
  var div = iframe.contentDocument.createElement("div");
  div.id = "neutronium-loading-overlay-div";
  div.style.position = "fixed";
  div.style.boxSizing = "border-box";
  div.style.left = 0;
  div.style.top = 0;
  div.style.right = 0;
  div.style.bottom = 0;
  div.style.width = "100vw";
  div.style.height = "100vh";
  div.style.backgroundColor = "black";
  div.style.color = "#E8E8E8";
  div.style.opacity = 0.85;
  div.style.fontFamily = "Menlo, Consolas, monospace";
  div.style.fontSize = "large";
  div.style.padding = "2rem";
  div.style.lineHeight = "1.2";
  div.style.whiteSpace = "pre-wrap";
  div.style.overflow = "auto";
  iframe.contentDocument.body.appendChild(div);
  return div;
}

function ensureOverlayDivExists(onOverlayDivReady) {
  // Create iframe and, when it is ready, a div inside it.
  var overlayIframe = createOverlayIframe(function onIframeLoad() {
    var overlayDiv = addOverlayDivTo(overlayIframe);
    // Now we can talk!
    onOverlayDivReady(overlayDiv, overlayIframe);
  });

  // Zalgo alert: onIframeLoad() will be called either synchronously
  // or asynchronously depending on the browser.
  // We delay adding it so `overlayIframe` is set when `onIframeLoad` fires.
  document.body.appendChild(overlayIframe);
}

function showOnOverlay() {
  ensureOverlayDivExists(function onOverlayDivReady(overlayDiv, iframe) {
    iframe.updateDisplay = function (message) {
        overlayDiv.innerHTML += ansiHTML(entities.encode(message)) +'<br>';
    };

    // Make it look similar to our terminal.
    overlayDiv.innerHTML =
      "<span style=\"color: #E36049\">Switching to live debug .</span><br><br>" +
      "<span style=\"color: #FFD080\">Please wait...</span><br><br>";
  });
}

showOnOverlay();