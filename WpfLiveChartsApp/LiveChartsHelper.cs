namespace Microshaoft
{
    using LiveCharts;
    using LiveCharts.Definitions.Series;
    using LiveCharts.Wpf;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    public static class LiveChartsHelper
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
                _onEqualsProcessFunc
                                = onEqualsProcessFunc;
                _onGetHashCodeProcessFunc
                                = onGetHashCodeProcessFunc;
            }
            public bool Equals(JToken x, JToken y)
            {
                return
                    _onEqualsProcessFunc(x, y);
            }
            public int GetHashCode(JToken target)
            {
                return
                    _onGetHashCodeProcessFunc(target);
            }
        };
        public static SeriesCollection
                MergeSeries<TGroupKey, TSeries, TChartValue>
                    (
                        this SeriesCollection target
                        , JToken data
                        , Func<JToken, JToken, bool>
                                            onLableDistinctEqualsPredictFunc
                        , Func<JToken, int> 
                                            onLableDistinctGetHashCodeProcessFunc
                        , Func<JToken, string>
                                            onLableFactoryProcessFunc
                        , Func<JToken, TGroupKey>
                                            onGroupingProcessFunc
                        , Func<IGrouping<TGroupKey, JToken>, string>
                                            onSetSeriesViewTitleProcessFunc
                        , Func<JToken, TChartValue>
                                            onAddChartValueProcessFunc
                        , out string[] labels
                    )
                        where
                            TSeries : Series, new()
        {
            var comparer = new JTokenEqualityComparer
                                    (
                                        onLableDistinctEqualsPredictFunc
                                        , onLableDistinctGetHashCodeProcessFunc
                                    );
            labels = data
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
                                        onLableFactoryProcessFunc(x);
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
            foreach (var group in groups)
            {
                ISeriesView seriesView = new TSeries
                {
                    Title = onSetSeriesViewTitleProcessFunc(group)
                };
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
                target
                    .Add
                        (
                            seriesView
                        );
            };
            return
                target;
        }
    }
}
