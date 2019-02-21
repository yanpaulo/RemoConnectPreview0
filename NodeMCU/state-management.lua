local state_tmr

function blink()
    if gpio.read(0) == 1 then
        gpio.write(0, gpio.LOW)
    else
        gpio.write(0, gpio.HIGH)
    end
end

function createTimer(time)
    return function()
        state_tmr = tmr.create()
        state_tmr:register(time, tmr.ALARM_AUTO, blink)
        state_tmr:start()
    end
end

app_states = {
    createTimer(1000),
    createTimer(300),
    createTimer(100),
    function ()
        gpio.write(0, gpio.LOW)
    end
}

function setState(index)
    if state_tmr ~= nil then
        state_tmr:stop()
        state_tmr = nil
    end
    app_states[index]()
end