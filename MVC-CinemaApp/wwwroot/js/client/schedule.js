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

function applyFilters() {
    const selectedGenre = document.getElementById('genreFilter').value;
    const selectedHallType = document.getElementById('hallTypeFilter').value;
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