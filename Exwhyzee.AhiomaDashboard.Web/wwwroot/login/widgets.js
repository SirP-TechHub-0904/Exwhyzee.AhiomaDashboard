"use strict";
var KTWidgets = (function () {
    var t = function (t) {
        var e = KTUtil.getById(t);
        KTUtil.on(e, "thead th .checkbox > input", "change", function (t) {
            for (var o = KTUtil.findAll(e, "tbody td .checkbox > input"), s = 0, a = o.length; s < a; s++) o[s].checked = this.checked;
        });
    };
    return {
        init: function () {
            !(function () {
                if (0 != $("#kt_dashboard_daterangepicker").length) {
                    var t = $("#kt_dashboard_daterangepicker"),
                        e = moment(),
                        o = moment();
                    t.daterangepicker(
                        {
                            direction: KTUtil.isRTL(),
                            startDate: e,
                            endDate: o,
                            opens: "left",
                            applyClass: "btn-primary",
                            cancelClass: "btn-light-primary",
                            ranges: {
                                Today: [moment(), moment()],
                                Yesterday: [moment().subtract(1, "days"), moment().subtract(1, "days")],
                                "Last 7 Days": [moment().subtract(6, "days"), moment()],
                                "Last 30 Days": [moment().subtract(29, "days"), moment()],
                                "This Month": [moment().startOf("month"), moment().endOf("month")],
                                "Last Month": [moment().subtract(1, "month").startOf("month"), moment().subtract(1, "month").endOf("month")],
                            },
                        },
                        s
                    ),
                        s(e, o, "");
                }
                function s(t, e, o) {
                    var s = "",
                        a = "";
                    e - t < 100 || "Today" == o ? ((s = "Today:"), (a = t.format("MMM D"))) : "Yesterday" == o ? ((s = "Yesterday:"), (a = t.format("MMM D"))) : (a = t.format("MMM D") + " - " + e.format("MMM D")),
                        $("#kt_dashboard_daterangepicker_date").html(a),
                        $("#kt_dashboard_daterangepicker_title").html(s);
                }
            })(),
                (function () {
                    var t = document.getElementById("kt_stats_widget_7_chart");
                    if (t) {
                        var e = {
                            series: [{ name: "Net Profit", data: [30, 45, 32, 70, 40] }],
                            chart: { type: "area", height: 150, toolbar: { show: !1 }, zoom: { enabled: !1 }, sparkline: { enabled: !0 } },
                            plotOptions: {},
                            legend: { show: !1 },
                            dataLabels: { enabled: !1 },
                            fill: { type: "solid", opacity: 1 },
                            stroke: { curve: "smooth", show: !0, width: 3, colors: [KTApp.getSettings().colors.theme.base.success] },
                            xaxis: {
                                categories: ["Feb", "Mar", "Apr", "May", "Jun", "Jul"],
                                axisBorder: { show: !1 },
                                axisTicks: { show: !1 },
                                labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                                crosshairs: { show: !1, position: "front", stroke: { color: KTApp.getSettings().colors.gray["gray-300"], width: 1, dashArray: 3 } },
                                tooltip: { enabled: !0, formatter: void 0, offsetY: 0, style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                            },
                            yaxis: { labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } } },
                            states: { normal: { filter: { type: "none", value: 0 } }, hover: { filter: { type: "none", value: 0 } }, active: { allowMultipleDataPointsSelection: !1, filter: { type: "none", value: 0 } } },
                            tooltip: {
                                style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] },
                                y: {
                                    formatter: function (t) {
                                        return "$" + t + " thousands";
                                    },
                                },
                            },
                            colors: [KTApp.getSettings().colors.theme.light.success],
                            markers: { colors: [KTApp.getSettings().colors.theme.light.success], strokeColor: [KTApp.getSettings().colors.theme.base.success], strokeWidth: 3 },
                        };
                        new ApexCharts(t, e).render();
                    }
                })(),
                (function () {
                    var t = document.getElementById("kt_stats_widget_8_chart");
                    if (t) {
                        var e = {
                            series: [{ name: "Net Profit", data: [30, 45, 32, 70, 40] }],
                            chart: { type: "area", height: 150, toolbar: { show: !1 }, zoom: { enabled: !1 }, sparkline: { enabled: !0 } },
                            plotOptions: {},
                            legend: { show: !1 },
                            dataLabels: { enabled: !1 },
                            fill: { type: "solid", opacity: 1 },
                            stroke: { curve: "smooth", show: !0, width: 3, colors: [KTApp.getSettings().colors.theme.base.danger] },
                            xaxis: {
                                categories: ["Feb", "Mar", "Apr", "May", "Jun"],
                                axisBorder: { show: !1 },
                                axisTicks: { show: !1 },
                                labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                                crosshairs: { show: !1, position: "front", stroke: { color: KTApp.getSettings().colors.gray["gray-300"], width: 1, dashArray: 3 } },
                                tooltip: { enabled: !0, formatter: void 0, offsetY: 0, style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                            },
                            yaxis: { labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } } },
                            states: { normal: { filter: { type: "none", value: 0 } }, hover: { filter: { type: "none", value: 0 } }, active: { allowMultipleDataPointsSelection: !1, filter: { type: "none", value: 0 } } },
                            tooltip: {
                                style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] },
                                y: {
                                    formatter: function (t) {
                                        return "$" + t + " thousands";
                                    },
                                },
                            },
                            colors: [KTApp.getSettings().colors.theme.light.danger],
                            markers: { colors: [KTApp.getSettings().colors.theme.light.danger], strokeColor: [KTApp.getSettings().colors.theme.base.danger], strokeWidth: 3 },
                        };
                        new ApexCharts(t, e).render();
                    }
                })(),
                (function () {
                    var t = document.getElementById("kt_stats_widget_9_chart");
                    if (t) {
                        var e = {
                            series: [{ name: "Net Profit", data: [30, 45, 32, 70, 40] }],
                            chart: { type: "area", height: 150, toolbar: { show: !1 }, zoom: { enabled: !1 }, sparkline: { enabled: !0 } },
                            plotOptions: {},
                            legend: { show: !1 },
                            dataLabels: { enabled: !1 },
                            fill: { type: "solid", opacity: 1 },
                            stroke: { curve: "smooth", show: !0, width: 3, colors: [KTApp.getSettings().colors.theme.base.primary] },
                            xaxis: {
                                categories: ["Feb", "Mar", "Apr", "May", "Jun"],
                                axisBorder: { show: !1 },
                                axisTicks: { show: !1 },
                                labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                                crosshairs: { show: !1, position: "front", stroke: { color: KTApp.getSettings().colors.gray["gray-300"], width: 1, dashArray: 3 } },
                                tooltip: { enabled: !0, formatter: void 0, offsetY: 0, style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                            },
                            yaxis: { labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } } },
                            states: { normal: { filter: { type: "none", value: 0 } }, hover: { filter: { type: "none", value: 0 } }, active: { allowMultipleDataPointsSelection: !1, filter: { type: "none", value: 0 } } },
                            tooltip: {
                                style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] },
                                y: {
                                    formatter: function (t) {
                                        return "$" + t + " thousands";
                                    },
                                },
                            },
                            colors: [KTApp.getSettings().colors.theme.light.primary],
                            markers: { colors: [KTApp.getSettings().colors.theme.light.primary], strokeColor: [KTApp.getSettings().colors.theme.base.primary], strokeWidth: 3 },
                        };
                        new ApexCharts(t, e).render();
                    }
                })(),
                (function () {
                    var t = document.getElementById("kt_stats_widget_10_chart"),
                        e = parseInt(KTUtil.css(t, "height")),
                        o = KTUtil.hasAttr(t, "data-color") ? KTUtil.attr(t, "data-color") : "info";
                    if (t) {
                        var s = {
                            series: [{ name: "Net Profit", data: [40, 40, 30, 30, 35, 35, 50] }],
                            chart: { type: "area", height: e, toolbar: { show: !1 }, zoom: { enabled: !1 }, sparkline: { enabled: !0 } },
                            plotOptions: {},
                            legend: { show: !1 },
                            dataLabels: { enabled: !1 },
                            fill: { type: "solid", opacity: 1 },
                            stroke: { curve: "smooth", show: !0, width: 3, colors: [KTApp.getSettings().colors.theme.base[o]] },
                            xaxis: {
                                categories: ["Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug"],
                                axisBorder: { show: !1 },
                                axisTicks: { show: !1 },
                                labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                                crosshairs: { show: !1, position: "front", stroke: { color: KTApp.getSettings().colors.gray["gray-300"], width: 1, dashArray: 3 } },
                                tooltip: { enabled: !0, formatter: void 0, offsetY: 0, style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                            },
                            yaxis: { min: 0, max: 55, labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } } },
                            states: { normal: { filter: { type: "none", value: 0 } }, hover: { filter: { type: "none", value: 0 } }, active: { allowMultipleDataPointsSelection: !1, filter: { type: "none", value: 0 } } },
                            tooltip: {
                                style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] },
                                y: {
                                    formatter: function (t) {
                                        return "$" + t + " thousands";
                                    },
                                },
                            },
                            colors: [KTApp.getSettings().colors.theme.light[o]],
                            markers: { colors: [KTApp.getSettings().colors.theme.light[o]], strokeColor: [KTApp.getSettings().colors.theme.base[o]], strokeWidth: 3 },
                        };
                        new ApexCharts(t, s).render();
                    }
                })(),
                (function () {
                    var t = document.getElementById("kt_stats_widget_11_chart"),
                        e = (parseInt(KTUtil.css(t, "height")), KTUtil.hasAttr(t, "data-color") ? KTUtil.attr(t, "data-color") : "success");
                    if (t) {
                        var o = {
                            series: [{ name: "Net Profit", data: [40, 40, 30, 30, 35, 35, 50] }],
                            chart: { type: "area", height: 150, toolbar: { show: !1 }, zoom: { enabled: !1 }, sparkline: { enabled: !0 } },
                            plotOptions: {},
                            legend: { show: !1 },
                            dataLabels: { enabled: !1 },
                            fill: { type: "solid", opacity: 1 },
                            stroke: { curve: "smooth", show: !0, width: 3, colors: [KTApp.getSettings().colors.theme.base[e]] },
                            xaxis: {
                                categories: ["Feb", "Mar", "Apr", "May", "Jun", "Aug", "Sep"],
                                axisBorder: { show: !1 },
                                axisTicks: { show: !1 },
                                labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                                crosshairs: { show: !1, position: "front", stroke: { color: KTApp.getSettings().colors.gray["gray-300"], width: 1, dashArray: 3 } },
                                tooltip: { enabled: !0, formatter: void 0, offsetY: 0, style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                            },
                            yaxis: { min: 0, max: 55, labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } } },
                            states: { normal: { filter: { type: "none", value: 0 } }, hover: { filter: { type: "none", value: 0 } }, active: { allowMultipleDataPointsSelection: !1, filter: { type: "none", value: 0 } } },
                            tooltip: {
                                style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] },
                                y: {
                                    formatter: function (t) {
                                        return "$" + t + " thousands";
                                    },
                                },
                            },
                            colors: [KTApp.getSettings().colors.theme.light[e]],
                            markers: { colors: [KTApp.getSettings().colors.theme.light[e]], strokeColor: [KTApp.getSettings().colors.theme.base[e]], strokeWidth: 3 },
                        };
                        new ApexCharts(t, o).render();
                    }
                })(),
                (function () {
                    var t = document.getElementById("kt_stats_widget_12_chart"),
                        e = parseInt(KTUtil.css(t, "height")),
                        o = KTUtil.hasAttr(t, "data-color") ? KTUtil.attr(t, "data-color") : "primary";
                    if (t) {
                        var s = {
                            series: [{ name: "Net Profit", data: [40, 40, 30, 30, 35, 35, 50] }],
                            chart: { type: "area", height: e, toolbar: { show: !1 }, zoom: { enabled: !1 }, sparkline: { enabled: !0 } },
                            plotOptions: {},
                            legend: { show: !1 },
                            dataLabels: { enabled: !1 },
                            fill: { type: "solid", opacity: 1 },
                            stroke: { curve: "smooth", show: !0, width: 3, colors: [KTApp.getSettings().colors.theme.base[o]] },
                            xaxis: {
                                categories: ["Feb", "Mar", "Apr", "May", "Jun", "Aug", "Sep"],
                                axisBorder: { show: !1 },
                                axisTicks: { show: !1 },
                                labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                                crosshairs: { show: !1, position: "front", stroke: { color: KTApp.getSettings().colors.gray["gray-300"], width: 1, dashArray: 3 } },
                                tooltip: { enabled: !0, formatter: void 0, offsetY: 0, style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                            },
                            yaxis: { min: 0, max: 55, labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } } },
                            states: { normal: { filter: { type: "none", value: 0 } }, hover: { filter: { type: "none", value: 0 } }, active: { allowMultipleDataPointsSelection: !1, filter: { type: "none", value: 0 } } },
                            tooltip: {
                                style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] },
                                y: {
                                    formatter: function (t) {
                                        return "$" + t + " thousands";
                                    },
                                },
                            },
                            colors: [KTApp.getSettings().colors.theme.light[o]],
                            markers: { colors: [KTApp.getSettings().colors.theme.light[o]], strokeColor: [KTApp.getSettings().colors.theme.base[o]], strokeWidth: 3 },
                        };
                        new ApexCharts(t, s).render();
                    }
                })(),
                (function () {
                    var t = document.getElementById("kt_charts_widget_1_chart");
                    if (t) {
                        var e = {
                            series: [
                                { name: "Net Profit", data: [44, 55, 57, 56, 61, 58] },
                                { name: "Revenue", data: [76, 85, 101, 98, 87, 105] },
                            ],
                            chart: { type: "bar", height: 350, toolbar: { show: !1 } },
                            plotOptions: { bar: { horizontal: !1, columnWidth: ["30%"], endingShape: "rounded" } },
                            legend: { show: !1 },
                            dataLabels: { enabled: !1 },
                            stroke: { show: !0, width: 2, colors: ["transparent"] },
                            xaxis: {
                                categories: ["Feb", "Mar", "Apr", "May", "Jun", "Jul"],
                                axisBorder: { show: !1 },
                                axisTicks: { show: !1 },
                                labels: { style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                            },
                            yaxis: { labels: { style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } } },
                            fill: { opacity: 1 },
                            states: { normal: { filter: { type: "none", value: 0 } }, hover: { filter: { type: "none", value: 0 } }, active: { allowMultipleDataPointsSelection: !1, filter: { type: "none", value: 0 } } },
                            tooltip: {
                                style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] },
                                y: {
                                    formatter: function (t) {
                                        return "$" + t + " thousands";
                                    },
                                },
                            },
                            colors: [KTApp.getSettings().colors.theme.base.success, KTApp.getSettings().colors.gray["gray-300"]],
                            grid: { borderColor: KTApp.getSettings().colors.gray["gray-200"], strokeDashArray: 4, yaxis: { lines: { show: !0 } } },
                        };
                        new ApexCharts(t, e).render();
                    }
                })(),
                (function () {
                    var t = document.getElementById("kt_charts_widget_2_chart");
                    if (t) {
                        var e = {
                            series: [
                                { name: "Net Profit", data: [44, 55, 57, 56, 61, 58] },
                                { name: "Revenue", data: [76, 85, 101, 98, 87, 105] },
                            ],
                            chart: { type: "bar", height: 350, toolbar: { show: !1 } },
                            plotOptions: { bar: { horizontal: !1, columnWidth: ["30%"], endingShape: "rounded" } },
                            legend: { show: !1 },
                            dataLabels: { enabled: !1 },
                            stroke: { show: !0, width: 2, colors: ["transparent"] },
                            xaxis: {
                                categories: ["Feb", "Mar", "Apr", "May", "Jun", "Jul"],
                                axisBorder: { show: !1 },
                                axisTicks: { show: !1 },
                                labels: { style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                            },
                            yaxis: { labels: { style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } } },
                            fill: { opacity: 1 },
                            states: { normal: { filter: { type: "none", value: 0 } }, hover: { filter: { type: "none", value: 0 } }, active: { allowMultipleDataPointsSelection: !1, filter: { type: "none", value: 0 } } },
                            tooltip: {
                                style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] },
                                y: {
                                    formatter: function (t) {
                                        return "$" + t + " thousands";
                                    },
                                },
                            },
                            colors: [KTApp.getSettings().colors.theme.base.warning, KTApp.getSettings().colors.gray["gray-300"]],
                            grid: { borderColor: KTApp.getSettings().colors.gray["gray-200"], strokeDashArray: 4, yaxis: { lines: { show: !0 } } },
                        };
                        new ApexCharts(t, e).render();
                    }
                })(),
                (function () {
                    var t = document.getElementById("kt_charts_widget_3_chart");
                    if (t) {
                        var e = {
                            series: [{ name: "Net Profit", data: [30, 40, 40, 90, 90, 70, 70] }],
                            chart: { type: "area", height: 350, toolbar: { show: !1 } },
                            plotOptions: {},
                            legend: { show: !1 },
                            dataLabels: { enabled: !1 },
                            fill: { type: "solid", opacity: 1 },
                            stroke: { curve: "smooth", show: !0, width: 3, colors: [KTApp.getSettings().colors.theme.base.info] },
                            xaxis: {
                                categories: ["Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug"],
                                axisBorder: { show: !1 },
                                axisTicks: { show: !1 },
                                labels: { style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                                crosshairs: { position: "front", stroke: { color: KTApp.getSettings().colors.theme.base.info, width: 1, dashArray: 3 } },
                                tooltip: { enabled: !0, formatter: void 0, offsetY: 0, style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                            },
                            yaxis: { labels: { style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } } },
                            states: { normal: { filter: { type: "none", value: 0 } }, hover: { filter: { type: "none", value: 0 } }, active: { allowMultipleDataPointsSelection: !1, filter: { type: "none", value: 0 } } },
                            tooltip: {
                                style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] },
                                y: {
                                    formatter: function (t) {
                                        return "$" + t + " thousands";
                                    },
                                },
                            },
                            colors: [KTApp.getSettings().colors.theme.light.info],
                            grid: { borderColor: KTApp.getSettings().colors.gray["gray-200"], strokeDashArray: 4, yaxis: { lines: { show: !0 } } },
                            markers: { strokeColor: KTApp.getSettings().colors.theme.base.info, strokeWidth: 3 },
                        };
                        new ApexCharts(t, e).render();
                    }
                })(),
                (function () {
                    var t = document.getElementById("kt_charts_widget_4_chart");
                    if (t) {
                        var e = {
                            series: [
                                { name: "Net Profit", data: [60, 50, 80, 40, 100, 60] },
                                { name: "Revenue", data: [70, 60, 110, 40, 50, 70] },
                            ],
                            chart: { type: "area", height: 350, toolbar: { show: !1 } },
                            plotOptions: {},
                            legend: { show: !1 },
                            dataLabels: { enabled: !1 },
                            fill: { type: "solid", opacity: 1 },
                            stroke: { curve: "smooth" },
                            xaxis: {
                                categories: ["Feb", "Mar", "Apr", "May", "Jun", "Jul"],
                                axisBorder: { show: !1 },
                                axisTicks: { show: !1 },
                                labels: { style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                                crosshairs: { position: "front", stroke: { color: KTApp.getSettings().colors.theme.light.success, width: 1, dashArray: 3 } },
                                tooltip: { enabled: !0, formatter: void 0, offsetY: 0, style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                            },
                            yaxis: { labels: { style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } } },
                            states: { normal: { filter: { type: "none", value: 0 } }, hover: { filter: { type: "none", value: 0 } }, active: { allowMultipleDataPointsSelection: !1, filter: { type: "none", value: 0 } } },
                            tooltip: {
                                style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] },
                                y: {
                                    formatter: function (t) {
                                        return "$" + t + " thousands";
                                    },
                                },
                            },
                            colors: [KTApp.getSettings().colors.theme.base.success, KTApp.getSettings().colors.theme.base.warning],
                            grid: { borderColor: KTApp.getSettings().colors.gray["gray-200"], strokeDashArray: 4, yaxis: { lines: { show: !0 } } },
                            markers: {
                                colors: [KTApp.getSettings().colors.theme.light.success, KTApp.getSettings().colors.theme.light.warning],
                                strokeColor: [KTApp.getSettings().colors.theme.light.success, KTApp.getSettings().colors.theme.light.warning],
                                strokeWidth: 3,
                            },
                        };
                        new ApexCharts(t, e).render();
                    }
                })(),
                (function () {
                    var t = document.getElementById("kt_charts_widget_5_chart");
                    if (t) {
                        var e = {
                            series: [
                                { name: "Net Profit", data: [40, 50, 65, 70, 50, 30] },
                                { name: "Revenue", data: [-30, -40, -55, -60, -40, -20] },
                            ],
                            chart: { type: "bar", stacked: !0, height: 350, toolbar: { show: !1 } },
                            plotOptions: { bar: { horizontal: !1, columnWidth: ["12%"], endingShape: "rounded" } },
                            legend: { show: !1 },
                            dataLabels: { enabled: !1 },
                            stroke: { show: !0, width: 2, colors: ["transparent"] },
                            xaxis: {
                                categories: ["Feb", "Mar", "Apr", "May", "Jun", "Jul"],
                                axisBorder: { show: !1 },
                                axisTicks: { show: !1 },
                                labels: { style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                            },
                            yaxis: { min: -80, max: 80, labels: { style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } } },
                            fill: { opacity: 1 },
                            states: { normal: { filter: { type: "none", value: 0 } }, hover: { filter: { type: "none", value: 0 } }, active: { allowMultipleDataPointsSelection: !1, filter: { type: "none", value: 0 } } },
                            tooltip: {
                                style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] },
                                y: {
                                    formatter: function (t) {
                                        return "$" + t + " thousands";
                                    },
                                },
                            },
                            colors: [KTApp.getSettings().colors.theme.base.info, KTApp.getSettings().colors.theme.base.primary],
                            grid: { borderColor: KTApp.getSettings().colors.gray["gray-200"], strokeDashArray: 4, yaxis: { lines: { show: !0 } } },
                        };
                        new ApexCharts(t, e).render();
                    }
                })(),
                (function () {
                    var t = document.getElementById("kt_charts_widget_6_chart");
                    if (t) {
                        var e = {
                            series: [
                                { name: "Net Profit", type: "bar", stacked: !0, data: [40, 50, 65, 70, 50, 30] },
                                { name: "Revenue", type: "bar", stacked: !0, data: [20, 20, 25, 30, 30, 20] },
                                { name: "Expenses", type: "area", data: [50, 80, 60, 90, 50, 70] },
                            ],
                            chart: { stacked: !0, height: 350, toolbar: { show: !1 } },
                            plotOptions: { bar: { stacked: !0, horizontal: !1, endingShape: "rounded", columnWidth: ["12%"] } },
                            legend: { show: !1 },
                            dataLabels: { enabled: !1 },
                            stroke: { curve: "smooth", show: !0, width: 2, colors: ["transparent"] },
                            xaxis: {
                                categories: ["Feb", "Mar", "Apr", "May", "Jun", "Jul"],
                                axisBorder: { show: !1 },
                                axisTicks: { show: !1 },
                                labels: { style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                            },
                            yaxis: { max: 120, labels: { style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } } },
                            fill: { opacity: 1 },
                            states: { normal: { filter: { type: "none", value: 0 } }, hover: { filter: { type: "none", value: 0 } }, active: { allowMultipleDataPointsSelection: !1, filter: { type: "none", value: 0 } } },
                            tooltip: {
                                style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] },
                                y: {
                                    formatter: function (t) {
                                        return "$" + t + " thousands";
                                    },
                                },
                            },
                            colors: [KTApp.getSettings().colors.theme.base.info, KTApp.getSettings().colors.theme.base.primary, KTApp.getSettings().colors.theme.light.primary],
                            grid: { borderColor: KTApp.getSettings().colors.gray["gray-200"], strokeDashArray: 4, yaxis: { lines: { show: !0 } }, padding: { top: 0, right: 0, bottom: 0, left: 0 } },
                        };
                        new ApexCharts(t, e).render();
                    }
                })(),
                (function () {
                    var t = document.getElementById("kt_charts_widget_7_chart");
                    if (t) {
                        var e = {
                            series: [
                                { name: "Net Profit", data: [30, 30, 50, 50, 35, 35] },
                                { name: "Revenue", data: [55, 20, 20, 20, 70, 70] },
                                { name: "Expenses", data: [60, 60, 40, 40, 30, 30] },
                            ],
                            chart: { type: "area", height: 300, toolbar: { show: !1 }, zoom: { enabled: !1 }, sparkline: { enabled: !0 } },
                            plotOptions: {},
                            legend: { show: !1 },
                            dataLabels: { enabled: !1 },
                            fill: { type: "solid", opacity: 1 },
                            stroke: { curve: "smooth", show: !0, width: 2, colors: [KTApp.getSettings().colors.theme.base.warning, "transparent", "transparent"] },
                            xaxis: {
                                categories: ["Feb", "Mar", "Apr", "May", "Jun", "Jul"],
                                axisBorder: { show: !1 },
                                axisTicks: { show: !1 },
                                labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                                crosshairs: { show: !1, position: "front", stroke: { color: KTApp.getSettings().colors.gray["gray-300"], width: 1, dashArray: 3 } },
                                tooltip: { enabled: !0, formatter: void 0, offsetY: 0, style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                            },
                            yaxis: { labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } } },
                            states: { normal: { filter: { type: "none", value: 0 } }, hover: { filter: { type: "none", value: 0 } }, active: { allowMultipleDataPointsSelection: !1, filter: { type: "none", value: 0 } } },
                            tooltip: {
                                style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] },
                                y: {
                                    formatter: function (t) {
                                        return "$" + t + " thousands";
                                    },
                                },
                            },
                            colors: [KTApp.getSettings().colors.theme.light.warning, KTApp.getSettings().colors.theme.light.info, KTApp.getSettings().colors.gray["gray-100"]],
                            grid: { borderColor: KTApp.getSettings().colors.gray["gray-200"], strokeDashArray: 4, yaxis: { lines: { show: !0 } } },
                            markers: {
                                colors: [KTApp.getSettings().colors.theme.light.warning, KTApp.getSettings().colors.theme.light.info, KTApp.getSettings().colors.gray["gray-100"]],
                                strokeColor: [KTApp.getSettings().colors.theme.base.warning, KTApp.getSettings().colors.theme.base.info, KTApp.getSettings().colors.gray["gray-500"]],
                                strokeWidth: 3,
                            },
                        };
                        new ApexCharts(t, e).render();
                    }
                })(),
                (function () {
                    var t = document.getElementById("kt_charts_widget_8_chart");
                    if (t) {
                        var e = {
                            series: [
                                { name: "Net Profit", data: [30, 30, 50, 50, 35, 35] },
                                { name: "Revenue", data: [55, 20, 20, 20, 70, 70] },
                                { name: "Expenses", data: [60, 60, 40, 40, 30, 30] },
                            ],
                            chart: { type: "area", height: 300, toolbar: { show: !1 }, zoom: { enabled: !1 }, sparkline: { enabled: !0 } },
                            plotOptions: {},
                            legend: { show: !1 },
                            dataLabels: { enabled: !1 },
                            fill: { type: "solid", opacity: 1 },
                            stroke: { curve: "smooth", show: !0, width: 2, colors: ["transparent", "transparent", "transparent"] },
                            xaxis: {
                                x: 0,
                                offsetX: 0,
                                offsetY: 0,
                                padding: { left: 0, right: 0, top: 0 },
                                categories: ["Feb", "Mar", "Apr", "May", "Jun", "Jul"],
                                axisBorder: { show: !1 },
                                axisTicks: { show: !1 },
                                labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                                crosshairs: { show: !1, position: "front", stroke: { color: KTApp.getSettings().colors.gray["gray-300"], width: 1, dashArray: 3 } },
                                tooltip: { enabled: !0, formatter: void 0, offsetY: 0, style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                            },
                            yaxis: {
                                y: 0,
                                offsetX: 0,
                                offsetY: 0,
                                padding: { left: 0, right: 0 },
                                labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                            },
                            states: { normal: { filter: { type: "none", value: 0 } }, hover: { filter: { type: "none", value: 0 } }, active: { allowMultipleDataPointsSelection: !1, filter: { type: "none", value: 0 } } },
                            tooltip: {
                                style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] },
                                y: {
                                    formatter: function (t) {
                                        return "$" + t + " thousands";
                                    },
                                },
                            },
                            colors: [KTApp.getSettings().colors.theme.light.success, KTApp.getSettings().colors.theme.light.danger, KTApp.getSettings().colors.theme.light.info],
                            grid: { borderColor: KTApp.getSettings().colors.gray["gray-200"], strokeDashArray: 4, padding: { top: 0, bottom: 0, left: 0, right: 0 } },
                            markers: {
                                colors: [KTApp.getSettings().colors.theme.light.success, KTApp.getSettings().colors.theme.light.danger, KTApp.getSettings().colors.theme.light.info],
                                strokeColor: [KTApp.getSettings().colors.theme.base.success, KTApp.getSettings().colors.theme.base.danger, KTApp.getSettings().colors.theme.base.info],
                                strokeWidth: 3,
                            },
                        };
                        new ApexCharts(t, e).render();
                    }
                })(),
                (function () {
                    var t = document.getElementById("kt_mixed_widget_1_chart"),
                        e = parseInt(KTUtil.css(t, "height"));
                    if (t) {
                        var o = {
                            series: [{ name: "Net Profit", data: [30, 45, 32, 70, 40, 40, 40] }],
                            chart: {
                                type: "area",
                                height: e,
                                toolbar: { show: !1 },
                                zoom: { enabled: !1 },
                                sparkline: { enabled: !0 },
                                dropShadow: { enabled: !0, enabledOnSeries: void 0, top: 5, left: 0, blur: 3, color: "#D13647", opacity: 0.5 },
                            },
                            plotOptions: {},
                            legend: { show: !1 },
                            dataLabels: { enabled: !1 },
                            fill: { type: "solid", opacity: 0 },
                            stroke: { curve: "smooth", show: !0, width: 3, colors: ["#D13647"] },
                            xaxis: {
                                categories: ["Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug"],
                                axisBorder: { show: !1 },
                                axisTicks: { show: !1 },
                                labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                                crosshairs: { show: !1, position: "front", stroke: { color: KTApp.getSettings().colors.gray["gray-300"], width: 1, dashArray: 3 } },
                            },
                            yaxis: { min: 0, max: 80, labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } } },
                            states: { normal: { filter: { type: "none", value: 0 } }, hover: { filter: { type: "none", value: 0 } }, active: { allowMultipleDataPointsSelection: !1, filter: { type: "none", value: 0 } } },
                            tooltip: {
                                style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] },
                                y: {
                                    formatter: function (t) {
                                        return "$" + t + " thousands";
                                    },
                                },
                                marker: { show: !1 },
                            },
                            colors: ["transparent"],
                            markers: { colors: [KTApp.getSettings().colors.theme.light.danger], strokeColor: ["#D13647"], strokeWidth: 3 },
                        };
                        new ApexCharts(t, o).render();
                    }
                })(),
                (function () {
                    var t = document.getElementById("kt_mixed_widget_2_chart"),
                        e = parseInt(KTUtil.css(t, "height"));
                    if (t) {
                        var o = {
                            series: [{ name: "Net Profit", data: [30, 45, 32, 70, 40, 40, 40] }],
                            chart: {
                                type: "area",
                                height: e,
                                toolbar: { show: !1 },
                                zoom: { enabled: !1 },
                                sparkline: { enabled: !0 },
                                dropShadow: { enabled: !0, enabledOnSeries: void 0, top: 5, left: 0, blur: 3, color: "#287ED7", opacity: 0.5 },
                            },
                            plotOptions: {},
                            legend: { show: !1 },
                            dataLabels: { enabled: !1 },
                            fill: { type: "solid", opacity: 0 },
                            stroke: { curve: "smooth", show: !0, width: 3, colors: ["#287ED7"] },
                            xaxis: {
                                categories: ["Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug"],
                                axisBorder: { show: !1 },
                                axisTicks: { show: !1 },
                                labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                                crosshairs: { show: !1, position: "front", stroke: { color: KTApp.getSettings().colors.gray["gray-300"], width: 1, dashArray: 3 } },
                                tooltip: { enabled: !0, formatter: void 0, offsetY: 0, style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                            },
                            yaxis: { min: 0, max: 80, labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } } },
                            states: { normal: { filter: { type: "none", value: 0 } }, hover: { filter: { type: "none", value: 0 } }, active: { allowMultipleDataPointsSelection: !1, filter: { type: "none", value: 0 } } },
                            tooltip: {
                                style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] },
                                y: {
                                    formatter: function (t) {
                                        return "$" + t + " thousands";
                                    },
                                },
                                marker: { show: !1 },
                            },
                            colors: ["transparent"],
                            markers: { colors: [KTApp.getSettings().colors.theme.light.info], strokeColor: ["#287ED7"], strokeWidth: 3 },
                        };
                        new ApexCharts(t, o).render();
                    }
                })(),
                (function () {
                    var t = document.getElementById("kt_mixed_widget_3_chart"),
                        e = parseInt(KTUtil.css(t, "height"));
                    if (t) {
                        var o = KTApp.getSettings().colors.theme.base.white,
                            s = {
                                series: [{ name: "Net Profit", data: [30, 45, 32, 70, 40, 40, 40] }],
                                chart: {
                                    type: "area",
                                    height: e,
                                    toolbar: { show: !1 },
                                    zoom: { enabled: !1 },
                                    sparkline: { enabled: !0 },
                                    dropShadow: { enabled: !0, enabledOnSeries: void 0, top: 5, left: 0, blur: 3, color: o, opacity: 0.3 },
                                },
                                plotOptions: {},
                                legend: { show: !1 },
                                dataLabels: { enabled: !1 },
                                fill: { type: "solid", opacity: 0 },
                                stroke: { curve: "smooth", show: !0, width: 3, colors: [o] },
                                xaxis: {
                                    categories: ["Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug"],
                                    axisBorder: { show: !1 },
                                    axisTicks: { show: !1 },
                                    labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                                    crosshairs: { show: !1, position: "front", stroke: { color: KTApp.getSettings().colors.gray["gray-300"], width: 1, dashArray: 3 } },
                                    tooltip: { enabled: !0, formatter: void 0, offsetY: 0, style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                                },
                                yaxis: { min: 0, max: 80, labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } } },
                                states: { normal: { filter: { type: "none", value: 0 } }, hover: { filter: { type: "none", value: 0 } }, active: { allowMultipleDataPointsSelection: !1, filter: { type: "none", value: 0 } } },
                                tooltip: {
                                    style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] },
                                    y: {
                                        formatter: function (t) {
                                            return "$" + t + " thousands";
                                        },
                                    },
                                    marker: { show: !1 },
                                },
                                colors: ["transparent"],
                                markers: { colors: [KTApp.getSettings().colors.theme.light.dark], strokeColor: [o], strokeWidth: 3 },
                            };
                        new ApexCharts(t, s).render();
                    }
                })(),
                (function () {
                    var t = document.getElementById("kt_mixed_widget_4_chart"),
                        e = parseInt(KTUtil.css(t, "height"));
                    if (t) {
                        var o = {
                            series: [
                                { name: "Net Profit", data: [35, 65, 75, 55, 45, 60, 55] },
                                { name: "Revenue", data: [40, 70, 80, 60, 50, 65, 60] },
                            ],
                            chart: { type: "bar", height: e, toolbar: { show: !1 }, sparkline: { enabled: !0 } },
                            plotOptions: { bar: { horizontal: !1, columnWidth: ["30%"], endingShape: "rounded" } },
                            legend: { show: !1 },
                            dataLabels: { enabled: !1 },
                            stroke: { show: !0, width: 1, colors: ["transparent"] },
                            xaxis: {
                                categories: ["Feb", "Mar", "Apr", "May", "Jun", "Jul"],
                                axisBorder: { show: !1 },
                                axisTicks: { show: !1 },
                                labels: { style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                            },
                            yaxis: { min: 0, max: 100, labels: { style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } } },
                            fill: { type: ["solid", "solid"], opacity: [0.25, 1] },
                            states: { normal: { filter: { type: "none", value: 0 } }, hover: { filter: { type: "none", value: 0 } }, active: { allowMultipleDataPointsSelection: !1, filter: { type: "none", value: 0 } } },
                            tooltip: {
                                style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] },
                                y: {
                                    formatter: function (t) {
                                        return "$" + t + " thousands";
                                    },
                                },
                                marker: { show: !1 },
                            },
                            colors: ["#ffffff", "#ffffff"],
                            grid: { borderColor: KTApp.getSettings().colors.gray["gray-200"], strokeDashArray: 4, yaxis: { lines: { show: !0 } }, padding: { left: 20, right: 20 } },
                        };
                        new ApexCharts(t, o).render();
                    }
                })(),
                (function () {
                    var t = document.getElementById("kt_mixed_widget_5_chart"),
                        e = parseInt(KTUtil.css(t, "height"));
                    if (t) {
                        var o = {
                            series: [
                                { name: "Net Profit", data: [35, 65, 75, 55, 45, 60, 55] },
                                { name: "Revenue", data: [40, 70, 80, 60, 50, 65, 60] },
                            ],
                            chart: { type: "bar", height: e, toolbar: { show: !1 }, sparkline: { enabled: !0 } },
                            plotOptions: { bar: { horizontal: !1, columnWidth: ["30%"], endingShape: "rounded" } },
                            legend: { show: !1 },
                            dataLabels: { enabled: !1 },
                            stroke: { show: !0, width: 1, colors: ["transparent"] },
                            xaxis: {
                                categories: ["Feb", "Mar", "Apr", "May", "Jun", "Jul"],
                                axisBorder: { show: !1 },
                                axisTicks: { show: !1 },
                                labels: { style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                            },
                            yaxis: { min: 0, max: 100, labels: { style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } } },
                            fill: { type: ["solid", "solid"], opacity: [0.25, 1] },
                            states: { normal: { filter: { type: "none", value: 0 } }, hover: { filter: { type: "none", value: 0 } }, active: { allowMultipleDataPointsSelection: !1, filter: { type: "none", value: 0 } } },
                            tooltip: {
                                style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] },
                                y: {
                                    formatter: function (t) {
                                        return "$" + t + " thousands";
                                    },
                                },
                                marker: { show: !1 },
                            },
                            colors: ["#ffffff", "#ffffff"],
                            grid: { borderColor: KTApp.getSettings().colors.gray["gray-200"], strokeDashArray: 4, yaxis: { lines: { show: !0 } }, padding: { left: 20, right: 20 } },
                        };
                        new ApexCharts(t, o).render();
                    }
                })(),
                (function () {
                    var t = document.getElementById("kt_mixed_widget_6_chart"),
                        e = parseInt(KTUtil.css(t, "height"));
                    if (t) {
                        var o = {
                            series: [
                                { name: "Net Profit", data: [35, 65, 75, 55, 45, 60, 55] },
                                { name: "Revenue", data: [40, 70, 80, 60, 50, 65, 60] },
                            ],
                            chart: { type: "bar", height: e, toolbar: { show: !1 }, sparkline: { enabled: !0 } },
                            plotOptions: { bar: { horizontal: !1, columnWidth: ["30%"], endingShape: "rounded" } },
                            legend: { show: !1 },
                            dataLabels: { enabled: !1 },
                            stroke: { show: !0, width: 1, colors: ["transparent"] },
                            xaxis: {
                                categories: ["Feb", "Mar", "Apr", "May", "Jun", "Jul"],
                                axisBorder: { show: !1 },
                                axisTicks: { show: !1 },
                                labels: { style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                            },
                            yaxis: { min: 0, max: 100, labels: { style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } } },
                            fill: { type: ["solid", "solid"], opacity: [0.25, 1] },
                            states: { normal: { filter: { type: "none", value: 0 } }, hover: { filter: { type: "none", value: 0 } }, active: { allowMultipleDataPointsSelection: !1, filter: { type: "none", value: 0 } } },
                            tooltip: {
                                style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] },
                                y: {
                                    formatter: function (t) {
                                        return "$" + t + " thousands";
                                    },
                                },
                                marker: { show: !1 },
                            },
                            colors: ["#ffffff", "#ffffff"],
                            grid: { borderColor: KTApp.getSettings().colors.gray["gray-200"], strokeDashArray: 4, yaxis: { lines: { show: !0 } }, padding: { left: 20, right: 20 } },
                        };
                        new ApexCharts(t, o).render();
                    }
                })(),
                (function () {
                    var t = document.getElementById("kt_mixed_widget_13_chart"),
                        e = parseInt(KTUtil.css(t, "height"));
                    if (t) {
                        var o = {
                            series: [{ name: "Net Profit", data: [30, 25, 45, 30, 55, 55] }],
                            chart: { type: "area", height: e, toolbar: { show: !1 }, zoom: { enabled: !1 }, sparkline: { enabled: !0 } },
                            plotOptions: {},
                            legend: { show: !1 },
                            dataLabels: { enabled: !1 },
                            fill: { type: "solid", opacity: 1 },
                            stroke: { curve: "smooth", show: !0, width: 3, colors: [KTApp.getSettings().colors.theme.base.info] },
                            xaxis: {
                                categories: ["Feb", "Mar", "Apr", "May", "Jun", "Jul"],
                                axisBorder: { show: !1 },
                                axisTicks: { show: !1 },
                                labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                                crosshairs: { show: !1, position: "front", stroke: { color: KTApp.getSettings().colors.gray["gray-300"], width: 1, dashArray: 3 } },
                                tooltip: { enabled: !0, formatter: void 0, offsetY: 0, style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                            },
                            yaxis: { min: 0, max: 60, labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } } },
                            states: { normal: { filter: { type: "none", value: 0 } }, hover: { filter: { type: "none", value: 0 } }, active: { allowMultipleDataPointsSelection: !1, filter: { type: "none", value: 0 } } },
                            tooltip: {
                                style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] },
                                y: {
                                    formatter: function (t) {
                                        return "$" + t + " thousands";
                                    },
                                },
                            },
                            colors: [KTApp.getSettings().colors.theme.light.info],
                            markers: { colors: [KTApp.getSettings().colors.theme.light.info], strokeColor: [KTApp.getSettings().colors.theme.base.info], strokeWidth: 3 },
                        };
                        new ApexCharts(t, o).render();
                    }
                })(),
                (function () {
                    var t = document.getElementById("kt_mixed_widget_14_chart"),
                        e = parseInt(KTUtil.css(t, "height"));
                    if (t) {
                        var o = {
                            series: [74],
                            chart: { height: e, type: "radialBar" },
                            plotOptions: {
                                radialBar: {
                                    hollow: { margin: 0, size: "65%" },
                                    dataLabels: { showOn: "always", name: { show: !1, fontWeight: "700" }, value: { color: KTApp.getSettings().colors.gray["gray-700"], fontSize: "30px", fontWeight: "700", offsetY: 12, show: !0 } },
                                    track: { background: KTApp.getSettings().colors.theme.light.success, strokeWidth: "100%" },
                                },
                            },
                            colors: [KTApp.getSettings().colors.theme.base.success],
                            stroke: { lineCap: "round" },
                            labels: ["Progress"],
                        };
                        new ApexCharts(t, o).render();
                    }
                })(),
                (function () {
                    var t = document.getElementById("kt_mixed_widget_15_chart"),
                        e = parseInt(KTUtil.css(t, "height"));
                    if (t) {
                        var o = {
                            series: [{ name: "Net Profit", data: [30, 30, 60, 25, 25, 40] }],
                            chart: { type: "area", height: e, toolbar: { show: !1 }, zoom: { enabled: !1 }, sparkline: { enabled: !0 } },
                            plotOptions: {},
                            legend: { show: !1 },
                            dataLabels: { enabled: !1 },
                            fill: { type: "gradient", opacity: 1, gradient: { type: "vertical", shadeIntensity: 0.5, gradientToColors: void 0, inverseColors: !0, opacityFrom: 1, opacityTo: 0.375, stops: [25, 50, 100], colorStops: [] } },
                            stroke: { curve: "smooth", show: !0, width: 3, colors: [KTApp.getSettings().colors.theme.base.danger] },
                            xaxis: {
                                categories: ["Feb", "Mar", "Apr", "May", "Jun", "Jul"],
                                axisBorder: { show: !1 },
                                axisTicks: { show: !1 },
                                labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                                crosshairs: { show: !1, position: "front", stroke: { color: KTApp.getSettings().colors.gray["gray-300"], width: 1, dashArray: 3 } },
                                tooltip: { enabled: !0, formatter: void 0, offsetY: 0, style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                            },
                            yaxis: { min: 0, max: 65, labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } } },
                            states: { normal: { filter: { type: "none", value: 0 } }, hover: { filter: { type: "none", value: 0 } }, active: { allowMultipleDataPointsSelection: !1, filter: { type: "none", value: 0 } } },
                            tooltip: {
                                style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] },
                                y: {
                                    formatter: function (t) {
                                        return "$" + t + " thousands";
                                    },
                                },
                            },
                            colors: [KTApp.getSettings().colors.theme.light.danger],
                            markers: { colors: [KTApp.getSettings().colors.theme.light.danger], strokeColor: [KTApp.getSettings().colors.theme.base.danger], strokeWidth: 3 },
                        };
                        new ApexCharts(t, o).render();
                    }
                })(),
                (function () {
                    var t = document.getElementById("kt_mixed_widget_16_chart"),
                        e = parseInt(KTUtil.css(t, "height"));
                    if (t) {
                        var o = {
                            series: [60, 50, 75, 80],
                            chart: { height: e, type: "radialBar" },
                            plotOptions: {
                                radialBar: {
                                    hollow: { margin: 0, size: "30%" },
                                    dataLabels: {
                                        showOn: "always",
                                        name: { show: !1, fontWeight: "700" },
                                        value: { color: KTApp.getSettings().colors.gray["gray-700"], fontSize: "18px", fontWeight: "700", offsetY: 10, show: !0 },
                                        total: {
                                            show: !0,
                                            label: "Total",
                                            fontWeight: "bold",
                                            formatter: function (t) {
                                                return "60%";
                                            },
                                        },
                                    },
                                    track: { background: KTApp.getSettings().colors.gray["gray-100"], strokeWidth: "100%" },
                                },
                            },
                            colors: [KTApp.getSettings().colors.theme.base.info, KTApp.getSettings().colors.theme.base.danger, KTApp.getSettings().colors.theme.base.success, KTApp.getSettings().colors.theme.base.primary],
                            stroke: { lineCap: "round" },
                            labels: ["Progress"],
                        };
                        new ApexCharts(t, o).render();
                    }
                })(),
                (function () {
                    var t = document.getElementById("kt_mixed_widget_17_chart"),
                        e = KTUtil.hasAttr(t, "data-color") ? KTUtil.attr(t, "data-color") : "warning",
                        o = parseInt(KTUtil.css(t, "height"));
                    if (t) {
                        var s = {
                            series: [{ name: "Net Profit", data: [30, 25, 45, 30, 55, 55] }],
                            chart: { type: "area", height: o, toolbar: { show: !1 }, zoom: { enabled: !1 }, sparkline: { enabled: !0 } },
                            plotOptions: {},
                            legend: { show: !1 },
                            dataLabels: { enabled: !1 },
                            fill: { type: "solid", opacity: 1 },
                            stroke: { curve: "smooth", show: !0, width: 3, colors: [KTApp.getSettings().colors.theme.base[e]] },
                            xaxis: {
                                categories: ["Feb", "Mar", "Apr", "May", "Jun", "Jul"],
                                axisBorder: { show: !1 },
                                axisTicks: { show: !1 },
                                labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                                crosshairs: { show: !1, position: "front", stroke: { color: KTApp.getSettings().colors.gray["gray-300"], width: 1, dashArray: 3 } },
                                tooltip: { enabled: !0, formatter: void 0, offsetY: 0, style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                            },
                            yaxis: { min: 0, max: 60, labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } } },
                            states: { normal: { filter: { type: "none", value: 0 } }, hover: { filter: { type: "none", value: 0 } }, active: { allowMultipleDataPointsSelection: !1, filter: { type: "none", value: 0 } } },
                            tooltip: {
                                style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] },
                                y: {
                                    formatter: function (t) {
                                        return "$" + t + " thousands";
                                    },
                                },
                            },
                            colors: [KTApp.getSettings().colors.theme.light[e]],
                            markers: { colors: [KTApp.getSettings().colors.theme.light[e]], strokeColor: [KTApp.getSettings().colors.theme.base[e]], strokeWidth: 3 },
                        };
                        new ApexCharts(t, s).render();
                    }
                })(),
                (function () {
                    var t = document.getElementById("kt_mixed_widget_18_chart"),
                        e = parseInt(KTUtil.css(t, "height"));
                    if (t) {
                        var o = {
                            series: [74],
                            chart: { height: e, type: "radialBar", offsetY: 0 },
                            plotOptions: {
                                radialBar: {
                                    startAngle: -90,
                                    endAngle: 90,
                                    hollow: { margin: 0, size: "70%" },
                                    dataLabels: {
                                        showOn: "always",
                                        name: { show: !0, fontSize: "13px", fontWeight: "700", offsetY: -5, color: KTApp.getSettings().colors.gray["gray-500"] },
                                        value: { color: KTApp.getSettings().colors.gray["gray-700"], fontSize: "30px", fontWeight: "700", offsetY: -40, show: !0 },
                                    },
                                    track: { background: KTApp.getSettings().colors.theme.light.primary, strokeWidth: "100%" },
                                },
                            },
                            colors: [KTApp.getSettings().colors.theme.base.primary],
                            stroke: { lineCap: "round" },
                            labels: ["Progress"],
                        };
                        new ApexCharts(t, o).render();
                    }
                })(),
                (function () {
                    var t = document.getElementById("kt_tiles_widget_1_chart"),
                        e = KTUtil.hasAttr(t, "data-color") ? KTUtil.attr(t, "data-color") : "primary",
                        o = parseInt(KTUtil.css(t, "height"));
                    if (t) {
                        var s = {
                            series: [{ name: "Net Profit", data: [20, 22, 30, 28, 25, 26, 30, 28, 22, 24, 25, 35] }],
                            chart: { type: "area", height: o, toolbar: { show: !1 }, zoom: { enabled: !1 }, sparkline: { enabled: !0 } },
                            plotOptions: {},
                            legend: { show: !1 },
                            dataLabels: { enabled: !1 },
                            fill: { type: "gradient", opacity: 1, gradient: { type: "vertical", shadeIntensity: 0.55, gradientToColors: void 0, inverseColors: !0, opacityFrom: 1, opacityTo: 0.2, stops: [25, 50, 100], colorStops: [] } },
                            stroke: { curve: "smooth", show: !0, width: 3, colors: [KTApp.getSettings().colors.theme.base[e]] },
                            xaxis: {
                                categories: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
                                axisBorder: { show: !1 },
                                axisTicks: { show: !1 },
                                labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                                crosshairs: { show: !1, position: "front", stroke: { color: KTApp.getSettings().colors.gray["gray-300"], width: 1, dashArray: 3 } },
                                tooltip: { enabled: !0, formatter: void 0, offsetY: 0, style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                            },
                            yaxis: { min: 0, max: 37, labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } } },
                            states: { normal: { filter: { type: "none", value: 0 } }, hover: { filter: { type: "none", value: 0 } }, active: { allowMultipleDataPointsSelection: !1, filter: { type: "none", value: 0 } } },
                            tooltip: {
                                style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] },
                                y: {
                                    formatter: function (t) {
                                        return "$" + t + " thousands";
                                    },
                                },
                            },
                            colors: [KTApp.getSettings().colors.theme.light[e]],
                            markers: { colors: [KTApp.getSettings().colors.theme.light[e]], strokeColor: [KTApp.getSettings().colors.theme.base[e]], strokeWidth: 3 },
                            padding: { top: 0, bottom: 0 },
                        };
                        new ApexCharts(t, s).render();
                    }
                })(),
                (function () {
                    var t = document.getElementById("kt_tiles_widget_2_chart");
                    if (t) {
                        var e = KTUtil.colorDarken(KTApp.getSettings().colors.theme.base.danger, 20),
                            o = KTUtil.colorDarken(KTApp.getSettings().colors.theme.base.danger, 10),
                            s = {
                                series: [{ name: "Net Profit", data: [10, 10, 20, 20, 12, 12] }],
                                chart: { type: "area", height: 75, zoom: { enabled: !1 }, sparkline: { enabled: !0 }, padding: { top: 0, bottom: 0 } },
                                dataLabels: { enabled: !1 },
                                fill: { type: "solid", opacity: 1 },
                                stroke: { curve: "smooth", show: !0, width: 3, colors: [e] },
                                xaxis: {
                                    categories: ["Feb", "Mar", "Apr", "May", "Jun", "Jul"],
                                    axisBorder: { show: !1 },
                                    axisTicks: { show: !1 },
                                    labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                                    crosshairs: { show: !1, position: "front", stroke: { color: KTApp.getSettings().colors.gray["gray-300"], width: 1, dashArray: 3 } },
                                },
                                yaxis: { min: 0, max: 22, labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } } },
                                states: { normal: { filter: { type: "none", value: 0 } }, hover: { filter: { type: "none", value: 0 } }, active: { allowMultipleDataPointsSelection: !1, filter: { type: "none", value: 0 } } },
                                tooltip: {
                                    style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] },
                                    fixed: { enabled: !1 },
                                    x: { show: !1 },
                                    y: {
                                        title: {
                                            formatter: function (t) {
                                                return t + "";
                                            },
                                        },
                                    },
                                },
                                colors: [o],
                                markers: { colors: [KTApp.getSettings().colors.theme.light.danger], strokeColor: [e], strokeWidth: 3 },
                            };
                        new ApexCharts(t, s).render();
                    }
                })(),
                (function () {
                    var t = document.getElementById("kt_tiles_widget_5_chart");
                    if (t) {
                        var e = {
                            series: [
                                { name: "Net Profit", data: [10, 15, 18, 14] },
                                { name: "Revenue", data: [8, 13, 16, 12] },
                            ],
                            chart: { type: "bar", height: 75, zoom: { enabled: !1 }, sparkline: { enabled: !0 }, padding: { left: 20, right: 20 } },
                            plotOptions: { bar: { horizontal: !1, columnWidth: ["25%"], endingShape: "rounded" } },
                            dataLabels: { enabled: !1 },
                            fill: { type: ["solid", "gradient"], opacity: 0.25 },
                            xaxis: { categories: ["Feb", "Mar", "Apr", "May"] },
                            yaxis: { min: 0, max: 20 },
                            states: { normal: { filter: { type: "none", value: 0 } }, hover: { filter: { type: "none", value: 0 } }, active: { allowMultipleDataPointsSelection: !1, filter: { type: "none", value: 0 } } },
                            tooltip: {
                                style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] },
                                fixed: { enabled: !1 },
                                x: { show: !1 },
                                y: {
                                    title: {
                                        formatter: function (t) {
                                            return t + "";
                                        },
                                    },
                                },
                                marker: { show: !1 },
                            },
                            colors: ["#ffffff", "#ffffff"],
                        };
                        new ApexCharts(t, e).render();
                    }
                })(),
                (function () {
                    var t = document.getElementById("kt_tiles_widget_8_chart"),
                        e = (parseInt(KTUtil.css(t, "height")), KTUtil.hasAttr(t, "data-color") ? KTUtil.attr(t, "data-color") : "danger");
                    if (t) {
                        var o = {
                            series: [{ name: "Net Profit", data: [20, 20, 30, 15, 40, 30] }],
                            chart: { type: "area", height: 150, toolbar: { show: !1 }, zoom: { enabled: !1 }, sparkline: { enabled: !0 } },
                            plotOptions: {},
                            legend: { show: !1 },
                            dataLabels: { enabled: !1 },
                            fill: { type: "solid" },
                            stroke: { curve: "smooth", show: !0, width: 3, colors: [KTApp.getSettings().colors.theme.base[e]] },
                            xaxis: {
                                categories: ["Jan", "Feb", "Mar", "Apr", "May", "Jun"],
                                axisBorder: { show: !1 },
                                axisTicks: { show: !1 },
                                labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                                crosshairs: { show: !1, position: "front", stroke: { color: KTApp.getSettings().colors.gray["gray-300"], width: 1, dashArray: 3 } },
                                tooltip: { enabled: !0, formatter: void 0, offsetY: 0, style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                            },
                            yaxis: { min: 0, max: 45, labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } } },
                            states: { normal: { filter: { type: "none", value: 0 } }, hover: { filter: { type: "none", value: 0 } }, active: { allowMultipleDataPointsSelection: !1, filter: { type: "none", value: 0 } } },
                            tooltip: {
                                style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] },
                                y: {
                                    formatter: function (t) {
                                        return "$" + t + " thousands";
                                    },
                                },
                            },
                            colors: [KTApp.getSettings().colors.theme.light[e]],
                            markers: { colors: [KTApp.getSettings().colors.theme.light[e]], strokeColor: [KTApp.getSettings().colors.theme.base[e]], strokeWidth: 3 },
                            padding: { top: 0, bottom: 0 },
                        };
                        new ApexCharts(t, o).render();
                    }
                })(),
                (function () {
                    var t = document.getElementById("kt_tiles_widget_17_chart");
                    if (t) {
                        var e = {
                            series: [{ name: "Net Profit", data: [10, 20, 20, 8] }],
                            chart: { type: "area", height: 150, zoom: { enabled: !1 }, sparkline: { enabled: !0 }, padding: { top: 0, bottom: 0 } },
                            dataLabels: { enabled: !1 },
                            fill: { type: "solid", opacity: 1 },
                            stroke: { curve: "smooth", show: !0, width: 3, colors: [KTApp.getSettings().colors.theme.base.success] },
                            xaxis: {
                                categories: ["Feb", "Mar", "Apr", "May"],
                                axisBorder: { show: !1 },
                                axisTicks: { show: !1 },
                                labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                                crosshairs: { show: !1, position: "front", stroke: { color: KTApp.getSettings().colors.gray["gray-300"], width: 1, dashArray: 3 } },
                            },
                            yaxis: { min: 0, max: 22, labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } } },
                            states: { normal: { filter: { type: "none", value: 0 } }, hover: { filter: { type: "none", value: 0 } }, active: { allowMultipleDataPointsSelection: !1, filter: { type: "none", value: 0 } } },
                            tooltip: {
                                style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] },
                                fixed: { enabled: !1 },
                                x: { show: !1 },
                                y: {
                                    title: {
                                        formatter: function (t) {
                                            return t + "";
                                        },
                                    },
                                },
                            },
                            colors: [KTApp.getSettings().colors.theme.light.success],
                            markers: { colors: [KTApp.getSettings().colors.theme.light.success], strokeColor: [KTApp.getSettings().colors.theme.base.success], strokeWidth: 3 },
                        };
                        new ApexCharts(t, e).render();
                    }
                })(),
                (function () {
                    var t = document.getElementById("kt_tiles_widget_20_chart_xxx");
                    if (t) {
                        var e = {
                            series: [44],
                            chart: { height: 250, type: "radialBar", offsetY: 0 },
                            plotOptions: {
                                radialBar: {
                                    startAngle: -90,
                                    endAngle: 90,
                                    hollow: { margin: 0, size: "70%" },
                                    dataLabels: {
                                        showOn: "always",
                                        name: { show: !0, fontSize: "13px", fontWeight: "400", offsetY: -5, color: KTApp.getSettings().colors.gray["gray-300"] },
                                        value: { color: KTApp.getSettings().colors.theme.inverse.primary, fontSize: "22px", fontWeight: "bold", offsetY: -40, show: !0 },
                                    },
                                    track: { background: KTUtil.colorLighten(KTApp.getSettings().colors.theme.base.primary, 7), strokeWidth: "100%" },
                                },
                            },
                            colors: [KTApp.getSettings().colors.theme.inverse.primary],
                            stroke: { lineCap: "round" },
                            labels: ["Progress"],
                        };
                        new ApexCharts(t, e).render();
                    }
                })(),
                (function () {
                    var t = document.getElementById("kt_tiles_widget_21_chart"),
                        e = parseInt(KTUtil.css(t, "height")),
                        o = KTUtil.hasAttr(t, "data-color") ? KTUtil.attr(t, "data-color") : "info";
                    if (t) {
                        var s = {
                            series: [{ name: "Net Profit", data: [20, 20, 30, 15, 30, 30] }],
                            chart: { type: "area", height: e, toolbar: { show: !1 }, zoom: { enabled: !1 }, sparkline: { enabled: !0 } },
                            plotOptions: {},
                            legend: { show: !1 },
                            dataLabels: { enabled: !1 },
                            fill: { type: "solid", opacity: 1 },
                            stroke: { curve: "smooth", show: !0, width: 3, colors: [KTApp.getSettings().colors.theme.base[o]] },
                            xaxis: {
                                categories: ["Feb", "Mar", "Apr", "May", "Jun", "Jul"],
                                axisBorder: { show: !1 },
                                axisTicks: { show: !1 },
                                labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                                crosshairs: { show: !1, position: "front", stroke: { color: KTApp.getSettings().colors.gray["gray-300"], width: 1, dashArray: 3 } },
                                tooltip: { enabled: !0, formatter: void 0, offsetY: 0, style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                            },
                            yaxis: { min: 0, max: 32, labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } } },
                            states: { normal: { filter: { type: "none", value: 0 } }, hover: { filter: { type: "none", value: 0 } }, active: { allowMultipleDataPointsSelection: !1, filter: { type: "none", value: 0 } } },
                            tooltip: {
                                style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] },
                                y: {
                                    formatter: function (t) {
                                        return "$" + t + " thousands";
                                    },
                                },
                            },
                            colors: [KTApp.getSettings().colors.theme.light[o]],
                            markers: { colors: [KTApp.getSettings().colors.theme.light[o]], strokeColor: [KTApp.getSettings().colors.theme.base[o]], strokeWidth: 3 },
                        };
                        new ApexCharts(t, s).render();
                    }
                })(),
                (function () {
                    var t = document.getElementById("kt_tiles_widget_23_chart"),
                        e = (parseInt(KTUtil.css(t, "height")), KTUtil.hasAttr(t, "data-color") ? KTUtil.attr(t, "data-color") : "primary");
                    if (t) {
                        var o = {
                            series: [{ name: "Net Profit", data: [15, 25, 15, 40, 20, 50] }],
                            chart: { type: "area", height: 125, toolbar: { show: !1 }, zoom: { enabled: !1 }, sparkline: { enabled: !0 } },
                            plotOptions: {},
                            legend: { show: !1 },
                            dataLabels: { enabled: !1 },
                            fill: { type: "solid", opacity: 1 },
                            stroke: { curve: "smooth", show: !0, width: 3, colors: [KTApp.getSettings().colors.theme.base[e]] },
                            xaxis: {
                                categories: ["Jan, 2020", "Feb, 2020", "Mar, 2020", "Apr, 2020", "May, 2020", "Jun, 2020"],
                                axisBorder: { show: !1 },
                                axisTicks: { show: !1 },
                                labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                                crosshairs: { show: !1, position: "front", stroke: { color: KTApp.getSettings().colors.gray["gray-300"], width: 1, dashArray: 3 } },
                                tooltip: { enabled: !0, formatter: void 0, offsetY: 0, style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } },
                            },
                            yaxis: { min: 0, max: 55, labels: { show: !1, style: { colors: KTApp.getSettings().colors.gray["gray-500"], fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] } } },
                            states: { normal: { filter: { type: "none", value: 0 } }, hover: { filter: { type: "none", value: 0 } }, active: { allowMultipleDataPointsSelection: !1, filter: { type: "none", value: 0 } } },
                            tooltip: {
                                style: { fontSize: "12px", fontFamily: KTApp.getSettings()["font-family"] },
                                y: {
                                    formatter: function (t) {
                                        return "$" + t + " thousands";
                                    },
                                },
                            },
                            colors: [KTApp.getSettings().colors.theme.light[e]],
                            markers: { colors: [KTApp.getSettings().colors.theme.light[e]], strokeColor: [KTApp.getSettings().colors.theme.base[e]], strokeWidth: 3 },
                        };
                        new ApexCharts(t, o).render();
                    }
                })(),
                t("kt_advance_table_widget_1"),
                t("kt_advance_table_widget_2"),
                t("kt_advance_table_widget_3"),
                t("kt_advance_table_widget_4");
        },
    };
})();
"undefined" != typeof module && (module.exports = KTWidgets),
    jQuery(document).ready(function () {
        KTWidgets.init();
    });
