$(document).ready(function() {
    const $rows = $("#movieTableBody tr");
    const $visibleCount = $("#visibleCount");
    const $noResults = $("#noResults");
    const $table = $("#movieTable");

    $("#movieSearch").on("input", function() {
        const value = $(this).val().toLowerCase();
        let visibleCount = 0;

        $rows.each(function() {
            const $row = $(this);
            const text = $row.find(".movie-title, .director-badge").text().toLowerCase();
            const isMatch = text.includes(value);

            $row.toggle(isMatch);
            if (isMatch) visibleCount++;
        });

        $visibleCount.text(visibleCount);
        $table.toggleClass('d-none', visibleCount === 0);
        $noResults.toggleClass('d-none', visibleCount !== 0);
    });

    let sortDirections = {};
    $(".sortable").on("click", function () {
        const $headers = $(".sortable i");
        const columnIndex = $(this).data("column");
        const tbody = $("#movieTableBody");
        const rows = tbody.find("tr").get();

        sortDirections[columnIndex] = !sortDirections[columnIndex];
        const asc = sortDirections[columnIndex];

        $headers.removeClass("bi-arrow-up bi-arrow-down").addClass("bi-arrow-down-up");
        $(this).find("i").toggleClass("bi-arrow-up", asc).toggleClass("bi-arrow-down", !asc);

        rows.sort(function (a, b) {
            const A = $(a).children("td").eq(columnIndex).text().trim().toLowerCase();
            const B = $(b).children("td").eq(columnIndex).text().trim().toLowerCase();
            return asc ? A.localeCompare(B) : B.localeCompare(A);
        });

        $.each(rows, (i, row) => tbody.append(row));
    });
});