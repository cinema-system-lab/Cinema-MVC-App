function initializeDashboardChart(salesRawData) {
    const ctx = document.getElementById('salesChart');

    if (!ctx) {
        console.error('Chart canvas element not found');
        return;
    }

    const maxRev = Math.max(...salesRawData.map(d => d.rev), 100);
    const maxTix = Math.max(...salesRawData.map(d => d.tix), 10);

    new Chart(ctx.getContext('2d'), {
        type: 'line',
        data: {
            labels: salesRawData.map(d => d.date),
            datasets: [
                {
                    label: 'Revenue (₴)',
                    data: salesRawData.map(d => d.rev),
                    borderColor: '#3b82f6',
                    backgroundColor: 'rgba(59, 130, 246, 0.08)',
                    borderWidth: 2,
                    fill: true,
                    tension: 0.4,
                    yAxisID: 'y',
                    pointRadius: 4,
                    pointBackgroundColor: '#3b82f6',
                    pointBorderColor: '#2c3034',
                    pointBorderWidth: 2,
                    pointHitRadius: 10
                },
                {
                    label: 'Tickets Sold',
                    data: salesRawData.map(d => d.tix),
                    borderColor: '#f59e0b',
                    borderDash: [5, 5],
                    borderWidth: 2,
                    tension: 0.4,
                    yAxisID: 'y1',
                    pointRadius: 0
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            layout: {
                padding: { top: 20, bottom: 10, left: 15, right: 15 }
            },
            plugins: {
                legend: {
                    position: 'top',
                    align: 'end',
                    labels: {
                        color: '#94a3b8',
                        boxWidth: 12,
                        font: { size: 11, weight: '600' },
                        padding: 20
                    }
                },
                tooltip: {
                    mode: 'index',
                    intersect: false,
                    backgroundColor: '#1e293b',
                    padding: 12,
                    cornerRadius: 8
                }
            },
            scales: {
                y: {
                    position: 'left',
                    suggestedMax: maxRev * 1.15,
                    grid: {
                        color: 'rgba(255, 255, 255, 0.04)',
                        drawBorder: false
                    },
                    ticks: {
                        color: '#64748b',
                        font: { size: 10 },
                        callback: (value) => '₴' + value.toLocaleString()
                    }
                },
                y1: {
                    position: 'right',
                    suggestedMax: maxTix * 1.25,
                    grid: { display: false },
                    ticks: {
                        color: '#64748b',
                        font: { size: 10 }
                    }
                },
                x: {
                    bounds: 'ticks',
                    grid: { display: false },
                    ticks: {
                        color: '#64748b',
                        font: { size: 10 },
                        padding: 10
                    }
                }
            }
        }
    });
}