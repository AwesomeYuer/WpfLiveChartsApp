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


            var seriesCollection =
                    new SeriesCollection()
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
                                                x["bn"]
                                                    .Value<string>()
                                                    .ToLower()
                                                    .GetHashCode();
                                        }
                                        , (x) =>
                                        {
                                            return
                                                x["bn"]
                                                    .Value<string>()
                                                    .ToLower();
                                        }
                                        , (x) =>
                                        {
                                            return
                                                x["yyyyMM"]
                                                    .Value<string>()
                                                    .ToLower();
                                        }
                                        , (x) =>
                                        {
                                            return
                                                x.Key;
                                        }
                                        , (x) =>
                                        {
                                            return
                                                x["amount"].Value<double>();
                                        }
                                        , out var labels
                                    )
                            .MergeSeries<string, LineSeries, double>
                                    (
                                        jObject
                                        , (x, y) =>
                                        {
                                            var r =
                                                    (
                                                        string
                                                            .Compare
                                                                (
                                                                    x["yyyyMM"]
                                                                            .Value<string>()
                                                                    , y["yyyyMM"]
                                                                            .Value<string>()
                                                                    , true
                                                                )
                                                        ==
                                                        0
                                                    );
                                            return
                                                    r;
                                        }
                                        , (x) =>
                                        {
                                            return
                                                x["yyyyMM"]
                                                    .Value<string>()
                                                    .ToLower()
                                                    .GetHashCode();
                                        }
                                        , (x) =>
                                        {
                                            return
                                                x["yyyyMM"]
                                                    .Value<string>()
                                                    .ToLower();
                                        }
                                        , (x) =>
                                        {
                                            return
                                                x["bn"]
                                                    .Value<string>()
                                                    .ToLower();
                                        }
                                        , (x) =>
                                        {
                                            return
                                                x.Key;
                                        }
                                        , (x) =>
                                        {
                                            return
                                                x["amount"]
                                                    .Value<double>();
                                        }
                                        , out var labels2
                                    );
            labels.ToList().AddRange(labels2);
            SeriesCollection = seriesCollection;
            Labels = labels2.ToArray();
            Formatter = value => value.ToString("N");
            DataContext = this;
        }
    }
}
