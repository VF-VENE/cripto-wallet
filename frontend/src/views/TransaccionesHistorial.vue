<template>
  <div class="container py-5">
    
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h2 class="mb-0">
        <i class="bi bi-journal-text me-2 text-primary"></i>
        Historial de Movimientos
      </h2>
      <button @click="presionarBotonVolverAtras" class="btn btn-outline-secondary btn-sm shadow-sm">
        <i class="bi bi-arrow-left"></i> 
        Regresar al Menú
      </button>
    </div>

    <div class="card shadow-sm border-0 mb-4 bg-light">
      <div class="card-body p-4">
        <div class="row align-items-end g-3">
          <div class="col-md-8">
            <label class="form-label fw-bold">Filtrar por Cliente</label>
            <div class="input-group">
              <span class="input-group-text bg-white border-end-0">
                <i class="bi bi-person-filter text-muted"></i>
              </span>
              <select v-model="identificadorDelClienteSeleccionado" class="form-select border-start-0">
                <option :value="null">-- Todos los Clientes --</option>
                <option v-for="c in listaDeClientes" :key="c.clienteID" :value="c.clienteID">
                  {{ c.nombre }}
                </option>
              </select>
            </div>
          </div>
          <div class="col-md-4">
            <button @click="traerTransaccionesDelServidor" class="btn btn-primary w-100 shadow-sm py-2" :disabled="estaCargandoInformacion">
              <span v-if="estaCargandoInformacion" class="spinner-border spinner-border-sm me-2"></span>
              <i v-else class="bi bi-search me-2"></i>
              Actualizar Historial
            </button>
          </div>
        </div>
      </div>
    </div>

    <div class="card shadow-sm border-0 overflow-hidden">
      <div class="table-responsive">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light">
            <tr>
              <th class="ps-4 py-3">Fecha y Hora</th>
              <th class="py-3">Cripto</th>
              <th class="py-3">Tipo</th>
              <th class="py-3 text-end">Cantidad</th>
              <th class="py-3 text-end">Monto Final</th>
              <th class="text-center py-3 pe-4">Acciones</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="t in listaDeTransacciones" :key="t.id">
              <td class="ps-4 small text-muted">{{ convertirFechaAFormatoArgentino(t.datetime) }}</td>
              <td>
                <div class="d-flex align-items-center">
                  <div class="circulo-icono-xs me-2">
                    <i class="bi bi-coin"></i>
                  </div>
                  <span class="fw-bold">{{ t.crypto_code.toUpperCase() }}</span>
                </div>
              </td>
              <td>
                <span v-if="t.action === 'purchase'" class="badge badge-soft-success">
                  <i class="bi bi-arrow-down-left-circle me-1"></i>Compra
                </span>
                <span v-else class="badge badge-soft-danger">
                  <i class="bi bi-arrow-up-right-circle me-1"></i>Venta
                </span>
              </td>
              <td class="text-end font-monospace">{{ t.crypto_amount }}</td>
              <td class="text-end fw-bold text-dark">
                $ {{ convertirMontoAFormatoMoneda(t.money) }}
              </td>
              <td class="text-center pe-4">
                <div class="d-flex justify-content-center gap-2">
                  <button class="btn btn-outline-info btn-sm" title="Ver" @click="mostrarDetallesDeLaTransaccion(t)">
                    <i class="bi bi-eye"></i>
                  </button>
                  <button class="btn btn-outline-warning btn-sm" title="Editar" @click="abrirFormularioDeEdicion(t)">
                    <i class="bi bi-pencil"></i>
                  </button>
                  <button class="btn btn-outline-danger btn-sm" title="Eliminar" @click="prepararEliminacionDeRegistro(t.id)">
                    <i class="bi bi-trash"></i>
                  </button>
                </div>
              </td>
            </tr>

            <tr v-if="listaDeTransacciones.length === 0 && !estaCargandoInformacion">
              <td colspan="6" class="text-center py-5">
                <div class="py-4">
                  <i class="bi bi-inbox fs-1 text-muted d-block mb-3"></i>
                  <p class="text-muted fw-bold">No se encontraron movimientos</p>
                  <small class="text-muted">Intente cambiar el filtro de cliente o actualizar.</small>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <BaseModal id="modalDetalles" ref="referenciaModalVer" title="Detalles de la Operación" icon="bi-info-circle" confirmText="Entendido" @confirm="referenciaModalVer.hide()">
      <div v-if="registroParaMostrar" class="p-2">
        <div class="list-group list-group-flush border rounded">
          <div class="list-group-item d-flex justify-content-between align-items-center py-3">
            <span class="text-muted"><i class="bi bi-person me-2"></i>Cliente</span>
            <span class="fw-bold">{{ registroParaMostrar.nombreCliente }}</span>
          </div>
          <div class="list-group-item d-flex justify-content-between align-items-center py-3">
            <span class="text-muted"><i class="bi bi-currency-bitcoin me-2"></i>Activo</span>
            <span class="badge bg-primary px-3">{{ registroParaMostrar.crypto_code.toUpperCase() }}</span>
          </div>
          <div class="list-group-item d-flex justify-content-between align-items-center py-3">
            <span class="text-muted"><i class="bi bi-cash-stack me-2"></i>Monto Invertido</span>
            <span class="text-success fw-bold fs-5">$ {{ convertirMontoAFormatoMoneda(registroParaMostrar.money) }}</span>
          </div>
          <div class="list-group-item d-flex justify-content-between align-items-center py-3">
            <span class="text-muted"><i class="bi bi-building me-2"></i>Exchange</span>
            <span class="badge bg-secondary px-3">{{ registroParaMostrar.nombreExchange }}</span>
          </div>
        </div>
      </div>
    </BaseModal>

    <BaseModal id="modalEditar" ref="referenciaModalEditar" title="Editar Transacción" icon="bi-pencil-square" btnClass="btn-primary" @confirm="validarYGuardarEdicion">
      <div class="row g-4 p-2">
        <div class="col-12">
          <div class="alert alert-warning border-0 shadow-sm small d-flex align-items-center">
            <i class="bi bi-exclamation-triangle-fill fs-4 me-3"></i>
            <div>
              <strong>Aviso:</strong> Esta edición es directa en base de datos. No se realizarán validaciones de saldo ni recalculo automático de precios.
            </div>
          </div>
        </div>
        <div class="col-12">
          <label class="form-label fw-bold">Cantidad de Cripto</label>
          <div class="input-group">
            <span class="input-group-text bg-white"><i class="bi bi-123"></i></span>
            <input type="number" step="0.00000001" v-model.number="datosParaEditar.crypto_amount" class="form-control" placeholder="0.00">
          </div>
        </div>
        <div class="col-12">
          <label class="form-label fw-bold">Fecha y Hora</label>
          <div class="input-group">
            <span class="input-group-text bg-white"><i class="bi bi-calendar-event"></i></span>
            <input type="datetime-local" v-model="datosParaEditar.datetime" class="form-control">
          </div>
        </div>
      </div>
    </BaseModal>

    <BaseModal id="modalBorrar" ref="referenciaModalBorrar" title="Confirmar Eliminación" icon="bi-exclamation-triangle" btnClass="btn-danger" headerClass="bg-danger" @confirm="borrarRegistroDefinitivamente">
      <div class="text-center py-3">
        <div class="mb-3">
          <i class="bi bi-trash text-danger" style="font-size: 3rem;"></i>
        </div>
        <p class="mb-1 fw-bold fs-5">¿Está seguro de eliminar este registro?</p>
        <p class="text-muted small">Esta acción no se puede deshacer y afectará los saldos históricos del cliente.</p>
      </div>
    </BaseModal>

    <ToastNotification ref="referenciaToastAvisos" />
  </div>
