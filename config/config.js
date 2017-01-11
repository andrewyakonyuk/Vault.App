var path = require('path'),
    rootPath = path.normalize(__dirname + '/..'),
    env = process.env.NODE_ENV || 'development';

var config = {
  development: {
    root: rootPath,
    app: {
      name: 'vault-app'
    },
    port: process.env.PORT || 3000,
    db: 'postgres://postgres:abc123!@localhost:5432/vault-app-development'
  },

  test: {
    root: rootPath,
    app: {
      name: 'vault-app'
    },
    port: process.env.PORT || 3000,
    db: 'postgres://localhost/vault-app-test'
  },

  production: {
    root: rootPath,
    app: {
      name: 'vault-app'
    },
    port: process.env.PORT || 3000,
    db: 'postgres://localhost/vault-app-production'
  }
};

module.exports = config[env];
