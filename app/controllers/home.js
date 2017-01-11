var express = require('express'),
  router = express.Router(),
  db = require('../models'),
  passport = require('passport');

module.exports = function (app) {
  app.use('/', router);
};

router.get('/', function (req, res, next) {
  db.Article.findAll().then(function (articles) {
    res.render('index', {
      title: 'Generator-Express MVC',
      articles: articles,
      username: req.isAuthenticated() ? req.user.username : ''
    });
  });
});
