import * as alt from "alt-client";
import * as native from "natives";
import * as helper from "../helper.js";

var view = null;

alt.everyTick(() => {
  const blips = alt.Blip.all;
  if (blips.length === 0)
    return;
  
  const local = alt.Player.local;

  const entity = local.vehicle ? local.vehicle.scriptID : local.scriptID;
  const vector = native.getEntityVelocity(entity);
  const frameTime = native.getFrameTime();
  
  for (let i = 0; i < blips.length; i++) {
    const blip = blips[i];
    if (blip.name !== "Haus")
      continue;
    
    const dist = helper.distance2d(local.pos, blip.pos);
    if (dist >= 10)
      continue;
    
    const scale = 1 - (0.8 * dist) / 20;
    const fontSize = 0.6 * scale;
      
    helper.drawText3d(`HAUS`, blip.pos.x + vector.x * frameTime, blip.pos.y + vector.y * frameTime, blip.pos.z + vector.z * frameTime, fontSize, 4, 255, 255, 255, 255);
  }
});

alt.onServer("house:get", (id, owner, price, locked) => {
  if (view !== null)
    return;
  
  console.log(`${id} | ${owner} | ${price} | ${locked}`);
  
  alt.toggleVoiceControls(false);
  alt.toggleGameControls(false);
  alt.showCursor(true);

  view = new alt.WebView("http://resource/client/house/html/index.html");
  view.focus();
  view.emit("update", id, owner, price, locked);
});

alt.on("keydown", (key) => {
  // Prevent keys when displaying a user interface
  if (!alt.gameControlsEnabled())
  {
    if (view !== null && key === 27 /* ESC */) {
      view.unfocus();
      view.destroy();
      view = null;

      alt.toggleVoiceControls(true);
      alt.toggleGameControls(true);
      alt.showCursor(false);
    }
    return;
  }

  let nearHouse = false;
  const local = alt.Player.local;
  const blips = alt.Blip.all;
  for (let i = 0; i < blips.length; i++) {
    const blip = blips[i];
    if (blip.name !== "Haus")
      continue;
    
    const dist = helper.distance2d(local.pos, blip.pos);
    if (dist >= 5)
      continue;
    
    nearHouse = true;
    break;
  }

  if (!nearHouse)
    return;

  switch (key) {
    case 69: // E
      alt.emitServer("house:get");
      break;
  }
});