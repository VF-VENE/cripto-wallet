<template>
  <div class="modal fade" :id="id" tabindex="-1" aria-hidden="true" ref="modalRef">
    <div class="modal-dialog modal-dialog-centered" :class="sizeClass">
      <div class="modal-content border-0 shadow">
        <div class="modal-header" :class="headerClass">
          <h5 class="modal-title text-white">
            <i :class="icon" class="me-2"></i>{{ title }}
          </h5>
          <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
        </div>
        <div class="modal-body p-4">
          <slot></slot>
        </div>
        <div class="modal-footer bg-light">
          <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
            {{ cancelText }}
          </button>
          <button type="button" :class="['btn', btnClass]" @click="$emit('confirm')">
            {{ confirmText }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, defineExpose } from 'vue';
import { Modal } from 'bootstrap';

const props = defineProps({
  id: { type: String, required: true },
  title: { type: String, default: 'Confirmación' },
  icon: { type: String, default: 'bi-info-circle' },
  confirmText: { type: String, default: 'Aceptar' },
  cancelText: { type: String, default: 'Cancelar' },
  btnClass: { type: String, default: 'btn-primary' },
  headerClass: { type: String, default: 'bg-primary' },
  sizeClass: { type: String, default: '' }
});

defineEmits(['confirm']);

const modalRef = ref(null);
let modalInstance = null;

onMounted(() => {
  modalInstance = new Modal(modalRef.value);
});

const show = () => modalInstance.show();
const hide = () => modalInstance.hide();

defineExpose({ show, hide });
</script>