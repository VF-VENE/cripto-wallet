<template>
  <div class="container py-5">
    
    <div class="contenedor-boton-volver mb-4">
      <router-link to="/" class="btn btn-outline-secondary btn-sm">
        <i class="bi bi-arrow-left me-1"></i> 
        Ir a la Pantalla de Inicio
      </router-link>
    </div>

    <header class="cabecera-principal text-center mb-5">
      <h1 class="titulo-gigante fw-bold text-success">
        Panel de Control de Transacciones
      </h1>
      <p class="subtitulo-ayuda text-muted">
        ¿Qué desea hacer hoy? Seleccione una de las opciones de abajo para continuar.
      </p>
    </header>

    <div class="row g-4 justify-content-center">
      
      <div class="col-md-5">
        <div class="tarjeta-opcion card h-100 shadow-sm border-success">
          <div class="card-body text-center p-5">
            
            <div class="contenedor-icono mb-4 text-success">
              <i class="bi bi-plus-circle-dotted" style="font-size: 3.5rem;"></i>
            </div>

            <h3 class="titulo-tarjeta">Registrar Operación</h3>
            
            <p class="descripcion-tarjeta text-muted">
              Use esta opción para cargar una nueva compra o una nueva venta de criptomonedas.
            </p>

            <router-link to="/transacciones/operaciones" class="btn btn-success btn-lg w-100">
              Ir a Cargar Operación
            </router-link>

          </div>
        </div>
      </div>

      <div class="col-md-5">
        <div class="tarjeta-opcion card h-100 shadow-sm border-info">
          <div class="card-body text-center p-5">
            
            <div class="contenedor-icono mb-4 text-info">
              <i class="bi bi-journal-text" style="font-size: 3.5rem;"></i>
            </div>

            <h3 class="titulo-tarjeta">Consultar Historial</h3>
            
            <p class="descripcion-tarjeta text-muted">
              Use esta opción para ver la lista de movimientos ya registrados.
            </p>

            <router-link to="/transacciones/historial" class="btn btn-info btn-lg w-100 text-white">
              Ir al Listado Histórico
            </router-link>

          </div>
        </div>
      </div>

    </div>
  </div>
</template>

<script setup>
</script>

<style scoped>

.tarjeta-opcion {
  transition: transform 0.3s ease, box-shadow 0.3s ease;
  cursor: pointer;
  border-width: 2px;
}

.tarjeta-opcion:hover {
  transform: translateY(-10px);
  box-shadow: 0 15px 30px rgba(0, 0, 0, 0.15) !important;
}

.btn-info {
  background-color: #0dcaf0;
  border: none;
}

.btn-lg {
  padding-top: 15px;
  padding-bottom: 15px;
  font-weight: bold;
}
</style>