/**
    * Enable CSS active pseudo styles in Mobile Safari
    * http://alxgbsn.co.uk/2011/10/17/enable-css-active-pseudo-styles-in-mobile-safari/
    */

/*global define */

define([], function () {
    'use strict';

    var enableActive = function () {
        document.addEventListener('touchstart', function () { }, false);
    };

    return enableActive;
});
