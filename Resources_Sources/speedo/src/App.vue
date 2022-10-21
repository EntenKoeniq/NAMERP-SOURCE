<template>
  <main>
    <section class="none-bg kmh">
      <span>{{ speedEl }}</span> KM/H
    </section>
    <section style="display:flex">
      <div>
        <i class="fa-solid fa-gas-pump"></i>
      </div>
      <div style="text-align:right"><span id="speed">78</span>%</div>
    </section>
    <section style="display:flex;justify-content:center">
      <i class="fa-solid fa-key" :style="[ lockEl ? 'color:yellow' : none ]"></i>
      <i class="fa-solid fa-bolt" :style="[ engineEl ? 'color:yellow' : none ]"></i>
      <i class="fa-solid fa-lightbulb" :style="[ lightEl ? 'color:yellow' : none ]"></i>
    </section>
  </main>
</template>

<script>
if (window.alt === undefined) {
  window.alt = {
    emit: () => {},
    on: () => {}
  };
}

export default {
  data: () => ({
    speedEl: 0,
    fuelEl: 0,
    lockEl: false,
    engineEl: false,
    lightEl: false
  }),
  mounted() {
    alt.on("update", (speed, lock, engine, light) => this.update(speed, lock, engine, light));
  },
  methods: {
    update(speed, lock, engine, light) {
      this.speedEl = Math.round(speed * 3.6);
      this.lockEl = lock;
      this.engineEl = engine,
      this.lightEl = light;
    }
  }
}
</script>

<style>
  html,
  body {
    background-color: transparent;
    font-family: sans-serif;
  }
</style>

<style lang="scss" scoped>
  main {
    position: fixed;
    right: 0.2vw;
    bottom: 0.2vw;
    margin: 0 auto;
    width: 12vw;

    section {
      color: #fff;

      &.kmh {
        text-align: center;
        font-size: 1vw;
      }

      i:not(:first-child) {
        margin-left: .5vw;
      }

      &:not(.none-bg) {
        background: linear-gradient(135deg, #000000c4, #00000079);
        border: 0.15vw solid #303030;
        border-radius: 3px;
        padding: 0.4vw;
        font-size: 0.8vw;

        div {
          width: 100%;
        }

        &:not(:first-child) {
          margin-top: 0.2vw;
        }
      }
    }
  }
</style>