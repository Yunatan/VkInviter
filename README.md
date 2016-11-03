# VkInviter
A command line tool that allows to bulk invite people to vk events, using [AntiGate](https://anti-captcha.com/) service as captcha resolver.

# Usage example
```
./VkInviter.exe -vkAppId 123456 -vkLogin qweasd@gmail.com -vkPassword qwerty -inviteTo 123123 -inviteFrom 3213212 -antiGateId 1eli2y4e1982ph1oijwelq8lou23o189j23
```
* **-vkAppId** is ID of your application [registered in VK](https://vk.com/apps?act=manage)
* **-vkLogin** is your VK account login
* **-vkPassword** is your VK account password
* **-inviteTo** is ID of the event you are inving people to 
* **-inviteFrom** is ID of the group you are inving people from, which shoud be put as an organizer of the event
* **-antiGateId** is your AntiGate account key 
