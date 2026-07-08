(function() {
  let isInitializing = false;

  function initReviews() {
    if (isInitializing) return;
    
    // Check if we are on a product page
    const match = window.location.pathname.match(/\/app\/products\/([^/]+)/);
    if (!match) return;
    
    const productId = match[1];

    // Find the rating header by searching for the text "(X reseñas)"
    const spans = Array.from(document.querySelectorAll('span'));
    let ratingHeader = null;
    for (const span of spans) {
      if (span.innerText && span.innerText.match(/\(\d+ reseña/)) {
        ratingHeader = span.parentElement;
        break;
      }
    }

    if (!ratingHeader) return;

    // To prevent infinite loops with MutationObserver, mark that we've injected
    if (ratingHeader.dataset.reviewsInjected) return;
    
    isInitializing = true;
    ratingHeader.dataset.reviewsInjected = "true";

    // Build the interactive stars
    fetch(`/api/products/${productId}/reviews`)
      .then(res => res.json())
      .then(reviews => {
        let avgRating = 5;
        if (reviews && reviews.length > 0) {
            avgRating = reviews.reduce((acc, r) => acc + r.rating, 0) / reviews.length;
        }

        renderStarsHeader(ratingHeader, avgRating, reviews ? reviews.length : 0);
        
        // Find a place to inject the Reviews Section.
        // It's a 2-column grid. The container of the grid is what we want.
        let mainContainer = document.querySelector('.grid.grid-cols-1.md\\:grid-cols-2');
        if (!mainContainer) {
            // fallback: find the parent of rating header that represents the right column
            const rightCol = ratingHeader.parentElement;
            if (rightCol && rightCol.parentElement) {
                mainContainer = rightCol.parentElement.parentElement;
            }
        }
        
        if (mainContainer && !document.getElementById('custom-reviews-section')) {
           const reviewsSection = document.createElement('div');
           reviewsSection.id = 'custom-reviews-section';
           reviewsSection.className = 'mt-12 bg-white rounded-3xl p-8 shadow-sm border border-gray-100 w-full col-span-full';
           
           // Append it after the main container if possible
           if (mainContainer.parentElement) {
               mainContainer.parentElement.appendChild(reviewsSection);
           } else {
               mainContainer.appendChild(reviewsSection);
           }
           
           renderReviewsSection(reviewsSection, productId, reviews || []);
        }

        isInitializing = false;
      })
      .catch(err => {
        console.error("Error fetching reviews:", err);
        isInitializing = false;
      });
  }

  function renderStarsHeader(container, avgRating, totalReviews) {
      container.innerHTML = '';
      
      const starsWrapper = document.createElement('div');
      starsWrapper.className = 'flex items-center text-yellow-500 cursor-pointer';
      starsWrapper.title = "Haz clic para calificar y dejar una reseña";
      
      for (let i = 1; i <= 5; i++) {
         const star = document.createElementNS("http://www.w3.org/2000/svg", "svg");
         star.setAttribute('viewBox', "0 0 20 20");
         star.setAttribute('class', `w-6 h-6 transition-colors duration-200 ${i <= Math.round(avgRating) ? 'fill-current' : 'text-gray-300 fill-current'}`);
         star.dataset.value = i;
         
         const path = document.createElementNS("http://www.w3.org/2000/svg", "path");
         path.setAttribute('d', "M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z");
         star.appendChild(path);
         
         // Hover effects
         star.addEventListener('mouseenter', () => {
             const allStars = starsWrapper.querySelectorAll('svg');
             allStars.forEach((s, idx) => {
                 if (idx < i) {
                     s.classList.remove('text-gray-300');
                 } else {
                     s.classList.add('text-gray-300');
                 }
             });
         });
         
         starsWrapper.appendChild(star);
      }
      
      starsWrapper.addEventListener('mouseleave', () => {
          const allStars = starsWrapper.querySelectorAll('svg');
          allStars.forEach((s, idx) => {
              if (idx < Math.round(avgRating)) {
                  s.classList.remove('text-gray-300');
              } else {
                  s.classList.add('text-gray-300');
              }
          });
      });
      
      starsWrapper.addEventListener('click', () => {
         const section = document.getElementById('custom-reviews-section');
         if(section) {
             section.scrollIntoView({ behavior: 'smooth' });
             const textarea = section.querySelector('textarea');
             if(textarea) textarea.focus();
         }
      });
      
      const ratingText = document.createElement('span');
      ratingText.className = 'ml-2 font-semibold text-gray-900 text-lg';
      ratingText.innerText = avgRating.toFixed(1);
      starsWrapper.appendChild(ratingText);
      
      const separator = document.createElement('span');
      separator.className = 'text-gray-300 mx-2';
      separator.innerText = '|';
      
      const reviewsText = document.createElement('span');
      reviewsText.className = 'text-sm text-gray-500';
      reviewsText.innerText = `(${totalReviews} reseña${totalReviews !== 1 ? 's' : ''})`;
      
      container.appendChild(starsWrapper);
      container.appendChild(separator);
      container.appendChild(reviewsText);
  }

  function renderReviewsSection(container, productId, reviews) {
      const userStr = localStorage.getItem('pescaderia_user');
      const userObj = userStr ? JSON.parse(userStr) : null;
      const currentUserId = userObj ? userObj.id : "";
      const hasReviewed = reviews.some(r => r.userId === currentUserId);

      container.innerHTML = `
        <h2 class="text-2xl font-bold text-gray-900 mb-6">Reseñas del Producto</h2>
        
        <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
            <div>
                <h3 class="text-lg font-semibold mb-4">Escribir una reseña</h3>
                ${hasReviewed && currentUserId ? 
                '<div class="bg-blue-50 text-blue-800 p-4 rounded-xl font-medium">Ya has dejado una reseña para este producto.</div>' :
                `<form id="review-form" class="space-y-4">
                    <div>
                        <label class="block text-sm font-medium text-gray-700 mb-1">Tu calificación</label>
                        <div id="form-stars" class="flex items-center space-x-1 cursor-pointer">
                            ${[1,2,3,4,5].map(i => `
                            <svg data-value="${i}" viewBox="0 0 20 20" class="form-star w-8 h-8 text-gray-300 fill-current transition-colors">
                                <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"/>
                            </svg>`).join('')}
                        </div>
                        <input type="hidden" id="form-rating" value="5">
                    </div>
                    <div>
                        <label class="block text-sm font-medium text-gray-700 mb-1">Tu nombre</label>
                        <input type="text" id="form-name" required class="w-full border-gray-300 rounded-lg shadow-sm focus:border-blue-500 focus:ring-blue-500 p-3 border">
                    </div>
                    <div>
                        <label class="block text-sm font-medium text-gray-700 mb-1">Comentario</label>
                        <textarea id="form-comment" required rows="4" class="w-full border-gray-300 rounded-lg shadow-sm focus:border-blue-500 focus:ring-blue-500 p-3 border"></textarea>
                    </div>
                    <button type="submit" class="bg-blue-600 text-white px-6 py-3 rounded-xl font-semibold hover:bg-blue-700 transition-colors w-full">
                        Enviar Reseña
                    </button>
                </form>`}
            </div>
            
            <div class="space-y-6 max-h-[500px] overflow-y-auto pr-4" id="reviews-list">
                <!-- Reviews will be rendered here -->
            </div>
        </div>
      `;
      
      const formStars = container.querySelectorAll('.form-star');
      const ratingInput = container.querySelector('#form-rating');
      let currentRating = 5;
      
      function updateFormStars(rating) {
          formStars.forEach((s, idx) => {
              if (idx < rating) {
                  s.classList.remove('text-gray-300');
                  s.classList.add('text-yellow-500');
              } else {
                  s.classList.remove('text-yellow-500');
                  s.classList.add('text-gray-300');
              }
          });
      }
      
      if (formStars.length > 0) {
          updateFormStars(currentRating);
          
          formStars.forEach((star, idx) => {
              star.addEventListener('mouseenter', () => updateFormStars(idx + 1));
              star.addEventListener('mouseleave', () => updateFormStars(currentRating));
              star.addEventListener('click', () => {
                  currentRating = idx + 1;
                  ratingInput.value = currentRating;
                  updateFormStars(currentRating);
              });
          });
      }
      
      const reviewsList = container.querySelector('#reviews-list');
      
      function renderList(list) {
          if (list.length === 0) {
              reviewsList.innerHTML = '<p class="text-gray-500 italic">Aún no hay reseñas para este producto. ¡Sé el primero!</p>';
              return;
          }
          
          reviewsList.innerHTML = list.map(r => `
             <div class="bg-gray-50 rounded-2xl p-6 border border-gray-100 mb-4">
                <div class="flex justify-between items-start mb-3">
                    <div class="flex items-center space-x-3">
                        <div class="w-10 h-10 bg-blue-100 text-blue-600 rounded-full flex items-center justify-center font-bold text-lg">
                            ${r.userName ? r.userName.charAt(0).toUpperCase() : 'U'}
                        </div>
                        <div>
                            <p class="font-semibold text-gray-900">${r.userName}</p>
                            <p class="text-xs text-gray-500">${r.date || new Date().toLocaleDateString()}</p>
                        </div>
                    </div>
                    <div class="flex text-yellow-500">
                        ${Array(5).fill(0).map((_, i) => `<svg viewBox="0 0 20 20" class="w-5 h-5 ${i < r.rating ? 'fill-current' : 'text-gray-300 fill-current'}"><path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"/></svg>`).join('')}
                    </div>
                </div>
                <p class="text-gray-700 leading-relaxed">${r.comment}</p>
             </div>
          `).join('');
      }
      
      renderList(reviews);
      
      const form = container.querySelector('#review-form');
      if (form) {
      form.addEventListener('submit', (e) => {
          e.preventDefault();
          const btn = form.querySelector('button');
          btn.disabled = true;
          btn.innerText = 'Enviando...';
          
          let generatedId = Date.now().toString();
          if (typeof crypto !== 'undefined' && crypto.randomUUID) {
              generatedId = crypto.randomUUID();
          }

          const userStr = localStorage.getItem('pescaderia_user');
          const userObj = userStr ? JSON.parse(userStr) : null;
          const currentUserId = userObj ? userObj.id : "";

          const reviewData = {
              id: generatedId,
              productId: productId,
              userId: currentUserId,
              userName: document.getElementById('form-name').value,
              userAvatar: "",
              rating: parseInt(ratingInput.value),
              comment: document.getElementById('form-comment').value,
              date: new Date().toISOString().split('T')[0]
          };
          
          fetch(`/api/products/${productId}/reviews`, {
              method: 'POST',
              headers: { 'Content-Type': 'application/json' },
              body: JSON.stringify(reviewData)
          })
          .then(async res => {
             if(!res.ok) {
                 const errJson = await res.json().catch(()=>({}));
                 throw new Error(errJson.message || "Server error");
             }
             return res.json();
          })
          .then(newReview => {
              reviews.push(newReview);
              
              // Render the whole section again to hide the form
              renderReviewsSection(container, productId, reviews);
              
              // Update stars header
              const ratingHeader = document.querySelector('[data-reviews-injected]');
              if (ratingHeader) delete ratingHeader.dataset.reviewsInjected;
              initReviews();
          })
          .catch(err => {
              console.error(err);
              btn.disabled = false;
              btn.innerText = 'Enviar Reseña';
              alert(err.message || 'Hubo un error al enviar la reseña.');
          });
      });
      }
  }

  // Use MutationObserver to detect DOM changes when navigating in SPA
  const observer = new MutationObserver(() => {
     initReviews();
  });
  
  observer.observe(document.body, { childList: true, subtree: true });
  
  // Initial check
  if (document.readyState === 'loading') {
      window.addEventListener('DOMContentLoaded', () => setTimeout(initReviews, 500));
  } else {
      setTimeout(initReviews, 500);
  }
})();
