
window.loadChart = (chartId, title, legend, labelData, labelColor, valueData, baselineData, durationData) => {

    document.getElementById(chartId).innerHTML =
        '<canvas id="' + chartId + '-chart"></canvas>';

    if (labelColor == null || labelColor == undefined) {
        labelColor = 'rgb(0, 0, 255)';
    }
    else if (labelColor.toLowerCase() == "primary") {
        labelColor = 'rgb(0, 0, 255)';
    }
    else if (labelColor.toLowerCase() == "secondary") {
        labelColor = 'rgb(50, 50, 50)';
    }
    else if (labelColor.toLowerCase() == "danger") {
        labelColor = 'rgb(255, 0, 0)';
    }
    else if (labelColor.toLowerCase() == "success") {
        labelColor = 'rgb(0, 255, 0)';
    }
    else if (labelColor.toLowerCase() == "info") {
        labelColor = 'rgb(55, 55, 255)';
    }
    else if (labelColor.toLowerCase() == "warning") {
        labelColor = 'rgb(255, 55, 255)';
    }
    else {
        labelColor = 'rgb(0, 0, 255)';
    }

    var baselineColor = 'rgb(200, 200, 200)';
    var durationColor = 'rgb(255, 200, 124)';

    const data = {
        labels: labelData,
        datasets: [{
            label: legend,
            backgroundColor: labelColor,
            borderColor: labelColor,
            data: valueData,
            yAxisID: 'y',
        }, {
            backgroundColor: baselineColor,
            borderColor: baselineColor,
            data: baselineData,
            yAxisID: 'y',
        }, {
            backgroundColor: durationColor,
            borderColor: durationColor,
            data: durationData,
            type: 'bar',
            yAxisID: 'y1',
        }]
    };    

    var lineWidth = 3;

    if (valueData && valueData.length > 40) {
        lineWidth = 2;
    }

    if (valueData && valueData.length > 80) {
        lineWidth = 1;
    }

    const config = {
        type: 'line',
        data: data,
        options: {
            plugins: {
                title: {
                    display: title == "" ? false : true,
                    text: title
                },
                legend: {
                    display: legend == "" ? false : true
                },
            },
            animation: false,
            elements: {
                point: {
                    radius: lineWidth
                },
                line: {
                    borderWidth: lineWidth
                }
            },
            y: {
                type: 'linear',
                min: 0,
            },
            y1: {
                type: 'linear',
                display: true,
                position: 'right',

                // grid line settings
                grid: {
                    drawOnChartArea: false, // only want the grid lines for one axis to show up
                },
            },
        }
    };

    new Chart(
        document.getElementById(chartId + "-chart"),
        config
    );
}