</template>

<style scoped>
/* Estilos unificados con Clientes.vue */
.table thead th {
  font-size: 0.75rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  color: #6c757d;
  border-top: none;
}

.card {
  border-radius: 15px;
}

.circulo-icono-xs {
  width: 28px;
  height: 28px;
  background-color: #f0f4f8;
  color: #F7931A;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.8rem;
}

/* Badges suaves para mejor estética */
.badge-soft-success {
  background-color: #d1e7dd;
  color: #0f5132;
  padding: 0.5em 0.8em;
  border-radius: 6px;
}

.badge-soft-danger {
  background-color: #f8d7da;
  color: #842029;
  padding: 0.5em 0.8em;
  border-radius: 6px;
}

.font-monospace {
  font-family: 'SFMono-Regular', Consolas, 'Liberation Mono', Menlo, monospace;
  font-size: 0.9rem;
}

.btn-group .btn {
  padding: 0.4rem 0.6rem;
}
</style>

<script setup>
/* ... (Lógica de script se mantiene igual ya que es funcional) ... */
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import BaseModal from '../components/BaseModal.vue'
import ToastNotification from '../components/ToastNotification.vue'

const herramientaRouter = useRouter()
const referenciaToastAvisos = ref(null)
const referenciaModalVer = ref(null)
const referenciaModalEditar = ref(null)
const referenciaModalBorrar = ref(null)

