local ws = websocket.createClient()

ws:on(
  "connection",
  function(ws)
    print("Got ws connection, registering...")
    local code = "8cc34813-475d-4096-b965-f230613c8fe9"
    local dataObj = {
      operation = "Register",
      data = {
        deviceId = code
      }
    }
    ws:send(sjson.encode(dataObj))
    setState(4)
  end
)
ws:on(
  "receive",
  function(_, msg, opcode)
    print("got message:", msg, opcode)
    if msg == "on" then
      gpio.write(1, gpio.LOW)
    elseif msg == "off" then
      gpio.write(1, gpio.HIGH)
    end
  end
)
ws:on(
  "close",
  function(_, status)
    print("connection closed, will try to reconnect", status)
    wsTimer:start()
    ws = nil -- required to Lua gc the websocket client
  end
)

function connect()
  print("Connecting to ws...")
  setState(3)
  ws:connect("ws://remo-connect-preview0.azurewebsites.net/ws")
end

connect()
