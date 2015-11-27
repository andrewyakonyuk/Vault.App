/*global define */

define(function (require, exports, module) {
    'use strict';

    var Pace = require('pace');
    Pace.start({
        restartOnPushState: true
    });

    var scaleFix = require('utils/scaleFix'),
        hideUrlBarOnLoad = require('utils/hideUrlBar'),
        enableActive = require('utils/enableActive');

    //config mobile helper functions
    scaleFix();
    hideUrlBarOnLoad();
    enableActive();

    if (window.addEventListener) {
        window.addEventListener('hashchange', function () {
            if (Pace.options.restartOnPushState) {
                Pace.restart();
            }
        }, false);
    } else if (window.attachEvent) {
        window.attachEvent('onhashchange', function () {
            if (Pace.options.restartOnPushState) {
                Pace.restart();
            }
        });
    }

    var router = require('packages/router'),
        ko = require('knockout'),
        validation = require('knockout-validation');

    //knockout validation issue: https://github.com/Knockout-Contrib/Knockout-Validation/issues/259
    window.ko = ko;

    require('bindings/controller');
    require('bindings/i18n');
    require('bindings/route')

    // Components can be packaged as AMD modules, such as the following:
    ko.components.register('nav-bar', {
        require: 'components/nav-bar/nav-bar'
    });
    ko.components.register('kudos', {
        require: 'components/kudos/kudos'
    });
    ko.components.register('breadcrumbs', {
        require: 'components/breadcrumbs/breadcrumbs'
    });

    ko.components.register('home-page', {
        require: 'pages/home-page/home'
    });
    ko.components.register('signin-page', {
        require: 'pages/signin-page/signin'
    });
    ko.components.register('article-page', {
        require: 'pages/article-page/article'
    });
    ko.components.register('register-page', {
        require: 'pages/register-page/register'
    });
    ko.components.register('search-page', {
        require: 'pages/search-page/search'
    });
    ko.components.register('notfound-page', {
        require: 'pages/notfound-page/notfound'
    });
    ko.components.register('collection-page', {
        require: 'pages/collection-page/collection'
    });
    ko.components.register('dashboard-page', {
        require: 'pages/dashboard-page/dashboard'
    });
    ko.components.register('settings-page', {
        require: 'pages/settings-page/settings'
    });
    ko.components.register('labels-page', {
        require: 'pages/labels-page/labels'
    });
    ko.components.register('maps-page', {
       require: 'pages/maps-page/maps'
    });
    ko.components.register('musics-page', {
        require: 'pages/musics-page/musics'
    });
    ko.components.register('calendar-page', {
        require: 'pages/calendar-page/calendar'
    });

    // ... or for template-only components, you can just point to a .html file directly:
    ko.components.register('about-page', {
        template: {
            require: 'text!pages/about-page/about.html'
        }
    });

    // [Scaffolded component registrations will be inserted here. To retain this feature, don't remove this comment.]

    var auth = require('packages/auth');
    // Start the application
    ko.applyBindings({
        route: router.currentRoute,
        authorized: ko.computed(function(){
            return auth.isAuthorized();
        },this)
    });
});
