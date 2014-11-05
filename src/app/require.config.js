// require.js looks for the following global when initializing
var require = {
    baseUrl: ".",
    paths: {
        "crossroads": "bower_modules/crossroads/dist/crossroads.min",
        "hasher": "bower_modules/hasher/dist/js/hasher.min",
        "jquery": "bower_modules/zepto/zepto.min",
        "knockout": "bower_modules/knockout/dist/knockout",
        "knockout-projections": "bower_modules/knockout-projections/dist/knockout-projections.min",
        "knockout-validation": "bower_modules/knockout-validation/Dist/knockout.validation.min",
        "signals": "bower_modules/js-signals/dist/signals.min",
        "text": "bower_modules/requirejs-text/text",
        "underscore": "bower_modules/underscore/underscore-min",
        "pace": "bower_modules/pace/pace.min"
    },
    shim: {
        "jquery": {
            exports: '$'
        },
        "knockout-validation": {
            deps: ["knockout"]
        }
    }
};
