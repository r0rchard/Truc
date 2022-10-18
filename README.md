# Usage
## Connection
When the user spawns, they are faced with a keyboard. They must enter a username and press enter. They will then be connected.
## Controls
### Interaction ray
The interaction is automatically emitted from the user's hands and are used to select the object the user wants to interact with.
### Trigger
The controller's trigger is the basic interaction, allowing to interact with UI and 3D objects alike. Notably, a user can move from room to room by pointing at a door and pressing the trigger.
### Joystick
To move inside a room, the user must point at the floor and then push the joystick towards the up direction.
### Keyboard
Interaction with the keyboard is a bit different. When they must interact with it, the user's interaction rays are replaced by two mallets. Those are used to activate the keys by simple contact.

# Assumptions

A few assumptions are made for the program to work correctly :

## Icon names
It is assumed that the icons associated with each host of the network is named correctly. Indeed, CyberRange files don't have semantic data allowing to identify a host type. It is therefore necessary for the icons to be correctly attributed to the hosts (for instance a server must be associated to an icon with "server" in its name).

## Spatial disposition
In order for the user to find a familiar environment, it is assumed that the icons were correctly placed in the 2D space. That way, the map the user will have to navigate in the 3D environment will be similar to the 2D schematics they are used to.

# Network

## Synchronisation
The environment is generated independently by each client.
The objects that are synchronised between clients are the positions of the users and the data pipelines (i.e. the transfer size and transfer speed of each pipeline).
The master client (the user that connects first) is the one that determines the initial parameters. Functions have been created that can allow a client to modify the parameters for all users.
## Rooms
Currently, all the users connecting will share the same environment (room). This can be modified in the future.

