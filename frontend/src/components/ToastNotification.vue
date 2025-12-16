<template>
  <div class="toast-container position-fixed bottom-0 end-0 p-3">
    <div 
      v-for="toast in toasts" 
      :key="toast.id"
      class="toast show align-items-center text-white border-0 mb-2 shadow"
      :class="getBgClass(toast.type)"
      role="alert" 
      aria-live="assertive" 
      aria-atomic="true"
    >
      <div class="d-flex">
        <div class="toast-body">
          <i :class="getIcon(toast.type)" class="me-2"></i>
          {{ toast.message }}
        </div>
        <button 
          type="button" 
          class="btn-close btn-close-white me-2 m-auto" 
          @click="removeToast(toast.id)"
        ></button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';

const toasts = ref([]);
let nextId = 0;

const addToast = (message, type = 'success') => {
  const id = nextId++;
  toasts.value.push({ id, message, type });

  // Auto-eliminar después de 4 segundos
  setTimeout(() => {
    removeToast(id);
  }, 4000);
};

const removeToast = (id) => {
  toasts.value = toasts.value.filter(t => t.id !== id);
};

const getBgClass = (type) => {
  return {
    'bg-success': type === 'success',
    'bg-danger': type === 'error',
    'bg-warning text-dark': type === 'warning',
    'bg-info': type === 'info'
  };
};

const getIcon = (type) => {
  return {
    'bi bi-check-circle-fill': type === 'success',
    'bi bi-exclamation-octagon-fill': type === 'error',
    'bi bi-exclamation-triangle-fill': type === 'warning',
    'bi bi-info-circle-fill': type === 'info'
  };
};

// Exponemos la función para que el padre pueda usarla
defineExpose({ addToast });
</script>

<style scoped>
.toast-container {
  z-index: 1060;
}
.toast {
  min-width: 250px;
  transition: opacity 0.3s ease;
}
</style>