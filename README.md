
# BitBullet

BitBullet is an FPS 1v1 online made for an university discipline.
The objective of the game is to kill the other player, the player
with the most kills wins.

## Implementation

For the implementation I started with the implementation
of the game and then made the online adjustments for it.

### Implementation of the game

The implementation of the game its very simple you shoot if it hits
another player removes health until its dead. When shooting the gun
loses bullets if there is no bullets it reloads instead.

### Implementation of online

The online part I used the Unity package "Netcode for GameObjects" (NGO).
I implemented a Server-Client architecture, where the client only sends
messages to the server and the server makes all the logic, the only thing
that the server sends to the client its the update of the UI. The reason
of this approach is to avoid cheating from the side of the Client, because
he doesn't have access to the code being a dumb terminal that only sends
messages.

First I created a GameObject and renamed as NetworkManager and added a
NetworkManager script from the NGO, after that I added a NetworkTransform
script to the player prefab so that the player can synchronize with the server.
This won't work because NetworkTransform only send messages from the server to the
client, but for now we force it by doing a script that inherits from NetworkTranform
to make some tests. After that we use NetworkVariables to share variables but only
the server can write on. After that I used Rpc methods so the client can call methods
from the server and after that being implemented I removed the script that inherit from
TransformNetwork.

## Network architecture diagram

This is the diagram of the network architecture

![Network](ReportImages/Network.png)




