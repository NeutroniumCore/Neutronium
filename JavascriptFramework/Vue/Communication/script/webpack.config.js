var path = require('path')
var webpack = require('webpack')
var FriendlyErrorsPlugin = require('friendly-errors-webpack-plugin')

var bubleOptions = {
  target: null,
  // { chrome: 52 },
  objectAssign: 'Object.assign'
}

module.exports = {
  entry: {
    communication: './src/communication.js',
  },
  output: {
    path: __dirname + '/dist',
    publicPath: '/dist/',
    filename: '[name].js',
  },
  externals:{
    'neutronium_listener' : '__neutronium_listener__'
  },
  resolve: {
    alias: {
      vue$: 'vue/dist/vue.common.js'
    }
  },
  module: {
    rules: [
      {
        test: /\.js$/,
        loader:  'buble-loader',
        exclude: /node_modules|vue\/dist|vuex\/dist/,
        options: bubleOptions
      },
      {
        test: /\.vue$/,
        loader: 'vue-loader',
        options: {
          preserveWhitespace: false,
          buble: bubleOptions
        }
      },
      {
        test: /\.(png|woff2)$/,
        loader: 'url-loader?limit=0'
      }
    ]
  },
  performance: {
    hints: false
  },
  plugins: [
    new webpack.DefinePlugin({
      'process.env': {
        NODE_ENV: '"production"'
      }
    }),
    new webpack.optimize.UglifyJsPlugin({
      compress: {
        warnings: false
      }
    })
  ],

  devServer: {
    quiet: true
  }
}
