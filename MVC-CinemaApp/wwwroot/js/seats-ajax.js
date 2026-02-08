document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.seat-toggle-form').forEach(form => {
        form.addEventListener('submit', async function (e) {
            e.preventDefault();

            const button = this.querySelector('.seat-btn');
            const formData = new FormData(this);

            button.style.opacity = '0.6';
            button.style.pointerEvents = 'none';

            try {
                const response = await fetch(this.action, {
                    method: 'POST',
                    body: formData,
                    headers: {'X-Requested-With': 'XMLHttpRequest'}
                });

                if (response.ok) {
                    const data = await response.json();
                    if (data.success) {
                        if (button.classList.contains('seat-regular')) {
                            button.classList.remove('seat-regular');
                            button.classList.add('seat-premium');
                        } else {
                            button.classList.remove('seat-premium');
                            button.classList.add('seat-regular');
                        }
                        updateStatsUI(data.stats);
                    }
                }
            } catch (error) {
            } finally {
                button.style.opacity = '1';
                button.style.pointerEvents = 'auto';
            }
        });
    });

    document.querySelectorAll('.seat-delete-form').forEach(form => {
        form.addEventListener('submit', async function (e) {
            e.preventDefault();

            if (!confirm('Видалити це місце?')) return;

            const seatWrapper = this.closest('.seat-wrapper');
            const formData = new FormData(this);

            seatWrapper.style.opacity = '0.5';

            try {
                const response = await fetch(this.action, {
                    method: 'POST',
                    body: formData,
                    headers: {'X-Requested-With': 'XMLHttpRequest'}
                });

                if (response.ok) {
                    const data = await response.json();
                    if (data.success) {
                        seatWrapper.style.transition = 'all 0.3s ease';
                        seatWrapper.style.transform = 'scale(0)';
                        seatWrapper.style.opacity = '0';

                        setTimeout(() => {
                            const spacer = document.createElement('div');
                            spacer.className = 'seat-spacer';
                            seatWrapper.replaceWith(spacer);
                        }, 300);

                        updateStatsUI(data.stats);
                    } else {
                        seatWrapper.style.opacity = '1';
                    }
                } else {
                    seatWrapper.style.opacity = '1';
                }
            } catch (error) {
                seatWrapper.style.opacity = '1';
            }
        });
    });

    function updateStatsUI(stats) {
        if (!stats) return;
        const totalEl = document.getElementById('total-seats-count');
        const regularEl = document.getElementById('regular-seats-count');
        const premiumEl = document.getElementById('premium-seats-count');

        if (totalEl) totalEl.textContent = stats.totalSeats;
        if (regularEl) regularEl.textContent = stats.regularSeats;
        if (premiumEl) premiumEl.textContent = stats.premiumSeats;
    }
});