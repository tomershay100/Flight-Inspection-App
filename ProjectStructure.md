# Project Structure
The structure of the application is based on the MVVM Design Pattern which divides the project files and its departments into 3 main parts, the Model, the ViewModel and the View. When each of them has a responsibility for a different part of the app while communicating with each other through *Binding* and through PropertyChanged.

## View
### MainWindow:
This class uses a XMAL file which defines the application design. It includes several buttons for uploading files and UserControls such as Media Controls, Sliders, Indicators, Joysticks, etc. This class is the View which is responsible for displaying the main Tab in the app and therefore does nothing but update the ViewModel on changes that have come from the user (like button clicks). For this purpose, the class contains ViewModel. Like updating the ViewModel in the user actions, the class is responsible for displaying the information to the user and therefore receives PropertyChanged notifications from the ViewModel so that it can update the data values as soon as they change.
### GraphsUC:
This class is a UserControl which is responsible for displaying the Graph Tab, which has 4 graphs, a list of the flight features, a button for uploading a DLL file, and a list of anomalies.

## View Model
### ViewModel:
This class drives all the software, is responsible for the logic of the application and is the one who decides what to do and when, what functions to run and what to send to whom. The class holds various Models and receives from them PropertyChanged notifications when a certain statistic has changed. It also knows when the user performed actions in the application from the View and therefore knows how to synchronize all actions of the application. For example, when the user enters the play speed of the flight and presses submit, the View notifies the ViewModel and it decides to change the playspeed field in the Model.

## Model
### AnomalyData:
This class is one of the Models, it responsible for the flight data. Contains SimpleAnomalyDetector which is responsible for flight analysis. The model holds a List of the flight data to be displayed in the Graphs Tab, holds the column that the user has selected and knows how to analyze, process and notify the ViewModel of changes in the various graphs and some flight data that displayed in the Main Tab. This model also communicates with the DLL and knows how to send it the relevant flight files and receives from it the information about the graphs to be displayed. For example, when updating the selected column, the class knows how to change the values of the relevant point lists, change the regression line and ask the DLL for its new graph.
### XmlAnalyzer:
The class contains a static method that gets an XML file and analyze it. The return value is a feature list.
### AnomalyDetectionUtil:
The class is a kind of library of mathematical operations such as average, correlation, variance and regression line creation. This class is used for learning and processing flight data.
