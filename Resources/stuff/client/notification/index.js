import * as alt from "alt-client";

var view = null;
var timer = null;

alt.on("showNotification", (failed, message) => show(failed, message));
alt.onServer("notification:show", (failed, message) => show(failed, message));

function cleanup() {
  if (timer !== null) {
    clearInterval(timer);
    timer = null;
  }

  if (view !== null) {
    view.unfocus();
    view.destroy();
    view = null;
  }
}

function show(failed, message) {
  cleanup();
  
  view = new alt.WebView("http://resource/client/notification/html/index.html");
  view.focus();
  view.emit("message", failed, message);
  timer = setInterval(() => cleanup(), 5000); // Delete after 5 seconds
}