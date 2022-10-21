/* ========== */
import "/client/chat";
import "/client/login";
import "/client/notification";
import "/client/vehicle";
import "/client/house";
/* ========== */
import * as alt from "alt-client";
import * as native from "natives";
import * as helper from "/client/helper.js";

// Set the alt:V watermark to the top center
alt.setWatermarkPosition(3);

var altTick = null;

function renderNameTags() {
  const local = alt.Player.local;

  for (let i = 0, n = alt.Player.all.length; i < n; i++) {
    const player = alt.Player.all[i];
    if (!player.valid || player.scriptID === local.scriptID)
      continue;
    
    const name = player.getSyncedMeta("NAME");
    const id = player.getSyncedMeta("ID");
    if (!(name, id))
      continue;

    if (!native.hasEntityClearLosToEntity(local.scriptID, player.scriptID, 17))
      continue;

    const dist = helper.distance2d(player.pos, player.pos);
    if (dist > 10)
      continue;
    const scale = 1 - (0.8 * dist) / 20;
    const fontSize = 0.6 * scale;

    let headPos = { ...native.getPedBoneCoords(player.scriptID, 12844, 0, 0, 0) };
    headPos.z += 0.75;
    const entity = player.vehicle ? player.vehicle.scriptID : player.scriptID;
    const vector = native.getEntityVelocity(entity);
    const frameTime = native.getFrameTime();
    helper.drawText3d(`${name}\nID: ${id}`, headPos.x + vector.x * frameTime, headPos.y + vector.y * frameTime, headPos.z + vector.z * frameTime, fontSize, 4, 255, 255, 255, 255);
  }
}

alt.on("showHud", () => {
  if (altTick !== null)
    return;
  
  altTick = alt.everyTick(() => {
    renderNameTags();
    
    const localPos = alt.Player.local.pos;
  
    // Street names
    const getStreetHash = native.getStreetNameAtCoord(localPos.x, localPos.y, localPos.z, 0, 0);
    const streetName = native.getStreetNameFromHashKey(getStreetHash[1]);
    const zone = native.getLabelText(native.getNameOfZone(localPos.x, localPos.y, localPos.z));
    helper.drawText2d(`${streetName}\n${zone}`, 0.215, 0.925, 0.5, 4, 244, 210, 66, 255);
  });
});