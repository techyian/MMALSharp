// <copyright file="ImageFxData.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System.Collections.Generic;
using MMALSharp.Native;

namespace MMALSharp.Tests.Data
{
    public class ImageFxData
    {
        public static IEnumerable<object[]> Data
            => new List<object[]>
            {
                // { Effect, throws exception }
                new object[] { MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_CARTOON, false },
                new object[] { MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_BLACKBOARD, true },
                new object[] { MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_BLUR, false },
                new object[] { MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_COLOURBALANCE, false },
                new object[] { MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_COLOURPOINT, false },
                new object[] { MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_COLOURSWAP, false },
                new object[] { MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_DENOISE, false },
                new object[] { MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_EMBOSS, false },
                new object[] { MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_FILM, false },
                new object[] { MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_GPEN, false },
                new object[] { MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_HATCH, false },
                new object[] { MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_NEGATIVE, false },
                new object[] { MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_OILPAINT, false },
                new object[] { MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_PASTEL, false },
                new object[] { MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_POSTERISE, false },
                new object[] { MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_SATURATION, false },
                new object[] { MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_SKETCH, false },
                new object[] { MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_SOLARIZE, false },
                new object[] { MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_WASHEDOUT, false },
                new object[] { MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_WHITEBOARD, true }
            };
    }
}
