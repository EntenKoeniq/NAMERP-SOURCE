import * as alt from "alt-client";
import * as native from "natives";

var isInVehicle = false;
var isDriver = false;
var view = null;
var altTick = null;

// Vehicle stuff
var vInterval = null;
var vId = 0;
var vFuel = 0;
var vFuel_tank = 0;
var vFuel_consumption = 0;
var vMulti = 0;

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
  
  const localVeh = local.vehicle;
  vId = localVeh.getSyncedMeta("id");
  vFuel = localVeh.getSyncedMeta("fuel");
  vFuel_tank = localVeh.getSyncedMeta("fuel_tank");
  vFuel_consumption = localVeh.getSyncedMeta("fuel_consumption");
  vMulti = localVeh.getSyncedMeta("multi");

  vInterval = alt.setInterval(() => {
    const trip = localVeh.speed / 1000;
    if (vFuel === 0) {
      alt.emitServer("vehicle:update", vId, 0);
      return;
    }
    vFuel -= vFuel_consumption * trip;
    if (vFuel < 0)
      vFuel = 0;
  }, 1000);

  view = new alt.WebView("http://resource/client/vehicle/html/index.html");
  view.focus();

  altTick = alt.everyTick(() => {
    const veh = alt.Player.local.vehicle;
    if (veh && view)
      view.emit("update", veh.speed, (vFuel / vFuel_tank * 100).toFixed(2), veh.lockState === 2);
  });
});

alt.onServer("vehicle:leave", () => {
  isInVehicle = false;
  if (isDriver) {
    alt.emitServer("vehicle:update", vId, vFuel);

    if (altTick !== null) {
      alt.clearEveryTick(altTick);
      altTick = null;
    }

    if (vInterval !== null) {
      alt.clearInterval(vInterval);
      vInterval = null;
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