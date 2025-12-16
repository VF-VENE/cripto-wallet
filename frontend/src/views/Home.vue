<template>
  <div class="container py-5">
    
    <header class="text-center mb-5">
      <h1 class="display-4 fw-bold">¡Bienvenido a CriptoWallet!</h1>
      <p class="text-muted">Seleccione una opción para continuar</p>
    </header>

    <div class="row g-4 justify-content-center">
      
      <div class="col-md-5">
        <div class="card h-100 shadow-sm border-1 tarjeta-opcion">
          <div class="card-body text-center p-5">
            <div class="espacio-icono">
              <div class="circulo-azul">
                <i class="bi bi-people-fill"></i>
              </div>
            </div>
            <h3 class="card-title fw-bold">Gestión de Clientes</h3>
            <p class="card-text text-muted">
              Administre sus clientes, edite y elimine perfiles.
            </p>
            <router-link to="/clientes" class="btn btn-primary btn-lg w-100">
              Ingresar
            </router-link>
          </div>
        </div>
      </div>

      <div class="col-md-5">
        <div class="card h-100 shadow-sm border-1 tarjeta-opcion">
          <div class="card-body text-center p-5">
            <div class="espacio-icono">
              <div class="circulo-naranja">
                <i class="bi bi-currency-bitcoin"></i>
              </div>
            </div>
            <h3 class="card-title fw-bold">Transacciones</h3>
            <p class="card-text text-muted">
              Consulte el historial de movimientos y genere una nueva operación.
            </p>
            <router-link to="/transacciones" class="btn btn-success btn-lg w-100">
              Ver Movimientos
            </router-link>
          </div>
        </div>
      </div>

    </div>
  </div>
</template>

<style scoped>
.tarjeta-opcion {
  border-radius: 15px;
  transition: transform 0.2s ease;
}

.tarjeta-opcion:hover {
  transform: translateY(-5px);
}

.espacio-icono {
  margin-bottom: 20px;
}

.circulo-azul {
  width: 80px;
  height: 80px;
  background-color: #E7F0FF;
  color: #0D6EFD;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto;
  font-size: 2.5rem;
}

.circulo-naranja {
  width: 80px;
  height: 80px;
  background-color: #FFF4E5;
  color: #F7931A;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto;
  font-size: 3rem;
}

.bi-currency-bitcoin, .bi-people-fill {
  transition: transform 0.2s ease;
}

.tarjeta-opcion:hover .bi-currency-bitcoin,
.tarjeta-opcion:hover .bi-people-fill {
  transform: scale(1.1);
}
</style>