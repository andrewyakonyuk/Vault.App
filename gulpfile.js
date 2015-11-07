/*global require, console */
(function () {
    'use strict';

    // Node modules
    var fs = require('fs'),
        vm = require('vm'),
        merge = require('deeply'),
        chalk = require('chalk'),
        es = require('event-stream');

    // Gulp and plugins
    var gulp = require('gulp'),
        rjs = require('gulp-requirejs-bundler'),
        concat = require('gulp-concat'),
        clean = require('gulp-clean'),
        replace = require('gulp-replace'),
        uglify = require('gulp-uglify'),
        htmlreplace = require('gulp-html-replace'),
        minifyCSS = require('gulp-minify-css'),
        concatCss = require('gulp-concat-css'),
        less = require('gulp-less'),
        path = require('path');

    // Config
    var requireJsRuntimeConfig = vm.runInNewContext(fs.readFileSync('src/scripts/require.config.js') + '; require;'),
        requireJsOptimizerConfig = merge(requireJsRuntimeConfig, {
            out: 'scripts.js',
            baseUrl: './src',
            name: 'scripts/startup',
            paths: {
                requireLib: 'bower_modules/requirejs/require'
            },
            include: [
                'requireLib',
                'components/nav-bar/nav-bar',
                'components/breadcrumbs/breadcrumbs',
                'components/kudos/kudos',
                'pages/home-page/home',
                'text!pages/about-page/about.html',
                'pages/signin-page/signin',
                'pages/article-page/article',
                'pages/register-page/register-page',
                'pages/search-page/search-page',
                'pages/notfound-page/notfound',
                'pages/collection-page/collection',
                'pages/dashboard-page/dashboard',
                'pages/settings-page/settings',
                'pages/labels-page/labels'
            ],
            insertRequire: ['scripts/startup'],
            bundles: {
                // If you want parts of the site to load on demand, remove them from the 'include' list
                // above, and group them into bundles here.
                // 'bundle-name': [ 'some/module', 'another/module' ],
                // 'another-bundle-name': [ 'yet-another-module' ]
            }
        });

    // Discovers all AMD dependencies, concatenates together all required .js files, minifies them
    gulp.task('js', function () {
        return rjs(requireJsOptimizerConfig)
            .pipe(uglify({
                preserveComments: 'some'
            }))
            .pipe(gulp.dest('./dist/'));
    });

    // Concatenates CSS files, rewrites relative paths to Font Awesome, copies Font Awesome fonts
    gulp.task('css', function () {
        var styles = gulp.src('src/styles/main.css')
                .pipe(replace(/url\((')?\.\.\/img\//g, 'url($1img/'))
                .pipe(replace(/url\((')?\.\.\/bower_modules\/font-awesome\/fonts/g, 'url($1fonts/'))
                .pipe(minifyCSS({}))
                .pipe(concat('styles.css'))
                .pipe(gulp.dest('./dist/'));
    });

    // Copies index.html, replacing <script> and <link> tags to reference production URLs
    gulp.task('html', function () {
        return gulp.src('./src/index.html')
            .pipe(htmlreplace({
                'css': 'styles.css',
                'js': 'scripts.js'
            }))
            .pipe(gulp.dest('./dist/'));
    });

    // Removes all files from ./dist/
    gulp.task('clean', function () {
        return gulp.src('./dist/**/*', {
            read: false
        })
            .pipe(clean());
    });

    gulp.task('images', function () {
        return gulp.src('./src/img/**/*')
            .pipe(gulp.dest('./dist/img'));
    });

    gulp.task('less', function () {
        var compiledCss = gulp.src('./src/styles/main.less')
          .pipe(less({
              paths: [path.join(__dirname, 'less', 'includes')]
          }))
          .pipe(replace(/url\((')?\.\.\/fonts\//g, 'url($1../bower_modules/font-awesome/fonts/'))
          .pipe(gulp.dest('./src/styles'));

    });

    gulp.task('dev-fonts', function () {

        var fontFiles = gulp.src('./src/bower_modules/font-awesome/fonts/*', {
            base: './src/bower_modules/font-awesome'
        })
         .pipe(gulp.dest('./src'));
    });


    gulp.task('fonts', function () {

        var fontFiles = gulp.src('./src/bower_modules/font-awesome/fonts/*', {
            base: './src/bower_modules/font-awesome'
        })
         .pipe(gulp.dest('./dist'));
    });

    gulp.task('default', ['html', 'js', 'css', 'images', 'fonts'], function (callback) {
        callback();
        console.log('\nPlaced optimized files in ' + chalk.magenta('dist/\n'));
    });

    gulp.task('dev', ['less', 'dev-fonts'], function (callback) {
        callback();
    });

})();
