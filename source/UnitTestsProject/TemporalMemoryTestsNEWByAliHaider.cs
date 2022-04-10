// Copyright (c) Damir Dobric. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoCortexApi;
using NeoCortexApi.Entities;
using NeoCortexApi.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTestsProject

{
    /// <summary>
    /// Unit Tests for Temporal Memory Class.
    /// </summary>

    [TestClass]
    public class TemporalMemoryTestNEWByAliHaider
    {

       
        /// <summary>
        /// Implementation of Parameters Class
        /// </summary>
        private Parameters getDefaultParameters()
        {
            Parameters retVal = Parameters.getTemporalDefaultParameters();
            retVal.Set(KEY.COLUMN_DIMENSIONS, new int[] { 32 }); //
            retVal.Set(KEY.CELLS_PER_COLUMN, 4);
            retVal.Set(KEY.ACTIVATION_THRESHOLD, 3); //e number of active connected synapses in a  segment is ≥ 3
            retVal.Set(KEY.INITIAL_PERMANENCE, 0.21); //value for a synapse
            retVal.Set(KEY.CONNECTED_PERMANENCE, 0.5); // the permanence value for a synapse is ≥ 0.5, it is “connected”. 
            retVal.Set(KEY.MIN_THRESHOLD, 2); //Mini threshold for a segment
            retVal.Set(KEY.MAX_NEW_SYNAPSE_COUNT, 3); 
            retVal.Set(KEY.PERMANENCE_INCREMENT, 0.10); // the permanence values of its active  synapses are incremented by 0.10
            retVal.Set(KEY.PERMANENCE_DECREMENT, 0.10); // after predicted cell the synaps will inactive after .10 decrement
            retVal.Set(KEY.PREDICTED_SEGMENT_DECREMENT, 0.0);
            retVal.Set(KEY.RANDOM, new ThreadSafeRandom(42));
            retVal.Set(KEY.SEED, 42);

            return retVal;
        }

        /// <summary>
        /// Implementation of HtmConfig Class
        /// </summary>
        private HtmConfig GetDefaultTMParameters()
        {
            HtmConfig htmConfig = new HtmConfig(new int[] { 32 }, new int[] { 32 })
            {
                CellsPerColumn =  4,
                ActivationThreshold = 3,
                InitialPermanence = 0.21,
                ConnectedPermanence = 0.5,
                MinThreshold = 2,
                MaxNewSynapseCount = 3,
                PermanenceIncrement = 0.1,
                PermanenceDecrement = 0.1,
                PredictedSegmentDecrement = 0,
                Random = new ThreadSafeRandom(42),
                RandomGenSeed = 42
            };

            return htmConfig;
        }

        /// <summary>
        /// Factory method. Return global <see cref="Parameters"/> object with default values
        /// </summary>
        /// <returns><see cref="retVal"/></returns>
        private Parameters getDefaultParameters(Parameters p, string key, Object value)
        {
            Parameters retVal = p == null ? getDefaultParameters() : p;
            retVal.Set(key, value);

            return retVal;
        }
        /// <summary>
        /// Activates all of the cells in an unpredicted active column
        /// </summary>
        [TestMethod]
        [TestCategory("Prod")]
        public void TestActivatedunpredictedActiveColumn()
        {
            HtmConfig htmConfig = GetDefaultTMParameters(); 
            Connections cn = new Connections(htmConfig);
            TemporalMemory tm = new TemporalMemory();
            tm.Init(cn); ///use connection for specified object to build to implement algoarithm 
            Random random = cn.HtmConfig.Random;
            int[] prevActiveColumns = { 1, 2, 3, 4 }; /// 
            Column column = cn.GetColumn(6); /// Retrieve column 6 
            IList<Cell> preActiveCells = cn.GetCellSet(new int[] { 0, 1, 2, 3 }); /// 4 pre-active cells
            IList<Cell> preWinnerCells = cn.GetCellSet(new int[] { 0, 1 }); ///Pre- winners cells from pre avtive once
            List<DistalDendrite> matchingsegments = new List<DistalDendrite>(cn.GetCell(3).DistalDendrites); ///Matching segment from Distal dentrite list
            //We have passed Random value into Burst column function of Temporal memory algorithm
            var BustingResult = tm.BurstColumn(cn, column, matchingsegments,
                                 preActiveCells, preWinnerCells, 0.10, 0.10,
                                                new ThreadSafeRandom(100), true); 
            // Assert.AreEqual(, BustingResult);
            Assert.AreEqual(6, BustingResult.BestCell.ParentColumnIndex);
            Assert.AreEqual(1, BustingResult.BestCell.DistalDendrites.Count());
        }
       
        /// <summary>
        ///Test a active cell, winner cell and predictive cell in two active columns
        /// </summary>

        [TestMethod]
        [TestCategory("Prod")]
        public void TestWithTwoActiveColumns()
        {
            TemporalMemory tm = new TemporalMemory();
            Connections cn = new Connections();
            Parameters p = getDefaultParameters();
            p.apply(cn);
            tm.Init(cn);

            int[] previousActiveColumns = { 2, 3 }; ///2 pre active cells
            Cell cell5 = cn.GetCell(6); /// get cell 6 by calling connection method
            Cell cell6 = cn.GetCell(7);

            DistalDendrite activeSegment = cn.CreateDistalSegment(cell5); /// Created a Distal dendrite segment of a cell5
            //  DistalDendrite activeSegment1 = cn.CreateDistalSegment(cell6);
            cn.CreateSynapse(activeSegment, cn.GetCell(0), 0.5); /// Created a synapse on a distal segment of a cell index 0
            cn.CreateSynapse(activeSegment, cn.GetCell(1), 0.5); /// Created a synapse on a distal segment of a cell index 1
            cn.CreateSynapse(activeSegment, cn.GetCell(2), 0.5); /// Created a synapse on a distal segment of a cell index 2
            cn.CreateSynapse(activeSegment, cn.GetCell(3), 0.5); /// Created a synapse on a distal segment of a cell index 3


            ComputeCycle cc = tm.Compute(previousActiveColumns, true) as ComputeCycle;
            Assert.IsFalse(cc.ActiveCells.Count == 0); ///  count a active cell from preciously active column
            Assert.IsFalse(cc.WinnerCells.Count == 0); ///   count a winner cell from preciously active column
            Assert.IsTrue(cc.PredictiveCells.Count == 0); ///   count a predictive cell from preciously active column

            int[] zeroColumns = new int[0];
            ComputeCycle cc2 = tm.Compute(zeroColumns, true) as ComputeCycle; ///learn = true
            Assert.IsTrue(cc2.ActiveCells.Count == 0); /// Active cell ==0
            Assert.IsTrue(cc2.WinnerCells.Count == 0);  /// wineer cell equal to 0
            Assert.IsTrue(cc2.PredictiveCells.Count == 0); ///lost of depolirized cells equal to 0
        }
        ///<summary>
        /// Test adapt segment from syapse to centre 
        /// <Summary>
        [TestMethod]
        [TestCategory("Prod")]
        public void TestAdaptSegmentToCentre()
        {
            TemporalMemory tm = new TemporalMemory();
            Connections cn = new Connections();
            Parameters p = Parameters.getAllDefaultParameters();
            p.apply(cn);
            tm.Init(cn);

            DistalDendrite dd = cn.CreateDistalSegment(cn.GetCell(0)); /// Create a distal segment of a cell index 0 to learn sequence
            Synapse s1 = cn.CreateSynapse(dd, cn.GetCell(23), 0.5); /// create a synapse on a dital segment to get cell index 23 

          /// get a permanence 0.6 of the segment's synapse if the synapse's presynaptic cell index 23 was active. 
          /// If it was not active, then it will decrement the permanence to 0.1
            TemporalMemory.AdaptSegment(cn, dd, cn.GetCellSet(new int[] { 23 }), cn.HtmConfig.PermanenceIncrement, cn.HtmConfig.PermanenceDecrement);
            Assert.AreEqual(0.6, s1.Permanence, 0.1);

         /// get a permanence 0.7 of the segment's synapse if the synapse's presynaptic cell index 23 was active. 
         /// If it was not active, then it will decrement the permanence to 0.1
            TemporalMemory.AdaptSegment(cn, dd, cn.GetCellSet(new int[] { 23 }), cn.HtmConfig.PermanenceIncrement, cn.HtmConfig.PermanenceDecrement);
            Assert.AreEqual(0.7, s1.Permanence, 0.1);
        }

        /// <summary>
        ///Test an Array which has none cells in it
        /// </summary>
        [TestMethod]
        public void TestArrayNotContainingCells()
        {

            HtmConfig htmConfig = GetDefaultTMParameters();
            Connections cn = new Connections(htmConfig);

            TemporalMemory tm = new TemporalMemory();

            tm.Init(cn);

            int[] activeColumns = { 4, 5 }; ///two active columns 4 and 5
            Cell[] burstingCells = cn.GetCells(new int[] { 0, 1, 2, 3, }); ///get bursting cell of a index 0,1,2,3

            ComputeCycle cc = tm.Compute(activeColumns, true) as ComputeCycle;
          ///get active cells from bursting cells and its should be null
            Assert.IsFalse(cc.ActiveCells.SequenceEqual(burstingCells));
        }

        /// <summary>
        ///Test a  if no cells have active segments, activate all the cells which cant be predicted in columns
        /// </summary>
        [TestMethod]
        [TestCategory("Prod")]
        public void TestBurstNotpredictedColumns()
        {
            TemporalMemory tm = new TemporalMemory();
            Connections cn = new Connections();
            Parameters p = getDefaultParameters();
            p.apply(cn);
            tm.Init(cn);

            int[] activeColumns = { 1, 2 }; //Cureently Active column
            IList<Cell> burstingCells = cn.GetCellSet(new int[] { 0, 1, 2, 3 }); //Number of Cell Index

            ComputeCycle cc = tm.Compute(activeColumns, true) as ComputeCycle; //COmpute class object 

            Assert.IsFalse(cc.ActiveCells.SequenceEqual(burstingCells));
        }
      
        /// <summary>
        ///Test a active column Where most used cell in a column and after every test its alter the cell
        /// </summary>
        [TestMethod]
        [TestCategory("Prod")]
        public void TestRandomMostUsedCell()
        {
            TemporalMemory tm = new TemporalMemory();
            Connections cn = new Connections();
            Parameters p = getDefaultParameters(null, KEY.COLUMN_DIMENSIONS, new int[] { 2 }); /// column dimension set to 3
            p = getDefaultParameters(p, KEY.CELLS_PER_COLUMN, 2); /// 2 cell per column
            p.apply(cn);
            tm.Init(cn);

            DistalDendrite dd = cn.CreateDistalSegment(cn.GetCell(1)); /// Created a Distal dendrite segment of a cell index 1 
            cn.CreateSynapse(dd, cn.GetCell(0), 0.30); /// Create a synapse on a distal segment of a cell index 0

             Assert.AreEqual(3, TemporalMemory.GetLeastUsedCell(cn, cn.GetColumn(1).Cells, cn.HtmConfig.Random).Index);
            
            
        }

    }
}
