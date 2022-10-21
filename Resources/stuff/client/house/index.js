import * as alt from "alt-client";
import * as native from "natives";
import * as helper from "../helper.js";

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