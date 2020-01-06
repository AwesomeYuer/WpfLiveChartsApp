using LiveCharts;
using LiveCharts.Definitions.Series;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfLiveChartsApp
{
    public static class ChartViewHelper
    {

        private class JTokenEqualityComparer : IEqualityComparer<JToken>
        {

            private Func<JToken, JToken, bool> _onEqualsProcessFunc;
            private Func<JToken, int> _onGetHashCodeProcessFunc;
            public JTokenEqualityComparer
                (
                    Func<JToken, JToken, bool> onEqualsProcessFunc
                    , Func<JToken, int> onGetHashCodeProcessFunc
                )
            {
                _onEqualsProcessFunc = onEqualsProcessFunc;
                _onGetHashCodeProcessFunc = onGetHashCodeProcessFunc;
            }

            public bool Equals(JToken x, JToken y)
            {
                return _onEqualsProcessFunc(x, y);
            }

            public int GetHashCode(JToken obj)
            {
                return _onGetHashCodeProcessFunc(obj);
            }
        };

        public static 
            (
                string[]                //labels
                , SeriesCollection
            )
                Create<TGroupKey, TSeries, TChartValue>
                    (
                        JToken data
                        , Func<JToken, JToken, bool> onLableEqualsProcessFunc
                        , Func<JToken, int> onLableGetHashCodeProcessFunc
                        , Func<JToken, string> onLableProcessFunc
                        , Func<JToken, TGroupKey> onGroupingProcessFunc
                        , Func<IGrouping<TGroupKey, JToken>, TSeries> onSeriesViewFactoryProcessFunc
                        , Func<JToken, TChartValue> onAddChartValueProcessFunc
                    )
                        where
                            TSeries : ISeriesView//, new()
        {
            var comparer = new JTokenEqualityComparer
                                    (
                                        onLableEqualsProcessFunc
                                        , onLableGetHashCodeProcessFunc
                                    );

            var lables = data
                            .AsJEnumerable()
                            .Distinct
                                (
                                    comparer
                                )
                            .Select
                                (
                                    (x) =>
                                    {
                                        return
                                            onLableProcessFunc(x);
                                    }
                                )
                            .ToArray();
            var groups = data
                            .AsJEnumerable()
                            .GroupBy
                                (
                                    (x) =>
                                    {
                                        return
                                            onGroupingProcessFunc(x);
                                    }
                                );
            var seriesCollection = new SeriesCollection();
            foreach (var group in groups)
            {
                ISeriesView seriesView = onSeriesViewFactoryProcessFunc(group);
                foreach (var item in group)
                {
                    if
                        (
                            seriesView
                                    .Values
                            ==
                            null
                        )
                    {
                        seriesView
                                .Values = new ChartValues<TChartValue>();
                    }

                    var adding = onAddChartValueProcessFunc(item);
                    seriesView
                        .Values
                        .Add
                            (
                                adding
                            );
                }
                seriesCollection
                                .Add
                                    (
                                        seriesView
                                    );

            };


            return 
                (
                    lables
                    , seriesCollection
                );
        }

    }
}
