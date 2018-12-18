module.exports = {
  baseUrl: "/dist/",
  filenameHashing: false,
  chainWebpack: config => {
    config.devtool("eval-source-map");
  }
};
