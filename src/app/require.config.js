// require.js looks for the following global when initializing
var require = {
    baseUrl: ".",
    paths: {
        "crossroads": "bower_modules/crossroads/dist/crossroads",
        "hasher": "bower_modules/hasher/dist/js/hasher",
        "jquery": "bower_modules/jquery/dist/jquery",
        "knockout": "bower_modules/knockout/dist/knockout.debug",
        "knockout-projections": "bower_modules/knockout-projections/dist/knockout-projections",
        "knockout-validation": "bower_modules/knockout-validation/Dist/knockout.validation",
        "signals": "bower_modules/js-signals/dist/signals",
        "text": "bower_modules/requirejs-text/text",
        "underscore": "bower_modules/underscore/underscore",
        "pace": "bower_modules/pace/pace",
        "bootstrap": "bower_modules/components-bootstrap/js/bootstrap",
        "bootstrap-material-design": "bower_modules/bootstrap-material-design/dist/js/material",
        "ripples": "bower_modules/bootstrap-material-design/dist/js/ripples",
        "hammer": "bower_modules/hammerjs/hammer",
        "jasny-bootstrap": "bower_modules/jasny-bootstrap/dist/js/jasny-bootstrap"
    },
    shim: {
        "jquery": {
            exports: '$'
        },
        "knockout-validation": {
            deps: ["knockout"]
        },
        "bootstrap": {
            deps: ["jquery"]
        },
        "bootstrap-material-design": {
            deps: ["jquery", "bootstrap", "ripples"]
        },
        "jasny-bootstrap": {
            deps: ["jquery", "bootstrap"]
        }
    }
};
