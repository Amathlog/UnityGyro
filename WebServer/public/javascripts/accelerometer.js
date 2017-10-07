var Accelerometer = (function () {
    var _buffer = new Buffer3D(10);
    var _valid  = false;

    function onDeviceMotion(event){
        _valid = event.accelerationIncludingGravity.x != null;
        if(_valid){
            _buffer.append([event.accelerationIncludingGravity.x,
                            event.accelerationIncludingGravity.y,
                            event.accelerationIncludingGravity.z]);
        }
    } 

    if(window.DeviceMotionEvent){
        window.addEventListener("devicemotion", onDeviceMotion, false);
    }else{
        console.log("DeviceMotionEvent is not supported");
    }

    return {
        valid: function(){
            return _valid;
        },
        format: function(f) {
            return _valid? _buffer.filtered().map(f) : "Invalid";
        },
        value: function() {
            return _valid? _buffer.filtered() : [0,0,0];
        }
    };
})();

// for tests
function updateAccelText(){
    $('#accl').text(Accelerometer.format(format));
    emitAcclMessage(Accelerometer.value())
    setTimeout(function () { updateAccelText() }, 500);
}

// updateAccelText()