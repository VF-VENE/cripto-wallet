<template>
  <div v-if="loading" class="spinner-overlay d-flex flex-column align-items-center justify-content-center">
    <div class="spinner-border text-primary" role="status" :style="{ width: size, height: size }">
      <span class="visually-hidden">Cargando...</span>
    </div>
    <p v-if="text" class="mt-2 fw-bold text-primary">{{ text }}</p>
  </div>
</template>

<script setup>
defineProps({
  loading: { type: Boolean, default: false },
  text: { type: String, default: 'Cargando datos...' },
  size: { type: String, default: '3rem' }
});
</script>

<style scoped>
.spinner-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(255, 255, 255, 0.7);
  z-index: 1000;
  border-radius: inherit;
}
</style>