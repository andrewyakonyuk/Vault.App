/*global define, console*/
define(['utils/guid', 'jquery', 'indexedDbShim'], function (guid, $) {
    'use strict';

    window.indexedDB = window.indexedDB || window.mozIndexedDB || window.webkitIndexedDB || window.msIndexedDB;
    window.IDBTransaction = window.IDBTransaction || window.webkitIDBTransaction || window.msIDBTransaction;
    window.IDBKeyRange = window.IDBKeyRange || window.webkitIDBKeyRange || window.msIDBKeyRange;

    var DB_NAME = "whattoread",
        DB_VERSION = 3,
        openedDb,

        wrapToPromise = function (request) {
            var def = $.Deferred();
            request.onsuccess = function (e) {
                console.info(e.target.result);
                def.resolve(e.target.result);
            };
            request.onerror = function (e) {
                console.error(e.target.errorCode || e.target.error);
                def.rejectWith(null, e.target.errorCode);
            };
            return def.promise();
        },

        getObjectStore = function (name, mode) {
            var tx = openedDb.transaction(name, mode);
            return tx.objectStore(name);
        },

        upgradeNeeded = function (db, oldVersion, newVersion) {
            if (oldVersion < 1) {
                var articlesStore = db.createObjectStore('articles', {
                    keyPath: 'id'
                });

                articlesStore.createIndex('title', 'title', {
                    unique: false
                });
                articlesStore.createIndex('tags', 'tags', {
                    unique: false
                });
                articlesStore.createIndex('pubDate', 'pubDate', {
                    unique: false
                });
            }
        },

        open = function () {
            var request = window.indexedDB.open(DB_NAME, DB_VERSION);
            request.onupgradeneeded = function (e) {
                var db = e.target.result,
                    oldVersion = e.oldVersion,
                    newVersion = e.newVersion;

                upgradeNeeded(db, oldVersion, newVersion);
            };
            return wrapToPromise(request)
                .then(function (result) {
                    openedDb = result;
                });
        },

        addArticle = function (article) {
            if (!article.id) {
                article.id = guid.newGuid();
            }
            var store = getObjectStore('articles', 'readwrite'),
                req = store.put(article);
            return wrapToPromise(req);
        },

        removeArticle = function (key) {
            var store = getObjectStore('articles', 'readwrite'),
                req = store.delete(key);
            return wrapToPromise(req);
        },

        getArticle = function (key) {
            var store = getObjectStore('articles', 'readonly'),
                req = store.get(key);
            return wrapToPromise(req);
        },

        loadArticles = function (index, count) {
            var store = getObjectStore('articles', 'readonly'),
                position = index || 0,
                advancing = true,
                def = $.Deferred(),
                items = [],
                cursor = store.openCursor();
            cursor.onsuccess = function (e) {
                var cursor = e.target.result;
                if (cursor) {
                    if (index && advancing) {
                        cursor.advance(position);
                        advancing = false;
                    } else if (count === void(0) || position < index + count) {
                        wrapToPromise(store.get(cursor.key))
                            .done(function (item) {
                                items.push(item);
                            });
                        position++;
                        cursor.continue();
                    }
                } else {
                    def.resolve(items);
                }
            };
            cursor.onerror = function (e) {
                def.rejectWith(null, e.target.errorCode || e.target.error);
            };

            return def.promise();
        },

        storage = {
            open: open,
            name: DB_NAME,
            version: DB_VERSION,
            articles: {
                add: addArticle,
                remove: removeArticle,
                get: getArticle,
                load: loadArticles
            }
        };

    return storage;
});
