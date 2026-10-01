// Turns a CSS variable like 'var(--cat-food)' into the colour it currently holds,
// so charts follow the light/dark theme defined in app.css.
function resolveThemeColor(value, fallback) {
    if (typeof value === 'string' && value.trim().startsWith('var(')) {
        const name = value.trim().slice(4, -1).trim();
        const resolved = getComputedStyle(document.documentElement).getPropertyValue(name).trim();
        return resolved || fallback;
    }
    return value || fallback;
}
// FinTrack Chart.js interoperability helper

window.finTrackCharts = {
    instances: {},

    renderSpendingDonutChart: function (canvasId, labels, data, backgroundColors, totalSpentFormatted) {
        const canvas = document.getElementById(canvasId);
        if (!canvas) return;

        if (this.instances[canvasId]) {
            this.instances[canvasId].destroy();
            delete this.instances[canvasId];
        }

        if (!data || data.length === 0 || data.every(v => !v || v === 0)) {
            return;
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

                                // Main total: "GH₵4,250"
                ctx.font = '700 24px "Plus Jakarta Sans", sans-serif';
                ctx.fillStyle = getComputedStyle(document.documentElement).getPropertyValue('--text-primary').trim() || '#0F172A';
                ctx.fillText(totalSpentFormatted || 'GH\u20B50.00', centerX, centerY + 14);

                ctx.restore();
            }
        };

        this.instances[canvasId] = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: labels,
                datasets: [{
                    data: data,
                    backgroundColor: (backgroundColors || []).map(c => resolveThemeColor(c, '#64748B')),
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
                                return ` GH\u20B5${value.toLocaleString()}`;
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
            delete this.instances[canvasId];
        }

        if (!labels || labels.length === 0 || (!incomeData?.length && !expenseData?.length)) {
            return;
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
                        backgroundColor: resolveThemeColor('var(--chart-income)', '#059669'),
                        borderRadius: 6,
                        barPercentage: 0.6,
                        categoryPercentage: 0.6
                    },
                    {
                        label: 'Expenses',
                        data: expenseData,
                        backgroundColor: resolveThemeColor('var(--chart-expense)', '#DC2626'),
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
                                    return 'GH\u20B5' + (value / 1000) + 'k';
                                }
                                return 'GH\u20B5' + value;
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
