define(["jquery", "packages/cookie"], function($, cookie){
   'use strict';

    var webapi = {
        callService: function(url, type, data){
            console.log(url);

            var webserviceUrl = 'ajax/' + url;
            type = 'get'; //todo: this should be removed when we have a real service url

            var addHeaders = function(jqXHR){
                if(cookie.hasItem('auth')){
                    xhr.setRequestHeader("Authorization", "Basic " + cookie.getItem('auth'));
                }
            };

            var ajaxSettings = {
                type: type,
                data: data,
                beforeSend: addHeaders,

                crossDomain: true,
                contentType: "application/json",
                dataType: "json"
            };

            return $.ajax(webserviceUrl, ajaxSettings);
        }
    };

    return webapi;
});
