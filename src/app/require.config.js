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
        "hammer": "bower_modules/hammerjs/hammer",
        "bindings": "./app/bindings/",
        "packages": "./app/packages/",
        "utils": "./app/utils"
    },
    waitSeconds: 30,
    shim: {
        "jquery": {
            exports: '$'
        },
        "knockout-validation": {
            deps: ["knockout"]
        }
    }
};