<template>
  <div class="nueva-compra">
    <h2>Registrar nueva compra</h2>

    <form @submit.prevent="registrarCompra">
      <div>
        <label>Criptomoneda:</label>
        <select v-model="form.crypto_code" required>
          <option disabled value="">Selecciona una cripto</option>
          <option value="bitcoin">Bitcoin</option>
          <option value="usdc">USDC</option>
          <option value="ethereum">Ethereum</option>
        </select>
      </div>

      <div>
        <label>Cantidad:</label>
        <input type="number" step="0.00001" v-model.number="form.crypto_amount" required />
      </div>

      <div>
        <label>Cliente:</label>
        <select v-model="form.client_id" required>
          <option disabled value="">Selecciona un cliente</option>
          <option v-for="cliente in clientes" :key="cliente.id" :value="cliente.id">
            {{ cliente.nombre }}
          </option>
        </select>
      </div>

      <div>
        <label>Fecha y hora:</label>
        <input type="datetime-local" v-model="form.datetime" required />
      </div>

      <button type="submit">Registrar</button>
    </form>

    <p v-if="mensaje">{{ mensaje }}</p>
  </div>
</template>

<script>

//https://localhost:7241/
export default {
  name: "NuevaCompra",
  data() {
    return {
      form: {
        crypto_code: "",
        crypto_amount: null,
        client_id: "",
        datetime: "",
      },
      clientes: [],
      mensaje: "",
    };
  },
  mounted() {
    this.cargarClientes();
  },
  methods: {
    async cargarClientes() {
      try {
        const res = await fetch("https://localhost:7241/api/cliente");
        const data = await res.json();
        this.clientes = data;
      } catch (error) {
        console.error("Error al cargar clientes:", error);
      }
    },
    async registrarCompra() {
      if (this.form.crypto_amount <= 0) {
        this.mensaje = "La cantidad debe ser mayor a 0";
        return;
      }

      const body = {
        crypto_code: this.form.crypto_code,
        action: "purchase",
        client_id: this.form.client_id,
        crypto_amount: this.form.crypto_amount,
        datetime: this.form.datetime,
      };

      try {
        const res = await fetch("https://localhost:7241/api/transaccion", {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify(body),
        });

        if (res.ok) {
          this.mensaje = "Compra registrada correctamente";
        } else {
          const error = await res.text();
          this.mensaje = `Error: ${error}`;
        }
      } catch (error) {
        this.mensaje = "Error al registrar la compra";
        console.error(error);
      }
    },
  },
};
</script>

<style scoped>
.nueva-compra {
  max-width: 500px;
  margin: 0 auto;
  padding: 1rem;
}
form div {
  margin-bottom: 1rem;
}
label {
  display: block;
  font-weight: bold;
}
input,
select {
  width: 100%;
  padding: 0.5rem;
}
button {
  padding: 0.6rem 1rem;
  background-color: #42b983;
  color: white;
  border: none;
  cursor: pointer;
}
</style>
