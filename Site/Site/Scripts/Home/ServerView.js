/// <reference path="../jquery-1.5.1.js" />
/// <reference path="DataHandler.js" />
/// <reference path="../ThirdParty/flotr2/flotr2.js" />
/// <reference path="../ThirdParty/moment.min.js" />


var UT = UT || {};
UT.Pages = UT.Pages || {};
UT.Pages.HomeIndex = UT.Pages.HomeIndex || {};

UT.Pages.HomeIndex.ServerView = function (container, dataHandler) {
    container = $(container);
    var self = this;
    if (false)
        dataHandler = new UT.Pages.HomeIndex.DataHandler();
    var expandedDetailsIsVisible = false;
    var expandedDetailsRow;
    var expandedDetailsCell;

    var regionName;
    var serverCategory;


    function init() {
        $('.expand-details a', container).click(toggleExpandedDetails);
        expandedDetailsRow = container.next(".serverType-ExpandedDetails");
        expandedDetailsCell = $('td', expandedDetailsRow);
        regionName = container.data('region');
        serverCategory = container.data('serverCategory');
        self.update();
    }

    this.update = function () {
        var categoryModel = dataHandler.getServerCategory(regionName, serverCategory);
        updateArrowIcon(categoryModel.State);
        draw24HourGraph(categoryModel);
        draw30DayGraph(categoryModel);
        fillStats(categoryModel);
    };

    function toggleExpandedDetails() {
        if (expandedDetailsIsVisible) {
            hideExpandedDetails();
        } else {
            showExpandedDetails();
        }
        expandedDetailsIsVisible = !expandedDetailsIsVisible;
        self.update();
    }

    function showExpandedDetails() {
        expandedDetailsRow.show();

    }

    function hideExpandedDetails() {
        expandedDetailsRow.hide();
    }

    function updateArrowIcon(state) {
        if (state === "Up") {
            $('.status img', container).attr('src', '/Content/images/UpArrow.png');
        } else {
            $('.status img', container).attr('src', '/Content/images/DownArrow.png');
        }
    }

    function draw30DayGraph(categoryModel) {
        if (!expandedDetailsIsVisible)
            return;

        var dataToPlot = [];
        $.each(categoryModel.Uptime30Days, function () {
            var uptimeInTimespan = this;
            var time = moment(uptimeInTimespan.T).valueOf();
            dataToPlot.push([time, uptimeInTimespan.P * 100]);
        });

        var chartContainer = $('.30DaysChart', expandedDetailsCell).get(0);

        Flotr.draw(chartContainer, [dataToPlot], {
            yaxis: {
                min: 0,
                max: 100,
                tickFormatter: percentTickFormatter
            },
            xaxis: {
                minorTickFreq: 1,
                mode: 'time',
                tickFormatter: formatAsDate
            },
            grid: {
                minorVerticalLines: true
            }
        });
    }

    function draw24HourGraph(categoryModel) {
        if (!expandedDetailsIsVisible)
            return;

        var dataToPlot = [];
        $.each(categoryModel.UptimeLast24Hours, function () {
            var uptimeInTimespan = this;
            var time = moment(uptimeInTimespan.T).valueOf();
            dataToPlot.push([time, uptimeInTimespan.P * 100]);
        });

        var chartContainer = $('.24HourChart', expandedDetailsCell).get(0);

        Flotr.draw(chartContainer, [dataToPlot], {
            yaxis: {
                min: 0,
                max: 100,
                tickFormatter: percentTickFormatter
            },
            xaxis: {
                minorTickFreq: 4,
                mode: 'time'
            },
            grid: {
                minorVerticalLines: true
            }
        });
    }

    function fillStats(categoryModel) {
        $('.values .downtimePercentage', expandedDetailsRow).html(Math.round((categoryModel.DowntimePercentage * 100)) + '%');
        $('.values .dayOfWeek', expandedDetailsRow).html(categoryModel.DayOfWeek);
        $('.values .hourOfDay', expandedDetailsRow).html(categoryModel.HourOfDay);
    }

    function percentTickFormatter(number) {
        return number + '%';
    }

    function formatAsDate(d) {
        return moment(d).format('MMM Do');
    }

    init();
};