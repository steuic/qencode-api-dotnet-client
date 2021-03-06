﻿using Qencode.Api.CSharp.Client.Classes.CustomParams;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Qencode.Api.CSharp.Client.Classes
{
    public class CustomTranscodingParams
    {
        /// <summary>
        /// Source video URI. Can be http(s) url or tus uri
        /// </summary>
        public string source { get; set; }

        /// <summary>
        /// Source video URI. Can be http(s) url or tus uri
        /// </summary>
        public List<StitchVideoItem> stitch { get; set; }

        /// <summary>
        /// A list of objects, each describing params for a single output video stream (MP4, WEBM, HLS or MPEG-DASH).
        /// </summary>
        public List<Format> format { get; set; }

        /// <summary>
        /// Task callback URL to get notifications from Qencode on task events.
        /// </summary>
        public string callback_url { get; set; }

        public CustomTranscodingParams()
        {
            format = new List<Format>();
        }

        public static CustomTranscodingParams FromFile(string filename)
        {
            var query = System.IO.File.ReadAllText(filename);
            return FromJSON(query);
        }

        public static CustomTranscodingParams FromJSON(string json)
        {
            if (json.IndexOf("\"query\"") != -1)
            {
                var wrapper = JsonConvert.DeserializeObject<QueryWrapper>(json);
                return wrapper.query;
            }
            
            return JsonConvert.DeserializeObject<CustomTranscodingParams>(json);
        }
    }
}
