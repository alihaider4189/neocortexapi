#Project Title: 
ML 21/22 - 28 Improve Unit Test (Spatial Pooler and Temporal Memory)
        

#Introduction # 

This repository is the open source implementation of the Hierarchical Temporal Memory in C#/.NET Core. 
This repository contains set of libraries around **NeoCortext** API .NET Core library. **NeoCortex** API focuses implementation
of _Hierarchical Temporal Memory Cortical Learning Algorithm_. Current version is first implementation of this algorithm on.
 platform. It includes the **Spatial Pooler**, **Temporal Pooler**, various encoders and **CorticalNetwork**  algorithms.
Implementation of this library aligns to existing Python and JAVA implementation of HTM. Due similarities between JAVA and C#, 
current API of SpatialPooler in C# is very similar to JAVA API. However the implementation of future versions will include some
API changes to API style, which is additionally more aligned to C# community.
This repository also cotains first experimental implementation of distributed highly scalable HTM CLA based on Actor Programming Model.
The code published here is experimental code implemented during my research at daenet and Frankfurt University of Applied Sciences.

#Group Name: 
Unit Code Master

SpatialPooler.md - NeoCortex Document folder
#1) Spatial Pooler
Spatial Pooler (SP) is a learning algorithm that is designed to replicate the neurons functionality of human brain. Essentially, if a brain sees one thing multiple times, it is going to strengthen the synapses that react to the specific input result in the recognition of the object. Similarly, if several similar SDRs are presented to the SP algorithm, it will reinforce the columns that are active according to the on bits in the SDRs. If the number of training iterations is big enough, the SP will be able to identify the objects by producing different set of active columns within the specified size of SDR for different objects.

