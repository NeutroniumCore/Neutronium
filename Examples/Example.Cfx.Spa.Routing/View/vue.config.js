module.exports = {
  baseUrl: "./",
  filenameHashing: false,
  chainWebpack: config => {
    config.devtool("eval-source-map");
  }
};
