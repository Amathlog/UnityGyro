var express = require('express');
var router = express.Router();

var x = 0

/* GET home page. */
router.get('/', function(req, res, next) {
    if(x === 0)
        res.render('shooter', {});
    else
        res.send("Hello World")
});

module.exports = router;
