# Unity
```
MOTAR's Unity SDK
```
MOTAR's Unity SDK makes it incredibly easy to develop training apps across devices andhave them instantly feature the MOTAR login, link to the MOTAR Training LMS/LRS, and be (^)
discovered in MOTAR Hub.
Need Additional Help? Reference the functionality. API documentation for more details on
# Getting Started
```
This is the getting-started documentation for theaccess the Unity plugin repo from MOTAR Studio. MOTAR Unity SDK. You can
```
## Before You Start^
Using the MOTAR Unity SDK requires a MOTAR developer userID with access toPRODUCTION. You will obtain this ID as part of the onboarding process, and use it to sign (^)
on to the MOTAR Unity SDK. The apps you create, however, initially point to the Sandboxenvironment. To access it, and to test apps that you build, you will need one or more
Sandbox userIDs. To learn about creating Sandbox userIDs, click here.
#### Projects Requirements
```
NOTE: This SDK uses C# .NET 4.6 equivalent. This package was built using Unity 8.0 features and supports scripting runtime version 2020.3 LTS
```
(^) **Version Number:**
In Project Settings under the Player section confirm your App Compatibility Level is set to.NET 4.x
## SDK Prerequisites^
Step 1: Open your Unity Project
Step 2: Go to Window -> Package Manager -> and click this setting icon. From this menuselect Advanced Project Settings
From the Projects Settings window, select the Package Manager tab here you will need to"Enable Preview Packages" and "Show Dependencies".
Exit the Project Settings window and view the Project Manager window again. Click on the"Packages in Project" dropdown and select "Unity Registry" type "Editor" in the windows
search bar.
Find the "Install" button on the bottom right of the "Package Manager" window.
```
Install: Editor Coroutines
```
Back to the Package Manager search bar, next search for "UI" and install "UI Builder". Installthe preview .14 version (latest shown).
Next click “+” in Package Manager
Choose Add Package from Git URL
Type ARE GREYED OUT – CLOSE AND REOPEN THE PROJECTcom.unity.ui to install the UI Toolkit package IF UI TOOLKIT OPTIONS
IN THE PACKAGE MANAGER look at the UI Toolkit Entry. TextCore should bethere and installed (if not install it. Also, Import the Samples.)
## Install The Unity Package^
#### Studio Download
As a developer now it's time to import the DX-Unity package. Download the SDK from thelink found in MOTAR Studio here.
After downloading the package open your unity project window and click Assets -> ImportPackage -> Custom Package
#### Unity Import
The device file browser will open, select the unity package and import it. Select All and clickImport.
After Importing the package you will now have access to the following MOTAR tabs used tointeract with the SDK:
```
MOTAR Discover Tab /discoverstudio/sdks/unity/motar-sdk/motar-
MOTAR SDK Tab /studio/sdks/unity/motar-sdk
MOTAR-StateMachine-Animator /graphstudio/sdks/unity/motar-sdk/motar-
```
# MOTAR SDK Tab
## Introduction
The MOTAR SDK tab organizes the core features of the SDK for easy developer access.
When interacting with the MOTAR SDK tab the user will start off with an Authentication UI.Here the user will authenticate using their MOTAR account credentials.
After authentication, the user will see four tabs **APIs, MOTAR Discover. MOTAR Setup, MOTAR Prefabs, MOTAR**
## MOTAR Setup^
#### App Selection
This App Selection section pulls sandbox application data into the SDK from the user'sstudio organization. Here the developer has easy access to the app icon, name, description, (^)
client-id & client-secret.
```
If you don't see your application in the app selection tile, make sure you have apair of clientId & clientSeceret Values generated on MOTAR Studio.
```
#### MOTAR Sandbox User Impersonation
As a developer, MOTAR Studio account holders have access to the Sandbox-Trainingenvironment. Select a user from the available name dropdown list shown here:
For this example, this sandbox user has been added to a class on MOTAR Training sandboxand has access to a course with an assessment module. Selecting this user's name from
the list, clicking "Save Settings" and then "Impersonate" automatically pulled all that datafrom the course into the SDK with no friction for the developer.
```
To interact with the Training related tiles mentioned below you need to ensureyour application has the Training Permission Scope on MOTAR Studio
```
#### Class Info
In this tile, the developer has access to all classes from the selected course. Here the classname will display along with groupID and join code values.
```
Group ID & Join Code values are required when interacting with class-related APIcalls! Here the SDK pulls and displays what you need as soon as a course is
"Name" is selected!
```
#### Course Info
As a developer, I will see Course Info displayed in this tile. This information is updatedbased on the class selected in the previous tab. Here the developer has access to the course
name, description, and the exposed course ID string.
#### Lesson Info
In this Lesson Info Tile, the developer has access to any needed lesson informationincluding (Name, Description, and LessonID).
```
All the exposed training data from the selected sandbox course displayed in thistile encourages the developer to interact with other available API calls quickly.
```
#### Questions
The questions tile will display all assessment questions from the developer's selectedlesson module from the previous tile.
### MOTAR Prefabs
The MOTAR Unity SDK uses prefabs both to represent the state of a given assessmentapplication and to facilitate communication with the MOTAR back end. The SDK’s template (^)
state machine represents a typical flow for a student taking an assessment after learningmaterial.
It can be used as-is for most applications, or easily modified as needed to introduce newstates as the specific app requires. The visual prefabs – Logged in/Logged out state, Login (^)
Form, User Profile info – are all designed to be branded as needed for the application youare developing. So long as you maintain the prefab structure and the connected scripts, the (^)
application can be updated visually without any disruption to its flow. “Note: we areworking closely with Unity’s Visual Scripting team and will be updating the SDK on an (^)
ongoing basis, to introduce additional no-code/low-code capabilities”
#### MOTAR Authentication
Login State Button: This prefab handles / represents the users holds a script called "MOTAR Login State Button Handler" with the following data. TheLogin State. The prefab
prefabs are shown below.
Login UI Canvas: This prefab is a Canvas UI used to login. Here the developer will enter theusername and password in the fields found on the right of the canvas. In this scene, the
user will input sandbox user information.
After auth the information. MOTAR-UserInfoCanvas will populate with the sandbox user's account
User Profile Canvas: The user profile canvas sandbox user info (profilePicture, first & last name) after authentication from theMOTAR-UserInfoCanvas populates with
Login UI Canvas.
#### Assessment Components
Start Module Button: The start module button prefab removes/hides the for the user and shows theQuestion Canvas in our demo example. This Start ModuleUserInfoCanvas
Button can be configured to push the user to any user-defined canvas/scene.
Questions Canvas: The Question Canvas prefab is set up to load questions from thedeveloper's assessment lesson. This canvas prefab gives you the ability to set up an app-
based assessment lesson in minutes!
Test Result Canvas: The test result canvas displays the user's assessment score.
## MOTAR APIs^
This tab in the SDK pulls in data directly from our Gitbook documentation.
On the left side of the panel, you will see all the API documentation sections and pages.Here the developer can select a page -> select an API call from the HTTP METHOD
ENDPOINT table -> to view the summary and description of the request.
```
To see the full documentation of any API on the web, click on button
```
The easiest way to call REST APIs from a MOTAR Unity application is by using theDXAPIRequest gateway, which properly authenticates the API with the student's credentials. (^)
To generate a code snippet that would properly call any of our REST APIs using thestudent’s credentials, select the API you need, and use the clipboard icon to populate the (^)
code snippet into your clipboard buffer. You can then paste the code anywhere it isrequired. The data is returned as a Json.net object or a Texture2D where applicable. For (^)
brevity, the snippet returns the data into a lambda completion function, which of course canbe replaced by a delegate function of your choice.
## MOTAR Discover^
```
Coming Soon
MOTAR Discover Tab /discoverstudio/sdks/unity/motar-sdk/motar-
```
# MOTAR-StateMachine-Animator
The MOTAR-StateMachine gives the developer access to a State Machine setup page usinga graph diagram format. Each state machine can be different / user-created.. here are some (^)
important things to know:
1. Each node represents a piece state.2. Arcs or "arrows between nodes" represent the transitions between state (^)
State machines can be designed and updated easily with little to no coding.
Visit this link for more information about State Machine Basics.
# MOTAR Discover Tab
Coming Soon
The MOTAR Discover page will pull in digital asset and AI listings from MOTAR Hub.
As a developer, this will allow quick and easy implementation of digital assets into yourUnity project.
# Accessing REST APIs
The MOTAR Unity SDK integrates your access to apps, classes, lessons, and the APIsrequired to authenticate students, and update their performance via assessments, all using (^)
built-in Unity objects and prefabs.
Additional functionality is available via the DXAPIRequest gateway to MOTAR’s REST APIs.
To generate a code snippet that would properly call any of our REST APIs using thestudent’s credentials:
12.. OPEN the MOTAR API tab under the MOTAR Unity SDKSELECT the API you need, and use the clipboard icon to populate the code snippet into
3. your clipboard buffer.COPY and paste the code anywhere it is required.^
The data is returned as a Json.net object or a Texture2D where applicable.
For brevity, the snippet returns the data into a lambda completion function,which of course can be replaced by a delegate function of your choice.
API /studio/api
```
Link to MOTAR APIs Section of MOTAR SDK
```
MOTAR SDK Tab https://docs.motar.io/studio/sdks/unity/motar-sdk#motar-apis
docs.motar.iodocs.motar.io
MOTAR SDK Tab
Unity SDK