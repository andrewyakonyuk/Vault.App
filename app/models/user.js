var bcrypt = require('bcrypt-node');

module.exports = function (sequelize, DataTypes) {

  var User = sequelize.define('User', {
    username: {type: DataTypes.STRING, allowNull: false},
    hash: {type: DataTypes.STRING, allowNull: false},
    id: { type: DataTypes.INTEGER, autoIncrement: true, primaryKey: true },
  }, {
    classMethods: {
      associate: function (models) {
        // example on how to add relations
        // Article.hasMany(models.Comments);
      },
      hashPassword: function(password){
        bcrypt.getSalt(function(e, salt){
          bcrypt.hash(passport, salt, null, function(e, hash){
            this.salt = salt;
            this.hash = hash;
          });
        });
      }
    }
  });

  return User;
};
