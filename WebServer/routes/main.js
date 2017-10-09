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
    	console.log(mode.currentMode)
        if(mode.currentMode === 1)
            res.render('shooter', {})
        else
            res.render('vote', {})
    }
})

router.get('/voteStatus', function(req, res, next){
    res.json(200, {voting:mode.voting})
})

module.exports = router;
