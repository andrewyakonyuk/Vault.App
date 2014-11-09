/*global define */

define(["knockout"], function (ko) {
    'use strict';

    ko.bindingHandlers.pageTitle = {
        update: function (element, valueAccessor) {
            var title = valueAccessor();
            document.title = title;
        }
    };

    ko.virtualElements.allowedBindings.pageTitle = true;

});
