<template>
  <nav class="navbar navbar-expand-lg navbar-dark bg-dark mb-4">
    <div class="container">
      <router-link class="navbar-brand" hide-after-click to="/">
        <i class="bi bi-currency-bitcoin"></i> CriptoWallet
      </router-link>
      <div class="navbar-nav">
        <router-link class="nav-link" to="/">Inicio</router-link>
        <router-link class="nav-link" to="/clientes">Clientes</router-link>
        <router-link class="nav-link" to="/transacciones">Transacciones</router-link>
      </div>
    </div>
  </nav>

  <main>
    <router-view></router-view>
  </main>
</template>

<style>
/* Estilos globales (puedes quitar el 'scoped' aquí) */
body {
  background-color: #f8f9fa;
}

/* Animación opcional para las transiciones de página */
.fade-enter-active, .fade-leave-active {
  transition: opacity 0.5s ease;
}
.fade-enter-from, .fade-leave-to {
  opacity: 0;
}
</style>