<template>
  <div class="container py-5">
    <div class="row justify-content-center">
      <div class="col-md-8 col-lg-6">
        
        <button @click="presionarBotonRegresar" class="btn btn-outline-secondary btn-sm mb-4 shadow-sm">
          <i class="bi bi-arrow-left"></i> Volver al Menú
        </button>

        <div class="card shadow-lg border-0">
          <div :class="['card-header py-3 text-white text-center', datosDeLaOperacion.accion === 'purchase' ? 'bg-primary' : 'bg-danger']">
            <h3 class="mb-0">
              <i :class="datosDeLaOperacion.accion === 'purchase' ? 'bi bi-cart-plus' : 'bi bi-graph-down-arrow'"></i>
              <span v-if="datosDeLaOperacion.accion === 'purchase'"> Registrar Nueva Compra</span>
              <span v-else> Registrar Nueva Venta</span>
            </h3>
          </div>

          <div class="card-body p-4">
            <form @submit.prevent="gestionarEnvioDelFormulario">
              
              <div class="mb-4 text-center">
                <label class="form-label d-block fw-bold mb-3">Tipo de Movimiento</label>
                <div class="btn-group w-100 shadow-sm" role="group">
                  <input type="radio" class="btn-check" name="opcionAccion" id="botonCompra" value="purchase" v-model="datosDeLaOperacion.accion">
                  <label class="btn btn-outline-primary py-2" for="botonCompra">COMPRA</label>
                  
                  <input type="radio" class="btn-check" name="opcionAccion" id="botonVenta" value="sale" v-model="datosDeLaOperacion.accion">
                  <label class="btn btn-outline-danger py-2" for="botonVenta">VENTA</label>
                </div>
              </div>

              <div class="row g-3">
                <div class="col-12">
                  <label class="form-label fw-bold">Seleccionar Cliente</label>
                  <div class="input-group">
                    <span class="input-group-text bg-white"><i class="bi bi-person text-muted"></i></span>
                    <select class="form-select border-start-0" v-model="datosDeLaOperacion.clienteID">
                      <option :value="null" disabled>-- Seleccione un cliente --</option>
                      <option v-for="c in listaDeClientes" :key="c.clienteID" :value="c.clienteID">
                        {{ c.nombre }}
                      </option>
                    </select>
                  </div>
                </div>

                <div class="col-md-6">
                  <label class="form-label fw-bold">Criptomoneda</label>
                  <div class="input-group">
                    <span class="input-group-text bg-white"><i class="bi bi-coin text-muted"></i></span>
                    <select class="form-select border-start-0" v-model="datosDeLaOperacion.cryptoCode">
                      <option value="" disabled>Elegir...</option>
                      <option v-for="cripto in listaDeCriptomonedas" :key="cripto.cryptoCode" :value="cripto.cryptoCode">
                        {{ cripto.nombre }} ({{ cripto.cryptoCode.toUpperCase() }})
                      </option>
                    </select>
                  </div>
                </div>

                <div class="col-md-6">
                  <label class="form-label fw-bold">Cantidad</label>
                  <div class="input-group">
                    <span class="input-group-text bg-white"><i class="bi bi-123 text-muted"></i></span>
                    <input type="number" step="0.00000001" class="form-control border-start-0" v-model.number="datosDeLaOperacion.cantidadCripto" placeholder="0.00">
                  </div>
                </div>

                <div class="col-md-12">
                  <label class="form-label fw-bold">Exchange</label>
                  <div class="input-group">
                    <span class="input-group-text bg-white"><i class="bi bi-building text-muted"></i></span>
                    <select class="form-select border-start-0" v-model.number="datosDeLaOperacion.ExchangeID">
                      <option :value="null" disabled>-- Seleccione un mercado --</option>
                      <option v-for="ex in listaDeExchanges" :key="ex.exchangeID" :value="ex.exchangeID">
                        {{ ex.nombre }}
                      </option>
                    </select>
                  </div>
                </div>

                <div class="col-12">
                  <label class="form-label fw-bold">Fecha de Registro</label>
                  <input type="datetime-local" class="form-control bg-light" v-model="datosDeLaOperacion.fecha" readonly />
                </div>
              </div>

              <hr class="my-4">

              <div class="d-grid gap-2">
                <button type="submit" :class="['btn btn-lg text-white shadow-sm', datosDeLaOperacion.accion === 'purchase' ? 'btn-primary' : 'btn-danger']" :disabled="estaProcesando">
                  <span v-if="estaProcesando" class="spinner-border spinner-border-sm me-2"></span>
                  Confirmar Registro
                </button>
                <button type="button" @click="navegarAlHistorial" class="btn btn-link text-decoration-none text-muted small">
                  <i class="bi bi-clock-history"></i> Ver Historial Completo
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
    <ToastNotification ref="referenciaToastAvisos" />
  </div>
