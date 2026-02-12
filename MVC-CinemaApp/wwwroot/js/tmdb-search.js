$(document).ready(function () {
    $('#btnSearchTmdb').click(function () {
        performTmdbSearch();
    });

    $('#tmdbSearchInput').keypress(function (e) {
        if (e.which == 13) {
            performTmdbSearch();
        }
    });

    function performTmdbSearch() {
        var query = $('#tmdbSearchInput').val();
        if (!query) return;

        $('#tmdbResultsList').empty();
        $('#tmdbLoading').removeClass('d-none');

        $.ajax({
            url: '/Admin/Movies/SearchTmdbJson',
            data: {query: query},
            success: function (data) {
                $('#tmdbLoading').addClass('d-none');
                if (data.length === 0) {
                    $('#tmdbResultsList').html('<div class="alert alert-warning">Nothing found.</div>');
                    return;
                }

                data.forEach(function (movie) {
                    var img = movie.poster
                        ? `<img src="${movie.poster}" class="tmdb-result-img me-3">`
                        : `<div class="tmdb-result-img me-3 d-flex align-items-center justify-content-center bg-dark"><i class="bi bi-film"></i></div>`;

                    var item = `
                                    <a href="/Admin/Movies/CreateFromTmdb?tmdbId=${movie.id}" class="tmdb-result-item d-flex align-items-center">
                                        ${img}
                                        <div class="tmdb-result-info flex-grow-1">
                                            <h6 class="mb-0">${movie.title}</h6>
                                            <div class="d-flex align-items-center gap-2 mb-1">
                                                <span class="badge bg-dark text-warning small">${movie.year}</span>
                                                <span class="text-muted small"><i class="bi bi-star-fill text-warning"></i> ${movie.vote_average || 'N/A'}</span>
                                            </div>
                                            <p class="mb-0 text-truncate" style="max-width: 450px;">${movie.overview || 'No description available.'}</p>
                                        </div>
                                        <div class="ms-3">
                                            <span class="btn btn-sm btn-outline-success rounded-pill px-3">Select</span>
                                        </div>
                                    </a>
                                `;
                    $('#tmdbResultsList').append(item);
                });
            },
            error: function () {
                $('#tmdbLoading').addClass('d-none');
                $('#tmdbResultsList').html('<div class="alert alert-danger">Error searching TMDB.</div>');
            }
        });
    }

    function updatePosterPreview(url) {
        if (url) {
            $('#posterPreview').html(`<img src="${url}" class="img-fluid rounded shadow" style="width: 100%; height: auto; display: block;" />`);
        }
    }

    updatePosterPreview($('#posterUrlInput').val());

    $('#posterUrlInput').on('input', function () {
        updatePosterPreview($(this).val());
    });
});