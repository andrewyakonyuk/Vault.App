var express = require('express'),
  router = express.Router(),
  db = require('../models'),
  passport = require('passport'),
  bcrypt = require('bcrypt-node');

module.exports = function (app) {
  app.use('/', router);
};

router.get('/sign-in', function(req, res, next){
  res.render('signin', {
    title: 'Login'
  });
});
router.post('/sign-in', passport.authenticate('local', {
    successRedirect: '/profile',
    failureRedirect: '/sign-in'
}));
router.get('/sign-up', function(req, res, next){
  res.render('signup', {
    title: 'Register'
  });
});
router.post('/sign-up', function(req, res, next){
  db.User.findAll({
    where: {
      username: req.body.username
    }
  }).then(function(users){
    if(!users.length){
        bcrypt.hash(req.body.password, null, null, function(e, hash){
          db.User.create({
            username: req.body.username,
            hash: hash
          }).then(function(){
            res.redirect('/');
          }, function(e){
            console.log(arguments);
            res.render('signup', {
              username: req.username,
              title: 'Register'
            });
          });
        });
    }
    else{
      res.render('signup', {
        username: req.username,
        title: 'Register'
      });
    }
  }).catch((function(err){
    console.log(err);
  }));

});
