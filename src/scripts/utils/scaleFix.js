/**
 * Fix for iPhone viewport scale bug
 * http://www.blog.highub.com/mobile-2/a-fix-for-iphone-viewport-scale-bug/
 */

/*global define */

define([], function () {
    'use strict';

    var viewportmeta = document.querySelector && document.querySelector('meta[name="viewport"]'),
        gestureStart = function () {
            viewportmeta.content = 'width=device-width, minimum-scale=0.25, maximum-scale=1.6';
        },
        scaleFix = function () {
            if (viewportmeta && /iPhone|iPad|iPod/.test(navigator.userAgent) && !/Opera Mini/.test(navigator.userAgent)) {
                viewportmeta.content = 'width=device-width, minimum-scale=1.0, maximum-scale=1.0';
                document.addEventListener('gesturestart', gestureStart, false);
            }
        };

    return scaleFix;
});
