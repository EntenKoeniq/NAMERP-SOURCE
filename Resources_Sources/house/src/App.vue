<template>
  <main>
    <div>
      <div style="display:flex">
        <div style="margin-right: 1vw">
          <small>ID</small>
          <p>{{ id }}</p>
        </div>
        <div>
          <small>Eigentümer</small>
          <p>{{ owner }}</p>
        </div>
      </div>
      <div style="margin-top: 1vw">
        <small>Preis</small>
        <p>$ {{ price }}</p>
      </div>
      <div style="margin-top: 1vw">
        <button :class="[ owner === 0 ? 'green' : 'red' ]">{{ owner === 0 ? 'Kaufen' : 'Verkaufen' }}</button>
        <button :class="[ locked ? 'green' : 'red' ]">{{ locked ? 'Öffnen' : 'Schließen' }}</button>
        <button @click="enter()">Betreten</button>
        <button>Garage</button>
      </div>
    </div>
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
    id: 12345,
    owner: 12345,
    price: 123456,
    locked: true
  }),
  mounted() {
    alt.on("update", (id, owner, price, locked) => {
      this.id = id;
      this.owner = owner;
      this.price = price;
      this.locked = locked;
    });
  },
  methods: {
    enter() {
      alt.emit("enter", this.id);
    }
  }
}
</script>

<style lang="scss">
  * {
    font-family: sans-serif;
    margin: 0;
    padding: 0;
  }
  html, body {
    height: 100%;
    background: rgba(0, 0, 0, 0.2);
    color: #fff;
  }

  main {
    position: fixed;
    left: 50%;
    bottom: 50%;
    transform: translate(-50%, 50%);
    background-color: #292929;
    min-width: 15vw;
    border-radius: 3px;
    padding: 2vw;
  }

  small {
    font-size: .75vw;
  }

  p {
    font-size: 1vw;
  }

  button {
    display: block;
    padding: .3vw;
    font-size: .75vw;
    font-weight: bold;
    background-color: #168bcf;
    color: #fff;
    outline: none;
    border: none;
    width: 100%;
    cursor: pointer;

    &:hover {
      background-color: #1883c0;
    }
    &:active {
      background-color: #1a79b1;
    }

    &:first-of-type {
      border-top-left-radius: 2px;
      border-top-right-radius: 2px;
    }
    &:last-of-type {
      border-bottom-left-radius: 2px;
      border-bottom-right-radius: 2px;
    }

    &.green {
      background-color: #29c550;

      &:hover {
      background-color: #2abd4f;
      }
      &:active {
        background-color: #2cad4d;
      }
    }

    &.red {
      background-color: #c52929;

      &:hover {
      background-color: #b82a2a;
      }
      &:active {
        background-color: #a72828;
      }
    }
  }
</style>
