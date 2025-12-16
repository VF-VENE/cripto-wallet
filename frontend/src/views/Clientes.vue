<template>
  <div class="container py-5">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h2 class="mb-0">
        <i class="bi bi-people me-2"></i>
        Gestión de Clientes
      </h2>
      <button @click="presionarBotonNuevoCliente" class="btn btn-primary">
        <i class="bi bi-person-plus me-1"></i> 
        Nuevo Cliente
      </button>
    </div>

    <div class="card shadow-sm border-0 mb-4">
      <div class="card-body">
        <div class="input-group">
          <span class="input-group-text bg-white border-end-0">
            <i class="bi bi-search text-muted"></i>
          </span>
          <input 
            type="text" 
            v-model="textoABuscar" 
            class="form-control border-start-0" 
            placeholder="Buscar por nombre o email..."
          >
        </div>
      </div>
    </div>

    <div class="card shadow-sm border-0 position-relative overflow-hidden">
      <LoadingSpinner :loading="estaCargandoDatos" text="Cargando lista de clientes..." />

      <div class="table-responsive">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light">
            <tr>
              <th class="px-4">Nombre y Apellido</th>
              <th>Contacto</th>
              <th>Fecha Registro</th>
              <th class="text-center">Patrimonio (ARS)</th>
              <th class="text-center">Acciones</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="cliente in listaDeClientesFiltrados" :key="cliente.clienteID">
              <td class="px-4 fw-bold text-primary">{{ cliente.nombre }}</td>
              <td>
                <div class="small"><i class="bi bi-envelope me-1"></i>{{ cliente.email }}</div>
                <div class="small text-muted"><i class="bi bi-telephone me-1"></i>{{ cliente.telefono }}</div>
              </td>
              <td>{{ formatearFechaArgentina(cliente.fechaRegistro) }}</td>
              <td class="text-center">
                <button class="btn btn-outline-info btn-sm" @click="abrirModalDeSaldo(cliente)">
                  <i class="bi bi-wallet2 me-1"></i> Ver Saldo
                </button>
              </td>
              <td class="text-center">
                <div class="d-flex justify-content-center gap-2">
                  <button class="btn btn-outline-warning btn-sm" @click="presionarBotonEditar(cliente)">
                    <i class="bi bi-pencil"></i>
                  </button>
                  <button class="btn btn-outline-danger btn-sm" @click="presionarBotonEliminar(cliente)">
                    <i class="bi bi-trash"></i>
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <BaseModal 
      id="modalCliente" 
      ref="referenciaModalFormulario" 
      :title="tituloDelModal" 
      icon="bi-person"
      :confirmText="textoDelBotonConfirmar"
      @confirm="validarYGuardarDatos"
    >
      <form class="row g-3">
        <div class="col-12">
          <label class="form-label fw-bold">Nombre Completo</label>
          <input type="text" v-model="formularioCliente.nombre" class="form-control">
        </div>
        <div class="col-md-7">
          <label class="form-label fw-bold">Email</label>
          <input type="email" v-model="formularioCliente.email" class="form-control">
        </div>
        <div class="col-md-5">
          <label class="form-label fw-bold">Teléfono</label>
          <input type="text" v-model="formularioCliente.telefono" class="form-control">
        </div>
      </form>
    </BaseModal>

    <BaseModal 
      id="modalSaldo" 
      ref="referenciaModalSaldo" 
      :title="'Resumen de: ' + nombreClienteSaldo" 
      icon="bi-cash-stack" 
      confirmText="Cerrar" 
      @confirm="referenciaModalSaldo.hide()"
    >
      <div class="position-relative" style="min-height: 180px;">
        <LoadingSpinner :loading="estaCargandoSaldos" text="Calculando con precios de CriptoYa..." />
        
        <div v-show="listaResumenSaldos.length > 0" class="mb-4" style="max-width: 250px; margin: 0 auto;">
          <canvas id="graficoCripto"></canvas>
        </div>

        <table class="table table-sm align-middle">
          <thead>
            <tr>
              <th>Cripto</th>
              <th class="text-center">Compras</th>
              <th class="text-center">Ventas</th>
              <th class="text-end">Valor ARS</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in listaResumenSaldos" :key="item.moneda">
              <td class="fw-bold">{{ item.moneda.toUpperCase() }}</td>
              <td class="text-center text-success">{{ item.compras }}</td>
              <td class="text-center text-danger">{{ item.ventas }}</td>
              <td class="text-end fw-bold text-primary">$ {{ formatearDinero(item.montoPesos) }}</td>
            </tr>
          </tbody>
          <tfoot v-if="listaResumenSaldos.length > 0">
            <tr class="table-dark">
              <td colspan="3" class="text-end px-2">Patrimonio Total Estimado:</td>
              <td class="text-end fw-bold px-2">$ {{ formatearDinero(sumaTotalPesos) }}</td>
            </tr>
          </tfoot>
        </table>
      </div>
    </BaseModal>

    <BaseModal 
      id="modalDelete" 
      ref="referenciaModalEliminar" 
      title="Eliminar Cliente" 
      icon="bi-trash" 
      btnClass="btn-danger" 
      headerClass="bg-danger"
      @confirm="ejecutarBorradoSeguro"
    >
      <p v-if="clienteQueVamosAEliminar">¿Desea eliminar a <strong>{{ clienteQueVamosAEliminar.nombre }}</strong>?</p>
    </BaseModal>

    <ToastNotification ref="referenciaToast" />
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, nextTick } from 'vue';
import Chart from 'chart.js/auto'; // IMPORTACIÓN DE CHART.JS
import BaseModal from '../components/BaseModal.vue';
import ToastNotification from '../components/ToastNotification.vue';
import LoadingSpinner from '../components/LoadingSpinner.vue';

