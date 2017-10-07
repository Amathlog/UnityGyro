var Gyroscope = (function () {
    var _valid  = false;
    var alpha,beta,gamma

    function onDeviceOrientation(event){
        // _valid = event.alpha != null;
        _valid = true
        if(_valid){
            alpha = event.alpha;
            beta = event.beta;
            gamma = event.gamma;
        }
    } 

    if(window.DeviceOrientationEvent){
        window.addEventListener("deviceorientation", onDeviceOrientation, true);
    }else{
        console.log("DeviceOrientationEvent is not supported");
    }

    return {
        valid: function(){
            return _valid;
        },
        format: function(f) {
            return _valid? [alpha, beta, gamma].map(f) : "Invalid";
        },
        value: function() {
            return _valid? [alpha, beta, gamma] : [0,0,0];
        }
    };
})();

// for tests
function updateGyroText(){
    $('#accl').text(Accelerometer.format(format));
    emitAcclMessage(Accelerometer.value())
    setTimeout(function () { updateGyroText() }, 500);
}

// updateGyroText()