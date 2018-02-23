# Cutout

### Description
A two-players game developed with **Microsoft Kinect**. Players will act as gingerbread men, pose themselves to pass through cutouts on the moulds and race with each other.

### Feature
- **Realtime Avatar Mirroring**: 	
The gingerbread men avatars are reflecting players' poses in realtime. 

- **Collision Detection**:
We didn't use any collider in this game. Instead, we detect the collision between gingerbread man and the walls by measuring the actual angles  on the Kinect skeleton and compare them with the cutout angles. And we also record 10 frames angle data before the character reaches the position of the wall in order to ensure the playability.

- **Auto-generated Map**
Different cutouts are categorized by difficulty and generated on the road aligning with the tempo.


### Note
This is a course project from class Building Virtual Worlds in ETC of CMU. It was developed by 2 programmers, 2 artists and 1 sound designer in just **1** week. 
