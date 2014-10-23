// require.js looks for the following global when initializing
var require = {
    baseUrl: ".",
    paths: {
        "bootstrap":            "bower_modules/components-bootstrap/js/bootstrap",
        "crossroads":           "bower_modules/crossroads/dist/crossroads",
        "hasher":               "bower_modules/hasher/dist/js/hasher",
        "jquery":               "bower_modules/jquery/dist/jquery",
        "knockout":             "bower_modules/knockout/dist/knockout.debug",
        "knockout-projections": "bower_modules/knockout-projections/dist/knockout-projections",
        "knockout-validation":  "bower_modules/knockout-validation/Dist/knockout.validation",
        "signals":              "bower_modules/js-signals/dist/signals",
        "text":                 "bower_modules/requirejs-text/text"
    },
    shim: {
        "jquery": {
            exports: '$'
        },
        "bootstrap": { deps: ["jquery"] },
        "knockout-validation": {
            deps: ["knockout"]
        }
    }
};
