var moveDetection = (function () {
    var _alpha, _beta, _gamma
    var _buffer = new Buffer3D(10);
    var _valid  = false;
    var _gyro = false

    var _calibrated_center = [0,0,0]

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

    function _calibrate(){
        _calibrated_center = _raw_value()
    }

    function _raw_value(){
        return _valid? (_gyro? [_alpha, _beta, _gamma] : _buffer.filtered()) : [0,0,0];
    }

    function _value(){
        r_v = _raw_value();
        r_v[0] -= _calibrated_center[0]
        if (r_v[0] > 180)
            r_v[0] -= 360
        r_v[1] -= _calibrated_center[1]
        if (r_v[1] > 180)
            r_v[1] -= 360
        r_v[2] -= _calibrated_center[2]
        if (r_v[2] > 90)
            r_v[2] -= 180
        return r_v
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
            return _valid? _value().map(f) : "Invalid";
        },
        value: function() {
            return _value()
        },
        calibrate: function() {
            _calibrate();
        }
    };
})();

// for tests
function updateText(){
    $('#accl').text(moveDetection.format(format));
    emitAcclMessage(moveDetection.value())
    setTimeout(function () { updateText() }, 33);
}

updateText()