</template>

<script setup>
import { reactive, ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import ToastNotification from '../components/ToastNotification.vue'

const herramientaRouter = useRouter()
const referenciaToastAvisos = ref(null)
const estaProcesando = ref(false)

const datosDeLaOperacion = reactive({
  clienteID: null,
  cryptoCode: '',
  accion: 'purchase',
  cantidadCripto: null,
  fecha: '',
  ExchangeID: null
})

const listaDeClientes = ref([])
const listaDeCriptomonedas = ref([])
const listaDeExchanges = ref([])

function obtenerFechaLocal() {
  const ahora = new Date()
  const offset = ahora.getTimezoneOffset() * 60000
  return new Date(ahora - offset).toISOString().slice(0, 16)
}

function presionarBotonRegresar() { herramientaRouter.back() }
function navegarAlHistorial() { herramientaRouter.push('/transacciones/historial') }

function reiniciarFormulario() {
  datosDeLaOperacion.clienteID = null
  datosDeLaOperacion.cryptoCode = ''
  datosDeLaOperacion.cantidadCripto = null
  datosDeLaOperacion.ExchangeID = null
  datosDeLaOperacion.fecha = obtenerFechaLocal()
}

onMounted(async function() {
  datosDeLaOperacion.fecha = obtenerFechaLocal()
  try {
    const [resCli, resCri, resEx] = await Promise.all([
      fetch('https://localhost:7241/api/Cliente'),
      fetch('https://localhost:7241/api/Criptomoneda'),
      fetch('https://localhost:7241/api/Exchange')
    ])
    listaDeClientes.value = await resCli.json()
    listaDeCriptomonedas.value = await resCri.json()
    listaDeExchanges.value = await resEx.json()
  } catch (e) {
    referenciaToastAvisos.value?.addToast('Error de conexión', 'error')
  }
})

async function gestionarEnvioDelFormulario() {
  if (!datosDeLaOperacion.clienteID || !datosDeLaOperacion.cryptoCode || !datosDeLaOperacion.cantidadCripto || !datosDeLaOperacion.ExchangeID) {
    referenciaToastAvisos.value?.addToast('Complete todos los campos', 'warning')
    return
  }

  estaProcesando.value = true

  if (datosDeLaOperacion.accion === 'sale') {
    const saldoSuficiente = await verificarSaldoReal()
    if (!saldoSuficiente) {
      referenciaToastAvisos.value?.addToast('Operación cancelada: El cliente no posee saldo suficiente de esta criptomoneda', 'error')
      estaProcesando.value = false
      return
    }
  }

  await ejecutarRegistro()
}

async function verificarSaldoReal() {
  try {
    const res = await fetch(`https://localhost:7241/api/Transaccion/historial?clienteId=${datosDeLaOperacion.clienteID}`)
    const historial = await res.json()

    let balance = 0
    const monedaSeleccionada = datosDeLaOperacion.cryptoCode.toLowerCase().trim()

    historial.forEach(mov => {
      const monedaEnHistorial = mov.crypto_code.toLowerCase().trim()

      if (monedaEnHistorial === monedaSeleccionada) {
        const cant = parseFloat(mov.crypto_amount)
        
        if (mov.action === 'purchase') {
          balance += cant
        } else if (mov.action === 'sale') {
          balance -= cant
        }
      }
    })

    return (balance + 0.00000001) >= datosDeLaOperacion.cantidadCripto
  } catch (e) {
    console.error("Error validando saldo:", e)
    return false
  }
}

async function ejecutarRegistro() {
  const payload = {
    clienteID: parseInt(datosDeLaOperacion.clienteID),
    cryptoCode: datosDeLaOperacion.cryptoCode.toLowerCase().trim(),
    accion: datosDeLaOperacion.accion,
    cantidadCripto: parseFloat(datosDeLaOperacion.cantidadCripto),
    fecha: datosDeLaOperacion.fecha,
    exchangeID: parseInt(datosDeLaOperacion.ExchangeID)
  }

  try {
    const res = await fetch('https://localhost:7241/api/Transaccion', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload)
    })

    if (res.ok) {
      referenciaToastAvisos.value?.addToast('¡Operación registrada con éxito!', 'success')
      reiniciarFormulario()
    } else {
      const msg = await res.text()
      referenciaToastAvisos.value?.addToast('Error: ' + msg, 'error')
    }
  } catch (e) {
    referenciaToastAvisos.value?.addToast('Error al conectar con el servidor', 'error')
  } finally {
    estaProcesando.value = false
  }
}
</script>

<style scoped>
.card { border-radius: 15px; overflow: hidden; }
.form-select, .form-control { border-radius: 8px; padding: 10px; }
.input-group-text { background-color: #f8f9fa; border-radius: 8px 0 0 8px; }
</style>