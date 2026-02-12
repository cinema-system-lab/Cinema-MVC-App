$(document).ready(function () {
    const $movieSelect = $('#movieSelect');
    const $hallSelect = $('#hallSelect');
    const $startTimeInput = $('#startTimeInput');
    const $endTimeInput = $('#endTimeInput');
    const $priceInput = $('#priceInput');
    const $sessionForm = $('#sessionForm');

    const $parentContainer = $('.create-card, .edit-card').first();
    const $selects = $movieSelect.add($hallSelect);

    $selects.select2({
        width: '100%',
        minimumResultsForSearch: -1,
        dropdownParent: $parentContainer.length ? $parentContainer : $('body')
    });

    const $previewBox = $('#previewBox');
    const $previewMovie = $('#previewMovie');
    const $previewHall = $('#previewHall');
    const $previewTime = $('#previewTime');
    const $previewDate = $('#previewDate');
    const $previewPrice = $('#previewPrice');
    const $previewDuration = $('#previewDuration');

    function updatePreview() {
        if (!$previewBox.length) return;

        const movieText = $movieSelect.find('option:selected').text();
        const hallText = $hallSelect.find('option:selected').text();
        const startTime = $startTimeInput.val();
        const endTime = $endTimeInput.val();
        const price = $priceInput.val();

        if ($previewMovie.length) $previewMovie.text($movieSelect.val() ? movieText : "-");
        if ($previewHall.length) $previewHall.text($hallSelect.val() ? hallText : "-");

        if ($previewPrice.length) $previewPrice.text(price ? `₴${parseFloat(price).toFixed(2)}` : "-");

        if (startTime) {
            const date = new Date(startTime);

            if ($previewDate.length) {
                $previewDate.text(date.toLocaleDateString('en-US', {
                    month: 'short',
                    day: '2-digit',
                    year: 'numeric'
                }));
            }

            if ($previewTime.length) {
                const startTimeString = date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit', hour12: false });
                let timeDisplay = startTimeString;

                if (endTime) {
                    const endDate = new Date(endTime);
                    const endTimeString = endDate.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit', hour12: false });
                    timeDisplay = `${startTimeString} - ${endTimeString}`;
                }
                $previewTime.text(timeDisplay);
            }
        }

        if ($previewDuration.length) {
            if (startTime && endTime) {
                const start = new Date(startTime);
                const end = new Date(endTime);
                const diffMs = end - start;
                const totalMinutes = Math.floor(diffMs / 60000);

                if (totalMinutes > 300) {
                    $previewDuration.text(`${totalMinutes} min (Exceeds 5h!)`).addClass('text-danger').addClass('fw-bold');
                } else if (totalMinutes <= 0) {
                    $previewDuration.text("Invalid time range").addClass('text-danger');
                } else {
                    $previewDuration.text(`${totalMinutes} minutes`).removeClass('text-danger').removeClass('fw-bold');
                }
            } else {
                $previewDuration.text("-").removeClass('text-danger');
            }
        }
    }

    $sessionForm.on('submit', function (e) {
        const start = new Date($startTimeInput.val());
        const end = new Date($endTimeInput.val());
        const diffMins = Math.floor((end - start) / 60000);

        const $errorBlock = $('#jsCustomError');
        const $errorText = $('#jsErrorText');

        let error = "";

        if (diffMins > 300) {
            error = "The session duration cannot exceed 5 hours (300 minutes).";
        } else if (diffMins <= 0) {
            error = "End time must be after start time.";
        }

        if (error) {
            e.preventDefault();

            $errorText.text(error);
            $errorBlock.removeClass('d-none');

            window.scrollTo({ top: 0, behavior: 'smooth' });
            return false;
        }

        $errorBlock.addClass('d-none');
    });

    const formInputs = [$movieSelect, $hallSelect, $startTimeInput, $endTimeInput, $priceInput];
    formInputs.forEach($el => {
        $el.on('change input', updatePreview);
    });

    updatePreview();

    function updateAriaSelected(element) {
        const $el = $(element);
        const currentValue = $el.val();
        const select2Data = $el.data('select2');
        if (!select2Data || !select2Data.$results) return;

        select2Data.$results.find('.select2-results__option').each(function () {
            const $option = $(this);
            const data = $option.data('data');
            $option.attr('aria-selected', (data && data.id === currentValue) ? 'true' : 'false');
        });
    }

    $selects.on('select2:select select2:open', function () {
        setTimeout(() => updateAriaSelected(this), 30);
    });
});