const referenciaToast = ref(null);
const referenciaModalFormulario = ref(null);
const referenciaModalEliminar = ref(null);
const referenciaModalSaldo = ref(null);

const listaOriginalDeClientes = ref([]);
const textoABuscar = ref('');
const estaEditandoUnCliente = ref(false); 
const estaCargandoDatos = ref(false);
const estaCargandoSaldos = ref(false);

const clienteQueVamosAEliminar = ref(null);
const nombreClienteSaldo = ref('');
const listaResumenSaldos = ref([]);

let instanciaGrafico = null;

const tituloDelModal = ref('Nuevo Cliente');
const textoDelBotonConfirmar = ref('Registrar Cliente');

const formularioCliente = reactive({
  clienteID: 0, nombre: '', email: '', telefono: '', fechaRegistro: null
});

const sumaTotalPesos = computed(function() {
  let acumulador = 0;
  listaResumenSaldos.value.forEach(function(item) {
    acumulador = acumulador + item.montoPesos;
  });
  return acumulador;
});

const listaDeClientesFiltrados = computed(function() {
  const busqueda = textoABuscar.value.toLowerCase();
  return listaOriginalDeClientes.value.filter(cliente => 
    cliente.nombre.toLowerCase().includes(busqueda) || 
    cliente.email.toLowerCase().includes(busqueda)
  );
});

async function traerClientesDesdeServidor() {
  estaCargandoDatos.value = true;
  try {
    const respuesta = await fetch("https://localhost:7241/api/Cliente");
    listaOriginalDeClientes.value = await respuesta.json();
  } finally {
    estaCargandoDatos.value = false;
  }
}

function renderizarGrafico() {
  const ctx = document.getElementById('graficoCripto');
  if (!ctx) return;

  if (instanciaGrafico) {
    instanciaGrafico.destroy();
  }

  const etiquetas = listaResumenSaldos.value.map(item => item.moneda.toUpperCase());
  const datos = listaResumenSaldos.value.map(item => item.montoPesos);

  instanciaGrafico = new Chart(ctx, {
    type: 'pie',
    data: {
      labels: etiquetas,
      datasets: [{
        data: datos,
        backgroundColor: ['#F7931A', '#627EEA', '#26A17B', '#0033AD', '#E84142', '#999999'],
        borderWidth: 1
      }]
    },
    options: {
      responsive: true,
      plugins: {
        legend: { position: 'bottom' }
      }
    }
  });
}

