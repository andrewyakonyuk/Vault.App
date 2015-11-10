/*global define, console */

define(["knockout", "packages/router"], function (ko, router) {
    'use strict';

    ko.bindingHandlers.route = {
        update: function (element, valueAccessor) {
            var value = valueAccessor();

            if (typeof value === "undefined") {
                element.setAttribute('href', '');
            }
            if (typeof value == 'string' || value instanceof String) {
                element.setAttribute('href', '#/' + router.urlForRoute(value));
            }
            else {
                var json = value;
                for (var attr in json) {
                    var getValueAccessor = function (text) {
                        return function () { var x = {}; x[attr] = text; return x; };
                    };
                }
            }
        }
    };

    ko.virtualElements.allowedBindings.pageTitle = true;
});
