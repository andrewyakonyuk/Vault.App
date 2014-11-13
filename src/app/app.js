/*global define */

define(['knockout'], function (ko) {
    'use strict';

    window.app = window.app || {
        pageTitle: ko.observable('What to read'),
        authorized: ko.observable(false)
    };

    return window.app;

});