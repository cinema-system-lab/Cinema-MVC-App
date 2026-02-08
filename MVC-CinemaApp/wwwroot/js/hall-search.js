$(document).ready(function () {
    const $searchInput = $('#hallSearch');
    const $rows = $('.hall-row');
    const $visibleCount = $('#visibleCount');
    const $noResults = $('#noResults');
    const $table = $('#hallTable');

    $searchInput.on('keyup', function () {
        const value = $(this).val().toLowerCase();
        let visibleCounter = 0;

        $rows.each(function () {
            const name = $(this).find('.hall-name').text().toLowerCase();
            const type = $(this).find('.type-badge').text().toLowerCase();

            const isVisible = name.includes(value) || type.includes(value);
            $(this).toggle(isVisible);

            if (isVisible) visibleCounter++;
        });

        $visibleCount.text(visibleCounter);
        $noResults.toggleClass('d-none', visibleCounter > 0);
        $table.toggleClass('d-none', visibleCounter === 0);
    });

    $('.sortable').on('click', function () {
        const column = $(this).data('column');
        const isAsc = $(this).hasClass('asc');
        const $tbody = $('#hallTableBody');

        $rows.sort(function (a, b) {
            const valA = $(a).find('td').eq(column).text().trim().toLowerCase();
            const valB = $(b).find('td').eq(column).text().trim().toLowerCase();

            if (isAsc) return valA < valB ? 1 : -1;
            return valA > valB ? 1 : -1;
        });

        $(this).toggleClass('asc', !isAsc);
        $tbody.append($rows);

        $('.sortable i').attr('class', 'bi bi-arrow-down-up ms-1');
        $(this).find('i').attr('class', isAsc ? 'bi bi-arrow-up ms-1' : 'bi bi-arrow-down ms-1');
    });
});