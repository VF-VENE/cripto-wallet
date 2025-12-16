import { createRouter, createWebHistory } from 'vue-router'
import Home from '../views/Home.vue'
import TransaccionesOperaciones from '../views/TransaccionesOperaciones.vue'
import TransaccionesHistorial from '../views/TransaccionesHistorial.vue'

const routes = [
  { 
    path: '/',
     component: Home
},
  { 
    path: '/clientes', 
    component: () => import('../views/Clientes.vue') 
  },
  { 
    path: '/transacciones', 
    component: () => import('../views/Transacciones.vue') 
  },
  {
    path: '/transacciones/operaciones',
    component: TransaccionesOperaciones
  },
  {
    path: '/transacciones/historial',
    component: TransaccionesHistorial
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router