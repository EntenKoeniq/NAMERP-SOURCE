<template>
  <main style="display:flex">
    <div style="width:50%">
      <form onsubmit="event.preventDefault()">
        <p v-if="error !== null" class="error">{{ error }}</p>

        <span>E-Mail eingeben</span>
        <input v-model="email" type="email" placeholder="name@example.com">
        <span>Passwort eingeben</span>
        <input v-model="password" type="password" placeholder="Password">

        <button type="submit" @click="login()">Anmelden</button>
      </form>
    </div>
    <div class="register" style="width:50%">
      <button type="submit" id="registerSubmit">registrieren</button>
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
    email: "",
    password: "",
    error: null
  }),
  mounted() {
    alt.on("error", (msg) => this.error = msg);
  },
  methods: {
    login() {
      if (!(this.email, this.password)) {
        this.error = "Email oder Passwort fehlt!";
        return;
      }
      
      alt.emit("loginPressed", this.email, this.password);
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
    width: 50vw;
    background-color: #2479aa;
    border-radius: 3px;

    form {
      padding: 1vw;

      .error {
        color: #df2f2f;
        font-size: 0.9vw;
        font-weight: bold;
      }

      span {
        font-size: 0.7vw;
        text-transform: uppercase;
      }

      input {
        width: 100%;
        border: 2px solid #aaa;
        border-radius: 4px;
        margin: 0.2vw 0;
        outline: none;
        padding: 0.8vw;
        box-sizing: border-box;
        font-size: 0.8vw;
        transition: border-color .3s ease-in-out;

        &:focus {
          border-color: dodgerblue;
          box-shadow: 0 0 8px 0 dodgerblue;
        }
      }

      button {
        padding: 1vw;
        width: 100%;
        font-size: 1vw;
        background: linear-gradient(130deg, #ff7300, #ffbb00);
        color: #fff;
        border: 0;
        border-radius: 4px;
        text-transform: uppercase;
        font-weight: bold;
        cursor: pointer;
      }
    }
  }

  .register {
    text-align: center;

    button {
      position: fixed;
      bottom: 50%;
      transform: translate(-50%, 50%);

      padding: 1vw;
      width: 20vw;
      font-size: 1vw;
      background: linear-gradient(130deg, #df2f2f, #dd6b6b);
      color: #fff;
      border: 0;
      border-radius: 4px;
      text-transform: uppercase;
      font-weight: bold;
      cursor: pointer;
    }
  }
</style>
