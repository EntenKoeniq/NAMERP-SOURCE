import * as alt from "alt-client";
import * as native from "natives";

var isInVehicle = false;
var isDriver = false;
var view = null;
var altTick = null;

alt.onServer("vehicle:enter", (driver) => {
  isInVehicle = true;
  isDriver = driver;

  const local = alt.Player.local;
  native.setPedConfigFlag(local, 429, true); // prevent engine enabling on entering veh
  native.setPedConfigFlag(local, 184, true); // PED_FLAG_DISABLE_SHUFFLING_TO_DRIVER_SEAT
  native.setPedConfigFlag(local, 241, true); // prevent engine shut down on leaving veh
  native.setPedConfigFlag(local, 35, false); // disable using helmets

  if (!driver)
    return;

  view = new alt.WebView("http://resource/client/vehicle/html/index.html");
  view.focus();

  altTick = alt.everyTick(() => {
    const veh = alt.Player.local.vehicle;
    if (veh && view)
      view.emit("update", veh.speed, veh.lockState === 2, veh.engineOn, veh.daylightOn);
  });
});

alt.onServer("vehicle:leave", () => {
  isInVehicle = false;
  if (isDriver) {
    if (altTick !== null) {
      alt.clearEveryTick(altTick);
      altTick = null;
    }

    isDriver = false;
  }

  if (view !== null)
  {
    view.unfocus();
    view.destroy();
    view = null;
  }
});

alt.onServer("vehicle:indicatorLights", (vehId, light, time) => {
  const veh = alt.Vehicle.getByID(vehId);
  if (!veh || veh.indicatorLights === light)
    return;
  
  alt.emit("showNotification", false, `Fahrzeugtüren wurden ${veh.lockState === 1 ? 'geöffnet' : 'verschlossen'}!`);
  veh.indicatorLights = light;
  alt.setTimeout(() => veh.indicatorLights = 0, time);
});

alt.on("keydown", (key) => {
  // Prevent keys when displaying a user interface
  if (!alt.gameControlsEnabled())
    return;
  
  switch (key) {
    case 17: // CTRL (left)
      if (isInVehicle && isDriver) {
        alt.emitServer("vehicle:toggleEngine");
      }
      break;
    case 76: // L
      alt.emitServer("vehicle:lock");
      break;
  }
});