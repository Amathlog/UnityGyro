
function sum(total, value){
    return [(total[0] + value[0]), total[1] + value[1], total[2] + value[2]]
}


class Buffer3D{

    constructor(size){
        this.size = size
        this.h = 0
        this.buffer = []
    }

    append(value){
        if(this.buffer.length < this.size)
            this.buffer.push(value)
        else
            this.buffer[this.h] = value
        this.h = (this.h + 1) % this.size
    }

    filtered(){
        var len = this.buffer.length
        if(len == 0)
            return [0,0,0]
        var bufferSum = this.buffer.reduce(sum, [0,0,0])
        return [bufferSum[0]/len , bufferSum[1]/len, bufferSum[2]/len]
    }

}