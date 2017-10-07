var express = require('express');
var router = express.Router();
mode = require('../public/javascripts/mode.js')

/* GET home page. */
router.get('/', function(req, res, next) {
    res.render('main', {});
});

router.post('/', function(req, res, next){
    if(req.body.needToConnect === 'true')
        res.render('connection', {})
    else{
        if(mode.currentMode === 0)
            res.render('shooter', {})
        else
            res.render('vote', {})
    }
})

module.exports = router;
