var passport = require('passport'),
 LocalStrategy = require('passport-local').Strategy,
 authenticationMiddleware = require('./middleware'),
 db = require('../models'),
 bcrypt = require('bcrypt-node');

 function findUser (username, callback) {
   db.User.findAll({
     where: {
       username: username
     }
   }).then(function(users){
      if(users.length === 1)
        callback(null, users[0]);
      else callback(null);
   });
 }

 passport.serializeUser(function (user, cb) {
   cb(null, user.username);
 });

 passport.deserializeUser(function (username, cb) {
   findUser(username, cb);
 });

 function initPassport () {
   passport.use(new LocalStrategy(
     function(username, password, done) {
       findUser(username, function (err, user) {
         if (err) {
           return done(err);
         }
         if (!user) {
           return done(null, false);
         }
         bcrypt.compare(password, user.hash, function(err, result){
           console.log(result);
           console.log(err);
           if(result){
            done(null, user);}
          else {done(null, false);}
         });
       });
     }
   ));

   passport.authenticationMiddleware = authenticationMiddleware;
 }

 module.exports = initPassport;
