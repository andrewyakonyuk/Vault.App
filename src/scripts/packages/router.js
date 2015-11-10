/*global define */

define(["knockout", "crossroads", "hasher", "packages/auth"], function (ko, crossroads, hasher, auth) {
    'use strict';

    //knockout validation issue: https://github.com/Knockout-Contrib/Knockout-Validation/issues/259
    window.ko = ko;

    // This module configures crossroads.js, a routing library. If you prefer, you
    // can use any other routing library (or none at all) as Knockout is designed to
    // compose cleanly with external libraries.
    //
    // You *don't* have to follow the pattern established here (each route entry
    // specifies a 'page', which is a Knockout component) - there's nothing built into
    // Knockout that requires or even knows about this technique. It's just one of
    // many possible ways of setting up client-side routes.

    function activateCrossroads() {
        function parseHash(newHash, oldHash) {
            crossroads.parse(newHash);
        }
        crossroads.normalizeFn = crossroads.NORM_AS_OBJECT;
        hasher.initialized.add(parseHash);
        hasher.changed.add(parseHash);
        hasher.init();
    }

    function Router(config) {
        var self = this,
            currentRoute = this.currentRoute = ko.observable({});

        this.navigate = function(routeName){
            var routeDefinition = {};
            ko.utils.arrayForEach(config.routes, function(item){
               if(item.params.page == routeName) {
                   routeDefinition = item;
                   return true;
               }
            });
            hasher.setHash(routeDefinition.url);
        };

        this.urlForRoute = function(routeName){
             var routeDefinition = {
                 url: ''
             };
            ko.utils.arrayForEach(config.routes, function(item){
               if(item.params.page == routeName) {
                   routeDefinition = item;
                   return true;
               }
            });
            return routeDefinition.url;
        }

        ko.utils.arrayForEach(config.routes, function (route) {
            crossroads.addRoute(route.url, function (requestParams) {
                console.log(route);
                if(route.params.authorizedOnly && !auth.isAuthorized()) {
                    self.navigate('signin-page');
                }
                else{
                    currentRoute(ko.utils.extend(requestParams, route.params));
                }
            });
        });

        activateCrossroads();

        crossroads.bypassed.add(function (request) {
            self.currentRoute(request);
        });
    }

    return new Router({
        routes: [
            {
                url: '',
                params: {
                    page: 'home-page',
                    authorizedOnly: false
                }
            },
            {
                url: 'about',
                params: {
                    page: 'about-page',
                    authorizedOnly: false
                }
            },
            {
                url: 'signin',
                params: {
                    page: 'signin-page',
                    authorizedOnly: false
                }
            },
            {
                url: 'join',
                params: {
                    page: 'register-page',
                    authorizedOnly: false
                }
            },
            {
                url: 'article/{id}',
                params: {
                    page: 'article-page',
                    authorizedOnly: true
                }
            },
            {
                url: 'search/{searchText}',
                params: {
                    page: 'search-page',
                    authorizedOnly: true
                }
            },
            {
                url: 'collections/:id:',
                params: {
                    page: 'collection-page',
                    authorizedOnly: true
                }
            },
            {
                url: 'dashboard',
                params: {
                    page: 'dashboard-page',
                    authorizedOnly: true
                }
            },
            {
                url: 'settings',
                params: {
                    page: 'settings-page',
                    authorizedOnly: true
                }
            },
            {
                url: 'labels/:id:',
                params: {
                    page: 'labels-page',
                    authorizedOnly: true
                }
            },
            {
                url: 'maps',
                params: {
                    page: 'maps-page',
                    authorizedOnly: true
                }
            },
            {
                url: 'musics',
                params: {
                    page: 'musics-page',
                    authorizedOnly: true
                }
            },
            {
                url: 'calendar',
                params: {
                    page: 'calendar-page',
                    authorizedOnly: true
                }
            }
        ]
    });
});
