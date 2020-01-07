using LiveCharts;
using LiveCharts.Definitions.Series;
using LiveCharts.Wpf;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microshaoft;

namespace WpfLiveChartsApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window//, IChartViewDataSource
    {
        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }
        
        public MainWindow()
        {
            InitializeComponent();
            var json = new StreamReader(File.OpenRead(@"data.json")).ReadToEnd();
            var jObject = JObject.Parse(json)["data"];


            var seriesCollection = new SeriesCollection();


           
            seriesCollection
                    .MergeSeries<string, ColumnSeries, double>
                        (
                            jObject
                            , (x, y) =>
                            {
                                var r =
                                    string.Compare
                                    (
                                        x["bn"].Value<string>()
                                        , y["bn"].Value<string>()
                                        , true
                                    ) == 0;
                                return
                                    r;
                            }
                            , (x) =>
                            {
                                return
                                    x["bn"].Value<string>().ToLower().GetHashCode();
                            }
                            , (x) =>
                            {
                                return
                                    x["bn"].Value<string>().ToLower();
                            }
                            , (x) =>
                            {
                                return
                                    x["yyyyMM"].Value<string>().ToLower();
                            }
                            , (x) =>
                            {
                                return
                                    new ColumnSeries
                                    {
                                        Title = x.Key
                                    };
                            }
                            , (x) =>
                            {
                                return
                                    x["amount"].Value<double>();
                            }
                            , out var labels
                        );
            SeriesCollection = seriesCollection;
            Labels = labels;
            Formatter = value => value.ToString("N");
            DataContext = this;


            //var cities = jObject["data"]
            //                        .Select
            //                            (
            //                                (x) =>
            //                                {
            //                                    return
            //                                    x["bn"].Value<string>();
            //                                }
            //                            )
            //                        .Distinct()
            //                        .ToArray();

            //var groups = jObject["data"]
            //                                .GroupBy
            //                                    (
            //                                        (x) =>
            //                                        {
            //                                            return
            //                                                x["yyyyMM"].Value<string>();
            //                                        }
            //                                    );
            //SeriesCollection = new SeriesCollection();
            //foreach (var group in groups)
            //{
            //    ISeriesView seriesView = new ColumnSeries
            //    {
            //        Title = group.Key
            //    };
            //    foreach (var item in group)
            //    {
            //        if 
            //            (
            //                seriesView
            //                        .Values
            //                ==
            //                null
            //            )
            //        {
            //            seriesView
            //                    .Values = new ChartValues<double>();
            //        }
            //        seriesView
            //            .Values
            //            .Add
            //                (
            //                    item["amount"].Value<double>()
            //                );
            //    }
            //    SeriesCollection
            //                    .Add
            //                        (
            //                            seriesView
            //                        );

            //};
        }

        public TResult ChartValueFormatter<T, TResult>(T chartValue)
        {
            throw new NotImplementedException();
        }
    }
}
