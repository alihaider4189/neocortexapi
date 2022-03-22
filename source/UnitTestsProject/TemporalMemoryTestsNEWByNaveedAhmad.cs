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
        /// Test adapt segment from syapse with different Permanence
        /// </summary>
        [TestMethod]
        [TestCategory("Prod")]
        public void TestAdaptSegment1()
        {
            TemporalMemory tm = new TemporalMemory();
            Connections cn = new Connections();
            Parameters p = Parameters.getAllDefaultParameters();
            p.apply(cn);
            tm.Init(cn);

            DistalDendrite dd = cn.CreateDistalSegment(cn.GetCell(0));
            Synapse s1 = cn.CreateSynapse(dd, cn.GetCell(23), 0.5);
            Synapse s2 = cn.CreateSynapse(dd, cn.GetCell(37), 0.6);
            Synapse s3 = cn.CreateSynapse(dd, cn.GetCell(477), 0.8);

            TemporalMemory.AdaptSegment(cn, dd, cn.GetCellSet(new int[] { 23, 37 }), cn.HtmConfig.PermanenceIncrement, cn.HtmConfig.PermanenceDecrement);

            Assert.AreNotEqual(0.7, s1.Permanence, 0.01);
            Assert.AreNotEqual(0.5, s2.Permanence, 0.01);
            Assert.AreNotEqual(0.8, s3.Permanence, 0.01);
        }




        /// <summary>
        ///Test a active cell, winner cell and predictive cell in 0 active columns
        /// </summary>

        /// <summary>
        ///Test an Array which has numerous active cells in it
        /// </summary>
        [TestMethod]
        public void TestArrayContainingMultipleCells()
        {

            HtmConfig htmConfig = GetDefaultTMParameters();
            Connections cn = new Connections(htmConfig);

            TemporalMemory tm = new TemporalMemory();

            tm.Init(cn);

            int[] activeColumns = { 2, 3, 4 };
            Cell[] burstingCells = cn.GetCells(new int[] { 0, 1, 2, 3, 4, 5 });

            ComputeCycle cc = tm.Compute(activeColumns, true) as ComputeCycle;

            Assert.IsFalse(cc.ActiveCells.SequenceEqual(burstingCells));
        }
        /// <summary>
        ///Test a active column where most cell used 
        /// </summary>
        [TestMethod]
        [TestCategory("Prod")]
        public void TestMostUsedCell()
        {
            TemporalMemory tm = new TemporalMemory();
            Connections cn = new Connections();
            Parameters p = getDefaultParameters(null, KEY.COLUMN_DIMENSIONS, new int[] { 2 });
            p = getDefaultParameters(p, KEY.CELLS_PER_COLUMN, 2);
            p.apply(cn);
            tm.Init(cn);

            DistalDendrite dd = cn.CreateDistalSegment(cn.GetCell(0));
            cn.CreateSynapse(dd, cn.GetCell(3), 0.3);

            for (int i = 0; i < 100; i++)
            {
                Assert.AreNotEqual(0, TemporalMemory.GetLeastUsedCell(cn, cn.GetColumn(0).Cells, cn.HtmConfig.Random).Index);
            }
        }




        /// <summary>
        /// test a funtion to unchange matching segment in predicted 0 active columns
        /// </summary>
        [TestMethod]
        [TestCategory("Prod")]
        public void TestNoChangeToMatchingSegmentsInPredictedActiveColumn()
        {
            TemporalMemory tm = new TemporalMemory();
            Connections cn = new Connections();
            Parameters p = getDefaultParameters();
            p.apply(cn);
            tm.Init(cn);

            int[] previousActiveColumns = { 0 };
            int[] activeColumns = { 1 };
            Cell[] previousActiveCells = { cn.GetCell(0), cn.GetCell(1), cn.GetCell(2), cn.GetCell(3) };
            Cell expectedActiveCell = cn.GetCell(4);
            List<Cell> expectedActiveCells = new List<Cell>(new Cell[] { expectedActiveCell });
            Cell otherBurstingCell = cn.GetCell(5);

            DistalDendrite activeSegment = cn.CreateDistalSegment(expectedActiveCell);
            cn.CreateSynapse(activeSegment, previousActiveCells[0], 0.5);
            cn.CreateSynapse(activeSegment, previousActiveCells[1], 0.5);
            cn.CreateSynapse(activeSegment, previousActiveCells[2], 0.5);
            cn.CreateSynapse(activeSegment, previousActiveCells[3], 0.5);

            DistalDendrite matchingSegmentOnSameCell = cn.CreateDistalSegment(expectedActiveCell);
            Synapse s1 = cn.CreateSynapse(matchingSegmentOnSameCell, previousActiveCells[0], 0.3);
            Synapse s2 = cn.CreateSynapse(matchingSegmentOnSameCell, previousActiveCells[1], 0.3);

            DistalDendrite matchingSegmentOnOtherCell = cn.CreateDistalSegment(otherBurstingCell);
            Synapse s3 = cn.CreateSynapse(matchingSegmentOnOtherCell, previousActiveCells[0], 0.3);
            Synapse s4 = cn.CreateSynapse(matchingSegmentOnOtherCell, previousActiveCells[1], 0.3);

            ComputeCycle cc = tm.Compute(previousActiveColumns, true) as ComputeCycle;
            Assert.IsTrue(cc.PredictiveCells.SequenceEqual(expectedActiveCells));
            tm.Compute(activeColumns, true);

            Assert.AreEqual(0.3, s1.Permanence, 0.01);
            Assert.AreEqual(0.3, s2.Permanence, 0.01);
            Assert.AreEqual(0.3, s3.Permanence, 0.01);
            Assert.AreEqual(0.3, s4.Permanence, 0.01);
        }



        /// <summary>
        ///Test a  LinkedHashSet{T}  containing the Cell specified by the passed in indexes
        /// </summary>
        [TestMethod]
        public void TestBurstpredictedColumns()
        {
            TemporalMemory tm = new TemporalMemory();
            Connections cn = new Connections();
            Parameters p = getDefaultParameters();
            p.apply(cn);
            tm.Init(cn);

            int[] activeColumns = { 1 }; //Cureently Active column
            IList<Cell> burstingCells = cn.GetCellSet(new int[] { 0, 1, 2, 3 }); //Number of Cell Indexs

            ComputeCycle cc = tm.Compute(activeColumns, false) as ComputeCycle; //COmpute class object 

            Assert.IsFalse(cc.ActiveCells.SequenceEqual(burstingCells));
        }
        




    }

}

