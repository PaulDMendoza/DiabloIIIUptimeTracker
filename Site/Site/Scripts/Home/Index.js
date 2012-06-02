/// <reference path="../jquery-1.5.1.js" />
/// <reference path="DataHandler.js" />
/// <reference path="ServerView.js" />
/// <reference path="../ThirdParty/moment.min.js" />


var UT = UT || {};
UT.Pages = UT.Pages || {};
UT.Pages.HomeIndex = UT.Pages.HomeIndex || {};

UT.Pages.HomeIndex.Controller = function () {
    var dataHandler = new UT.Pages.HomeIndex.DataHandler();

    var serverViews = [];
    $('#Home-Index .serverType').each(function () {
        var serverElem = this;
        serverViews.push(new UT.Pages.HomeIndex.ServerView(serverElem, dataHandler));
    });
    
    dataHandler.dataUpdated(function () {
        var updatedTimeText = moment().format('h:mm:ss A');
        $('#last-updated span').html(updatedTimeText);
        $.each(serverViews, function () {
            var sv = this;
            sv.update();
        });
    });

    setInterval(function () {
        dataHandler.refreshData();
    }, 1000 * 20);


};

$(function () {
    var controller = new UT.Pages.HomeIndex.Controller();
});