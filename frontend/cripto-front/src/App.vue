<template> 
  <form @submit.prevent="submitCompra">

    <label>Criptomoneda:</label>
    <select v-model="compra.cryptoCode" required>
      <option disabled value="">Selecciona una</option>
      <option v-for="crypto in criptomonedas" :key="crypto.id" :value="crypto.codigo">
      {{ crypto.nombre }}
      </option>
    </select>


    <label>Cantidad:</label>
    <input type="number" step="0.00001" v-model.number="compra.cantidadCripto" min="0.00001" required />

    <label>Cliente:</label>
    <select v-model.number="compra.clienteID" required>
      <option disabled value="">Selecciona un cliente</option>
      <option v-for="cliente in clientes" :key="cliente.id" :value="cliente.id">
        {{ cliente.nombre }}
      </option>
    </select>

    <label>Fecha y hora:</label>
    <input type="datetime-local" v-model="compra.fecha" required />

    <label>Exchange:</label>
    <select v-model.number="compra.exchangeID" required>
      <option disabled value="">Selecciona un exchange</option>
      <option v-for="exchange in exchanges" :key="exchange.id" :value="exchange.id">
       {{ exchange.nombre }}
      </option>
    </select>



    <button type="submit">Guardar compra</button>
  </form>
</template>

<script setup>
import { reactive, ref, onMounted } from 'vue'

// Estado reactivo para la compra con los nombres exactos que espera el backend
const compra = reactive({
  clienteID: null,
  cryptoCode: null,
  accion: 'compra',
  cantidadCripto: null,
  fecha: '',
  exchangeID: null
})

const clientes = ref([])
const criptomonedas = ref([])  // Aquí guardaremos la lista que venga del backend
const exchanges = ref([])


onMounted(async () => {
  try {
    // Traer clientes (ya lo tenés)
    const resClientes = await fetch('https://localhost:7241/api/Cliente')
    clientes.value = await resClientes.json()

    // Traer criptomonedas (nuevo)
    const resCriptos = await fetch('https://localhost:7241/api/Criptomoneda')
    criptomonedas.value = await resCriptos.json()

    const resExchanges = await fetch('https://localhost:7241/api/Exchange')
    exchanges.value = await resExchanges.json()

  } catch (error) {
    console.error('Error cargando datos', error)
  }
})


async function submitCompra() {
  console.log('clienteID:', compra.clienteID, typeof compra.clienteID)
console.log('cryptoCode:', compra.cryptoCode, typeof compra.cryptoCode)
console.log('exchangeID:', compra.exchangeID, typeof compra.exchangeID)


  if (
  compra.clienteID === null ||
  compra.cryptoCode === '' ||
  compra.exchangeID === null ||
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
    accion: 'compra',
    cantidadCripto: compra.cantidadCripto,
    fecha: fechaISO,
    exchangeID: compra.exchangeID
  }

  try {
    console.log(datosEnvio)
    const respuesta = await fetch('https://localhost:7241/api/Transaccion', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(datosEnvio)
    })

    if (!respuesta.ok) {
      const errorInfo = await respuesta.json()
      throw new Error(errorInfo.message || 'Error al guardar la compra')
    }

    alert('Compra guardada con éxito')

    // Limpiar formulario
    compra.clienteID = null
    compra.cryptoCode = ''
    compra.accion = 'compra'
    compra.cantidadCripto = null
    compra.fecha = ''
    compra.exchangeID = null

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
