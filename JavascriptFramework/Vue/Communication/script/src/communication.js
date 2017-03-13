import CircularJson from 'circular-json-es6'
import mitt from 'mitt'
import listener from 'neutronium_listener'

var emitter = new mitt()

const on = function (channel, cb) {
  const rawCb = data => {
    const dataValue = CircularJson.parse(data);
    cb(dataValue);
  };
  emitter.on(channel, rawCb);
}
listener.on = on;

listener.emit = function (channel, data) {
  emitter.emit(channel, data)
}

const rawPost = listener.postMessage
const post = function (channel, data = null) {
  var stringData = CircularJson.stringify(data)
  rawPost(channel, stringData)
}
listener.post = post;

listener.eval = function (codeChannel, code) {
  post(`code:${codeChannel}`, code)
}

listener.runCodeOn = function (codeChannel) {
  on(`code:${codeChannel}`, code => eval(code))
}

