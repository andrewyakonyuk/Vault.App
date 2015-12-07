/*global define, console */
define(["jquery", "packages/cookie"], function ($, cookie) {
    'use strict';

    var http = {
        callService: function (url, type, data) {
            console.log(url);

            var webserviceUrl = 'ajax/' + url;
            type = 'get'; //todo: this should be removed when we have a real service url

            var addHeaders = function (jqXHR) {
                if (cookie.hasItem('auth')) {
                    jqXHR.setRequestHeader("Authorization", "Basic " + cookie.getItem('auth'));
                }
            };

            var ajaxSettings = {
                type: type,
                data: data,
                beforeSend: addHeaders,

                crossDomain: true,
                contentType: "application/json",
                dataType: "json",
                cache: true
            };

            return $.ajax(webserviceUrl, ajaxSettings);
        }
    };

    return http;
});
