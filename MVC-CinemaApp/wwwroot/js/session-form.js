$(document).ready(function() {
    const $movieSelect = $('#movieSelect');
    const $hallSelect = $('#hallSelect');
    const $startTimeInput = $('#startTimeInput');
    const $endTimeInput = $('#endTimeInput');
    const $priceInput = $('#priceInput');

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
        if ($previewPrice.length) $previewPrice.text(price ? `$${parseFloat(price).toFixed(2)}` : "-");

        if (startTime) {
            const date = new Date(startTime);

            if ($previewDate.length) {
                const dateString = date.toLocaleDateString('en-US', {
                    month: 'short',
                    day: '2-digit',
                    year: 'numeric'
                });
                $previewDate.text(dateString);
            }

            if ($previewTime.length) {
                const startTimeString = date.toLocaleTimeString([], {
                    hour: '2-digit',
                    minute: '2-digit',
                    hour12: false
                });

                let timeDisplay = startTimeString;

                if (endTime) {
                    const endDate = new Date(endTime);
                    const endTimeString = endDate.toLocaleTimeString([], {
                        hour: '2-digit',
                        minute: '2-digit',
                        hour12: false
                    });
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

                if (diffMs > 0) {
                    const totalMinutes = Math.floor(diffMs / 60000);
                    $previewDuration.text(`${totalMinutes} minutes`).removeClass('text-danger');
                } else {
                    $previewDuration.text("Invalid time range").addClass('text-danger');
                }
            } else {
                $previewDuration.text("-").removeClass('text-danger');
            }
        }
    }

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

        select2Data.$results.find('.select2-results__option').each(function() {
            const $option = $(this);
            const data = $option.data('data');
            $option.attr('aria-selected', (data && data.id === currentValue) ? 'true' : 'false');
        });
    }

    $selects.on('select2:select select2:open', function() {
        const self = this;
        setTimeout(() => updateAriaSelected(self), 30);
    });
});