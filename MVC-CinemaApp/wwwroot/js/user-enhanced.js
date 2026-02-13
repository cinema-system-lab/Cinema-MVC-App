/**
 * Enhanced User Experience - Cinema App
 * Modern interactions without React
 */

(function() {
    'use strict';

    // Smooth scroll to top button
    function initScrollToTop() {
        const scrollBtn = document.createElement('button');
        scrollBtn.innerHTML = '<i class="bi bi-arrow-up"></i>';
        scrollBtn.className = 'scroll-to-top';
        scrollBtn.setAttribute('aria-label', 'Scroll to top');
        document.body.appendChild(scrollBtn);

        const style = document.createElement('style');
        style.textContent = `
            .scroll-to-top {
                position: fixed;
                bottom: 30px;
                right: 30px;
                width: 50px;
                height: 50px;
                border-radius: 50%;
                background: linear-gradient(135deg, #E30613 0%, #b30510 100%);
                color: white;
                border: none;
                box-shadow: 0 4px 16px rgba(227, 6, 19, 0.4);
                cursor: pointer;
                opacity: 0;
                visibility: hidden;
                transition: all 0.3s ease;
                z-index: 1000;
                font-size: 1.2rem;
            }
            .scroll-to-top.visible {
                opacity: 1;
                visibility: visible;
            }
            .scroll-to-top:hover {
                transform: translateY(-4px);
                box-shadow: 0 8px 24px rgba(227, 6, 19, 0.6);
            }
        `;
        document.head.appendChild(style);

        window.addEventListener('scroll', () => {
            if (window.pageYOffset > 300) {
                scrollBtn.classList.add('visible');
            } else {
                scrollBtn.classList.remove('visible');
            }
        });

        scrollBtn.addEventListener('click', () => {
            window.scrollTo({ top: 0, behavior: 'smooth' });
        });
    }

    // Add hover effects to cards
    function initCardEffects() {
        const cards = document.querySelectorAll('.movie-card, .session-card, .stats-card');
        cards.forEach(card => {
            card.addEventListener('mouseenter', function() {
                this.style.transition = 'transform 0.3s ease';
            });
        });
    }

    // Search functionality with debounce
    function initSearch() {
        const searchInput = document.querySelector('.filter-input[type="text"]');
        if (!searchInput) return;

        let debounceTimer;
        searchInput.addEventListener('input', function(e) {
            clearTimeout(debounceTimer);
            debounceTimer = setTimeout(() => {
                const query = e.target.value.toLowerCase();
                filterCards(query);
            }, 300);
        });
    }

    function filterCards(query) {
        const cards = document.querySelectorAll('.movie-card, .session-card');
        cards.forEach(card => {
            const text = card.textContent.toLowerCase();
            if (text.includes(query)) {
                card.style.display = '';
                card.classList.add('fade-in');
            } else {
                card.style.display = 'none';
            }
        });
    }

    // Lazy loading images
    function initLazyLoading() {
        const images = document.querySelectorAll('img[loading="lazy"]');
        
        if ('IntersectionObserver' in window) {
            const imageObserver = new IntersectionObserver((entries, observer) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        const img = entry.target;
                        img.src = img.dataset.src || img.src;
                        img.classList.add('fade-in');
                        observer.unobserve(img);
                    }
                });
            });

            images.forEach(img => imageObserver.observe(img));
        }
    }

    // Animate elements on scroll
    function initScrollAnimations() {
        const animateElements = document.querySelectorAll('.fade-in, .slide-up');
        
        if ('IntersectionObserver' in window) {
            const observer = new IntersectionObserver((entries) => {
                entries.forEach((entry, index) => {
                    if (entry.isIntersecting) {
                        setTimeout(() => {
                            entry.target.style.opacity = '1';
                            entry.target.style.transform = 'translateY(0)';
                        }, index * 100);
                        observer.unobserve(entry.target);
                    }
                });
            }, { threshold: 0.1 });

            animateElements.forEach(el => {
                el.style.opacity = '0';
                el.style.transform = 'translateY(20px)';
                el.style.transition = 'all 0.6s ease';
                observer.observe(el);
            });
        }
    }

    // Toast notifications
    function showToast(message, type = 'success') {
        const toast = document.createElement('div');
        toast.className = `toast-notification toast-${type}`;
        toast.innerHTML = `
            <i class="bi bi-${type === 'success' ? 'check-circle-fill' : 'exclamation-triangle-fill'} me-2"></i>
            ${message}
        `;

        const style = document.createElement('style');
        style.textContent = `
            .toast-notification {
                position: fixed;
                bottom: 30px;
                left: 50%;
                transform: translateX(-50%) translateY(100px);
                padding: 16px 24px;
                border-radius: 12px;
                color: white;
                font-weight: 600;
                box-shadow: 0 8px 32px rgba(0, 0, 0, 0.5);
                z-index: 9999;
                animation: toastSlideUp 0.3s ease forwards;
            }
            .toast-success {
                background: linear-gradient(135deg, #10b981 0%, #059669 100%);
            }
            .toast-error {
                background: linear-gradient(135deg, #ef4444 0%, #dc2626 100%);
            }
            @keyframes toastSlideUp {
                to {
                    transform: translateX(-50%) translateY(0);
                }
            }
        `;
        if (!document.querySelector('#toast-styles')) {
            style.id = 'toast-styles';
            document.head.appendChild(style);
        }

        document.body.appendChild(toast);

        setTimeout(() => {
            toast.style.animation = 'toastSlideUp 0.3s ease reverse';
            setTimeout(() => toast.remove(), 300);
        }, 3000);
    }

    // Filter tags functionality
    function initFilterTags() {
        const filterTags = document.querySelectorAll('.filter-tag');
        filterTags.forEach(tag => {
            tag.addEventListener('click', function() {
                this.classList.toggle('active');
                // Implement your filtering logic here
                const activeFilters = Array.from(document.querySelectorAll('.filter-tag.active'))
                    .map(t => t.textContent.trim().toLowerCase());
                console.log('Active filters:', activeFilters);
            });
        });
    }

    // Quick book button enhancement
    function initQuickBook() {
        document.addEventListener('click', function(e) {
            if (e.target.closest('.btn-quick-book')) {
                e.preventDefault();
                const btn = e.target.closest('.btn-quick-book');
                btn.innerHTML = '<i class="bi bi-check-lg me-2"></i>Opening...';
                setTimeout(() => {
                    window.location.href = btn.closest('a').href;
                }, 300);
            }
        });
    }

    // Add ripple effect to buttons
    function initRippleEffect() {
        const buttons = document.querySelectorAll('.btn, .btn-hero, .btn-quick-book');
        buttons.forEach(button => {
            button.addEventListener('click', function(e) {
                const ripple = document.createElement('span');
                const rect = this.getBoundingClientRect();
                const size = Math.max(rect.width, rect.height);
                const x = e.clientX - rect.left - size / 2;
                const y = e.clientY - rect.top - size / 2;
                
                ripple.style.cssText = `
                    position: absolute;
                    width: ${size}px;
                    height: ${size}px;
                    border-radius: 50%;
                    background: rgba(255, 255, 255, 0.5);
                    left: ${x}px;
                    top: ${y}px;
                    pointer-events: none;
                    animation: ripple-effect 0.6s ease-out;
                `;
                
                if (!this.style.position || this.style.position === 'static') {
                    this.style.position = 'relative';
                }
                this.style.overflow = 'hidden';
                
                this.appendChild(ripple);
                setTimeout(() => ripple.remove(), 600);
            });
        });

        const style = document.createElement('style');
        style.textContent = `
            @keyframes ripple-effect {
                to {
                    transform: scale(2);
                    opacity: 0;
                }
            }
        `;
        document.head.appendChild(style);
    }

    // Initialize all features
    function init() {
        // Run on page load
        document.addEventListener('DOMContentLoaded', () => {
            initScrollToTop();
            initCardEffects();
            initSearch();
            initLazyLoading();
            initScrollAnimations();
            initFilterTags();
            initQuickBook();
            initRippleEffect();
            
            console.log('✨ Enhanced UI initialized');
        });
    }

    // Make showToast available globally
    window.showToast = showToast;

    init();
})();
