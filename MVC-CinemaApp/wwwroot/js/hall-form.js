$(document).ready(function() {
    const $hallTypeSelect = $('#hallTypeSelect');
    const $hallNameInput = $('#hallNameInput');
    const $previewName = $('#previewName');
    const $previewType = $('#previewType');

    $hallTypeSelect.select2({
        placeholder: "Select hall type...",
        width: '100%',
        allowClear: false,
        minimumResultsForSearch: -1,
        dropdownParent: $('.create-card, .edit-card').first()
    });

    if ($hallNameInput.length && $previewName.length) {
        $hallNameInput.on('input', function() {
            const name = $(this).val().trim();
            $previewName.text(name || 'No name set')
                .toggleClass('text-muted', !name);
        });
    }

    $hallTypeSelect.on('change', function() {
        const type = $(this).find('option:selected').text();
        const value = $(this).val();

        if ($previewType.length) {
            if (value) {
                $previewType.text(type).removeClass('text-muted');
            } else {
                $previewType.text('No type selected').addClass('text-muted');
            }
        }

        if (value && typeof $(this).valid === 'function') {
            $(this).valid();
        }
    });

    function updateAriaSelected() {
        const currentValue = $hallTypeSelect.val();
        const select2Data = $hallTypeSelect.data('select2');

        if (!select2Data || !select2Data.$results) return;

        select2Data.$results.find('.select2-results__option').each(function() {
            const $option = $(this);
            const data = $option.data('data');
            const isSelected = data && data.id === currentValue;
            $option.attr('aria-selected', isSelected ? 'true' : 'false');
        });
    }

    $hallTypeSelect.on('select2:select select2:open', function() {
        setTimeout(updateAriaSelected, 30);
    });
});