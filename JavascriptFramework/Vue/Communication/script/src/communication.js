import CircularJson from 'circular-json-es6'
import mitt from 'mitt'
import listener from 'neutronium_listener'

var emitter = new mitt()

listener.on = function (channel, cb) {
  const rawCb = data => {
    const dataValue = CircularJson.parse(data);
    cb(dataValue);
  };

  emitter.on(channel, rawCb);
}

listener.emit = function (channel, data) {
  emitter.emit(channel, data)
}

const rawPost = listener.postMessage

listener.post = function (channel, data=null){
  var stringData = CircularJson.stringify(data);
  rawPost(channel, stringData)
}
