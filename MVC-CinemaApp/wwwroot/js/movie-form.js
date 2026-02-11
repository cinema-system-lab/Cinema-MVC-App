$(document).ready(function() {
    const $genresSelect = $('#genresSelect');

    $genresSelect.select2({
        placeholder: "Select movie genres",
        width: '100%',
        closeOnSelect: false,
        dropdownParent: $('#genresSelect').closest('.create-card, .edit-card'),
        allowClear: true
    });

    let selectedValues = $genresSelect.val() || [];

    function updateAriaSelected() {
        const currentValues = $genresSelect.val() || [];

        $genresSelect.data('select2').$results.find('.select2-results__option').each(function() {
            const $option = $(this);
            const value = $option.data('data')?.id;

            if (value && currentValues.includes(String(value))) {
                $option.attr('aria-selected', 'true');
            } else {
                $option.attr('aria-selected', 'false');
            }
        });
    }

    $genresSelect.on('select2:select select2:unselect', function(e) {
        selectedValues = $(this).val() || [];
        updateAriaSelected();
        setTimeout(updateAriaSelected, 0);
    });

    $genresSelect.on('select2:open', function() {
        setTimeout(updateAriaSelected, 50);
    });

    $genresSelect.on('select2:open', function() {
        const $results = $(this).data('select2').$results;
        $results.on('mouseenter', '.select2-results__option', function() {
            updateAriaSelected();
        });
    });

    $genresSelect.on('change', function() {
        if ($(this).val().length > 0) {
            $(this).valid();
        }
    });

    $('#posterUrlInput').on('input', function() {
        const url = $(this).val();
        const $preview = $('#posterPreview');
        if (url && url.startsWith('http')) {
            $preview.html(`<img src="${url}" alt="Movie Poster" onerror="this.parentElement.innerHTML='<i class=\\'bi bi-exclamation-triangle text-danger fs-1\\'></i>'">`);
        } else {
            $preview.html('<i class="bi bi-image text-muted fs-1"></i>');
        }
    });

    const formatRating = (input) => {
        let val = parseFloat(input.val());
        if (!isNaN(val)) input.val(val.toFixed(1));
    };

    $('#ratingInput').on('blur change', function() { formatRating($(this)); });
    formatRating($('#ratingInput'));
});