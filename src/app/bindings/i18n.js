/*global define */

define(['knockout', 'packages/i18n!nls/localizedStrings'], function (ko, localizedStrings) {
    'use strict';

    //taken from http://stackoverflow.com/a/6491621
    var path = function (o, s) {
        s = s.replace(/\[(\w+)\]/g, '.$1'); // convert indexes to properties
        s = s.replace(/^\./, '');           // strip a leading dot
        var a = s.split('.');
        while (a.length) {
            var n = a.shift();
            if (n in o) {
                o = o[n];
            } else {
                return;
            }
        }
        return o;
    },

    applyResourceWithCondition = function (resourceKeyHolder, updateFunc, element, getValueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        // resourceKeyHolder can be an object with resource key and condition when apply this resource key.
        // resourceKeyHolder: { resourceKey: condition } or just resourceKeyHolder: 'resourceKey'
        if (typeof resourceKeyHolder === "object") {
            for (var resKey in resourceKeyHolder) {
                var condition = ko.utils.unwrapObservable(resourceKeyHolder[resKey]);
                if (condition)
                    applyResource(resKey, updateFunc, element, getValueAccessor, allBindingsAccessor, viewModel, bindingContext);
            }
        } else {
            applyResource(resourceKeyHolder, updateFunc, element, getValueAccessor, allBindingsAccessor, viewModel, bindingContext);
        }
    },

    applyResource = function (resourceKey, updateFunc, element, getValueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var text = path(localizedStrings, resourceKey);
        updateFunc.call(this,
                        element,
                        getValueAccessor(text),
                        allBindingsAccessor,
                        viewModel,
                        bindingContext);
    };

    ko.bindingHandlers.i18n = {
        update: function (element, valueAccessor, allBindingsAccessor, viewModel, context) {
            var value = valueAccessor();

            if (typeof value === "undefined") {
                ko.utils.setTextContent(element, '');
            }
            if (typeof value == 'string' || value instanceof String) {
                ko.utils.setTextContent(element, path(localizedStrings, value));
            }
            else {
                var json = value;
                for (var attr in json) {
                    var getValueAccessor = function (text) {
                        return function () { var x = {}; x[attr] = text; return x; };
                    };

                    applyResourceWithCondition(json[attr], ko.bindingHandlers.attr.update, element, getValueAccessor, allBindingsAccessor, viewModel, context);
                }
            }
        }
    };

    ko.virtualElements.allowedBindings['i18n'] = true;
});