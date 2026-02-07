$(document).ready(function() {
    const $rows = $("#ordersTableBody tr");
    const $visibleCount = $("#visibleCount");
    const $noResults = $("#noResults");
    const $table = $("#ordersTable");

    // Search Logic
    $("#orderSearch").on("input", function() {
        const value = $(this).val().toLowerCase();
        let visibleCount = 0;

        $rows.each(function() {
            const $row = $(this);
            const text = $row.find(".movie-title, .hall-badge").text().toLowerCase();
            const isMatch = text.includes(value);

            $row.toggle(isMatch);
            if (isMatch) visibleCount++;
        });

        $visibleCount.text(visibleCount);
        $table.toggleClass('d-none', visibleCount === 0);
        $noResults.toggleClass('d-none', visibleCount !== 0);
    });

    // Sort Logic
    let sortDirections = {};
    $(".sortable").on("click", function () {
        const $headers = $(".sortable i");
        const columnIndex = $(this).data("column");
        const tbody = $("#ordersTableBody");
        const rows = tbody.find("tr").get();

        sortDirections[columnIndex] = !sortDirections[columnIndex];
        const asc = sortDirections[columnIndex];

        // Reset icons
        $headers.removeClass("bi-arrow-up bi-arrow-down").addClass("bi-arrow-down-up");
        $(this).find("i").removeClass("bi-arrow-down-up").toggleClass("bi-arrow-up", asc).toggleClass("bi-arrow-down", !asc);

        rows.sort(function (a, b) {
            const A = $(a).children("td").eq(columnIndex).text().trim().toLowerCase();
            const B = $(b).children("td").eq(columnIndex).text().trim().toLowerCase();
            return asc ? A.localeCompare(B) : B.localeCompare(A);
        });

        $.each(rows, (i, row) => tbody.append(row));
    });

    // Cancel Confirmation
    $(".cancel-btn").on("click", function() {
        const $form = $(this).closest(".cancel-form");

        if (confirm("Are you sure you want to cancel this order? This action cannot be undone.")) {
            $form.submit();
        }
    });

    // Auto-hide alerts after 5 seconds
    setTimeout(function() {
        $('.alert').fadeOut('slow');
    }, 5000);
});