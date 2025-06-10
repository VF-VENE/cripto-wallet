<template>
  <form @submit.prevent="submitCompra">
    <label>Criptomoneda:</label>
    <select v-model="compra.cryptoCode" required>
      <option disabled value="">Selecciona una</option>
      <option v-for="crypto in criptomonedas" :key="crypto.cryptoCode" :value="crypto.cryptoCode">
        {{ crypto.nombre }}
      </option>
    </select>

    <label>Cantidad:</label>
    <input type="number" step="0.00001" v-model.number="compra.cantidadCripto" min="0.00001" required />

    <label>Cliente:</label>
    <select v-model="compra.clienteID" required>
      <option disabled value="">Selecciona un cliente</option>
      <option v-for="cliente in clientes" :key="cliente.clienteID" :value="cliente.clienteID">
        {{ cliente.nombre }}
      </option>
    </select>

    <label>Fecha y hora:</label>
    <input type="datetime-local" v-model="compra.fecha" required />

    <label>Exchange:</label>
    <select v-model.number="compra.ExchangeID" required>
      <option disabled value="">Selecciona un exchange</option>
      <option v-for="exchange in exchanges" :key="exchange.exchangeID" :value="exchange.exchangeID">
        {{ exchange.nombre }}
      </option>
    </select>
    <button type="submit">Guardar compra</button>
    <button @click="irAlHistorial">Ver historial de transacciones</button>
  </form>
    <router-view />

</template>



<script setup>
import { reactive, ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

function irAlHistorial() {
  router.push('/historial')
}

const compra = reactive({
  clienteID: null,
  cryptoCode: '',
  accion: 'purchase',
  cantidadCripto: null,
  fecha: '',
  ExchangeID: null
})

const clientes = ref([])
const criptomonedas = ref([])
const exchanges = ref([])

onMounted(async () => {
  try {
    const resClientes = await fetch('https://localhost:7241/api/Cliente')
    clientes.value = await resClientes.json()

    const resCriptos = await fetch('https://localhost:7241/api/Criptomoneda')
    criptomonedas.value = await resCriptos.json()

    const resExchanges = await fetch('https://localhost:7241/api/Exchange')
    exchanges.value = await resExchanges.json()

  } catch (error) {
    console.error('Error cargando datos', error)
  }
})

async function submitCompra() {
  if (
    compra.clienteID === null ||
    compra.cryptoCode === '' ||
    compra.ExchangeID === null ||
    !compra.fecha ||
    compra.cantidadCripto === null || 
    compra.cantidadCripto <= 0
  ) {
    alert('Debes completar todos los campos obligatorios.')
    return
  }

  const fechaISO = new Date(compra.fecha).toISOString()

  const datosEnvio = {
    clienteID: compra.clienteID,
    cryptoCode: compra.cryptoCode,
    accion: 'purchase',
    cantidadCripto: compra.cantidadCripto,
    fecha: fechaISO,
    ExchangeID: compra.ExchangeID
  }

  try {
    console.log(datosEnvio)
    const respuesta = await fetch('https://localhost:7241/api/Transaccion', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(datosEnvio)
    })
    console.log("Datos enviados al backend:", JSON.stringify(datosEnvio, null, 2))

    if (!respuesta.ok) {
      const errorTexto = await respuesta.text()
      throw new Error(errorTexto || 'Error al guardar la compra')
    }

    alert('Compra guardada con éxito')

    compra.clienteID = null
    compra.cryptoCode = ''
    compra.accion = 'purchase'
    compra.cantidadCripto = null
    compra.fecha = ''
    compra.ExchangeID = null

  } catch (error) {
    alert(error.message)
  }
}
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
