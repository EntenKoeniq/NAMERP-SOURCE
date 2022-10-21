import * as alt from "alt-client";
import * as native from "natives";
import * as helper from "../helper.js";

var houseList = [];

alt.everyTick(() => {  
  const local = alt.Player.local;

  const entity = local.vehicle ? local.vehicle.scriptID : local.scriptID;
  const vector = native.getEntityVelocity(entity);
  const frameTime = native.getFrameTime();

  houseList.forEach(el => {
    const dist = helper.distance2d(local.pos, el.blip.pos);
    if (dist < 10) {
      const scale = 1 - (0.8 * dist) / 20;
      const fontSize = 0.6 * scale;
      
      helper.drawText3d(`HAUS\nID: ${el.id}`, el.blip.pos.x + vector.x * frameTime, el.blip.pos.y + vector.y * frameTime, el.blip.pos.z + vector.z * frameTime, fontSize, 4, 255, 255, 255, 255);
    }
  });
});

function createHouse(posX, posY, posZ, owner) {
  let blip = new alt.PointBlip(posX, posY, posZ);
  blip.sprite = 411;
  blip.color = owner == 0 ? 1 : 2;
  blip.scale = 1.0;
  blip.shortRange = false;
  blip.name = "Haus";

  return blip;
}

alt.onServer("house:loadAll", (houses) => {
  const result = JSON.parse(houses);
  if (result.count === 0)
    return;
  
  result.forEach(el => houseList.push({ id: el.ID, blip: createHouse(el.Location.X, el.Location.Y, el.Location.Z, el.Owner) }));
});

alt.onServer("house:removeAll", () => {
  houseList.forEach(el => {
    el.blip.destroy();
    el.blip.delete();
    el.blip = null;
  });

  houseList = [];
});

alt.onServer("house:loadSingle", (house) => {
  const result = JSON.parse(house);
  houseList.push({ id: result.ID, blip: createHouse(result.Location.X, result.Location.Y, result.Location.Z, result.Owner) });
});

alt.onServer("house:removeSingle", (house) => {

});