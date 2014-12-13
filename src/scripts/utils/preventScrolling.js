/*global define */

define([], function () {
    'use strict';

    /**
    * Prevent default scrolling on document window
    */

    var preventScrolling = function () {
        document.addEventListener('touchmove', function (e) {
            if (e.target.type === 'range') {
                return;
            }
            e.preventDefault();
        }, false);
    };

    return preventScrolling;
});
