# chromatin_study

## Overview
This project is part of an ongoing development poject for AR chromatin visualization.
NB. The majority of recent work is contained in the _study_ branch.

## Running the project
Opening the project in Unity should automatically load all required files and assets. The study manager object provides control over various parameters available.
Settings such as running in 2D or 3D can be changed as well as the starting trial.

## Known Issues
* One known issue is that at some point after introducing shaders when running the study previous visualizations don't always dissapear.
This should be a straightforward fix, but as a workaround due to time constraints I was just stopping and running the project and manually incrementing the trial number.

## File Structure

### /Assets/Scripts
This is where the majority of the code for the project is contained. Each file in the folder is for an object used by the project.

#### Curve.cs
This file managed the majority of operations for rendering the curve and is the primary entry point for modifying the visual properties of the rendered curve.
