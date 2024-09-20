Class-ify

Product Backlog - Team 20

Shelby Joyner, Gavin Fortwendel, Cass Mitchell, Clayton Whitten

Problem Statement
=================

While the world of online communication is vast, there are limited products where students can virtually come together to study in a fun classroom setting. When big study groups or students who have online classes need to meet, products like Zoom can't provide the immersion that Class-ify does. Class-ify is the only all-in-one virtual classroom experience that provides students with all of the tools they would need to not only succeed, but excel in their classes.

Background Information
======================

Audience:

Our audience is students who are taking online courses or would like to communicate about class materials online. We are specifically targeting students who are interested in video games and studying online, especially those who feel that other platforms like zoom do not provide enough interactivity.

Domain:

Game Development, Virtual Communications, Educational Software

Similar Platforms:

Zoom provides a means of virtual communication through online chat and virtual meetings along with additional scheduling features. Google Meet is an online conference platform for video calls. Gather is a game-ified version of virtual meetings that combines video calls with online map navigation.

Limitations:

While other platforms provide virtual communication, they fall short in offering an interactive and engaging environment for students. These apps lack the immersive classroom feel that allows for collaboration and keeps students engaged, which makes study sessions feel disconnected. Class-ify addresses this by providing a unique all-in-one virtual classroom experience that is tailored to students. With its collaborative tools and gamified features, Class-ify transforms online studying into an interactive, enjoyable experience that helps students stay engaged and excel in their coursework.

Functional Requirements
=======================

1.  User interactions:

1.  As a user, I would like to be able to move around inside a room.

2.  As a user, I would like to see other users' sprites in a concurrent room.

3.  As a user, I would like to be able to choose to create a room or join a room via a menu.

4.  As a user, I would like to see other user's display names.

5.  As a user, I would like to be able to give another user a thumbs up for good work.

1.  Room:

1.  As a user, I would like to join a room with a join code.

2.  As a user, I would like to create a room.

3.  As a user, I would like to be able to reopen a room I previously created.

4.  As a user, I would like to leave a room. 

1.  Login/sign-up:

1.  As a user, I would like to be able to create an account.

2.  As a user, I would like to be able to log in to my account with an email and password.

3.  As a user, I would like to be able to log in using Google, Apple, or Microsoft.

4.  As a user, I would like to be able to reset my password if I forget it.

1.  Settings Menu:

1.  As a user, I would like to open a menu for settings options.

2.  As a user, I would like to change my microphone input device.

3.  As a user, I would like to change my camera input device.

4.  As a user, I would like to change the volume.

5.  As a user, I would like to change my display name.

1.  File sharing:

1.  As a user, I would like to interact with a 2D file cabinet object that opens a file sharing menu.

2.  As a user, I would like to upload files from my device for anyone to view.

3.  As a user, I would like to export files to my device.

4.  As a user, I would like to view unrestricted files uploaded by other users.

5.  As a user, I would like to get currency if someone downloads a file I uploaded.

6.  As a user, I would like to have files I upload remain uploaded, even after I leave.

1.  Whiteboard:

1.  As a user, I would like to interact with a 2D whiteboard object that opens a whiteboard frame.

2.  As a user, I would like to draw on the whiteboard.

3.  As a user, I would like to erase my work from the whiteboard.

4.  As a user, I would like to see other users' drawings on the whiteboard in real time.

5.  As a user, I would like to use different colors on the whiteboard.

6.  As a user, I would like to be able to lock the whiteboard so only I can draw on it.

7.  As a user, I would like to draw on the whiteboard concurrently with other users.

1.  Shop/Customization:

1.  As a user, I would like to purchase a cosmetic of my choice from the shop using currency.

2.  As a user, I would like to gain currency when giving another user a thumbs up.

3.  As a user, I would like to receive currency when someone else interacts with my files.

4.  As a user, I would like to view the shop window and browse items.

5.  As a user, I would like to view the amount of currency I have.

1.  User inventory:

1.  As a user, I would like to click a button that opens an inventory menu.

2.  As a user, I would like to see all the cosmetics I have purchased for my sprite.

3.  As a user, I would like to see all the decorations I have purchased for the room.

4.  As a user, I would like to apply cosmetics to my sprite.

5.  As a user who created the room, I would like to apply decorations to the room.

1.  Audio communication:

1.  As a user, I would like to talk to other users in a concurrent room.

2.  As a user, I would like to mute my microphone on the game screen.

1.  Video Communication (If time allows):

1.  As a user, I would like to have a choice to join a video call with other users.

2.  As a user, I would like to be able to turn off my camera on the game screen.

Non-Functional Requirements
===========================

Architecture:

Class-ify will be built using Unity with its scripts written in C#. We will be using Firebase's no-cost Spark plan to store user account data and room data, as it has authentication features built into it and integrates smoothly with Unity.

Performance:

Class-ify is a multiplayer application, thus it is paramount that it has real-time concurrency between players and the server. We will use Photon to implement this, which has a limit of 16 players on the game at a time. Firebase will allow for smooth file upload and downloads, and will resume if the connection dissipates. Firebase can store up to 1GB worth of data, which should be enough for Class-ify  as a course project. The Firebase real-time database supports 100 simultaneous connections, which easily meets our project requirements.

Scalability:

The app should be created in a way where additional features can be added without breaking any of the existing features or affecting the performance of the application. Firebase can support a vast amount of user data depending upon the selected storage plan, allowing for flexibility in how many users Class-ify can manage.

Usability:

We will strive to make Class-ify as user friendly as possible, employing easy-to-navigate screens and clean UI features. We plan to use well-known key mappings used in video games to give Class-ify an intuitive control system. We plan to make the game as accessible as possible by limiting the system requirements to run it.

Security:

One of the security concerns with Class-ify will be how the user credentials are stored. We will be using Firebase's authentication system, which allows users to sign in using email/password, and also sign-in providers such as Google, Microsoft and Apple. Firebase will also provide security for files, only allowing the specified users access.  

Hosting/Deployment:

Class-ify will be developed using the Unity engine with servers hosted using Photon. For deployment, we will use Unity to create a build for WebGL, allowing us to host our game on a website.
