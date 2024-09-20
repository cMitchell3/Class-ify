![](https://lh7-rt.googleusercontent.com/docsz/AD_4nXf67whxrH4hGLdaXPQHbRvYaW3OGKIRki5eGpsACT9DKIftUns67gCYuSwZio7NdZjo6F_rz-4jH0HXE7Zr8i-rQziEWTQyiJ_pj18Jxkf3KiI-O4lGz5NwctLwJCid8C2CcnSmQTT-XzK1W8MWbs8ikjNK?key=3DZa9zryPLreVs5cxoiPVA)

Design Document

09/16/2024

Shelby Joyner

Gavin Fortwendel

Cass Mitchell

Clayton Whitten

Team 20

Index

-   Purpose  3

-   Functional Requirements  3

-   Non-Functional Requirements  5

-   Design Outline  8

-   High-Level Overview  8

-   Design Decisions and Component Identification  8

-   Interactions Between Components  9

-   State/Activity Diagram  10

-   Design Issues  11

-   Functional Issues  11

-   Non-Functional Issues  13

-   Design Details  15

-   Class Design  15

-   Class Interactions  16

-   Sequence Diagrams  18

-   Login  18

-   Create/Join room  19

-   Sharing files  20

-   Whiteboard  21

-   Shop  22

Purpose
=======

In today's vast world of online communication, very few products can provide students with fun and meaningful interactions they need to make virtual communication useful to their learning experiences. Class-ify aims to supply students with just that. The ability to communicate in an immersive and interactive virtual environment. Our software allows users to join "classrooms," in which they will actually have a visual character and the ability to actually collaborate in real-time with other students and their characters.

Many apps that are somewhat similar to Class-ify already exist. Examples of these include Zoom and Google Meet. These apps do provide video and audio communication between users. However, they cannot provide the virtual environment that Class-ify does. Class-ify makes it so that you must be "in proximity" of someone in order to be able to talk to them. This is just one of many fun and immersive layers Class-ify adds to the simple idea that Zoom and Google Meet start with.

Functional Requirements
=======================

1.  User interactions:

As a user interacting with the software,

1.  I would like to be able to move around inside a room.

2.  I would like to see other users' sprites in a concurrent room.

3.  I would like to be able to choose to create a room or join a room via a menu.

4.  I would like to see other user's display names.

5.  I would like to be able to give another user a thumbs up for good work.

1.  Room:

As a user,

1.  I would like to join a room with a join code.

2.  I would like to create a room.

3.  I would like to be able to reopen a room I previously created.

4.  I would like to leave the room I am in. 

1.  Login/sign-up:

As a user logging in,

1.  I would like to be able to create an account if I do not already have one.

2.  I would like to be able to log in to my account with an email and password.

3.  I would like to be able to log in using Google, Apple, or Microsoft.

4.  I would like to be able to reset my password if I forget it.

1.  Settings Menu:

As a user interacting with settings,

1.  I would like to open a menu for settings options.

2.  I would like to change my microphone input device.

3.  I would like to change my camera input device.

4.  I would like to change the volume.

5.  I would like to change my display name.

1.  File sharing:

As a user wanting to interact with the file system,

1.  I would like to interact with a 2D file cabinet object that opens a file sharing menu.

2.  I would like to upload files from my device for anyone to view.

3.  I would like to export files to my device.

4.  I would like to view unrestricted files uploaded by other users.

5.  I would like to get currency if someone downloads a file I uploaded.

6.  I would like to have files I upload remain uploaded, even after I leave.

1.  Whiteboard:

As a user interacting with the whiteboard system,

1.  I would like to interact with a 2D whiteboard object that opens a whiteboard frame.

2.  I would like to draw on the whiteboard.

3.  I would like to erase my work from the whiteboard.

4.  I would like to see other users' drawings on the whiteboard in real time.

5.  I would like to use different colors on the whiteboard.

6.  I would like to be able to lock the whiteboard so only I can draw on it.

7.  I would like to draw on the whiteboard concurrently with other users.

1.  Shop/Customization:

As a user interacting with the shop system,

1.  I would like to purchase a cosmetic of my choice from the shop using currency.

2.  I would like to gain currency when giving another user a thumbs up.

3.  I would like to receive currency when someone else interacts with my files.

4.  I would like to view the shop window and browse items.

5.  I would like to view the amount of currency I have.

1.  User inventory:

As a user interacting with the inventory system,

1.  I would like to click a button that opens an inventory menu.

2.  I would like to see all the cosmetics I have purchased for my sprite.

3.  I would like to see all the decorations I have purchased for the room.

4.  I would like to apply cosmetics to my sprite.

5.  As a user who created the room, I would like to apply decorations to the room.

1.  Audio communication:

As a user interacting with the audio communication system,

1.  I would like to talk to other users in a concurrent room.

2.  I would like to mute my microphone on the game screen.

1.  Video Communication (If time allows):

As a user interacting with the video communication system,

1.  I would like to have a choice to join a video call with other users.

2.  I would like to be able to turn off my camera on the game screen.

Non-Functional Requirements
===========================

1.  Architecture:

As a developer,

1.  I want to use the game engine Unity to create our game.

2.  I want to use C# to code scripts that will affect game objects in Unity and perform backend tasks.

3.  I want to use Firebase's no-cost Spark plan to store user account data and game data, as it integrates seamlessly with Unity. 

4.  I want to use the free tier of Photon Cloud  to host servers online.

1.  Performance:

As a user,

1.  I want Class-fy to run smoothly and have real-time interactions between players on the same server.

2.  I want to quickly upload and download files from the in-game "file cabinet".

As a developer,

1.  I want to use Firebase for storing file contents as it can support fast file uploads and downloads, and will resume connection to the database during these operations if it dissipates.

2.  I want to use Firebase to store user account data, files, and other game data as the free plan can store up to 1GB.

3.  I want to use Firebase and allow for 100 simultaneous connections to the real-time database.

1.  Scalability:

As a developer,

1.  I want the code base to be clean and well-organized so it is easy to add additional features and make bug fixes without breaking existing features.

2.  I want to optimize our code so the performance of the game is as good as possible.

3.  I want to use Firebase to store large amounts of user data since it is a flexible and easily scalable database tool.

1.  Usability:

As a user,

1.  I want Class-ify to be user friendly and easy to navigate, meaning its UI has a clean design that flows smoothly.

2.  I want the default controls for the game to be intuitive, meaning they should follow the industry standards (WASD for movement, etc).

3.  I want the game to be as accessible as possible; therefore the system requirements to run the game should be limited.

1.  Security:

As a developer,

1.  I want to use Firebase's authentication system to store user credentials and verify their login information.

2.  I want to allow users to sign in users an email/password.

3.  I want to allow users to sign in using Google, Microsoft, or Apple as an alternative.

1.  Hosting/Deployment:

As a developer,

1.  I want to create a build for WebGL in Unity once our game is complete so we can host it on a website.

2.  I want to use Photon Cloud to host the game's servers (classrooms) and have a maximum of 16 players on one server at a time.

3.  I want Photon to allow users to join servers (classrooms) made by other users using a join code.

Design Outline
==============

High-Level Overview
===================

This project will be a virtual classroom application designed to facilitate interactive online learning. The system uses a client-server architecture, where Photon Cloud acts as the server, managing real-time communication between users. Clients connect to Photon Cloud to interact in the virtual classroom by joining a room using a unique code. One client is designated as the host, granting them additional control over the room's settings, while still functioning similarly to other clients. Additionally, Firebase serves as the backend database, handling user data such as authentication, profiles, and room details.\
![](https://lh7-rt.googleusercontent.com/docsz/AD_4nXecHbgbJZBXUI0aHcMk_pTUqV8vpBYNIrzjhIZVg19b2TlXa5ektIPYsgmm6YXqYk0HhrmdHGgly5KfqenGRYXLKkPtBbfA2mrJnju2eH-4s3JldIq5aAx9QxhmSbMFtxWd2Hnq-cX-jtGefKS_K-PcODRS?key=3DZa9zryPLreVs5cxoiPVA)

Design Decisions and Component Identification
=============================================

1.  Client

1.  The client provides the user with an interface to navigate the virtual classroom and interact with other users.

2.  The client sends requests to the Photon Cloud to join or create a room.

3.  The client sends requests to Firebase for user authentication and profile retrieval.

4.  The client communicates user inputs, such as movements or actions, to the Photon Cloud.

3.  Photon Cloud

1.  The Photon Cloud manages the connection between the clients, ensuring real-time communication. 

2.  The Photon Cloud relays inputs and actions between the clients, synchronizing the room state for all users.

3.  The Photon Cloud facilitates the creation of rooms and connection of users with join codes.

5.  Firebase Database

1.  The Firebase Database stores user data such as account information, room details, and user-specific settings.

2.  The Firebase Database handles authentication requests by validating login credentials and managing user profiles.

Interactions Between Components
===============================

Client and Photon Cloud:\
Clients send requests to Photon Cloud to either create or join a room. Once connected, the client continuously communicates user actions, like movement or interactions to Photon Cloud. Photon Cloud then relays this information to other connected clients to ensure that the room remains synchronized for all users.

Client and Firebase:\
When users attempt to log in or sign up, the client sends authentication requests to Firebase. Firebase handles these requests by validating login credentials. Once authenticated, Firebase provides the necessary user profile data back to the client, which is then rendered in the user interface.

State/Activity Diagram
======================

At the start, the user will be sent to login or, if they are a new user, sign-up. Then the user will be sent to the Class-ify main menu. This is the place where users will find many functionalities that Class-ify offers, such as character customization, the shop, room creation/joining, and settings. If users join or create a room, they will gain access to the other features, like being able to pause and leave or interacting with the file cabinet or whiteboard.\
![](https://lh7-rt.googleusercontent.com/docsz/AD_4nXfhPKi1vk-RC9BAaOl6UVg43yGm-3rfJNnDxOaBm0PqX8iI5LsRHI9NNfzg5l15RHZpKJixrC82RjPbsSXHspEhqRVDUlFhK74MIKqgMsLYO6ro3hpold_-zzYjbCp2M7TXF029SeYxyy-1ywkmazmenuOy?key=3DZa9zryPLreVs5cxoiPVA)

Design Issues
=============

Functional Issues
=================

1.  How will users be allowed to log in to our game?

1.  Option 1: Users can log in using their email and password

2.  Option 2: Users can log in using a username and password

3.  Option 3: Users can log in using Google, Microsoft, or Apple

4.  Option 4: Users will not be required to log in

Choice: Options 1, 2, and 3

Justification: We want to provide the user with a number of ways to log in so they can more easily access our game through use of their preferred method, but we also need our users to log in. This information is essential to the game recording player information such as currency, skins, and file uploads for interaction with various parts of our game. We also want to give users the convenience of logging in with a preexisting Google, Microsoft, or Apple account if they so choose.

1.  How will users create an account for our game?

1.  Option 1: Email and password

2.  Option 2: Email, username and password

3.  Option 3: Username and Google, Microsoft, or Apple

4.  No account required

Choice: Options 2 and 3

Justification: In order to give users as many convenient log-in options as possible, we need to collect their username, email, and password, or a username and a Google, Microsoft, or Apple account. A username is required for our users so they can keep their emails private in-game but still have a unique identifier that is visible to other users. Usernames will also be used to show which files belong to whom and who is talking in the chat. 

1.  What happens when the host of a room leaves?

1.  Option 1: The room closes and all players in the room are kicked

2.  Option 2: Another player in the room becomes the host

3.  Option 3: A server becomes the host of the room rather than an individual

Choice: Option 2

Justification: We wish to provide users with a good experience in our game and part of that is a smooth interaction with the room systems. To achieve this, we want to allow users to remain in a room when a player leaves, regardless if that player is labeled as the host of the room. To retain the ability for users to take host actions, we will implement our room so another player becomes the host if a prior host leaves a room. 

1.  What options will users have for interaction?

1.  Option 1: Voice and video chat

2.  Option 2: File sharing and messaging

3.  Option 3: Live writing/drawing in a shared space

Choice: Options 1, 2, and 3

Justification: Our goal is to not only provide a service to be used for contacting others, but also to allow users to connect and have fun. Our program is a game as well as a meeting service thus leading us towards needing the standard forms of communication as well as more entertaining and interactive ones. We believe these added features will create a more fun and helpful environment for users to interact in.

1.  When should whiteboard drawings from users disappear?

1.  Option 1: A user's drawings disappear when they disengage with the whiteboard

2.  Option 2: A user's drawings disappear when they disconnect from the room.

3.  Option 3: A user's drawings never disappear (even when the server is not running) until they are erased or the "clear whiteboard" button is pressed.

Choice: Option 3

Justification: Whiteboard drawings should be persistent just like in real life so users can return to their work at a later time. Other user's should also be able to view work on the whiteboard even when the user who wrote it is not online, allowing for asynchronous collaboration.

Non-Functional Issues
=====================

1.  What database should be used to store user and game data?

1.  Option 1: Firebase Realtime Database and Firestore 

2.  Option 2: MongoDB

3.  Option 3: Google Cloud SQL

Choice: Option 1

Justification: For our needs of storing general user data that can have some delay in access and providing concurrent access to certain data points, Firebase is almost the perfect service. Firebase Realtime Databases provides syncing of data across all users in real time while the Firestore provides extra storage for all individual user data. Considering how fitting this storage method is to our needs and the convenient pricing, we chose Firebase for our database.

1.  Which engine is best suited for designing our game?

1.  Option 1: Unity

2.  Option 2: Unreal Engine

Choice: Option 1

Justification: Unity is well-suited for 2D games and while Unreal Engine has the capacity to create 2D games, its main strengths revolve around 3D games. Unity has a simpler user interface with defined tools for creating 2D games which is crucial for us since a couple of our team members are new to game development. Since Unreal is typically used for 3D games, it can cause more overhead and performance issues than Unity.

1.  Will servers be hosted on the room creator's computer or will they be hosted online?

1.  Option 1: Server is hosted completely online

2.  Option 2: Server is hosted from the room creator's computer

Choice:  Option 1

Justification: We decided to have the servers (classrooms) hosted online using Photon Cloud, as this would ensure that when the host of a room (user who created it) leaves, other users will not be kicked out of the room. Hosting it online is also a simpler solution, as some technical details are abstracted away by Photon Cloud.

1.  What system should be used for hosting rooms and implementing multiplayer in our game?

1.  Option 1: Photon Cloud

2.  Option 2: Unity Netcode

3.  Option 3: Photon Server

Choice: Option 1

Justification: We chose Photon Cloud because it is a well-documented, highly popular option for real-time multiplayer integration in Unity. It is a fast and scalable solution with a generous free tier available. Unity Netcode requires a Unity Gaming Services subscription. Photon server has many similar features to Photon Cloud, but we would have to run the servers from the host user's machine whereas Photon Cloud has a dedicated cloud server. Photon Cloud also comes with free voice and text chat tools that are easy to integrate. 

Design Details
==============

Class Design
============

![](https://lh7-rt.googleusercontent.com/docsz/AD_4nXfq58jpDyWmcGKF1mCRSYsHY6Zd_yb4T9DkiJrqAT6V6DGkV82XOeSSy-ZyKnhByrqG3o5IbbW0ebykuiNoAeZqG8lio7DCrG-dN0vA-HDxkhX6DlV5f18d515mfdgcweVAg_p-r5QI3hOWrmAmCxG7tYUn?key=3DZa9zryPLreVs5cxoiPVA)

Class Interactions
==================

User:

-   The User class stores important user information such as login credentials, currency amount, sprite and equipped cosmetics, and the pen color for drawing on the whiteboard.

-   The User class has a reference to the Settings class to keep track of every user's settings preferences.

-   The User class references the Backpack class to keep track of the cosmetics in their backpack.

-   The User class has getters and setters where applicable, and also has the methods to join/leave a room, draw on the whiteboard, and change the scene.

-   The User class also has a relationship with the Shop class, as a Shop can have many User objects associated with it.

-   The User class is associated with the Room class because a Room can have many Users in it.

Settings:

-   The Settings class stores the settings for a specific user, which includes their video/microphone input devices, whether their video/microphone is enabled, and whether their video/microphone should be enabled by default when they enter a room.

-   The Settings class includes getters and setters where applicable.

-   The Settings class is connected to the User class in that every user will have one Settings.

Backpack:

-   The Backpack class stores a given user's purchased cosmetics.

-   The Backpack class has a getter method to get the cosmetics the user has purchased, and a method to add a cosmetic to the purchased list when it is bought from the shop.

-   Each Backpack has one User associated with it, and each User has one Backpack.

Shop:

-   The Shop class stores all the cosmetics that the user can purchase for their sprite or the room.

-   There is one Shop in the whole program, and this Shop is associated with many Users. The users' views of the shop will all be different depending on what items they have purchased, but the items available in the shop will be consistent among all users.

Room:

-   The Room class stores all data pertaining to the state of the classroom including the join code, the user who created the room (host), the room name, the equipped cosmetics for the room, the file cabinet, and the whiteboard.

-   The Room class can have a reference to many Users, but a User can only be a part of one room at any given time.

-   The Room class references both the FileCabinet and Whiteboard classes, of which there can only be one of each. These objects can only be tied to one Room object as well.

FileCabinet:

-   The FileCabinet class stores an array of File objects. 

-   The FileCabinet class has a getter for the files as well as methods to import, export, add, and delete files.

-   Every Room will have one FileCabinet and one FileCabinet will only be associated with a single Room.

-   A single FileCabinet class is associated with many File objects, and all File objects are only associated with one FileCabinet.

File:

-   The File object stores file data such as the file name, description, owner, and contents.

-   The File class includes all the appropriate getters and setters.

-   The File class is associated with the FileCabinet because a FileCabinet will store many files while a File can only have one FileCabinet.

Whiteboard:

-   The Whiteboard class contains a method that allows players to erase all contents of the whiteboard. 

-   The Whiteboard class is associated with the Room class, as every Room has one Whiteboard and vice versa.

Sequence Diagrams
=================

![](https://lh7-rt.googleusercontent.com/docsz/AD_4nXeAg0GDuD52FOPA6K81NAkkLPTYfBqArTfeAv07qJKzt1xvx5COvEWxaZDCUgU29xgptjHh-u-wQCxP0RCZhE0NcYFX5ZiHop8pok1B69n6z6YCJFAEU8NRSWiKI4VYk0CNMxvzi7Jv6uY1kM0nbcradUNl?key=3DZa9zryPLreVs5cxoiPVA)

![](https://lh7-rt.googleusercontent.com/docsz/AD_4nXcdNk-2-KF8zV-KAQN6TGjcD-k3pVWuLustJ3sNqY_ebjQlmZUp9EGE0AP8ltRpWmrTMc_9zMKixeh1nlTbqV6Efg7mLsPzbblvG92-A1BCcvDbBJvwjX69t6lt1WVf8cdWwGdC-iGrnPaUUmc1Bkb6j00?key=3DZa9zryPLreVs5cxoiPVA)

![](https://lh7-rt.googleusercontent.com/docsz/AD_4nXfAfC-KmV9cFjZbY7s1wgRdHO7f5uGqrookSXSFgIJpP9L-E7YBu_dOb5BATuBP2voLPAjAcDfxHU8RwL_Bhk5XxwiJOso0P4bQr_2uRf_naNbH3c-RWft3LLV1NhtkS2xbbyicgfkh69gamJqy0wsmuQHb?key=3DZa9zryPLreVs5cxoiPVA)

![](https://lh7-rt.googleusercontent.com/docsz/AD_4nXcO5BpGxNmZUNlnk348DQHRwOM3EMjl0QH8_lI62r6-JNhTDVBT-PnIhfvUoj8zp8wOnkaSbKgqh5VYxEQtDeHZCZG8f29rZ06QaGKFr7GaFybbkggsNK73-uEILBDBNR6nFTzgYL5_Fytk_1-ULaYDXl0?key=3DZa9zryPLreVs5cxoiPVA)

![](https://lh7-rt.googleusercontent.com/docsz/AD_4nXdx5EQkc8_wGYe9PVXkrXoziGow9ekGHX7Ta1As9ggRxr2f4iN6SQ6k7uBTXBlKKolkaPULANnhNqt_o6dxMOk2LXT5TegJ5fGYmmwLsyvLi8PyTQ0qVARJNGypqiLSI495DNEyVMgw3tFYehH1YoN9Qn6g?key=3DZa9zryPLreVs5cxoiPVA)
