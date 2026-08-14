// FinTrack Chart.js interoperability helper

window.finTrackCharts = {
    instances: {},

    renderSpendingDonutChart: function (canvasId, labels, data, backgroundColors, totalSpentFormatted) {
        const canvas = document.getElementById(canvasId);
        if (!canvas) return;

        if (this.instances[canvasId]) {
            this.instances[canvasId].destroy();
        }

        const ctx = canvas.getContext('2d');

        // Custom plugin for drawing center text (Total Spent + Amount)
        const centerTextPlugin = {
            id: 'centerTextPlugin',
            afterDraw: (chart) => {
                const { ctx, chartArea: { top, bottom, left, right, width, height } } = chart;
                ctx.save();
                
                const centerX = (left + right) / 2;
                const centerY = (top + bottom) / 2;

                // Subtitle: "Total Spent"
                ctx.font = '500 13px "Plus Jakarta Sans", sans-serif';
                ctx.fillStyle = '#64748B';
                ctx.textAlign = 'center';
                ctx.textBaseline = 'middle';
                ctx.fillText('Total Spent', centerX, centerY - 14);

                // Main total: "$4,250"
                ctx.font = '700 24px "Plus Jakarta Sans", sans-serif';
                ctx.fillStyle = '#0F172A';
                ctx.fillText(totalSpentFormatted || '$0.00', centerX, centerY + 14);

                ctx.restore();
            }
        };

        this.instances[canvasId] = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: labels,
                datasets: [{
                    data: data,
                    backgroundColor: backgroundColors || [
                        '#052E36', // Dark Teal/Navy (Rent)
                        '#047857', // Emerald Green (Food)
                        '#0E4D5E', // Deep Cyan (Utilities)
                        '#93C5FD'  // Soft Blue (Fun)
                    ],
                    borderWidth: 0,
                    hoverOffset: 4
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                cutout: '72%',
                plugins: {
                    legend: {
                        display: false // We render custom legend below chart matching Figma
                    },
                    tooltip: {
                        backgroundColor: '#0F172A',
                        padding: 10,
                        cornerRadius: 8,
                        callbacks: {
                            label: function (context) {
                                const value = context.parsed;
                                return ` $${value.toLocaleString()}`;
                            }
                        }
                    }
                }
            },
            plugins: [centerTextPlugin]
        });
    },

    renderIncomeExpenseChart: function (canvasId, labels, incomeData, expenseData) {
        const canvas = document.getElementById(canvasId);
        if (!canvas) return;

        if (this.instances[canvasId]) {
            this.instances[canvasId].destroy();
        }

        const ctx = canvas.getContext('2d');

        this.instances[canvasId] = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [
                    {
                        label: 'Income',
                        data: incomeData,
                        backgroundColor: '#047857', // Emerald Green
                        borderRadius: 6,
                        barPercentage: 0.6,
                        categoryPercentage: 0.6
                    },
                    {
                        label: 'Expenses',
                        data: expenseData,
                        backgroundColor: '#052E36', // Dark Navy/Teal
                        borderRadius: 6,
                        barPercentage: 0.6,
                        categoryPercentage: 0.6
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    x: {
                        grid: {
                            display: false
                        },
                        ticks: {
                            color: '#94A3B8',
                            font: {
                                family: '"Plus Jakarta Sans", sans-serif',
                                size: 12
                            }
                        },
                        border: {
                            display: false
                        }
                    },
                    y: {
                        grid: {
                            color: '#F1F5F9',
                            drawTicks: false
                        },
                        ticks: {
                            color: '#94A3B8',
                            font: {
                                family: '"Plus Jakarta Sans", sans-serif',
                                size: 12
                            },
                            callback: function(value) {
                                if (value >= 1000) {
                                    return '$' + (value / 1000) + 'k';
                                }
                                return '$' + value;
                            }
                        },
                        border: {
                            display: false
                        }
                    }
                },
                plugins: {
                    legend: {
                        display: false // We render custom legend header
                    },
                    tooltip: {
                        backgroundColor: '#0F172A',
                        padding: 10,
                        cornerRadius: 8
                    }
                }
            }
        });
    }
};
