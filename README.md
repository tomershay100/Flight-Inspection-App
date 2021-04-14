# Flight-Inspection-App

#### Contributes
* Roei Gida
* Shilat Givati
* Tomer Shay
* Amit Sharabi

This is a desktop application for flight anomalies detections.
1. [General](#General)  
  * [Background](#background)
  * [Project Description](https://github.com/tomershay100/Flight-Inspection-App/blob/main/README.md#project-description)
  * [Project Stucture](https://github.com/tomershay100/Flight-Inspection-App/blob/main/README.md#project-stucture)
  * [Features](https://github.com/tomershay100/Flight-Inspection-App/blob/main/README.md#features)
2. [Dependencies](#dependencies)  
3. [Installation](#installation)

## General
### Background
This application displays filght data on a simulator and investigates them. The Users are flight reserches or pilots who want to view data that sampled at certain rate during a flight.
The flight data includes the steering mode, speed, direction etc, and are recorded into a text file which can be loaded in out application.
Tha app plays the data like a movie, graphiclly showing the plane relative to the earth (on FlightGear), the ruuder status and additional filght data in a number different views, including a view designed to find anomalies in the data.

### Project Description
This project inerfaces with the FlightGear simulator (instruction for download at [Dependencies](#dependencies)) and they (?) work side-by-side. The idea is that the user will upload a normal flight file as _train file_. The program will learn it and now the user can upload any flight file (as _test file_) according to the _train file_. Any _test file_ the program will represent as a movie - there is a playback that the user can control by pressing the buttons and slider. Furthermore, there is a presentation of some important flight data as **yaw, pitch, airspeed** etc, and some graphs that represent the flight data of a specific featre from the beggining to the current time of the simulation.

### Project Stucture
This project designed according to MVVM architecture. The classes can be divided into two groups in order to create total segregation between the pressentation logic and the business logic.
The pressentation logic implemented in:
* MainWindow class
* MediaControls class
* GraphsUS class
* Joystick class
* Gauges class

The business logic implemented in:
* Client class
* FlightGearClient class
* Panel class
* AnnomlyData class


This classes can communicate via the ViewModel class that constitutes an abstract layer of Model to the View and an abstract layer of View to the Model.
You can see more information about the class hierarchy in [UML](https://github.com/tomershay100/Flight-Inspection-App/blob/main/UML%20Diagram.pdf).

### Features
* **Upload CSV File:** When the user clicks the ```Upload CSV Test File```  and uploads CSV file, the flight will start and the flightgear simulation will show the flight according to this file. 
* **Graph Tab:** When the flight starts, a new tab opens. In this tab, the user can see the graphs of every flight feature and it's correlated feater.
* **Upload DLL:** The user can upload any annomly detection alogithem and the results will apear on a graph.
* **Flight Features Graphs:** The user can select a feature and a graph will be shown.
* **Joystick:** The ```Elevator``` and ```Aileron``` feature represented as joystick on Y-pos and X-pos **בהתאמה**.
* **Playback:** The flight is shown as a movie:
  - Back Button:    Brings the flight to the start.
  - Rewind Button:  Brings the flight 7.5 seconds back.
  - Pause Button:   Stops the flight.
  - Play Button:    If paused or stopped, start playing from the same spot that stoped.
  - Stop Button:    Stops the flight and brings the flight to the start.
  - Forward Button: Brings the flight 7.5 seconds ahead.
  - Skip Button:    Brings the flight to the end.
  - Play Speed:     Enable the user to decide ehat speed hw eants to see the flight.
  - Slider:         Enable the user to jump forwards and backward.
* **Upload Several Test Files:** The user can upload as many test files as he wishes. The last flight will stop and the next will start.

## Dependencies
1. [FlightGear](https://www.flightgear.org/download/)
2. .NET 5.0
3. Oxyplot
4. [CircularGauge](https://www.nuget.org/packages/CircularGauge)
5. 

#### Extra dependencies for developers:
* WPF
* C#

## Installation
1. Clone the repository:  
    ```
    $ git clone (https://github.com/tomershay100/Flight-Inspection-App.git)
    ```
2. Run the program from command line: 
     ```
    $ cd _pathTo Flight-Inspection-App_
    $ start bin\DesktopApp
    ```
