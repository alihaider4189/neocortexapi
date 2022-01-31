Learning OutCome: 
        1. The Main Idea is to understand the basic of Artificial General Intelligence we need to decrypt the intelligence of NeoCortex
        Reverse Engineering of Neocortex consists of the following steps
            1.Sparse Distributed Representation
            2.Encoding
            3.Spartial Pooler
            4.Temporal Memory

        2. HTM starts with the assumption that everything the neocortex does is based on memory and recall of sequences of patterns
                 —Hierarchical temporal memory (HTM) is a machine
                 learning algorithm inspired by the information processing mechanisms of the human neocortex and consists of a spatial pooler
                    (SP) and temporal memory (TM).

         The HTM network is a level hierarchy in the shape of a tree. These levels are made up of smaller components known as regions (or nodes).
		 Several regions may be contained within a single level of the hierarchy. There are usually fewer areas at higher levels of the system.
		 Higher levels of the hierarchy can combine patterns learnt at previous levels to memorize more complicated patterns.



           Spatial Pooling:

                The spatial pooler takes the input data and converts it into active columns. 
	             Assume that for a given input space, a spatial pooling attempts to learn the sequences, 
                 and that in order to learn the sequence, each micro column is linked to a particular number of synapses from the input. 

				 Spatial pooler is a fundamental component in Hierarchical Temporal Memory, mainly responsible for processing feedforward
				 sensory inputs into sparse distributed representations, and feature extraction in Hierarchical Temporal Memory (HTM) also.

                   
            Temporal Pooling:
		        We can grasp the sequential pattern throughout time thanks to Temporal Pooling. 
                It learns the current column's sequences from the Spatial Pooler and guesses what spatial pattern will appear next depending on the temporal context of each input.
		
				The Temporal Memory algorithm learns sequences and makes predictions. In the Temporal Memory algorithm, when a cell
                becomes active, it forms connections to other cells that were active just prior. Cells can then predict when they will become active
                by looking at their connections. If all the cells do this, collectively they can store and recall sequences, and they can predict what
                is likely to happen next.


            
		    SpatialPooler.md - NeoCortexAPI Documentation from project
				Learning OutCome:
					Currently the project supports three versions of SP are implemented and considered:
					Spatial Pooler - single threaded original version without algorithm specific changes.
					SP-MT multithreaded version - which supports multiple cores on a single machine and
					SP-Parallel - which supports multicore and multimode calculus of spatial pooler.
					Steps to execute Spatial Pooler algorithm:
						1. Initialize Required Parameters using HtmConfig.
					2. Call Sp Compute Method to execute.
					The parameters used in HtmConfig are given Below
						POTENTIAL_RADIUS:	Defines the radius in number of input cells visible to column cells. It is important to choose this value, so every input neuron is connected to at least a single column. For example, if the input has 50000 bits and the column topology is 500, then you must choose some value larger than 50000/500 > 100.
					POTENTIAL_PCT:	Defines the percent of inputs withing potential radius, which can/should be connected to the column.
					GLOBAL_INHIBITION:	If TRUE global inhibition algorithm will be used. If FALSE local inhibition algorithm will be used.
					INHIBITION_RADIUS:	Defines neighbourhood radius of a column.
					LOCAL_AREA_DENSITY:	Density of active columns inside of local inhibition radius. If set on value < 0, explicit number of active columns (NUM_ACTIVE_COLUMNS_PER_INH_AREA) will be used.
					NUM_ACTIVE_COLUMNS_PER_INH_AREA:	An alternate way to control the density of the active columns. If this value is specified then LOCAL_AREA_DENSITY must be less than 0, and vice versa.
					STIMULUS_THRESHOLD:	One mini-column is active if its overlap exceeds overlap threshold  of connected synapses.
					SYN_PERM_INACTIVE_DEC:	Decrement step of synapse permanence value withing every inactive cycle. It defines how fast the NeoCortex will forget learned patterns.
					SYN_PERM_ACTIVE_INC:	Increment step of connected synapse during learning process
					SYN_PERM_CONNECTED:	Defines Connected Permanence Threshold  , which is a float value, which must be exceeded to declare synapse as connected.
					DUTY_CYCLE_PERIOD:	Number of iterations. The period used to calculate duty cycles. Higher values make it take longer to respond to changes in boost. Shorter values make it more unstable and likely to oscillate.
					MAX_BOOST:	Maximum boost factor of a column.