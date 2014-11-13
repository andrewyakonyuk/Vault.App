/*global define, boilerplate */

define(['pace',
        'jquery',
        'knockout',
        './router',
        'knockout-projections',
        'knockout-validation',
        './ko.bindinghandlers',
       'bootstrap',
        'ripples',
       'bootstrap-material-design',
       './plugins',
       './helpers',
        './fastbutton'],
    function (Pace, $, ko, router) {
        'use strict';

        window.ko = ko;

        Pace.start({
            restartOnPushState: false
        });

        //config mobile helper functions
        boilerplate.scaleFix();
        boilerplate.hideUrlBarOnLoad();
        boilerplate.enableActive();

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

        // Components can be packaged as AMD modules, such as the following:
        ko.components.register('nav-bar', {
            require: 'components/nav-bar/nav-bar'
        });

        ko.components.register('home-page', {
            require: 'pages/home-page/home'
        });
        ko.components.register('signin-page', {
            require: 'pages/signin-page/signin'
        });
        ko.components.register('list-page', {
            require: 'pages/list-page/list-page'
        });
        ko.components.register('item-page', {
            require: 'pages/item-page/item-page'
        });
        ko.components.register('register-page', {
            require: 'pages/register-page/register-page'
        });
        ko.components.register('search-page', {
            require: 'pages/search-page/search-page'
        });

        // ... or for template-only components, you can just point to a .html file directly:
        ko.components.register('about-page', {
            template: {
                require: 'text!pages/about-page/about.html'
            }
        });

        // [Scaffolded component registrations will be inserted here. To retain this feature, don't remove this comment.]

        // Start the application
        ko.applyBindings({
            route: router.currentRoute
        });
    });
