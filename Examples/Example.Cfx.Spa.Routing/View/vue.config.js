module.exports = {
  publicPath: "./",
  filenameHashing: false,
  chainWebpack: config => {
    config.devtool("eval-source-map");
  }
};
