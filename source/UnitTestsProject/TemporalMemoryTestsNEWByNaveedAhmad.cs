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
    public class TemporalPoolerTestNEWByNaveedAhmad
    {

        /// <summary>
        /// Return Boolean value of generic collections Array 
        /// </summary>
        private static bool areDisjoined<T>(ICollection<T> arr1, ICollection<T> arr2)
        {
            foreach (var item in arr1)
            {
                if (arr2.Contains(item))
                    return false;
            }

            return true;
        }

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
                CellsPerColumn = 4,
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
        /// Test adapt segment from synapse with different Permanence.
        /// </summary>
        [TestMethod]
        [TestCategory("Prod")]
        public void TestAdaptSegment1()
        {
            TemporalMemory tm = new TemporalMemory();
            Connections cn = new Connections();
            Parameters p = Parameters.getAllDefaultParameters();
            p.apply(cn);
            tm.Init(cn); // use connection for specified object to build to implement algoarithm.

            DistalDendrite dd = cn.CreateDistalSegment(cn.GetCell(0));/// Created a Distal dendrite segment of a cell (0).
            Synapse s1 = cn.CreateSynapse(dd, cn.GetCell(23), 0.5);/// Creating Synapse for Cell(23) by Calling Connection Mathod.
            Synapse s2 = cn.CreateSynapse(dd, cn.GetCell(37), 0.6);/// Creating Synapse for Cell(37) by Calling Connection Mathod.
            Synapse s3 = cn.CreateSynapse(dd, cn.GetCell(477), 0.8);/// Creating Synapse for Cell(477) by Calling Connection Mathod.

            TemporalMemory.AdaptSegment(cn, dd, cn.GetCellSet(new int[] { 23, 37 }), cn.HtmConfig.PermanenceIncrement, cn.HtmConfig.PermanenceDecrement);
            Assert.AreNotEqual(0.7, s1.Permanence, 0.01);//Asserting the values for Segment1.
            Assert.AreNotEqual(0.5, s2.Permanence, 0.01);//Asserting the values for Segment2.
            Assert.AreNotEqual(0.8, s3.Permanence, 0.01);//Asserting the values for Segment3.
        }




        

        /// <summary>
        ///Test an Array which has numerous active cells in it.
        /// </summary>
        [TestMethod]
        public void TestArrayContainingMultipleCells()
        {

            HtmConfig htmConfig = GetDefaultTMParameters();
            Connections cn = new Connections(htmConfig);

            TemporalMemory tm = new TemporalMemory();

            tm.Init(cn);// use connection for specified object to build to implement algoarithm.

            int[] activeColumns = { 2, 3, 4 };//Mutlipe Active Columns inside an array.
            Cell[] burstingCells = cn.GetCells(new int[] { 0, 1, 2, 3, 4, 5 });//Test an array containing multiple cells.

            ComputeCycle cc = tm.Compute(activeColumns, true) as ComputeCycle;//Computing Active Columns inside the Array.

            Assert.IsFalse(cc.ActiveCells.SequenceEqual(burstingCells));
        }
        /// <summary>
        ///Test a active column where most cell used. 
        /// </summary>
        [TestMethod]
        [TestCategory("Prod")]
        public void TestMostUsedCell()
        {
            TemporalMemory tm = new TemporalMemory();
            Connections cn = new Connections();
            Parameters p = getDefaultParameters(null, KEY.COLUMN_DIMENSIONS, new int[] { 2 });// One active column which is mostly used (2).
            p = getDefaultParameters(p, KEY.CELLS_PER_COLUMN, 2);//Getting default Parameters for most used cell.
            p.apply(cn);
            tm.Init(cn);//use connection for specified object to build to implement the algoarithm.

            DistalDendrite dd = cn.CreateDistalSegment(cn.GetCell(0));//Creating a DistalDendrite for the cell.
            cn.CreateSynapse(dd, cn.GetCell(2), 0.2);//Created a synapse on a distal segment of a cell index 2

            for (int i = 0; i < 100; i++)//Applaying for loop on it.
            {
                Assert.AreNotEqual(0, TemporalMemory.GetLeastUsedCell(cn, cn.GetColumn(0).Cells, cn.HtmConfig.Random).Index);
            }
        }




        /// <summary>
        /// test a funtion to unchange matching segment in predicted 0 active columns.
        /// </summary>
        [TestMethod]
        [TestCategory("Prod")]
        public void TestNoChangeToMatchingSegmentsInPredictedActiveColumn()
        {
            TemporalMemory tm = new TemporalMemory();
            Connections cn = new Connections();
            Parameters p = getDefaultParameters();
            p.apply(cn);
            tm.Init(cn);//use connection for specified object to build to implement the algoarithm.

            int[] previousActiveColumns = { 0 };//1 PreActiveCell Column.
            int[] activeColumns = { 1 };//1 Active Column.
            Cell[] previousActiveCells = { cn.GetCell(0), cn.GetCell(1), cn.GetCell(2), cn.GetCell(3) };//4 PreviousActiveCells.
            Cell expectedActiveCell = cn.GetCell(4);//Obtained cell 4 by calling connection method.
            List<Cell> expectedActiveCells = new List<Cell>(new Cell[] { expectedActiveCell });
            Cell otherBurstingCell = cn.GetCell(5);

            DistalDendrite activeSegment = cn.CreateDistalSegment(expectedActiveCell);//Created a Distal dendrite segment of a cell.
            cn.CreateSynapse(activeSegment, previousActiveCells[0], 0.5);//Created a synapse on a distal segment of a cell index 0.
            cn.CreateSynapse(activeSegment, previousActiveCells[1], 0.5);//Created a synapse on a distal segment of a cell index 0.
            cn.CreateSynapse(activeSegment, previousActiveCells[2], 0.5);//Created a synapse on a distal segment of a cell index 0.
            cn.CreateSynapse(activeSegment, previousActiveCells[3], 0.5);//Created a synapse on a distal segment of a cell index 0.

            DistalDendrite matchingSegmentOnSameCell = cn.CreateDistalSegment(expectedActiveCell);
            Synapse s1 = cn.CreateSynapse(matchingSegmentOnSameCell, previousActiveCells[0], 0.3);//Matching Segment from previously Active Column on same cell(0).
            Synapse s2 = cn.CreateSynapse(matchingSegmentOnSameCell, previousActiveCells[1], 0.3);//Matching Segment from previously Active Column on same cell(1).

            DistalDendrite matchingSegmentOnOtherCell = cn.CreateDistalSegment(otherBurstingCell);
            Synapse s3 = cn.CreateSynapse(matchingSegmentOnOtherCell, previousActiveCells[0], 0.3); //Matching Segment from previously Active Column on other cell(0).
            Synapse s4 = cn.CreateSynapse(matchingSegmentOnOtherCell, previousActiveCells[1], 0.3); //Matching Segment from previously Active Column on same cell(1.

            ComputeCycle cc = tm.Compute(previousActiveColumns, true) as ComputeCycle;//Learn=True
            Assert.IsTrue(cc.PredictiveCells.SequenceEqual(expectedActiveCells));//Predicting Expected Active Cells.
            tm.Compute(activeColumns, true);//Learn=True, Computing Active columns. 

            Assert.AreEqual(0.3, s1.Permanence, 0.01);///Asserting the value for Segment1.
            Assert.AreEqual(0.3, s2.Permanence, 0.01);///Asserting the value for Segment2.
            Assert.AreEqual(0.3, s3.Permanence, 0.01);///Asserting the value for Segment3.
            Assert.AreEqual(0.3, s4.Permanence, 0.01);///Asserting the value for Segment4.
        }



        /// <summary>
        ///Test a  LinkedHashSet{T}  containing the Cell specified by the passed in indexes.
        /// </summary>
        [TestMethod]
        public void TestBurstpredictedColumns()
        {
            TemporalMemory tm = new TemporalMemory();
            Connections cn = new Connections();
            Parameters p = getDefaultParameters();
            p.apply(cn);
            tm.Init(cn);//use connection for specified object to build to implement the algoarithm.

            int[] activeColumns = { 1 }; //Currently Active column.
            IList<Cell> burstingCells = cn.GetCellSet(new int[] { 0, 1, 2, 3 }); //Number of Cell Indexs.

            ComputeCycle cc = tm.Compute(activeColumns, false) as ComputeCycle; //Computing class object.

            Assert.IsFalse(cc.ActiveCells.SequenceEqual(burstingCells));
        }
        




    }

}

