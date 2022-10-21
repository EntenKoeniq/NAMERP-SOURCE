CREATE TABLE accounts (
  id bigserial primary key,
  email varchar(320) unique not null,
  password_hash varchar(65) not null,
  admin int2 not null default 0,
  created_on timestamp not null default now(),
  updated_on timestamp not null default now()
);

CREATE TABLE online (
  account_id bigserial not null,
  created_on timestamp not null default now()
);

CREATE TABLE houses (
  id bigserial primary key,
  p_x float4 not null,
  p_y float4 not null,
  p_z float4 not null,
  price int4 not null,
  owner int4 default 0,
  locked boolean not null default true,
  interior int2 not null,
  created_on timestamp not null default now(),
  updated_on timestamp not null default now()
);

CREATE TABLE cfg_vehicles (
  id bigserial primary key,
  name varchar(20) not null,
  hash int4 not null,
  multi int4 not null default 1,
  price int4 not null default 0,
  fuel_tank float4 not null default 0,
  fuel_consumption float4 not null default 0
);

CREATE TABLE vehicles (
  id bigserial primary key,
  cfg_vehicle_id int4 not null,
  owner int4 default 0,
  organization int4 default 0,
  family int4 default 0,
  engine boolean default false,
  alive boolean default true,
  locked boolean default false,
  p_x float4 not null,
  p_y float4 not null,
  p_z float4 not null,
  r_r float4 not null,
  r_p float4 not null,
  r_y float4 not null,
  dim int4 default 0,
  insurance boolean default false,
  last_insurance timestamp not null default now(),
  fuel float4 not null default 0
);