async function abrirModalDeSaldo(cliente) {
  nombreClienteSaldo.value = cliente.nombre;
  listaResumenSaldos.value = [];
  referenciaModalSaldo.value.show();
  estaCargandoSaldos.value = true;

  try {
    const respuesta = await fetch("https://localhost:7241/api/Transaccion/historial?clienteId=" + cliente.clienteID);
    const historial = await respuesta.json();

    const mapaDeSaldos = {};
    
    historial.forEach(function(fila) {
      const moneda = fila.crypto_code;
      const cantidad = parseFloat(fila.crypto_amount);
      const accion = fila.action;

      if (!mapaDeSaldos[moneda]) {
        mapaDeSaldos[moneda] = { compras: 0, ventas: 0 };
      }
      
      if (accion === 'purchase') {
        mapaDeSaldos[moneda].compras += cantidad;
      } else if (accion === 'sale') {
        mapaDeSaldos[moneda].ventas += cantidad;
      }
    });

    const resultados = [];
    for (const moneda in mapaDeSaldos) {
      const datos = mapaDeSaldos[moneda];
      const saldoFinal = datos.compras - datos.ventas;

      if (datos.compras > 0 || datos.ventas > 0) {
        const respuestaPrecio = await fetch("https://criptoya.com/api/satoshitango/" + moneda + "/ars/1");
        const datosPrecio = await respuestaPrecio.json();
        
        resultados.push({
          moneda: moneda,
          compras: datos.compras.toFixed(6),
          ventas: datos.ventas.toFixed(6),
          montoPesos: saldoFinal * datosPrecio.ask
        });
      }
    }
    listaResumenSaldos.value = resultados;

    await nextTick();
    renderizarGrafico();

  } finally {
    estaCargandoSaldos.value = false;
  }
}

async function validarYGuardarDatos() {
  const nombreLimpio = formularioCliente.nombre.trim();
  const emailLimpio = formularioCliente.email.trim();
  const telefonoLimpio = formularioCliente.telefono.trim();

  if (nombreLimpio === "") {
    referenciaToast.value.addToast("El nombre es obligatorio", "warning");
    return;
  }

  const regexEmail = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  if (!regexEmail.test(emailLimpio)) {
    referenciaToast.value.addToast("Formato de email inválido", "warning");
    return;
  }

  const datosAEnviar = {
    Nombre: nombreLimpio,
    Email: emailLimpio,
    Telefono: telefonoLimpio
  };

  let metodo = 'POST';
  let url = "https://localhost:7241/api/Cliente";

  if (estaEditandoUnCliente.value === true) {
    metodo = 'PUT';
    url = url + "/" + formularioCliente.clienteID;
    datosAEnviar.ClienteID = formularioCliente.clienteID;
    datosAEnviar.FechaRegistro = formularioCliente.fechaRegistro;
  }

  const respuesta = await fetch(url, {
    method: metodo,
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(datosAEnviar)
  });

  if (respuesta.ok) {
    referenciaToast.value.addToast("Guardado con éxito", "success");
    referenciaModalFormulario.value.hide();
    traerClientesDesdeServidor();
  }
}

function presionarBotonNuevoCliente() {
  estaEditandoUnCliente.value = false;
  tituloDelModal.value = "Nuevo Cliente";
  textoDelBotonConfirmar.value = "Registrar Cliente";
  formularioCliente.clienteID = 0;
  formularioCliente.nombre = '';
  formularioCliente.email = '';
  formularioCliente.telefono = '';
  formularioCliente.fechaRegistro = new Date().toISOString();
  referenciaModalFormulario.value.show();
}

function presionarBotonEditar(cliente) {
  estaEditandoUnCliente.value = true;
  tituloDelModal.value = "Editar Cliente";
  textoDelBotonConfirmar.value = "Guardar Cambios";
  formularioCliente.clienteID = cliente.clienteID;
  formularioCliente.nombre = cliente.nombre;
  formularioCliente.email = cliente.email;
  formularioCliente.telefono = cliente.telefono;
  formularioCliente.fechaRegistro = cliente.fechaRegistro;
  referenciaModalFormulario.value.show();
}

function presionarBotonEliminar(cliente) {
  clienteQueVamosAEliminar.value = cliente;
  referenciaModalEliminar.value.show();
}

async function ejecutarBorradoSeguro() {
  const respuesta = await fetch("https://localhost:7241/api/Cliente/" + clienteQueVamosAEliminar.value.clienteID, { method: 'DELETE' });

  if (respuesta.ok) {
    referenciaToast.value.addToast("Eliminado con éxito", "success");
    referenciaModalEliminar.value.hide();
    traerClientesDesdeServidor();
  } else {
    referenciaToast.value.addToast("No se puede eliminar un cliente con transacciones", "error");
  }
}

function formatearFechaArgentina(fecha) {
  return fecha ? new Date(fecha).toLocaleDateString('es-AR') : "-";
}

function formatearDinero(monto) {
  return Number(monto).toLocaleString('es-AR', { minimumFractionDigits: 2 });
}

onMounted(traerClientesDesdeServidor);
</script>