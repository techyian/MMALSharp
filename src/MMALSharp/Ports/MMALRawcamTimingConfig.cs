// <copyright file="MMALRawcamTimingConfig.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

namespace MMALSharp.Ports
{
    /// <summary>
    /// Camera peripheral timing registers.
    /// </summary>
    public class MMALRawcamTimingConfig
    {
        /// <summary>
        /// Timing register 1 value.
        /// </summary>
        public int Timing1 { get; set; }

        /// <summary>
        /// Timing register 2 value.
        /// </summary>
        public int Timing2 { get; set; }

        /// <summary>
        /// Timing register 3 value.
        /// </summary>
        public int Timing3 { get; set; }

        /// <summary>
        /// Timing register 4 value.
        /// </summary>
        public int Timing4 { get; set; }

        /// <summary>
        /// Timing register 5 value.
        /// </summary>
        public int Timing5 { get; set; }

        /// <summary>
        /// Unsure.
        /// </summary>
        public int Term1 { get; set; }

        /// <summary>
        /// Unsure.
        /// </summary>
        public int Term2 { get; set; }

        /// <summary>
        /// CPI Timing register 1 value.
        /// </summary>
        public int CpiTiming1 { get; set; }

        /// <summary>
        /// CPI Timing register 2 value.
        /// </summary>
        public int CpiTiming2 { get; set; }
        
        /// <summary>
        /// Creates a new instance of <see cref="MMALRawcamTimingConfig"/>.
        /// </summary>
        /// <param name="timing1">Timing register 1 value.</param>
        /// <param name="timing2">Timing register 2 value.</param>
        /// <param name="timing3">Timing register 3 value.</param>
        /// <param name="timing4">Timing register 4 value.</param>
        /// <param name="timing5">Timing register 5 value.</param>
        /// <param name="term1">Term 1 value - Unsure?</param>
        /// <param name="term2">Term 2 value - Unsure?</param>
        /// <param name="cpiTiming1">CPI Timing register 1 value.</param>
        /// <param name="cpiTiming2">CPI Timing register 2 value.</param>
        public MMALRawcamTimingConfig(int timing1,
                                      int timing2,
                                      int timing3,
                                      int timing4,
                                      int timing5,
                                      int term1,
                                      int term2,
                                      int cpiTiming1,
                                      int cpiTiming2)
        {
            this.Timing1 = timing1;
            this.Timing2 = timing2;
            this.Timing3 = timing3;
            this.Timing4 = timing4;
            this.Timing5 = timing5;
            this.Term1 = term1;
            this.Term2 = term2;
            this.CpiTiming1 = cpiTiming1;
            this.CpiTiming2 = cpiTiming2;
        }
    }
}
