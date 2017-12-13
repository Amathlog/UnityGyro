var socket = null;
var colors;
var name;

function isConnected(){
    return (socket !== null)
}

function getUserId(){
    match = document.cookie.match(/\buser_id=([a-zA-Z0-9]{32})/)
    return match ? match[1] : null
}

function connect(){
    if(isConnected())
        return
    if(!/\buser_id=/.test(document.cookie)){
        document.cookie = 'user_id=' + generateHash(32);
    }
    socket = io.connect(adress);
    socket.on('connect', function(){
        socket.emit('userId', {userId:getUserId()})
    })
}

function configureDynamicHTML(firstTime){
    socket.on('userInfo', function(userInfo){
        colors = userInfo.colors;
        name = userInfo.username;
    });

    socket.on('needToConnect', function(){
        $('#dynamicHTML').load('/', {needToConnect:true, userId:getUserId()})
    })

    socket.on('connected', function(data){
        $('#dynamicHTML').load('/', {needToConnect:false, userId:getUserId()})
        $.get( "/voteStatus", function( data ) {
          configureVoting(data.voting)
        });
    })

    socket.on('newMode', function(data){
        $('#dynamicHTML').load('/', {needToConnect:false, userId:getUserId()})
        $.get( "/voteStatus", function( data ) {
          configureVoting(data.voting)
        });
    })

    socket.on('voteStatus', function(data){
        configureVoting(data.voting)
    })
}

function configureVoting(voting){
    if(voting){
        $('#voteText').text("Vote Now!")
        $('#voteButtons').show("fast")
        $('#choice1').click(function(){
            socket.emit('vote', {userId:getUserId(), username:name, vote:0})
        })
        $('#choice2').click(function(){
            socket.emit('vote', {userId:getUserId(), username:name, vote:1})
        })
        $('#choice3').click(function(){
            socket.emit('vote', {userId:getUserId(), username:name, vote:2})
        })
        $('#choice4').click(function(){
            socket.emit('vote', {userId:getUserId(), username:name, vote:3})
        })
    } else {
        $('#voteText').text("Vote (Wait for server...)")
        $('#voteButtons').hide("fast")
    }
}

function configureChat(){
    socket.on('userInfo', function(userInfo){
        colors = userInfo.colors;
        name = userInfo.username;
        $('#nick').val(name);
    });

    $('form').submit(function () {
        var name = $('#nick').val();
        if(name === '')
            return false;
        var msg = {name: name, msg:$('#m').val(), colors:colors};
        socket.emit('chat message', msg);
        msg = generate_message(msg);
        $('#messages').append(msg);
        $('#m').val('');
        return false;
    });

    socket.on('chat message', function (msg) {
        if(msg.server){
            $('#messages').append('<li><b><i>' + msg.msg + '</b></i></li>');
        } else {
            msg = generate_message(msg);
            $('#messages').append(msg);
        }

    });
}

function emitShootMessage(accl){
    if(!isConnected())
        return
    socket.emit("shoot", {userId:getUserId()})
}

function emitEnteringGameMessage(username){
    // Emit a message to the server in order to enter the game
    // Also indicates the username
    // Should only be trigger once. After the player comes back
    // it should reconnect automatically
    if(!isConnected())
        return
    socket.emit('enteringGame', {userId:getUserId(), username:username})
}

function emitAcclMessage(accl){
    if(!isConnected())
        return
    socket.emit("accl", {userId:getUserId(), accl:accl})
}

function generateHash(len){
    var symbols = 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890';
    var hash = ''
    for(var i = 0; i < len; i++){
        var symIndex = Math.floor(Math.random() * symbols.length);
        hash += symbols.charAt(symIndex)
    }
    return hash
}