The HTM spatial pooler represents a neurally inspired algorithm for learning sparse representations from noisy data streams in an online fashion. ([reference](https://www.frontiersin.org/articles/10.3389/fncom.2017.00111/full))

Right now, three versions of SP are implemented and considered:

Spatial Pooler algorithm requires 2 steps.

## 1. Parameters configuration

 There are 2 ways to configure Spatial Pooler's parameters.

 1.1. Using `HtmConfig` (**Preferred** way to intialize `SpatialPooler` )

 ```csharp
 public void SpatialPoolerInit()
 {
	HtmConfig htmConfig = new HtmConfig()
	{
	InputDimensions = new int[] { 32, 32 },
	 ColumnDimensions = new int[] { 64, 64 },
	 PotentialRadius = 16,
	 PotentialPct = 0.5,
	 GlobalInhibition = false,
	 LocalAreaDensity = -1.0,

	 // other parameters
	}				   
					  ;

 Connections connections = new Connections(htmConfig);

 SpatialPooler spatialPooler = new SpatialPoolerMT();
 spatialPooler.Init(connections);
`}

```

### Parameter desription

| Parameter Name                  | Meaning                                                                                                                                                                                                                                                                                                          |
| ------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| POTENTIAL_RADIUS                | Defines the radius in number of input cells visible to column cells. It is important to choose this value, so every input neuron is connected to at least a single column. For example, if the input has 50000 bits and the column topology is 500, then you must choose some value larger than 50000/500 > 100. |
| POTENTIAL_PCT                   | Defines the percent of inputs withing potential radius, which can/should be connected to the column.                                                                                                                                                                                                             |
| GLOBAL_INHIBITION               | If TRUE global inhibition algorithm will be used. If FALSE local inhibition algorithm will be used.                                                                                                                                                                                                              |
| INHIBITION_RADIUS               | Defines neighbourhood radius of a column.                                                                                                                                                                                                                                                                        |
| LOCAL_AREA_DENSITY              | Density of active columns inside of local inhibition radius. If set on value < 0, explicit number of active columns (NUM_ACTIVE_COLUMNS_PER_INH_AREA) will be used.                                                                                                                                              |
| NUM_ACTIVE_COLUMNS_PER_INH_AREA | An alternate way to control the density of the active columns. If this value is specified then LOCAL_AREA_DENSITY must be less than 0, and vice versa.                                                                                                                                                           |
| STIMULUS_THRESHOLD              | One mini-column is active if its overlap exceeds overlap threshold  of connected synapses.                                                                                                                                                                                                                       |
| SYN_PERM_INACTIVE_DEC           | Decrement step of synapse permanence value withing every inactive cycle. It defines how fast the NeoCortex will forget learned patterns.                                                                                                                                                                         |
| SYN_PERM_ACTIVE_INC             | Increment step of connected synapse during learning process                                                                                                                                                                                                                                                      |
| SYN_PERM_CONNECTED              | Defines Connected Permanence Threshold  , which is a float value, which must be exceeded to declare synapse as connected.                                                                                                                                                                                        |
| DUTY_CYCLE_PERIOD               | Number of iterations. The period used to calculate duty cycles. Higher values make it take longer to respond to changes in boost. Shorter values make it more unstable and likely to oscillate.                                                                                                                  |
| MAX_BOOST                       | Maximum boost factor of a column.                                                                                                                                                                                                                                                                                |

## 2. Invocation of `Compute()`

 ```csharp
 public void TestSpatialPoolerCompute()
 {
  // parameters configuration
 ...

  // Invoke Compute()
  int[] outputArray = sp.Compute(inputArray, learn: true);
 }
 ```

				- 

Ai Haider:[Member 1 ](https://github.com/alihaider4189/neocortexapi/blob/UnitCodeMaster/source/UnitTestsProject/TemporalMemoryTestsNEWByAliHaider.cs)
Naveed Ahmed:[Member 2 ](https://github.com/alihaider4189/neocortexapi/blob/UnitCodeMaster/source/UnitTestsProject/TemporalMemoryTestsNEWByNaveedAhmad.cs)
Ali Raza Kharl:[Member 3](https://github.com/alihaider4189/neocortexapi/blob/UnitCodeMaster/source/UnitTestsProject/SpatialPoolerTestsNEWByAliRazaKharl.cs)
			
         
In the future version of **NeoCortexApi** this will be changed by using properties as commonly used in C#.

Chosing right parameters for experiments is very complex task and it will not be further described in this guide.
Moreover, **SpatilaPooler** will use 1/4 of columns as inhibition inside of  inhibition (INHIBITION_RADIUS),
and 50% of columns inside of inhibition radius will be used as so called "winner columns". 
This set of parameter defines sparsity of algorithm

This code snippet shows how to set described parameters:
                   
# 2) Temporal Pooling:
 We can grasp the sequential pattern throughout time thanks to Temporal Pooling. 
 It learns the current column's sequences from the Spatial Pooler and guesses what spatial pattern will appear next depending on 
the temporal context of each input.
		
The Temporal Memory algorithm learns sequences and makes predictions. In the Temporal Memory algorithm, when a cell
becomes active, it forms connections to other cells that were active just prior. Cells can then predict when they will become active
 by looking at their connections. If all the cells do this, collectively they can store and recall sequences, and they can predict what
is likely to happen next.

## Parameter description

| Parameter                   | Description                                                                                                                                                              |
| --------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| CELLS_PER_COLUMN            | Number of cells per columns in the SDR                                                                                                                                   |
| ACTIVATION_THRESHOLD        | The activation threshold of a segment. If the number of active connected synapses on a distal segment is at least this threshold, the segment is declared as active one. |
| LEARNING_RADIUS             | Radius around cell from which it can sample to form distal connections.                                                                                                  |
| MIN_THRESHOLD               | If the number of synapses active on a segment is at least this threshold, it is selected as the best matching cell in a bursting column.                                 |
| MAX_NEW_SYNAPSE_COUNT       | The maximum number of synapses added to a segment during learning.                                                                                                       |
| MAX_SYNAPSES_PER_SEGMENT    | The maximum number of synapses that can be added to a segment.                                                                                                           |
| MAX_SEGMENTS_PER_CELL       | The maximum number of Segments a Cell can have.                                                                                                                          |
| INITIAL_PERMANENCE          | Initial permanence value for a synapse.                                                                                                                                  |
| CONNECTED_PERMANENCE        | If the permanence value for a synapse is ≥ this value, it is “connected”.                                                                                                |
| PERMANENCE_INCREMENT        | If a segment correctly predicted a cell’s activity, the permanence values of its active synapses are incremented by this amount.                                         |
| PERMANENCE_DECREMENT        | If a segment correctly predicted a cell’s activity, the permanence values of its inactive synapses are decremented by this amount.                                       |
| PREDICTED_SEGMENT_DECREMENT | If a segment incorrectly predicted a cell’s activity, the permanence values of its active synapses are decremented by this amount.     

The Temporal Memory can be initialized as follow:
```cs
public void TemporalMemoryInit()
{
HtmConfig htmConfig = Connections.GetHtmConfigDefaultParameters();
Connections connections = new Connections(htmConfig);

TemporalMemory temporalMemory = new TemporalMemory();

temporalMemory.Init(connections);
}
```

Similar to Spatial Pooler, Temporal memory has a same method to calculate the output SDR: `tm.Compute(int[] activeColumns, bool learn)`. This method takes the result of active columns in the stable output SDR from the Spatial Pooler (training is off) with learning option to produce a `ComputeCycle` object which holds the information of winner cells and predictive cells.

```cs
public ComputeCycle Compute(int[] activeColumns, bool learn)
{
 ...
}
```
Temporal Memory algorithm predicts what the next input SDR will be based on sequences of Sparse Distributed Representations (SDRs)
produced by the Spatial Pooling technique. Each column in the SDR consists of many cells. 
Each cell can have three states: active, predictive, and inactive. 
These cells should have one proximal segment and many distal dendrite segments. 
The proximal segment is the connection of its the column and several bits in the input space.
The distal dendrite segments represent the connection of the cell to nearby cells. W
Then a certain input is fed into the HTM system with no prior state or there is no context of the input, every cell in the active column is active.
This is called bursting. 
With the prior state, the algorithm will choose winner cell for each column based on the context of the previous input. 
From these winner cells, other cells will have the predictive state when the connections to
the current active cells in the distal segment of those cells reach a certain value of ACTIVATION_THRESHOLD.

**../Below there are some  unit tests whicg we Implemented from existing one. In Below Test a function which uses Random/ **
			

```cs
[TestMethod]
[TestCategory("Prod")]
public void TestActivatedunpredictedActiveColumn()
{
 HtmConfig htmConfig = GetDefaultTMParameters();
 Connections cn = new Connections(htmConfig);
 TemporalMemory tm = new TemporalMemory();
 tm.Init(cn);
 // Random random = cn.HtmConfig.Random;
 int[] prevActiveColumns = { 1, 2, 3, 4 };
   Column column = cn.GetColumn(6);
   IList<Cell> preActiveCells = cn.GetCellSet(new int[] { 0, 1, 2, 3 });
   IList<Cell> preWinnerCells = cn.GetCellSet(new int[] { 0, 1 });
   List<DistalDendrite> matchingsegments = new List<DistalDendrite>(cn.GetCell(3).DistalDendrites);
   var BustingResult = tm.BurstColumn(cn, column, matchingsegments,
                       preActiveCells, preWinnerCells, 0.10, 0.10,
                           new ThreadSafeRandom(100), true);
   // Assert.AreEqual(, BustingResult);
   Assert.AreEqual(6, BustingResult.BestCell.ParentColumnIndex);
   Assert.AreEqual(1, BustingResult.BestCell.DistalDendrites.Count());



}
```


```cs
[TestMethod]
[TestCategory("Prod")]
public void testNumberOfColumns_1()
{
            TemporalMemory tm = new TemporalMemory();
            Connections cn = new Connections();
            Parameters p = Parameters.getAllDefaultParameters();
            p.Set(KEY.COLUMN_DIMENSIONS, new int[] { 128, 128 });
            p.Set(KEY.CELLS_PER_COLUMN, 56);
            p.apply(cn);
            tm.Init(cn);

            Assert.AreEqual(128 * 128, cn.HtmConfig.NumColumns);
}
```
* **../Presentation/** 

```
This directory contains Everything regards our presentation. (Remarks- Work in progress)
Following images show how **SpatialPooler** creates (encodes) Sparse Distributed Representation of MNIST images.

SDR code of digit '3' by using of local inhibition and various receptive field (radius)
![image.png](/.attachments/image-494af819-a46e-43ef-bf88-d39a2d8e8ca6.png)

Same example by using of global inhibition mechanism:
![image.png](/.attachments/image-6bb495b4-84a7-45dc-9199-37fc629b8e55.png)

Following example shows encoding of different representations of digit '1' by using same set of parameters shown in code snippet above.
![image.png](/.attachments/image-da7ddc5c-ff0a-493a-a0d7-54b765b0aaa1.png)

# References

HTM School:
https://www.youtube.com/playlist?list=PL3yXMgtrZmDqhsFQzwUC9V8MeeVOQ7eZ9&app=desktop

HTM Overview:
https://en.wikipedia.org/wiki/Hierarchical_temporal_memory

A Machine Learning Guide to HTM:
https://numenta.com/blog/2019/10/24/machine-learning-guide-to-htm

Numenta on Github:
https://github.com/numenta

HTM Community:
https://numenta.org/

A deep dive in HTM Temporal Memory algorithm:
https://numenta.com/assets/pdf/temporal-memory-algorithm/Temporal-Memory-Algorithm-Details.pdf

Continious Online Sequence Learning with HTM:
https://www.mitpressjournals.org/doi/full/10.1162/NECO_a_00893#.WMBBGBLytE6

# Papers and conference proceedings
International Journal of Artificial Intelligence and Applications
Scaling the HTM Spatial Pooler
https://aircconline.com/abstract/ijaia/v11n4/11420ijaia07.html

AIS 2020 - 6th International Conference on Artificial Intelligence and Soft Computing (AIS 2020), Helsinki
The Parallel HTM Spatial Pooler with Actor Model
https://aircconline.com/csit/csit1006.pdf

Symposium on Pattern Recognition and Applications - Rome, Italy
On the Relationship Between Input Sparsity and Noise Robustness in Hierarchical Temporal Memory Spatial Pooler 
https://doi.org/10.1145/3393822.3432317

International Conference on Pattern Recognition Applications and Methods - ICPRAM 2021
```

Improved HTM Spatial Pooler with Homeostatic Plasticity Control (Awarded with: *Best Industrial Paper*)
https://www.insticc.org/node/TechnicalProgram/icpram/2021/presentationDetails/103142
# Contribution
The commitment of each person on program can be tracked by following table

| Name | Commitment on master branch | Remarks |
| :---------------: | :-------------: | :---------: |
| Ali Haider        | https://github.com/alihaider4189/neocortexapi/commits/UnitCodeMaster?author=alihaidershafique |  |
| Naveed Ahmed      | https://github.com/alihaider4189/neocortexapi/commits/UnitCodeMaster?author=naveed401 |  |
| Ali Raza Kharl    | https://github.com/alihaider4189/neocortexapi/commits/UnitCodeMaster?author=alirazakharl |  |

## Acknowledgments

* NeoCortexApi
* FUAS-SE-Cloud-2021-2022 colleagues
        


