/*
 * Prevent iOS from zooming onfocus
 * https://github.com/h5bp/mobile-boilerplate/pull/108
 * Adapted from original jQuery code here: http://nerd.vasilis.nl/prevent-ios-from-zooming-onfocus/
 */

/*global define */
define([], function () {
    'use strict';

    var viewportmeta = document.querySelector && document.querySelector('meta[name="viewport"]'),
        preventZoom = function () {
            var formFields = document.querySelectorAll('input, select, textarea'),
                contentString = 'width=device-width,initial-scale=1,maximum-scale=',
                i = 0;

            for (i = 0; i < formFields.length; i++) {
                formFields[i].onfocus = function () {
                    viewportmeta.content = contentString + '1';
                };
                formFields[i].onblur = function () {
                    viewportmeta.content = contentString + '10';
                };
            }
        };

    return preventZoom;
});