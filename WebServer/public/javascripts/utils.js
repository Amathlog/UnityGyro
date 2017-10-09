// var adress = "localhost:3000"
var adress = "192.168.1.110:3000"
//var adress = "192.168.1.124:3000"

function generate_message(o){
    var name = o.name;
    var colors = o.colors;
    var msg = o.msg;

    var color_css = 'rgb(' + colors[0] + ',' + colors[1] + ',' + colors[2] + ')';

    return '<li> <span style="color:' + color_css + '">' + name + '</span>: '  + msg + '</li>';
}

function format(value){
    return value.toFixed(2)
}

function lockScreen(){
    screen.orientation.lock("portrait-primary")
}