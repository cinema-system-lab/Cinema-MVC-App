(function($) {
    'use strict';

    class TableSearch {
        constructor(searchInputId, options = {}) {
            this.$searchInput = $(`#${searchInputId}`);
            if (!this.$searchInput.length) return;

            this.tableBodyId = options.tableBodyId || this.$searchInput.data('search-target');
            this.$tableBody = $(`#${this.tableBodyId}`);
            this.$rows = this.$tableBody.find('tr');

            this.searchColumns = options.searchColumns || this.$searchInput.data('search-columns') || 'td';
            this.$visibleCount = $('#visibleCount');
            this.$noResults = $('#noResults');
            this.$table = this.$tableBody.closest('table');

            this.init();
        }

        init() {
            this.$searchInput.on('input', (e) => this.search(e.target.value));
        }

        search(value) {
            const searchTerm = value.toLowerCase().trim();
            let visibleCount = 0;
            const matchingRows = [];

            this.$rows.each((index, row) => {
                const $row = $(row);
                const text = $row.find(this.searchColumns).text().toLowerCase();
                const isMatch = text.includes(searchTerm);

                $row.toggle(isMatch);

                if (isMatch) {
                    matchingRows.push(row);
                    visibleCount++;
                }
            });

            this.updateUI(visibleCount);

            const pagination = this.$table[0] ? this.$table[0].paginationInstance : null;
            if (pagination && typeof pagination.updateAfterSearch === 'function') {
                pagination.updateAfterSearch(matchingRows);
            }
        }

        updateUI(visibleCount) {
            if (this.$visibleCount.length) {
                this.$visibleCount.text(visibleCount);
            }

            if (this.$table.length && this.$noResults.length) {
                this.$table.toggleClass('d-none', visibleCount === 0);
                this.$noResults.toggleClass('d-none', visibleCount !== 0);
            }
        }
    }

    class TableSort {
        constructor(tableId, options = {}) {
            this.$table = $(`#${tableId}`);
            if (!this.$table.length) return;

            this.$tableBody = this.$table.find('tbody');
            this.sortDirections = {};
            this.pagination = this.$table[0].paginationInstance || null;

            this.init();
        }

        init() {
            const self = this;
            $('.sortable').on('click', function() {
                self.sort($(this));
            });
        }

        sort($header) {
            const columnIndex = $header.data('column');
            const $allHeaders = $('.sortable i');
            const $tbody = this.$tableBody;
            const rows = $tbody.find('tr').get();

            this.sortDirections[columnIndex] = !this.sortDirections[columnIndex];
            const asc = this.sortDirections[columnIndex];

            $allHeaders.removeClass('bi-arrow-up bi-arrow-down').addClass('bi-arrow-down-up');
            $header.find('i')
                .removeClass('bi-arrow-down-up')
                .toggleClass('bi-arrow-up', asc)
                .toggleClass('bi-arrow-down', !asc);

            rows.sort((a, b) => {
                const A = $(a).children('td').eq(columnIndex).text().trim().toLowerCase();
                const B = $(b).children('td').eq(columnIndex).text().trim().toLowerCase();

                const numA = parseFloat(A);
                const numB = parseFloat(B);

                if (!isNaN(numA) && !isNaN(numB)) {
                    return asc ? numA - numB : numB - numA;
                }

                return asc ? A.localeCompare(B) : B.localeCompare(A);
            });

            $.each(rows, (i, row) => $tbody.append(row));

            if (this.pagination) {
                this.pagination.initialize();
            }
        }
    }

    $(document).ready(function() {
        $('[data-search-target]').each(function() {
            const inputId = $(this).attr('id');
            if (inputId) {
                new TableSearch(inputId);
            }
        });

        $('table[data-sort-enabled="true"]').each(function() {
            const tableId = $(this).attr('id');
            if (tableId) {
                new TableSort(tableId);
            }
        });

        const searchMappings = {
            'ticketSearch': { tableBodyId: 'ticketTableBody', searchColumns: '.movie-title, .hall-badge' },
            'sessionSearch': { tableBodyId: 'sessionTableBody', searchColumns: '.movie-title, .hall-badge' },
            'paymentSearch': { tableBodyId: 'paymentTableBody', searchColumns: '.payment-id, .user-email' },
            'orderSearch': { tableBodyId: 'ordersTableBody', searchColumns: '.movie-title, .hall-badge, .session-id, .user-email' },
            'movieSearch': { tableBodyId: 'movieTableBody', searchColumns: '.movie-title, .director-badge' },
            'hallSearch': { tableBodyId: 'hallTableBody', searchColumns: '.hall-name, .type-badge' }
        };

        Object.keys(searchMappings).forEach(inputId => {
            if ($(`#${inputId}`).length && !$(`#${inputId}`).data('search-target')) {
                new TableSearch(inputId, searchMappings[inputId]);
            }
        });

        const tableMappings = ['ticketTable', 'sessionTable', 'paymentTable', 'ordersTable', 'movieTable', 'hallTable'];

        tableMappings.forEach(tableId => {
            const $table = $(`#${tableId}`);
            if ($table.length && $table.find('.sortable').length && !$table.data('sort-enabled')) {
                new TableSort(tableId);
            }
        });

        $('.cancel-btn').on('click', function(e) {
            const $form = $(this).closest('.cancel-form');
            if ($form.length) {
                e.preventDefault();
                if (confirm('Are you sure you want to cancel this order? This action cannot be undone.')) {
                    $form.submit();
                }
            }
        });

        setTimeout(function() {
            $('.alert-custom, .alert').fadeOut('slow');
        }, 5000);
    });


    window.TableSearch = TableSearch;
    window.TableSort = TableSort;

})(jQuery);