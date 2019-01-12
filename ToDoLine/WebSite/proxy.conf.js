const PROXY_CONFIG = [
  {
    context: ["/api", "/odata", "/signalr", "/InvokeLogin", "/InvokeLogout", "/Metadata", "/Files", "/SignIn", "/SignOut", "/ClientAppProfile", "/core", "/jobs"],
    target: "http://localhost:53149",
    secure: false
  }
];

module.exports = PROXY_CONFIG;
