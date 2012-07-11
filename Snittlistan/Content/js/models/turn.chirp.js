﻿(function (app_global, backbone, undefined) {
    "use strict";

    var turn = function (args) {
        var that = { };
        args = args || { };

        var season = function () {
            return args.season || '';
        };
        that.season = season;

        var number = function () {
            return args.number || 0;
        };
        that.number = number;

        var date_start = function () {
            return args.date_start || new Date();
        };
        that.date_start = date_start;

        var date_end = function () {
            return args.date_end || new Date();
        };
        that.date_end = date_end;

        return that;
    };

    app_global.turn = turn;
    app_global.Turn = backbone.Model.extend(turn);
})(window.App = window.App || { }, Backbone);