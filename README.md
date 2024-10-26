# story-graph-unity
Unity version 2021.3.6f1

For this project, you must install and run another StoryGraph repo(private), instruction:
https://github.com/iwonagg/StoryGraph

If you don't have Unity, follow instruction to download & install it

## **Instruction:**

1. Go to the page: https://public-cdn.cloud.unity3d.com/hub/prod/UnityHubSetup.exe?_ga=2.114418887.1383103553.1676639950-1675160909.1653504733
2. Install the downloaded file.
3. Skip the version installation.
4. Go to the page: https://unity.com/releases/editor/archive
5. Open the Unity 2021.X section.
6. Scroll down, and there you can find version 2021.3.6.
7. Select Unity Hub.
8. In the opened window, choose Universal Windows Platform Build Support & WindowsBuildSupport (IL2CPP).
9. Click "Install" and wait.
10. If you don't have Visual Studio, Unity Hub will install the packages for you. When the Visual Studio installer opens, click "Install" and "Add workload packages."
11. After installation, you can close the Visual Studio installer.
12. If you don't have a Unity license, click "Manage License," then click "Free Personal License" and close the preferences window.
13. Now we can add the project to Unity Hub. Click the "Add" button at the top, find your cloned project, and click "Open." The StoryGraph file should then appear in the list. Click on it, and the project will be launched.
14. Wait until all the packages are installed.
15. Next, we need to clone the StoryGraph API project. To do this, go to the page and download the project: https://github.com/iwonagg/StoryGraph
16. Open the project with any IDE; I use PyCharm.
17. Next, from the top, select the "webapi" file to run.
18. In the same file, on line 51, you can optionally change the value of the "world_name" variable. By copying the names in the comment and pasting them into the variable, you can change the game world. Initially, the variable has the value "World_PWK2021_misje_P."
19. Now you can run the API by clicking the green button at the top and wait for the script to start. If you need to stop the project, click the red square. The API must always be running before starting the game.
20. If the running scene is "untitled" or simply not "Boot," go to the Assets -> Scenes folder, find the "Boot" scene, and double-click on it.
21. Also, check if the scenes have been added to the startup list. To do this, select File -> Build Settings from the top. If two scenes, "Game" and "Boot," are listed at the top, everything is fine. Otherwise, you need to open each scene as described in the previous step, go to Build Settings, and click "Add Open Scenes."
22. Now we can go to the "Boot" scene, click the triangle at the top, and start the game. It might take a moment to load.
23. Use WASD for movement, jump with SPACE, open the inventory with TAB, and the production list with CAPS LOCK.
24. If you want to interact with an object, simply approach it and press CAPS LOCK. A list of actions you can perform will appear. To move to another location, just approach the portal.


Video, how to run project: https://github.com/user-attachments/assets/50bc93be-53a4-4360-8948-20145b61f259


## **Control:**
WASD - Move <br />
Space - Jump <br />
Tab - Inventory <br />
CapsLock - Activities
