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
            var BustingResult = tm.BurstColumn(cn, column, matchingsegments,
                                 preActiveCells, preWinnerCells, 0.10, 0.10,
                                                new ThreadSafeRandom(100), true); 
            // Assert.AreEqual(, BustingResult);
            Assert.AreEqual(6, BustingResult.BestCell.ParentColumnIndex);
            Assert.AreEqual(1, BustingResult.BestCell.DistalDendrites.Count());
        }
        /// <summary>
        ///Test a Number of columns within columns dimension of 128x128
        /// </summary>
        [TestMethod]
        [TestCategory("Prod")]
        public void testNumberOfColumns()
        {
            TemporalMemory tm = new TemporalMemory();
            Connections cn = new Connections();
            Parameters p = Parameters.getAllDefaultParameters();
            p.Set(KEY.COLUMN_DIMENSIONS, new int[] { 256, 256 }); /// 256x256 column dimension
            p.Set(KEY.CELLS_PER_COLUMN, 128); ///128 cells per column
            p.apply(cn); ///Sets the fields specified by Parameters on the specified Connections object.
            tm.Init(cn); 

            Assert.AreEqual(256 * 256, cn.HtmConfig.NumColumns); ///checking expected result from actual result by calling NUM COLUMN METHOD FROM connection class
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

            DistalDendrite activeSegment = cn.CreateDistalSegment(cell5);
            //  DistalDendrite activeSegment1 = cn.CreateDistalSegment(cell6);
            cn.CreateSynapse(activeSegment, cn.GetCell(0), 0.5);
            cn.CreateSynapse(activeSegment, cn.GetCell(1), 0.5);
            cn.CreateSynapse(activeSegment, cn.GetCell(2), 0.5);
            cn.CreateSynapse(activeSegment, cn.GetCell(3), 0.5);


            ComputeCycle cc = tm.Compute(previousActiveColumns, true) as ComputeCycle;
            Assert.IsFalse(cc.ActiveCells.Count == 0);
            Assert.IsFalse(cc.WinnerCells.Count == 0);
            Assert.IsTrue(cc.PredictiveCells.Count == 0);

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

            DistalDendrite dd = cn.CreateDistalSegment(cn.GetCell(0));
            Synapse s1 = cn.CreateSynapse(dd, cn.GetCell(23), 0.5); // central 

            TemporalMemory.AdaptSegment(cn, dd, cn.GetCellSet(new int[] { 23 }), cn.HtmConfig.PermanenceIncrement, cn.HtmConfig.PermanenceDecrement);
            Assert.AreEqual(0.6, s1.Permanence, 0.1);

            // Now permanence should be at mean
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

            int[] activeColumns = { 4, 5 };
            Cell[] burstingCells = cn.GetCells(new int[] { 0, 1, 2, 3, });

            ComputeCycle cc = tm.Compute(activeColumns, true) as ComputeCycle;

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
            IList<Cell> burstingCells = cn.GetCellSet(new int[] { 0, 1, 2, 3 }); //Number of Cell Indexs

            ComputeCycle cc = tm.Compute(activeColumns, true) as ComputeCycle; //COmpute class object 

            Assert.IsFalse(cc.ActiveCells.SequenceEqual(burstingCells));
        }
        /// <summary>
        ///Test a Un-change for non selected matching in no cells have active segments, activate 4 and 5 cell in the column
        /// </summary>

        [TestMethod]
        [TestCategory("Prod")]
        public void TestNoChangeToNoTSelectedMatchingSegmentsInBurstingColumn()
        {
            TemporalMemory tm = new TemporalMemory(); // TM class object
            Connections cn = new Connections();
            Parameters p = getDefaultParameters(null, KEY.PERMANENCE_DECREMENT, 0.08); // Used Permanence decrement parameter 

            p.apply(cn);
            tm.Init(cn);

            int[] previousActiveColumns = { 0 };
            int[] activeColumns = { 1 };
            Cell[] previousActiveCells = { cn.GetCell(0), cn.GetCell(1), cn.GetCell(2), cn.GetCell(3) };
            // no cells have active segments, activate 4 and 5 cell in the column
            Cell[] burstingCells = { cn.GetCell(4), cn.GetCell(5) };

            DistalDendrite selectedMatchingSegment = cn.CreateDistalSegment(burstingCells[0]);
            cn.CreateSynapse(selectedMatchingSegment, previousActiveCells[0], 0.3);
            cn.CreateSynapse(selectedMatchingSegment, previousActiveCells[1], 0.3);
            cn.CreateSynapse(selectedMatchingSegment, previousActiveCells[2], 0.3);
            cn.CreateSynapse(selectedMatchingSegment, cn.GetCell(81), 0.3);

            DistalDendrite otherMatchingSegment = cn.CreateDistalSegment(burstingCells[1]);
            Synapse as1 = cn.CreateSynapse(otherMatchingSegment, previousActiveCells[0], 0.3);
            Synapse is1 = cn.CreateSynapse(otherMatchingSegment, cn.GetCell(81), 0.3);

            tm.Compute(previousActiveColumns, true);
            tm.Compute(activeColumns, true);

            Assert.AreEqual(0.3, as1.Permanence, 0.01);
            Assert.AreEqual(0.3, is1.Permanence, 0.01);
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
            Parameters p = getDefaultParameters(null, KEY.COLUMN_DIMENSIONS, new int[] { 3 });
            p = getDefaultParameters(p, KEY.CELLS_PER_COLUMN, 2);
            p.apply(cn);
            tm.Init(cn);

            DistalDendrite dd = cn.CreateDistalSegment(cn.GetCell(1));
            cn.CreateSynapse(dd, cn.GetCell(0), 0.30);

            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual(4, TemporalMemory.GetLeastUsedCell(cn, cn.GetColumn(2).Cells, cn.HtmConfig.Random).Index);
            }
            
        }

    }
}
