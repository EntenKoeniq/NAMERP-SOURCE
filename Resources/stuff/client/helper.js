import * as native from "natives";

export function drawText2d(
  msg,
  x,
  y,
  scale,
  fontType,
  r,
  g,
  b,
  a,
  useOutline = true,
  useDropShadow = true,
  layer = 0,
  align = 0
) {
  let hex = msg.match('{.*}');
  if (hex) {
    const rgb = hexToRgb(hex[0].replace('{', '').replace('}', ''));
    r = rgb[0];
    g = rgb[1];
    b = rgb[2];
    msg = msg.replace(hex[0], '');
  }

  native.beginTextCommandDisplayText('STRING');
  native.addTextComponentSubstringPlayerName(msg);
  native.setTextFont(fontType);
  native.setTextScale(1, scale);
  native.setTextWrap(0.0, 1.0);
  native.setTextCentre(true);
  native.setTextColour(r, g, b, a);
  native.setTextJustification(align);

  if (useOutline) native.setTextOutline();

  if (useDropShadow) native.setTextDropShadow();

  native.endTextCommandDisplayText(x, y, 0);
}

export function drawText3d(
  msg,
  x,
  y,
  z,
  scale,
  fontType,
  r,
  g,
  b,
  a,
  useOutline = true,
  useDropShadow = true,
  layer = 0
) {
  let hex = msg.match('{.*}');
  if (hex) {
    const rgb = hexToRgb(hex[0].replace('{', '').replace('}', ''));
    r = rgb[0];
    g = rgb[1];
    b = rgb[2];
    msg = msg.replace(hex[0], '');
  }

  native.setDrawOrigin(x, y, z, 0);
  native.beginTextCommandDisplayText('STRING');
  native.addTextComponentSubstringPlayerName(msg);
  native.setTextFont(fontType);
  native.setTextScale(1, scale);
  native.setTextWrap(0.0, 1.0);
  native.setTextCentre(true);
  native.setTextColour(r, g, b, a);

  if (useOutline) native.setTextOutline();

  if (useDropShadow) native.setTextDropShadow();

  native.endTextCommandDisplayText(0, 0, 0);
  native.clearDrawOrigin();
}

export function distance2d(vector1, vector2) {
  return Math.sqrt(Math.pow(vector1.x - vector2.x, 2) + Math.pow(vector1.y - vector2.y, 2));
}