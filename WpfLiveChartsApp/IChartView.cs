using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfLiveChartsApp
{
    public interface IChartViewDataSource
    {
        string[] ChartLabels { set; get; }

        SeriesCollection ChartSeriesCollection { set; get; }

        TResult ChartValueFormatter<T, TResult>(T chartValue);
    }
}
