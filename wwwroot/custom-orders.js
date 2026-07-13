(function() {
  let isInitializing = false;

  function initOrders() {
    if (isInitializing) return;

    // Buscar el encabezado de "Pedidos Recientes"
    const headers = Array.from(document.querySelectorAll('h3'));
    const pedidosHeader = headers.find(h => h.innerText === 'Pedidos Recientes');
    if (!pedidosHeader) return;

    // Evitar bucles infinitos por el MutationObserver
    if (pedidosHeader.dataset.ordersInjected) return;
    
    isInitializing = true;
    pedidosHeader.dataset.ordersInjected = "true";

    // Encontrar el contenedor de la lista de pedidos (hermano del h3)
    const listContainer = pedidosHeader.nextElementSibling;
    if (!listContainer || !listContainer.classList.contains('space-y-3')) {
        isInitializing = false;
        return;
    }

    // Mostrar estado de carga
    listContainer.innerHTML = '<p class="text-stone-500 italic p-4 text-center">Cargando pedidos en tiempo real...</p>';

    const apiBase = window.API_BASE_URL || '';
    fetch(apiBase + '/api/orders')
      .then(res => res.json())
      .then(orders => {
         listContainer.innerHTML = '';
         
         if (!orders || orders.length === 0) {
             listContainer.innerHTML = '<p class="text-stone-500 italic p-4 text-center">No hay pedidos recientes.</p>';
             isInitializing = false;
             return;
         }

         // Ordenar por fecha descendente
         orders.sort((a, b) => new Date(b.date) - new Date(a.date));
         const recentOrders = orders.slice(0, 10); // Mostrar los últimos 10

         recentOrders.forEach(order => {
             const div = document.createElement('div');
             div.className = "flex items-center justify-between p-4 bg-amber-50 rounded-xl cursor-pointer hover:bg-amber-100 transition-colors shadow-sm";
             
             // Formatear fecha
             const d = new Date(order.date);
             const dateStr = !isNaN(d.getTime()) ? d.toLocaleDateString("es-MX") : order.date;
             
             // Mapeo de estados
             const statusMap = {
                 'delivered': 'Entregado',
                 'shipping': 'En Camino',
                 'preparing': 'Preparando',
                 'packed': 'Empacado',
                 'received': 'Recibido'
             };
             const displayStatus = statusMap[order.status] || order.status;
             
             div.innerHTML = `
                <div class="flex items-center space-x-3 pointer-events-none">
                    <div class="w-10 h-10 bg-amber-100 rounded-lg flex items-center justify-center">
                        <svg class="w-5 h-5 text-amber-600" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                           <circle cx="9" cy="21" r="1"></circle>
                           <circle cx="20" cy="21" r="1"></circle>
                           <path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"></path>
                        </svg>
                    </div>
                    <div>
                        <p class="font-semibold text-stone-800 text-sm">${order.id || 'N/A'}</p>
                        <p class="text-xs text-stone-500">${dateStr}</p>
                    </div>
                </div>
                <div class="text-right pointer-events-none">
                    <p class="font-semibold text-stone-800">$${(order.total || 0).toFixed(2)}</p>
                    <p class="text-xs ${order.status === 'delivered' ? 'text-green-600' : 'text-amber-600'} font-medium">${displayStatus}</p>
                </div>
             `;
             
             div.addEventListener('click', () => openOrderModal(order));
             listContainer.appendChild(div);
         });
         
         isInitializing = false;
      })
      .catch(err => {
         console.error("Error fetching orders:", err);
         listContainer.innerHTML = '<p class="text-red-500 italic p-4 text-center">Error al cargar pedidos. Verifica tu conexión.</p>';
         isInitializing = false;
      });
  }

  function openOrderModal(order) {
      // Crear overlay
      const overlay = document.createElement('div');
      overlay.className = "fixed inset-0 bg-black/60 flex items-center justify-center z-50 p-4 transition-opacity backdrop-blur-sm";
      overlay.id = "custom-order-modal";
      
      const modal = document.createElement('div');
      modal.className = "bg-white rounded-2xl w-full max-w-md shadow-2xl overflow-hidden transform transition-all";
      
      const statusMap = {
          'delivered': 'Entregado',
          'shipping': 'En Camino',
          'preparing': 'Preparando',
          'packed': 'Empacado',
          'received': 'Recibido'
      };
      const displayStatus = statusMap[order.status] || order.status;
      
      let itemsHtml = '<p class="text-gray-500 text-sm italic">Sin productos detallados.</p>';
      if (order.items && order.items.length > 0) {
          itemsHtml = order.items.map(item => `
              <div class="flex justify-between text-sm py-2 border-b border-gray-100 last:border-0">
                  <span class="text-gray-700"><span class="font-semibold">${item.quantity}x</span> ${item.productName || 'Producto'}</span>
                  <span class="font-semibold text-stone-800">$${(item.price * item.quantity).toFixed(2)}</span>
              </div>
          `).join('');
      }
      
      modal.innerHTML = `
          <div class="bg-amber-500 p-5 flex justify-between items-center text-white">
              <h3 class="text-lg font-bold flex items-center gap-2">
                  <svg class="w-5 h-5" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M14 2H6a2 2 0 0 0-2 2v16c0 1.1.9 2 2 2h12a2 2 0 0 0 2-2V8l-6-6z"></path><path d="M14 3v5h5M16 13H8M16 17H8M10 9H8"></path></svg>
                  Detalle de Venta
              </h3>
              <button id="close-modal-btn" class="text-white hover:text-amber-100 transition-colors">
                  <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path></svg>
              </button>
          </div>
          <div class="p-6">
              <div class="bg-amber-50 rounded-xl p-4 mb-6 border border-amber-100 flex items-center space-x-4">
                  <div class="w-12 h-12 bg-white rounded-full flex items-center justify-center text-amber-600 font-bold text-xl shadow-sm">
                      ${(order.customerName || 'U').charAt(0).toUpperCase()}
                  </div>
                  <div>
                      <p class="text-xs text-stone-500 uppercase font-semibold tracking-wide">Cliente del Pedido</p>
                      <p class="text-lg font-bold text-stone-900">${order.customerName || 'Cliente Genérico'}</p>
                  </div>
              </div>
              
              <div class="grid grid-cols-2 gap-4 mb-6">
                  <div class="bg-stone-50 p-3 rounded-lg">
                      <p class="text-xs text-stone-500 uppercase font-semibold mb-1">ID del Pedido</p>
                      <p class="font-bold text-stone-800">${order.id || 'N/A'}</p>
                  </div>
                  <div class="bg-stone-50 p-3 rounded-lg">
                      <p class="text-xs text-stone-500 uppercase font-semibold mb-1">Fecha</p>
                      <p class="font-bold text-stone-800">${order.date ? new Date(order.date).toLocaleDateString("es-MX") : 'N/A'}</p>
                  </div>
                  <div class="bg-stone-50 p-3 rounded-lg">
                      <p class="text-xs text-stone-500 uppercase font-semibold mb-1">Estado</p>
                      <span class="px-2.5 py-1 rounded-full text-xs font-bold shadow-sm ${order.status === 'delivered' ? 'bg-green-500 text-white' : 'bg-amber-400 text-amber-900'}">${displayStatus}</span>
                  </div>
                  <div class="bg-stone-50 p-3 rounded-lg">
                      <p class="text-xs text-stone-500 uppercase font-semibold mb-1">Total</p>
                      <p class="font-bold text-amber-600 text-lg">$${(order.total || 0).toFixed(2)}</p>
                  </div>
              </div>
              
              <div class="mb-8">
                  <p class="text-sm font-bold text-stone-800 mb-3 flex items-center gap-2">
                      <svg class="w-4 h-4 text-stone-500" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M6 2L3 6v14c0 1.1.9 2 2 2h14a2 2 0 0 0 2-2V6l-3-4H6zM3 6h18M16 10a4 4 0 0 1-8 0"></path></svg>
                      Productos Solicitados
                  </p>
                  <div class="bg-stone-50 p-4 rounded-xl border border-stone-100 shadow-inner max-h-[150px] overflow-y-auto">
                      ${itemsHtml}
                  </div>
              </div>
              
              <div class="flex space-x-3">
                  ${order.status !== 'delivered' ? `
                      <button id="mark-delivered-btn" class="flex-1 bg-green-500 text-white py-3 rounded-xl font-bold hover:bg-green-600 transition-colors shadow-md hover:shadow-lg flex items-center justify-center gap-2">
                          <svg class="w-5 h-5" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"></path><polyline points="22 4 12 14.01 9 11.01"></polyline></svg>
                          Pedido Entregado
                      </button>
                  ` : `
                      <button disabled class="flex-1 bg-gray-100 text-gray-400 py-3 rounded-xl font-bold cursor-not-allowed flex items-center justify-center gap-2 border border-gray-200">
                          <svg class="w-5 h-5" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"></path><polyline points="22 4 12 14.01 9 11.01"></polyline></svg>
                          Ya Entregado
                      </button>
                  `}
                  <button id="close-modal-btn-2" class="px-6 bg-stone-100 text-stone-700 py-3 rounded-xl font-bold hover:bg-stone-200 transition-colors border border-stone-200">
                      Cerrar
                  </button>
              </div>
          </div>
      `;
      
      overlay.appendChild(modal);
      document.body.appendChild(overlay);
      
      // Eventos
      const closeFn = () => document.body.removeChild(overlay);
      modal.querySelector('#close-modal-btn').addEventListener('click', closeFn);
      modal.querySelector('#close-modal-btn-2').addEventListener('click', closeFn);
      
      // Cerrar al clickear afuera
      overlay.addEventListener('click', (e) => {
          if (e.target === overlay) closeFn();
      });
      
      const deliverBtn = modal.querySelector('#mark-delivered-btn');
      if (deliverBtn) {
          deliverBtn.addEventListener('click', () => {
              deliverBtn.disabled = true;
              deliverBtn.innerHTML = "Actualizando...";
              
              const apiBase = window.API_BASE_URL || '';
              fetch(apiBase + '/api/orders/' + order.id + '/status', {
                  method: 'PUT',
                  headers: { 'Content-Type': 'application/json' },
                  body: JSON.stringify({ status: 'delivered' })
              })
              .then(res => {
                  if(!res.ok) throw new Error("Error al actualizar");
                  closeFn();
                  // Re-render orders
                  const header = document.querySelector('[data-orders-injected]');
                  if (header) delete header.dataset.ordersInjected;
                  initOrders();
              })
              .catch(err => {
                  console.error(err);
                  alert("No se pudo actualizar el estado del pedido.");
                  deliverBtn.disabled = false;
                  deliverBtn.innerHTML = "Pedido Entregado";
              });
          });
      }
  }

  // Usar MutationObserver para detectar navegación en la SPA
  const observer = new MutationObserver(() => {
     initOrders();
  });
  
  observer.observe(document.body, { childList: true, subtree: true });
  
  if (document.readyState === 'loading') {
      window.addEventListener('DOMContentLoaded', () => setTimeout(initOrders, 500));
  } else {
      setTimeout(initOrders, 500);
  }
})();
