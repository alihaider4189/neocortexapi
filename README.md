[![license](https://img.shields.io/github/license/mashape/apistatus.svg?maxAge=2592000)](https://github.com/ddobric/htmdotnet/blob/master/LICENSE)
[![buildStatus](https://github.com/ddobric/neocortexapi/workflows/.NET%20Core/badge.svg)](https://github.com/ddobric/neocortexapi/actions?query=workflow%3A%22.NET+Core%22)
# Project Title
ML21/22-28 Improve Unit Test (Spatial Pooler and Temporal Memory)

## Team Name
UnitCodeMaster

## Getting started

se-cloud-2021-2022
These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.
See the notes down on how to deploy the project and experiment on a live system.
NeoCortexApi is today built as .NET Standard 2.2 library. It contains all artefacts required to run **SpatialPooler**, **TemoralPooler** and few encoders.
First important step in using of any algorithm is initialization of various parameters, which are defined by class `Parameters`.

## Project Directory Guide
* **../ML21/22 28.  IImprove Unit Test (Spatial Pooler and Temporal Memory)/** 

This directory has our Final Project codes 
<a href="https://github.com/alihaider4189/neocortexapi/blob/UnitCodeMaster/source/UnitTestsProject/TemporalMemoryTests.cs"  target="_blank">Final Project Codes of Temporal Memory</a>
 <a href="https://github.com/alihaider4189/neocortexapi/blob/UnitCodeMaster/source/UnitTestsProject/SpatialPoolerTests.cs"  target="_blank">Final Project Codes of Spatial Pooler</a>
* **../Documents/** 

```
This directory contains all documents regard this project. Including experiment design, schedule, and the project document.(Remarks- Work in progress)

```
* **../Presentation/** 
```
This directory contains Everything regards our presentation. (Remarks- Work in progress)

```
# Project Plan
The commitment of each person on program can be tracked by following table

1. Read the all research papers and saw all relavent videos which are availble on youtube channel "Numenta" and from professor recorded lecture to make 
	understading of topic theoritically at some extent.

2. Analysing the existing units tests of spatial pooler and Temporal memory by changing its values then debug it and observe the output of the function.
3. While Analysing the code we usully go to defination of that line or variable see whats the actual meaning of that varible
	 for example
	 ```
	 DistalDendrite dd = cn.CreateDistalSegment(cn.GetCell(0));
	 ```
	 how actually  CreateDistalSegment used?
	 ```
	 the newly created segment or a reused segment
	 ```
	Its took alot our time to make full uderstanding of already done unit tests

4. Created some new unit test by using random variable and try to figure out whats actually happening after using it 
	```
	public BurstingResult BurstColumn(Connections conn, Column column, List<DistalDendrite> matchingSegments,
            ICollection<Cell> prevActiveCells, ICollection<Cell> prevWinnerCells, double permanenceIncrement, double permanenceDecrement,
                Random random, bool learn)

	
	```
5. Created many New Unit Tests and did some modification in already existing unit tests and try to improve in it

# Contribution
The commitment of each person on program can be tracked by following table

| Name | Commitment on master branch | Remarks |
| :---------------: | :-------------: | :---------: |
| Ali Haider        | https://github.com/alihaider4189/neocortexapi/commits/UnitCodeMaster?author=alihaidershafique |  |
| Naveed Ahmed      | https://github.com/alihaider4189/neocortexapi/commits/UnitCodeMaster?author=naveed401 |  |
| Ali Raza Kharl    | https://github.com/alihaider4189/neocortexapi/commits/UnitCodeMaster?author=alirazakharl |  |

Please Review our work give us a feed back


