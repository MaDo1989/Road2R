const MASTER = {
  getBaseUrl: () => {
    const env = MASTER.environmentDetected();
    if (env === "local")
      return "http://localhost:53644/Road%20to%20Recovery/pages/WebService.asmx/";
    if (env === "test")
      return "https://roadtorecovery.org.il/Gilad_test/Road%20to%20Recovery/pages/WebService.asmx/";
    return "https://roadtorecovery.org.il/prod/Road%20to%20Recovery/pages/WebService.asmx/";
  },
  environmentDetected: () => {
    if (location.href.includes("localhost") || location.protocol == "file:") {
      return "local";
    }
    if (location.href.includes("Gilad_test")) {
      return "test";
    }
    if (location.href.includes("prod")) {
      return "prod";
    }
    return "unknown";
  },
};
