local ws = websocket.createClient()
local wsTimer = tmr.create();

ws:on("connection", function(ws)
  print('got ws connection')
  setState(4)
end)
ws:on("receive", function(_, msg, opcode)
  print('got message:', msg, opcode)
  if msg == 'on' then
    gpio.write(1, gpio.LOW)
  elseif msg == 'off' then
    gpio.write(1, gpio.HIGH)
  end
    
end)
ws:on("close", function(_, status)
  print('connection closed, will try to reconnect', status)
  wsTimer:start()
  ws = nil -- required to Lua gc the websocket client
end)

function connect()
    print('Connecting...')
    setState(3)
    ws:connect('ws://remo-connect-preview0.azurewebsites.net/ws')    
end

-- wsTimer:register(2000, tmr.ALARM_SEMI, connect)
-- wsTimer:start()
-- print('Waiting before connect')
connect()
