import * as alt from "alt-client";
import * as native from "natives";

var view = null;
var camera = null;

alt.on("connectionComplete", () => {
  native.pauseClock(true);

  const player = alt.Player.local.scriptID;
  native.displayRadar(false);
  native.freezeEntityPosition(player, true);
  native.setEntityVisible(player, false, 0);

  camera = native.createCam("DEFAULT_SCRIPTED_CAMERA", true);
  native.setCamCoord(camera, 344.3341, -998.8612, -98.19622);
  native.setCamRot(camera, -370.61447, 0, -289.61447, 2);
  native.setCamFov(camera, 40);
  native.setCamActive(camera, true);
  native.renderScriptCams(true, false, 0, true, false, 0);

  view = new alt.WebView("http://resource/client/login/html/index.html");
  view.on("loginPressed", (email, password) => alt.emitServer("login:pressed", email, password));
  view.focus();
   
  alt.showCursor(true);
  alt.toggleGameControls(false);
  alt.toggleVoiceControls(false);
});

alt.onServer("login:error", (msg) => view.emit("error", msg));

alt.onServer("login:hide", () => {
  alt.toggleVoiceControls(true);
  alt.toggleGameControls(true);
  alt.showCursor(false);
  
  view.unfocus();
  view.destroy();
  view = null;

  native.renderScriptCams(false, false, 0, true, false, 0);
  native.setCamActive(camera, false);
  native.destroyCam(camera, true);
  camera = null;

  const player = alt.Player.local.scriptID;
  native.displayRadar(true);
  native.freezeEntityPosition(player, false);
  native.setEntityVisible(player, true, 0);

  alt.emit("showChat");
  alt.emit("showHud");
});