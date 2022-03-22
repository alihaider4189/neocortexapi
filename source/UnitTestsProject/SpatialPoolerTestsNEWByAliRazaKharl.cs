// Copyright (c) Damir Dobric. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoCortex;
using NeoCortexApi;
using NeoCortexApi.Entities;
using NeoCortexApi.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace  UnitTestsProject
{
    /// <summary>
    /// Test Spatial Pooler Class
    /// </summary>
    [TestClass]
     public class SpatialPoolerTestNEWByAliRazaKharl
     {
        private Parameters parameters; //instance of parameter class
        private SpatialPooler sp; //private instance of spatialpooler classs
        private Connections mem; //private instance of connection class
        /// <summary>
        /// SP algoarithmic parameters 
        /// </summary>
        public void setupParameters()
        {
            parameters = Parameters.getAllDefaultParameters(); ///parameter class object
            parameters.Set(KEY.INPUT_DIMENSIONS, new int[] { 5 }); 
            parameters.Set(KEY.COLUMN_DIMENSIONS, new int[] { 5 }); 
            parameters.Set(KEY.POTENTIAL_RADIUS, 5); /// potentialRadius must be set to the inputWidth
            parameters.Set(KEY.POTENTIAL_PCT, 0.5); ///nput bits are connected when the Spatial Pooling algorithm is initialized
            parameters.Set(KEY.GLOBAL_INHIBITION, false); /// winning columns are selected with respect to their local neighborhoods
            parameters.Set(KEY.LOCAL_AREA_DENSITY, -1.0);
            parameters.Set(KEY.NUM_ACTIVE_COLUMNS_PER_INH_AREA, 3.0);
            parameters.Set(KEY.STIMULUS_THRESHOLD, 0.0);
            parameters.Set(KEY.SYN_PERM_INACTIVE_DEC, 0.01);
            parameters.Set(KEY.SYN_PERM_ACTIVE_INC, 0.1);
            parameters.Set(KEY.SYN_PERM_CONNECTED, 0.1);
            parameters.Set(KEY.MIN_PCT_OVERLAP_DUTY_CYCLES, 0.1);
            parameters.Set(KEY.MIN_PCT_ACTIVE_DUTY_CYCLES, 0.1);
            parameters.Set(KEY.DUTY_CYCLE_PERIOD, 10);
            parameters.Set(KEY.MAX_BOOST, 10.0);
            parameters.Set(KEY.RANDOM, new ThreadSafeRandom(42));
        }

        /// <summary>
        /// Implement a HTMconfig class
        /// </summary>
        /// <returns></returns>
        private HtmConfig SetupHtmConfigParameters()
        {
            var htmConfig = new HtmConfig(new int[] { 5 }, new int[] { 5 })
            {
                PotentialRadius = 5,
                PotentialPct = 0.5,
                GlobalInhibition = false,
                LocalAreaDensity = -1.0,
                NumActiveColumnsPerInhArea = 3.0,
                StimulusThreshold = 0.0,
                SynPermInactiveDec = 0.01,
                SynPermActiveInc = 0.1,
                SynPermConnected = 0.1,
                MinPctOverlapDutyCycles = 0.1,
                MinPctActiveDutyCycles = 0.1,
                DutyCyclePeriod = 10,
                MaxBoost = 10,
                RandomGenSeed = 42,
                Random = new ThreadSafeRandom(42),
            };

            return htmConfig;
        }
        /// <summary>
        /// assign dummy elements to parameters used in SP
        /// </summary>
        public void setupDefaultParameters()
        {
            parameters = Parameters.getAllDefaultParameters();
            parameters.Set(KEY.INPUT_DIMENSIONS, new int[] { 32, 32 });
            parameters.Set(KEY.COLUMN_DIMENSIONS, new int[] { 64, 64 });
            parameters.Set(KEY.POTENTIAL_RADIUS, 16);
            parameters.Set(KEY.POTENTIAL_PCT, 0.5);
            parameters.Set(KEY.GLOBAL_INHIBITION, false);
            parameters.Set(KEY.LOCAL_AREA_DENSITY, -1.0);
            parameters.Set(KEY.NUM_ACTIVE_COLUMNS_PER_INH_AREA, 10.0);
            parameters.Set(KEY.STIMULUS_THRESHOLD, 0.0);
            parameters.Set(KEY.SYN_PERM_INACTIVE_DEC, 0.008);
            parameters.Set(KEY.SYN_PERM_ACTIVE_INC, 0.05);
            parameters.Set(KEY.SYN_PERM_CONNECTED, 0.10);
            parameters.Set(KEY.MIN_PCT_OVERLAP_DUTY_CYCLES, 0.001);
            parameters.Set(KEY.MIN_PCT_ACTIVE_DUTY_CYCLES, 0.001);
            parameters.Set(KEY.DUTY_CYCLE_PERIOD, 1000);
            parameters.Set(KEY.MAX_BOOST, 10.0);
            parameters.Set(KEY.SEED, 42);
            parameters.Set(KEY.RANDOM, new ThreadSafeRandom(42));
        }
        /// <summary>
        /// Assign values to SP parameters
        /// </summary>
        /// <returns></returns>
        private HtmConfig SetupHtmConfigDefaultParameters()
        {
            var htmConfig = new HtmConfig(new int[] { 32, 32 }, new int[] { 64, 64 })
            {
                PotentialRadius = 16,
                PotentialPct = 0.5,
                GlobalInhibition = false,
                LocalAreaDensity = -1.0,
                NumActiveColumnsPerInhArea = 10.0,
                StimulusThreshold = 0.0,
                SynPermInactiveDec = 0.008,
                SynPermActiveInc = 0.05,
                SynPermConnected = 0.10,
                MinPctOverlapDutyCycles = 0.001,
                MinPctActiveDutyCycles = 0.001,
                DutyCyclePeriod = 1000,
                MaxBoost = 10.0,
                RandomGenSeed = 42,
                Random = new ThreadSafeRandom(42)
            };

            return htmConfig;
        }
        /// <summary>
        /// Init Spatial pooler class method
        /// </summary>
        private void InitTestSPInstance()
        {
            sp = new SpatialPoolerMT();
            mem = new Connections();
            parameters.apply(mem);
            sp.Init(mem);
        }
        /// <summary>
        /// verify the construction of SP parameters values
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Prod")]
        [TestCategory("Config")]
        public void confirmSPConstruction2()
        {
            HtmConfig Config = SetupHtmConfigParameters();
            mem = new Connections(Config);

            sp = new SpatialPoolerMT();
            sp.Init(mem);

            Assert.AreEqual(5, mem.HtmConfig.InputDimensions[0]); //
            Assert.AreEqual(5, mem.HtmConfig.ColumnDimensions[0]);
            Assert.AreEqual(5, mem.HtmConfig.PotentialRadius);
            Assert.AreEqual(0.5, mem.HtmConfig.PotentialPct);//, 0);
            Assert.AreEqual(false, mem.HtmConfig.GlobalInhibition);
            Assert.AreEqual(-1.0, mem.HtmConfig.LocalAreaDensity);//, 0);
            Assert.AreEqual(3, mem.HtmConfig.NumActiveColumnsPerInhArea);//, 0);
            Assert.IsTrue(Math.Abs(1 - mem.HtmConfig.StimulusThreshold) <= 1);
            Assert.AreEqual(0.01, mem.HtmConfig.SynPermInactiveDec);//, 0);
            Assert.AreEqual(0.1, mem.HtmConfig.SynPermActiveInc);//, 0);
            Assert.AreEqual(0.1, mem.HtmConfig.SynPermConnected);//, 0);
            Assert.AreEqual(0.1, mem.HtmConfig.MinPctOverlapDutyCycles);//, 0);
            Assert.AreEqual(0.1, mem.HtmConfig.MinPctActiveDutyCycles);//, 0);
            Assert.AreEqual(10, mem.HtmConfig.DutyCyclePeriod);//, 0);
            Assert.AreEqual(10.0, mem.HtmConfig.MaxBoost);//, 0);
            Assert.AreEqual(42, mem.HtmConfig.RandomGenSeed);
            Assert.AreEqual(5, mem.HtmConfig.NumInputs);
            Assert.AreEqual(5, mem.HtmConfig.NumColumns);
        }

        /// <summary>
        /// Test Compute method of SP
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Prod")]
        public void testCompute2_1()
        {
            setupParameters();
            parameters.setInputDimensions(new int[] { 10 });
            parameters.setColumnDimensions(new int[] { 10 });
            parameters.setPotentialRadius(3);
            parameters.setPotentialPct(0.3);
            parameters.setGlobalInhibition(false);
            parameters.setLocalAreaDensity(-1.0);
            parameters.setNumActiveColumnsPerInhArea(3);
            parameters.setStimulusThreshold(1);
            parameters.setSynPermInactiveDec(0.01);
            parameters.setSynPermActiveInc(0.1);
            parameters.setMinPctOverlapDutyCycles(0.1);
            parameters.setMinPctActiveDutyCycles(0.1);
            parameters.setDutyCyclePeriod(10);
            parameters.setMaxBoost(10);
            parameters.setSynPermConnected(0.1);

            mem = new Connections();
            parameters.apply(mem);



            SpatialPoolerMock mock = new SpatialPoolerMock(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }); //10 inhibit columns
            mock.Init(mem);

            int[] inputVector = new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }; //
            int[] activeArray = new int[] { 0, 0, 0, 0, 0, 1, 1, 1, 1, 1 };
            for (int i = 0; i < 20; i++)
            {
                mock.compute(inputVector, activeArray, true); //updates the permanences value
            }

            for (int i = 0; i < mem.HtmConfig.NumColumns; i++)
            {
                int[] permanences = ArrayUtils.ToIntArray(mem.GetColumn(i).ProximalDendrite.RFPool.GetDensePermanences(mem.HtmConfig.NumInputs));
                //int[] potential = (int[])mem.getConnectedCounts().getSlice(i);
                int[] potential = (int[])mem.GetColumn(i).ConnectedInputBits;
                Assert.IsTrue(permanences.SequenceEqual(potential));
            } 
        }
      /// <Summary>
      /// When stimulusThreshold is > 0, don't allow columns without any overlap to
      /// become active. This test focuses on the global inhibition code path.
      ///<summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Prod")]
        [TestCategory("InitHtmConfig")]
        public void TestZeroOverlap_NoStimulusThreshold_GlobalInhibition1_1()
        {
            int inputSize = 20;
            int nColumns = 70;

            HtmConfig defaultConfig = new HtmConfig(new int[] { inputSize }, new int[] { nColumns });
            Connections cn = new Connections(defaultConfig);

            var htmConfig = cn.HtmConfig;
            htmConfig.PotentialRadius = 10;
            htmConfig.GlobalInhibition = true;
            htmConfig.NumActiveColumnsPerInhArea = 3.0;
            htmConfig.StimulusThreshold = 0.0;
            htmConfig.Random = new ThreadSafeRandom(42);
            htmConfig.RandomGenSeed = 42;

            SpatialPooler sp = new SpatialPooler();
            sp.Init(cn);

            int[] activeArray = new int[nColumns];
            sp.compute(new int[inputSize], activeArray, true);

            Assert.IsFalse(10 == activeArray.Count(i => i > 0));//, ArrayUtils.INT_GREATER_THAN_0).length);
        }
        [TestMethod]
        [DataRow(PoolerMode.SingleThreaded)]
        [DataRow(PoolerMode.Multicore)]
        [TestCategory("UnitTest")]
        [TestCategory("Prod")]
        public void testZeroOverlap_StimulusThreshold_GlobalInhibition1(PoolerMode poolerMode)
        {
            int inputSize = 10;
            int nColumns = 200;
            parameters = Parameters.getSpatialDefaultParameters();
            parameters.Set(KEY.INPUT_DIMENSIONS, new int[] { inputSize });
            parameters.Set(KEY.COLUMN_DIMENSIONS, new int[] { nColumns });
            parameters.Set(KEY.POTENTIAL_RADIUS, 10);
            parameters.Set(KEY.GLOBAL_INHIBITION, true);
            parameters.Set(KEY.NUM_ACTIVE_COLUMNS_PER_INH_AREA, 3.0);
            parameters.Set(KEY.STIMULUS_THRESHOLD, 1.0);
            parameters.Set(KEY.RANDOM, new ThreadSafeRandom(42));
            parameters.Set(KEY.SEED, 42);

            SpatialPooler sp = UnitTestHelpers.CreatePooler(poolerMode);
            Connections cn = new Connections();
            parameters.apply(cn);
            sp.Init(cn);

            int[] activeArray = new int[nColumns];
            sp.compute(new int[inputSize], activeArray, true);

            Assert.IsTrue(10 != activeArray.Count(i => i > 0));//, ArrayUtils.INT_GREATER_THAN_0).length);
        }
       


    }


}
