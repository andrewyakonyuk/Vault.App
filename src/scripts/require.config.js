// require.js looks for the following global when initializing
var require = {
    baseUrl: ".",
    paths: {
        "crossroads": "bower_modules/crossroads/dist/crossroads",
        "hasher": "bower_modules/hasher/dist/js/hasher",
        "jquery": "bower_modules/jquery/dist/jquery",
        "knockout": "bower_modules/knockout/dist/knockout.debug",
        "knockout-validation": "bower_modules/knockout-validation/Dist/knockout.validation",
        "signals": "bower_modules/js-signals/dist/signals",
        "text": "bower_modules/requirejs-text/text",
        "underscore": "bower_modules/underscore/underscore",
        "pace": "bower_modules/pace/pace",
        "jquery.nicescroll": "bower_modules/jquery.nicescroll/dist/jquery.nicescroll.min",

        "bindings": "./scripts/bindings/",
        "packages": "./scripts/packages/",
        "utils": "./scripts/utils",
        "pages": "./scripts/pages",
        "components": "./scripts/components",
        "nls": "./scripts/nls"
    },
    waitSeconds: 30,
    shim: {
        "knockout-validation": {
            deps: ["knockout"]
        }
    },
    locale: "en-us"
};
