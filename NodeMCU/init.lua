dofile("state-management.lua")

function startup()
    if file.open("init.lua") == nil then
        print("init.lua deleted or renamed")
    else
        print("Running")
        file.close("init.lua")
        -- the actual application is stored in 'application.lua'
        dofile("application.lua")
    end
end

-- Define WiFi station event callbacks 
wifi_connect_event = function(T) 
  print("Connection to AP("..T.SSID..") stablished!")
  print("Waiting for IP address...")
  if disconnect_ct ~= nil then disconnect_ct = nil end  
end

wifi_got_ip_event = function(T) 
  -- Note: Having an IP address does not mean there is internet access!
  -- Internet connectivity can be determined with net.dns.resolve().    
  print("Wifi connection is ready! IP address is: "..T.IP)
  setState(2)
  tmr.create():alarm(3000, tmr.ALARM_SINGLE, startup)
  
end

wifi_disconnect_event = function(T)
  if T.reason == wifi.eventmon.reason.ASSOC_LEAVE then 
    --the station has disassociated from a previously connected AP
    return 
  end
 
  --There are many possible disconnect reasons, the following iterates through 
  --the list and returns the string corresponding to the disconnect reason.
  for key,val in pairs(wifi.eventmon.reason) do
    if val == T.reason then
      print("Disconnect reason: "..val.."("..key..")")
      break
    end
  end

end

-- Register WiFi Station event callbacks
wifi.eventmon.register(wifi.eventmon.STA_CONNECTED, wifi_connect_event)
wifi.eventmon.register(wifi.eventmon.STA_GOT_IP, wifi_got_ip_event)
wifi.eventmon.register(wifi.eventmon.STA_DISCONNECTED, wifi_disconnect_event)

print("Connecting to WiFi access point...")
setState(1)