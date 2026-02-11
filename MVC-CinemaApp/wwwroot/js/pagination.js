class TablePagination {
    constructor(tableBodyId, options = {}) {
        this.tableBody = document.getElementById(tableBodyId);
        if (!this.tableBody) {
            console.error(`Table body with id "${tableBodyId}" not found`);
            return;
        }

        this.config = {
            itemsPerPage: options.itemsPerPage || 10,
            rowSelector: options.rowSelector || 'tr',
            paginationContainerId: options.paginationContainerId || 'paginationContainer',
            visibleCountId: options.visibleCountId || 'visibleCount',
            onPageChange: options.onPageChange || null,
            maxVisiblePages: options.maxVisiblePages || 7
        };

        this.currentPage = 1;
        this.filteredRows = [];

        this.initialize();
    }

    initialize() {
        this.filteredRows = Array.from(this.tableBody.querySelectorAll(this.config.rowSelector));
        this.currentPage = 1;
        this.renderPagination();
        this.displayPage(this.currentPage);
    }

    displayPage(page) {
        this.currentPage = page;
        const startIndex = (page - 1) * this.config.itemsPerPage;
        const endIndex = startIndex + this.config.itemsPerPage;

        this.filteredRows.forEach((row, index) => {
            if (index >= startIndex && index < endIndex) {
                row.style.display = '';
            } else {
                row.style.display = 'none';
            }
        });

        this.updatePaginationControls();
        this.updateVisibleCount();

        if (typeof this.config.onPageChange === 'function') {
            this.config.onPageChange(page, startIndex, endIndex);
        }
    }

    renderPagination() {
        const totalPages = Math.ceil(this.filteredRows.length / this.config.itemsPerPage);
        const paginationContainer = document.getElementById(this.config.paginationContainerId);

        if (!paginationContainer) return;

        if (totalPages <= 1) {
            paginationContainer.style.display = 'none';
            return;
        }

        paginationContainer.style.display = 'flex';
        this.updatePaginationControls();
    }

    updatePaginationControls() {
        const totalPages = Math.ceil(this.filteredRows.length / this.config.itemsPerPage);
        const prevBtn = document.getElementById('prevPage');
        const nextBtn = document.getElementById('nextPage');
        const pageInfo = document.getElementById('pageInfo');
        const pageNumbers = document.getElementById('pageNumbers');

        if (prevBtn) {
            prevBtn.disabled = this.currentPage === 1;
        }
        if (nextBtn) {
            nextBtn.disabled = this.currentPage === totalPages;
        }

        if (pageInfo) {
            const startItem = (this.currentPage - 1) * this.config.itemsPerPage + 1;
            const endItem = Math.min(this.currentPage * this.config.itemsPerPage, this.filteredRows.length);
            pageInfo.textContent = `${startItem}-${endItem} of ${this.filteredRows.length}`;
        }

        if (pageNumbers) {
            pageNumbers.innerHTML = '';
            const pages = this.getPageNumbers(this.currentPage, totalPages);

            pages.forEach(page => {
                if (page === '...') {
                    const ellipsis = document.createElement('span');
                    ellipsis.className = 'page-ellipsis';
                    ellipsis.textContent = '...';
                    pageNumbers.appendChild(ellipsis);
                } else {
                    const pageBtn = document.createElement('button');
                    pageBtn.className = 'page-number';
                    if (page === this.currentPage) {
                        pageBtn.classList.add('active');
                    }
                    pageBtn.textContent = page;
                    pageBtn.addEventListener('click', () => {
                        this.displayPage(page);
                    });
                    pageNumbers.appendChild(pageBtn);
                }
            });
        }
    }

    getPageNumbers(current, total) {
        const pages = [];
        const maxVisible = this.config.maxVisiblePages;

        if (total <= maxVisible) {
            for (let i = 1; i <= total; i++) {
                pages.push(i);
            }
        } else {
            pages.push(1);

            if (current > 3) {
                pages.push('...');
            }

            for (let i = Math.max(2, current - 1); i <= Math.min(total - 1, current + 1); i++) {
                pages.push(i);
            }

            if (current < total - 2) {
                pages.push('...');
            }

            pages.push(total);
        }

        return pages;
    }

    updateVisibleCount() {
        const visibleCount = document.getElementById(this.config.visibleCountId);
        if (visibleCount) {
            visibleCount.textContent = this.filteredRows.length;
        }
    }

    updateAfterSearch(searchResults) {
        this.filteredRows = searchResults;
        this.currentPage = 1;

        if (this.filteredRows.length === 0) {
            const paginationContainer = document.getElementById(this.config.paginationContainerId);
            if (paginationContainer) {
                paginationContainer.style.display = 'none';
            }
        } else {
            this.renderPagination();
            this.displayPage(1);
        }
    }

    nextPage() {
        const totalPages = Math.ceil(this.filteredRows.length / this.config.itemsPerPage);
        if (this.currentPage < totalPages) {
            this.displayPage(this.currentPage + 1);
        }
    }

    prevPage() {
        if (this.currentPage > 1) {
            this.displayPage(this.currentPage - 1);
        }
    }
}

document.addEventListener('DOMContentLoaded', function () {
    const paginatedTables = document.querySelectorAll('[data-pagination="true"]');

    paginatedTables.forEach(table => {
        const tbody = table.querySelector('tbody');
        if (!tbody || !tbody.id) return;

        const itemsPerPage = parseInt(table.dataset.paginationItems) || 10;
        const rowSelector = table.dataset.paginationRowSelector || 'tr';

        table.paginationInstance = new TablePagination(tbody.id, {
            itemsPerPage: itemsPerPage,
            rowSelector: rowSelector
        });
    });

    const prevBtn = document.getElementById('prevPage');
    const nextBtn = document.getElementById('nextPage');

    if (prevBtn) {
        prevBtn.addEventListener('click', function () {
            const table = document.querySelector('[data-pagination="true"]');
            if (table && table.paginationInstance) {
                table.paginationInstance.prevPage();
            }
        });
    }

    if (nextBtn) {
        nextBtn.addEventListener('click', function () {
            const table = document.querySelector('[data-pagination="true"]');
            if (table && table.paginationInstance) {
                table.paginationInstance.nextPage();
            }
        });
    }
});

if (typeof window !== 'undefined') {
    window.TablePagination = TablePagination;
}