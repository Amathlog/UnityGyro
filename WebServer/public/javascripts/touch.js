function configureTouch(id){
    var touchsurface = document.getElementById(id),
        startX,
        startY,
        dist,
        threshold = 400, //required min distance traveled to be considered swipe
        allowedTime = 300, // maximum time allowed to travel that distance
        elapsedTime,
        startTime
 
    function handleswipe(istopswipe){
        if (istopswipe){
            emitShootMessage(moveDetection.value())
            $('#touch').text("Swipe up detected") 
        }
        else{
            $('#touch').text("Swipe up not detected") 
        }
    }
 
    touchsurface.addEventListener('touchstart', function(e){
        $('#touch').text("") 
        var touchobj = e.changedTouches[0]
        dist = 0
        startX = touchobj.pageX
        startY = touchobj.pageY
        startTime = new Date().getTime() // record time when finger first makes contact with surface
        e.preventDefault()
    }, false)
 
    touchsurface.addEventListener('touchmove', function(e){
        e.preventDefault() // prevent scrolling when inside DIV
    }, false)
 
    touchsurface.addEventListener('touchend', function(e){
        var touchobj = e.changedTouches[0]
        distY = touchobj.pageY - startY // get total dist traveled by finger while in contact with surface
        distX = touchobj.pageX - startX
        
        elapsedTime = new Date().getTime() - startTime // get time elapsed
        // check that elapsed time is within specified, horizontal dist traveled >= threshold, and vertical dist traveled <= 100
        var swipetopBol = (elapsedTime <= allowedTime && distY <= -threshold && Math.abs(distX) <= 300) 
        handleswipe(swipetopBol)
        e.preventDefault()
    }, false)

    return touchsurface
}

