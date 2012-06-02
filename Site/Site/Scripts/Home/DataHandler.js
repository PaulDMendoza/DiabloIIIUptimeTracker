/// <reference path="../jquery-1.5.1.js" />
/// <reference path="../ThirdParty/JSLINQ.js" />

var UT = UT || {};
UT.Pages = UT.Pages || {};
UT.Pages.HomeIndex = UT.Pages.HomeIndex || {};

UT.Pages.HomeIndex.DataHandler = function () {
    var jsonModel = window.pageModel;
    if (false)
        jsonModel = { CategoryViewModels: [] };
    var self = this;
    var DATA_UPDATE_URL = '/Home/UptimeData';

    var dataUpdatedHandlers = [];

    function init() {

    }

    this.dataUpdated = function () {
        if (typeof arguments[0] === "function") {
            dataUpdatedHandlers.push(arguments[0]);
        } else {
            $.each(dataUpdatedHandlers, function () {
                var func = this;
                func();
            });
        }
    };

    this.refreshData = function () {
        $.ajax({
            url: DATA_UPDATE_URL,
            dataType: 'JSON',
            success: function (data) {
                jsonModel = data;
                self.dataUpdated();
            }
        });
    };

    this.getServerCategory = function (region, category) {
        return JSLINQ(jsonModel.CategoryViewModels).First(function (c) { return c.Region === region && c.ServerCategory === category; });
    };


    init();
};