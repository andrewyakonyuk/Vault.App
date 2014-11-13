/*global define */

define(['jquery'], function ($) {
    'use strict';

    var checkboxElements = ".checkbox > label > input[type=checkbox]",
        checkbox = function (selector) {
            // Add fake-checkbox to material checkboxes
            $((selector) = selector || checkboxElements)
                .data("mdproc", true)
                .after("<span class=ripple></span><span class=check></span>");
        };

    return checkbox;
});