const estaCargandoInformacion = ref(false)
const identificadorDelClienteSeleccionado = ref(null)
const listaDeClientes = ref([])
const listaDeTransacciones = ref([])
const registroParaMostrar = ref(null)
const identificadorParaBorrar = ref(null)

const datosParaEditar = reactive({ 
  id: null, 
  datetime: '', 
  crypto_amount: 0 
})

function presionarBotonVolverAtras() {
  herramientaRouter.back()
}

async function traerClientesDelServidor() {
  try {
    const respuesta = await fetch("https://localhost:7241/api/Cliente")
    listaDeClientes.value = await respuesta.json()
  } catch (error) {
    referenciaToastAvisos.value.addToast("Error al cargar lista de clientes", "error")
  }
}

async function traerTransaccionesDelServidor() {
  estaCargandoInformacion.value = true
  let direccionUrl = "https://localhost:7241/api/Transaccion/historial"
  
  if (identificadorDelClienteSeleccionado.value !== null) {
    direccionUrl += `?clienteId=${identificadorDelClienteSeleccionado.value}`
  }

  try {
    const respuesta = await fetch(direccionUrl)
    if (!respuesta.ok) throw new Error()
    listaDeTransacciones.value = await respuesta.json()
  } catch (error) {
    referenciaToastAvisos.value.addToast("Error al obtener el historial", "error")
  } finally {
    estaCargandoInformacion.value = false
  }
}

function mostrarDetallesDeLaTransaccion(objetoTransaccion) {
  registroParaMostrar.value = objetoTransaccion
  referenciaModalVer.value.show()
}

function abrirFormularioDeEdicion(objetoTransaccion) {
  datosParaEditar.id = objetoTransaccion.id
  datosParaEditar.crypto_amount = objetoTransaccion.crypto_amount
  datosParaEditar.datetime = prepararFechaParaCalendario(objetoTransaccion.datetime)
  referenciaModalEditar.value.show()
}

function validarYGuardarEdicion() {
  if (datosParaEditar.crypto_amount <= 0) {
    referenciaToastAvisos.value.addToast("La cantidad debe ser mayor a 0", "warning")
    return
  }
  if (!datosParaEditar.datetime) {
    referenciaToastAvisos.value.addToast("Debe seleccionar una fecha", "warning")
    return
  }
  guardarCambiosEditados()
}

async function guardarCambiosEditados() {
  const urlDeEdicion = `https://localhost:7241/api/Transaccion/${datosParaEditar.id}`
  const payload = {
    datetime: datosParaEditar.datetime,
    crypto_amount: datosParaEditar.crypto_amount
  }

  try {
    const respuesta = await fetch(urlDeEdicion, {
      method: 'PATCH',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload)
    })

    if (respuesta.ok) {
      referenciaModalEditar.value.hide()
      referenciaToastAvisos.value.addToast("Registro actualizado correctamente", "success")
      traerTransaccionesDelServidor()
    } else {
      throw new Error()
    }
  } catch (error) {
    referenciaToastAvisos.value.addToast("No se pudo actualizar el registro", "error")
  }
}

function prepararEliminacionDeRegistro(idParaBorrar) {
  identificadorParaBorrar.value = idParaBorrar
  referenciaModalBorrar.value.show()
}

async function borrarRegistroDefinitivamente() {
  const urlDeBorrado = `https://localhost:7241/api/Transaccion/${identificadorParaBorrar.value}`

  try {
    const respuesta = await fetch(urlDeBorrado, { method: 'DELETE' })
    if (respuesta.ok) {
      referenciaModalBorrar.value.hide()
      referenciaToastAvisos.value.addToast("Transacción eliminada", "success")
      traerTransaccionesDelServidor()
    } else {
      throw new Error()
    }
  } catch (error) {
    referenciaToastAvisos.value.addToast("Error al eliminar el registro", "error")
  }
}

function prepararFechaParaCalendario(fechaOriginal) {
  const fechaObjeto = new Date(fechaOriginal)
  const diferenciaMilisegundos = fechaObjeto.getTimezoneOffset() * 60000
  return new Date(fechaObjeto - diferenciaMilisegundos).toISOString().slice(0, 16)
}

function convertirFechaAFormatoArgentino(fechaISO) {
  if (!fechaISO) return "-"
  return new Date(fechaISO).toLocaleString('es-AR')
}

function convertirMontoAFormatoMoneda(numeroMonto) {
  return Number(numeroMonto).toLocaleString('es-AR', { 
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  })
}

onMounted(() => {
  traerClientesDelServidor()
  traerTransaccionesDelServidor()
})
</script>