<template>
  <main>
    <section style="width:30vw">
      <h1 :class="[ failed ? 'failed' : 'successfully' ]">Benachrichtigung</h1>
      <div class="progress-bar"></div>
      <p>{{ message }}</p>
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
    failed: false,
    message: ""
  }),
  mounted() {
    alt.on("message", (failed, message) => {
      this.failed = failed;
      this.message = message;
    });
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
    left: 50%;
    bottom: 5px;
    transform: translateX(-50%);
    margin: 0 auto;
    text-align: center;
  }

  h1 {
    font-size: 2vw;
    font-weight: 800;
    text-transform: uppercase;
    margin: 0.5vw 0;

    &.successfully {
      color: #12aa0c;
    }
    
    &.failed {
      color: #a81818;
    }
  }

  p {
    color: #fff;
    font-size: 1vw;
  }

  .progress-bar {
    height: 0.5vw;
    border-radius: 30px;
    transition: 0.4s linear;
    transition-property: width, background-color;
    animation: progress 5s infinite;
  }

  @keyframes progress {
    0% {
      width: 0%;
      background: #f9bcca;
    }

    100% {
      width: 100%;
      background: #f3c623;
      box-shadow: 0 0 40px #f3c623;
    }
  }
</style>