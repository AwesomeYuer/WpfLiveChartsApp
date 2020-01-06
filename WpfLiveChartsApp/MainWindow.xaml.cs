using LiveCharts;
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

namespace WpfLiveChartsApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            var json = new StreamReader(File.OpenRead(@"data.json")).ReadToEnd();

            var jObject = JObject.Parse(json);
            var cities = jObject["data"]
                                .Select
                                    (
                                        (x) =>
                                        {
                                            return
                                            x["bn"].Value<string>();
                                        }

                                    )
                                .Distinct()
                                .ToArray();

            var groupsYearMonth = jObject["data"]
                            .GroupBy
                                (
                                    (x) =>
                                    {
                                        return
                                            x["yyyyMM"].Value<string>();
                                    }
                                );

            SeriesCollection = new SeriesCollection();
            foreach (var group in groupsYearMonth)
            {
                Console.WriteLine(group.Key);
                ColumnSeries columnSeries = new ColumnSeries
                {
                    Title = group.Key
                    //,
                    //Values = new ChartValues<double> { 11, 56, 42 }
                };
                foreach (var item in group)
                {
                    Console.WriteLine(item["bn"].Value<string>());
                    if (columnSeries.Values == null)
                    {
                        columnSeries.Values = new ChartValues<double>();
                    }
                    columnSeries.Values.Add(item["amount"].Value<double>());
                }
                SeriesCollection
                            .Add(columnSeries);

            };
            Labels = cities;
            Formatter = value => value.ToString("N");
            DataContext = this;
        }
    }
}
