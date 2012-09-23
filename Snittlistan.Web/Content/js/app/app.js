﻿// @reference app.models.appstate.js
// @reference app.models.players.js
// @reference app.models.turns.js
// @reference app.views.header.js
// @reference app.views.turns.js
// @reference app.views.players.js
// @reference app.views.results.js
(function ($, backbone, app, undefined) {
    "use strict";

    // links:
    // Edit/read-only views: http://stackoverflow.com/questions/10863497/switching-between-readonly-view-and-edit-view-in-backbone-js
    // Backbone filters: http://coenraets.org/blog/2012/01/backbone-js-lessons-learned-and-improved-sample-app/
    // How to handle login: http://stackoverflow.com/questions/7220020/backbone-js-how-to-handle-login
    // Handling user login: http://stackoverflow.com/questions/5808655/backbone-js-handling-if-a-user-is-logged-in-or-not

    //var turns = new Turns();
    //turns.fetch();

    // define a custom close method
    // if special clean-up is necessary,
    // create a beforeClose function
    // see http://lostechies.com/derickbailey/2011/09/15/zombies-run-managing-page-transitions-in-backbone-apps/
    backbone.View.prototype.close = function () {
        if (this.beforeClose)
            this.beforeClose();
        this.remove();
        this.unbind();
    };

    // application router, create as instantiated singleton
    app.router = /*new (*/backbone.Router.extend({
        routes: {
            'turns': 'turns',
            'results': 'results',
            'players': 'players',
            '*other': 'turns'
        },
        initialize: function (options) {
            _.bindAll(this);
            // save references to models
            this.appState = options.app_state;
            this.turns = options.turns;
            this.seasons = options.seasons;
            this.players = options.players;
            // notify appState when changing routes
            var self = this;
            _(this.routes).each(function (route) {
                self.on('route:' + route, function () {
                    var f = self.appState[route + 'Menu'];
                    if (f === null || typeof f !== 'function')
                        throw Error('Missing handler for ' + route + 'Menu in appState');
                    f.apply(self.appState);
                });
            });
            // place a header first in the body element
            this.header = new app.Views.Header({
                model: options.app_state,
                router: this
            });
            $("body").prepend(this.header.render().el);
            // listen to login/logout
            var that = this;
            app.session.on('all', function () {
                that.showView(that.currentSelector, that.currentGetView);
            });
        },
        // handles views
        showView: function (selector, get_view) {
            if (this.currentView)
                this.currentView.close();
            var view = get_view.call(this);
            $(selector).html(view.render().el);
            this.currentView = view;
            this.currentGetView = get_view;
            this.currentSelector = selector;
            return view;
        },
/*        render: function () {
            // re-render current view
            this.showView(this.currentSelector, this.currentGetView);
        },*/
        // routes
        // show coming turns
        turns: function () {
            this.showView('#main', function () {
                return new app.Views.Turns({
                    collection: this.turns,
                    seasonStart: 2012,
                    seasonEnd: 2013
                });
            });
        },
        // show completed turns
        results: function () {
            this.showView('#main', function () {
                return new app.Views.Results();
            });
        },
        // show players
        players: function () {
            this.showView('#main', function () {
                return new app.Views.PlayersList({ model: this.players });
            });
        }
    })/*)({
        // initialize data here, and keep inside the router
        // application state keeps track of current page
        app_state: new app.Models.AppState(),
        turns: new app.Collections.Turns(app.initial_data.turns),
        players: new app.Models.PlayersList({ players: app.initial_data.players })
    })*/;

    $(function () {
        // Because hash-based history in Internet Explorer
        // relies on an <iframe>, be sure to only call start()
        // after the DOM is ready.
        backbone.history.start({ pushState: true, root: '/v2/' });
    });
}($, Backbone, window.App = window.App || { }));