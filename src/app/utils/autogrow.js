  /**
 * Autogrow
 * http://googlecode.blogspot.com/2009/07/gmail-for-mobile-html5-series.html
 */
/*global define, getComputedStyle */

define([], function () {
    'use strict';

    var autogrow = function (element, lh) {

        var setLineHeight = lh || 12,
            textLineHeight = element.currentStyle ? element.currentStyle.lineHeight : getComputedStyle(element, null).lineHeight;

        function handler(e) {
            var newHeight = this.scrollHeight,
                currentHeight = this.clientHeight;
            if (newHeight > currentHeight) {
                this.style.height = newHeight + 3 * textLineHeight + 'px';
            }
        }

        textLineHeight = (textLineHeight.indexOf('px') === -1) ? setLineHeight : parseInt(textLineHeight, 10);

        element.style.overflow = 'hidden';
        element.addEventListener ? element.addEventListener('input', handler, false) : element.attachEvent('onpropertychange', handler);
    };

    return autogrow;
});