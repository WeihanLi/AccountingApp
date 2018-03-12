// _getChartOption
function _getChartOption(chartType, metaData) {
    var option = {};
    switch (chartType) {
        case 'pie':
            option = {
                title: {
                    text: metaData.Title,
                    subtext: metaData.Subtitle,
                    x: 'center'
                },
                tooltip: {
                    trigger: 'item',
                    formatter: ' {b}【{c}】({d}%) '
                },
                legend: {
                    orient: 'vertical',
                    left: 'left',
                    data: metaData.names
                },
                series: [
                    {
                        type: 'pie',
                        radius: 100,
                        label: {
                            normal: {
                                formatter: ' {b}【{c}】({d}%)  '
                            }
                        },
                        data: metaData.data
                    }
                ]
            };
            break;
        case 'bar':
            option = {
                title: {
                    text: metaData.Title,
                    subtext: metaData.Subtitle,
                    x: 'center'
                },
                tooltip: {
                    trigger: 'item',
                    axisPointer: { // 坐标轴指示器，坐标轴触发有效
                        type: 'shadow' // 默认为直线，可选为：'line' | 'shadow'
                    }
                },
                grid: {
                    left: '3%',
                    right: '4%',
                    bottom: '3%',
                    containLabel: true
                },
                xAxis: [
                    {
                        type: 'category',
                        data: metaData.names,
                        axisTick: {
                            alignWithLabel: true
                        }
                    }
                ],
                yAxis: [
                    {
                        type: 'value'
                    }
                ],
                series: [
                    {
                        type: 'bar',
                        barWidth: '80%',
                        data: metaData.values
                    }
                ]
            };
            break;
        case 'line':
            option = {
                title: {
                    text: metaData.Title,
                    subtext: metaData.Subtitle,
                    x: 'left'
                },
                tooltip: {
                    trigger: 'axis'
                },
                legend: {
                    data: metaData.values,
                    x: 'right'
                },
                grid: {
                    left: '3%',
                    right: '4%',
                    bottom: '3%',
                    containLabel: true
                },
                xAxis: {
                    type: 'category',
                    boundaryGap: true,
                    data: metaData.names
                },
                yAxis: {
                    type: 'value',
                    min: 0
                },
                series: [
                ]
            };
            var min = metaData.data[0][0];
            for (var i = 0; i < metaData.values.length; i++) {
                var ser = {
                    name: metaData.values[i],
                    type: 'line',
                    data: metaData.data[i],
                    markPoint: {
                        data: [
                            { type: 'max', name: '最大值', symbol: 'circle' },
                            { type: 'min', name: '最小值', symbol: 'pin' }
                        ]
                    },
                    markLine: {
                        data: [
                            { type: 'average', name: '平均值' }
                        ]
                    }
                };
                option.series.push(ser);
                for (var j = 0; j < metaData.data[i].length; j++) {
                    if (min > metaData.data[i][j]) {
                        min = metaData.data[i][j];
                    }
                }
                option.yAxis.min = Math.round(min - min / 10);
            }
            break;
        default:
            break;
    }
    return option;
}
// loadChart
function loadChart(url, domId, chartType, title) {
    var blockSmsChart = echarts.init(document.getElementById(domId));
    blockSmsChart.showLoading();
    $.get(url,
        function (data) {
            if (data.Status === 200) {
                var metaData = data.Result;
                console.log('metaData:' + JSON.stringify(metaData));
                if (metaData.names && metaData.names.length > 0) {
                    metaData.Title = title;
                    var option = _getChartOption(chartType, metaData);
                    // 使用刚指定的配置项和数据显示图表。
                    blockSmsChart.setOption(option);
                    blockSmsChart.hideLoading();
                } else {
                    document.getElementById(domId).style.display = 'none';
                }
            } else {
                layer.msg(data.ErrorMsg);
            }
        });
}