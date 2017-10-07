var moveDetection = (function () {
    var _alpha, _beta, _gamma
    var _buffer = new Buffer3D(10);
    var _valid  = false;
    var _gyro = false

    // Accelerometer part
    function onDeviceMotion(event){
        _valid = event.accelerationIncludingGravity.x != null;
        if(_valid){
            _buffer.append([event.accelerationIncludingGravity.x,
                            event.accelerationIncludingGravity.y,
                            event.accelerationIncludingGravity.z]);
        }
    } 

    // Gyroscope part
    function onDeviceOrientation(event){
        _valid = event.alpha != null;
        if(_valid){
            _alpha = event.alpha;
            _beta = event.beta;
            _gamma = event.gamma;
        }
    } 

    if(window.DeviceOrientationEvent){
        window.addEventListener("deviceorientation", onDeviceOrientation, true);
        _gyro = true
    }else if(window.DeviceMotionEvent){
        window.addEventListener("devicemotion", onDeviceMotion, false);
    }else{
        console.log("DeviceMotionEvent is not supported");
    }

    return {
        valid: function(){
            return _valid;
        },
        gyro: function(){
            return _gyro;
        },
        format: function(f) {
            return _valid? (_gyro? [_alpha, _beta, _gamma].map(f) : _buffer.filtered().map(f)) : "Invalid";
        },
        value: function() {
            return _valid? (_gyro? [_alpha, _beta, _gamma] : _buffer.filtered()) : [0,0,0];
        }
    };
})();

// for tests
function updateText(){
    $('#accl').text(moveDetection.format(format));
    emitAcclMessage(moveDetection.value())
    setTimeout(function () { updateText() }, 500);
}

updateText()