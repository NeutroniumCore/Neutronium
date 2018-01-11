var path = require('path')
var webpack = require('webpack')
var utils = require('./utils')
var ExtractTextPlugin = require("extract-text-webpack-plugin")
const BabiliPlugin = require("babili-webpack-plugin")

var output = {
  path: path.resolve(__dirname, './dist'),
  filename: 'build.js'
};

var resolve = (p) => path.resolve(__dirname, p);

var cssOptionLoader;

if (process.env.NODE_ENV !== 'production') {
  cssOptionLoader = { sourceMap: false }
  output.publicPath = 'dist/'
} else {
  cssOptionLoader = { sourceMap: true, extract: true }
}

var webpackOptions = {
  output: output,
  module: {
    rules: [
      {
        test: /\.vue$/,
        loader: 'vue-loader',
        options: {
          loaders: utils.cssLoaders(cssOptionLoader),
          postcss: [
            require('autoprefixer')({
              browsers: ['last 2 versions']
            })
          ]
        }
      },
      {
        test: /\.js$/,
        loader: 'babel-loader',
        exclude: /node_modules/,
        include: [ // use `include` vs `exclude` to white-list vs black-list
          path.resolve(__dirname, "src"), // white-list your app source files
          require.resolve("bootstrap-vue"), // white-list bootstrap-vue
          require.resolve('vue-awesome') // vue-awesome
        ],
      },
      {
        test: /\.(png|jpe?g|gif|svg)(\?.*)?$/,
        loader: 'url-loader?limit=10000'
      },
      {
        test: /\.(woff2?|eot|ttf|otf)(\?.*)?$/,
        loader: 'url-loader?limit=10000'
      },
      {
        test: /\.json$/,
        loader: 'json-loader'
      },
      {
        test: /\.cjson$/,
        loader: 'raw-loader'
      },
      {
        test: /\.html$/,
        loader: 'vue-html-loader'
      },
      {
        test: /\.styl$/,
        loader: ['style-loader', 'css-loader', 'stylus-loader', {
          loader: 'vuetify-loader'
        }]
      }
    ]
  },
  devServer: {
    historyApiFallback: true,
    noInfo: true
  },
  resolve: {
    extensions: ['.js', '.vue', '.json', '.css', '.cjson'],
    alias: {
      'src': path.resolve(__dirname, '../src'),
      'assets': path.resolve(__dirname, '../src/assets'),
      'components': path.resolve(__dirname, '../src/components')
    }
  },
  plugins: [
    new ExtractTextPlugin('styles.css')
  ],
  devtool: 'source-map',
}

var buildMode = false;

switch (process.env.NODE_ENV) {
  case 'production':
    buildMode = true
    webpackOptions.externals = {
      'vue': 'Vue',
      'vueHelper': 'glueHelper'
    }
    webpackOptions.entry = './src/entry.js';

    webpackOptions.devtool = '#cheap-source-map'
    // http://vue-loader.vuejs.org/en/workflow/production.html
    webpackOptions.plugins = (webpackOptions.plugins || []).concat([
      new webpack.DefinePlugin({
        'process.env': {
          NODE_ENV: '"production"'
        }
      }),
      new webpack.optimize.ModuleConcatenationPlugin(),
      new BabiliPlugin({}, { comments: false }),
      new webpack.LoaderOptionsPlugin({
        minimize: true
      })
    ])
    break;

  case 'development':
    webpackOptions.plugins = (webpackOptions.plugins || []).concat([
      new webpack.HotModuleReplacementPlugin()
    ]);

    webpackOptions.resolve.alias = {
      'vue$': 'vue/dist/vue'
    }
    webpackOptions.entry = './src/main.js';
    break;

  case 'integrated':
    webpackOptions.plugins = (webpackOptions.plugins || []).concat([
      new webpack.HotModuleReplacementPlugin()
    ]);

    webpackOptions.externals = {
      'vue': 'Vue',
      'vueHelper': 'glueHelper'
    }
    webpackOptions.entry = './src/entry.js';
    break;
}

const styleOption = buildMode ? { sourceMap: true, extract: true } : { sourceMap: true };
webpackOptions.module.rules = webpackOptions.module.rules.concat(utils.styleLoaders(styleOption))

module.exports = webpackOptions