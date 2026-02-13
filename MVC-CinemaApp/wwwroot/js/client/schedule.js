function filterDate(dateStr, btn) {
    document.querySelectorAll('.date-card').forEach(b => b.classList.remove('active'));
    btn.classList.add('active');
    document.querySelectorAll('.date-section').forEach(s => s.classList.add('d-none'));

    const targetSection = document.getElementById('date-' + dateStr);
    if (targetSection) {
        targetSection.classList.remove('d-none');
    }
    applyFilters();
}

// Modern chip-based filters
function filterByGenre(genre, btn) {
    document.querySelectorAll('#genreChips .chip').forEach(c => c.classList.remove('active'));
    btn.classList.add('active');
    applyFilters();
}

function filterByHall(hallType, btn) {
    document.querySelectorAll('#hallTypeChips .chip').forEach(c => c.classList.remove('active'));
    btn.classList.add('active');
    applyFilters();
}

function applyFilters() {
    const selectedGenreBtn = document.querySelector('#genreChips .chip.active');
    const selectedHallBtn = document.querySelector('#hallTypeChips .chip.active');
    
    const selectedGenre = selectedGenreBtn ? selectedGenreBtn.getAttribute('data-genre') : 'all';
    const selectedHallType = selectedHallBtn ? selectedHallBtn.getAttribute('data-hall') : 'all';
    
    const activeSection = document.querySelector('.date-section:not(.d-none)');

    if (!activeSection) return;

    activeSection.querySelectorAll('.movie-row').forEach(row => {
        const movieGenres = row.getAttribute('data-genres');
        let hasVisibleSessions = 0;

        row.querySelectorAll('.session-box').forEach(session => {
            const sessionType = session.getAttribute('data-hall-type');

            const genreMatch = selectedGenre === 'all' || movieGenres.includes(selectedGenre);
            const typeMatch = selectedHallType === 'all' || sessionType === selectedHallType;

            if (genreMatch && typeMatch) {
                session.style.display = 'block';
                hasVisibleSessions++;
            } else {
                session.style.display = 'none';
            }
        });

        row.style.display = (hasVisibleSessions > 0) ? 'block' : 'none';
    });
}