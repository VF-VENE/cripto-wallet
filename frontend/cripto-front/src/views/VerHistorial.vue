<template>
  <div class="p-4">
    <h2 class="text-xl font-bold mb-4">Historial de Transacciones</h2>

    <div class="mb-4">
      <label for="cliente" class="mr-2">Seleccionar Cliente:</label>
      <select v-model="clienteSeleccionado" id="cliente" class="border p-1">
        <option disabled value="">-- Selecciona un cliente --</option>
        <option v-for="cliente in clientes" :key="cliente.clienteID" :value="cliente.clienteID">
        {{ cliente.nombre }}
        </option>

      </select>
      <button @click="buscarTransacciones" class="ml-2 bg-blue-500 text-white px-3 py-1 rounded">
        Buscar
      </button>
    </div>

    <table v-if="transacciones.length > 0" class="min-w-full border border-gray-300">
      <thead class="bg-gray-100">
        <tr>
          <th class="border px-2 py-1">Fecha</th>
          <th class="border px-2 py-1">Cliente</th>
          <th class="border px-2 py-1">Crypto</th>
          <th class="border px-2 py-1">Acción</th>
          <th class="border px-2 py-1">Cantidad</th>
          <th class="border px-2 py-1">Monto</th>
          <th class="border px-2 py-1">Exchange</th>
          <th class="border px-2 py-1">Acciones</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="t in transacciones" :key="t.id">
          <td class="border px-2 py-1">{{ new Date(t.datetime).toLocaleString() }}</td>
          <td class="border px-2 py-1">{{ t.nombreCliente }}</td>
          <td class="border px-2 py-1">{{ t.crypto_code }}</td>
          <td class="border px-2 py-1">{{ t.action }}</td>
          <td class="border px-2 py-1">{{ t.crypto_amount }}</td>
          <td class="border px-2 py-1">{{ t.money }}</td>
          <td class="border px-2 py-1">{{ t.nombreExchange }}</td>
          <td class="border px-2 py-1">
            <button class="text-blue-500 hover:underline" @click="verTransaccion(t.id)">Ver</button>
            <button class="text-yellow-600 hover:underline ml-2" @click="editarTransaccion(t.id)">Editar</button>
            <button class="text-red-500 hover:underline ml-2" @click="eliminarTransaccion(t.id)">Eliminar</button>
          </td>
        </tr>
      </tbody>
    </table>

    <p v-else class="text-gray-500">No hay transacciones para mostrar.</p>
  </div>
</template>

<script>
export default {
  data() {
    return {
      clienteSeleccionado: "",
      clientes: [],
      transacciones: [],
    };
  },
  methods: {
    async buscarTransacciones() {
      if (!this.clienteSeleccionado) return;
      try {
        const res = await fetch(`https://localhost:7241/api/Transaccion/historial?clienteId=${this.clienteSeleccionado}`);
        this.transacciones = await res.json();
      } catch (error) {
        console.error("Error al buscar transacciones:", error);
      }
    },
    async cargarClientes() {
      try {
        const res = await fetch("https://localhost:7241/api/Cliente");
        this.clientes = await res.json();
      } catch (error) {
        console.error("Error al cargar clientes:", error);
      }
    },
    verTransaccion(id) {
      alert(`Ver transacción ${id}`);
    },
    editarTransaccion(id) {
      alert(`Editar transacción ${id}`);
    },
    eliminarTransaccion(id) {
      alert(`Eliminar transacción ${id}`);
    },
  },
  mounted() {
    this.cargarClientes();
  },
};
</script>

<style scoped>
  table {
    width: 100%;
    border-collapse: collapse; /* Une bordes para evitar doble borde */
  }

  thead {
    background-color: #f3f4f6; /* Gris claro */
  }

  th, td {
    border: 1px solid #ccc; /* Bordes grises */
    padding: 8px 12px;
    text-align: left;
  }

  tbody tr:nth-child(even) {
    background-color: #f9fafb; /* Color de fila alternada */
  }

  tbody tr:hover {
    background-color: #e2e8f0; /* Resalta fila al pasar mouse */
  }

  th {
    font-weight: 600;
    color: #374151; /* Gris oscuro */
  }

  button {
    background: none;
    border: none;
    cursor: pointer;
    color: #2563eb; /* Azul */
  }

  button:hover {
    text-decoration: underline;
  }

  /* Opcional: separa botones */
  button + button {
    margin-left: 10px;
  }
</style>



