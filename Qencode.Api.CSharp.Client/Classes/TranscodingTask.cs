﻿using Newtonsoft.Json;
using Qencode.Api.CSharp.Client.Responses;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Qencode.Api.CSharp.Client.Classes
{
    public class TranscodingTask
    {
        private QencodeApiClient api;

        private string taskToken;
        /// <summary
        /// Transcoding task token
        /// </summary>
        public string TaskToken
        {
           get { return taskToken; }
        }

        private string statusUrl;
        /// <summary
        /// Task status url
        /// </summary>
        public string StatusUrl
        {
            get { return statusUrl; }
        }

        private TranscodingTaskStatus lastStatus;
        /// <summary
        /// Most recent task status
        /// </summary>
        public TranscodingTaskStatus LastStatus
        {
            get { return lastStatus; }
        }

        public double StartTime { get; set; }
        public double Duration { get; set; }


        /// <summary> Creates new transcoding task </summary>
        /// <param name="api">a reference to QencodeApiClient object</param>
        /// <param name="taskToken">transcoding task token</param>
        public TranscodingTask(QencodeApiClient api, string taskToken)
        {
            this.api = api;
            this.taskToken = taskToken;
            this.statusUrl = null;
        }

        /// <summary>Starts transcoding job using specified transcoding profile list</summary>
        /// <param name="transcodingProfiles">Array of transcoding profile identifiers</param>
        /// <param name="uri">a link to input video or TUS uri</param>
        /// <param name="transferMethod">Transfer method identifier</param>
        /// <param name="payload">Any string data of 1000 characters max length. E.g. you could pass id of your site user uploading the video or any json object.</param>
        public StartEncodeResponse Start(string[] transcodingProfiles, string uri, string transferMethod = null, string payload = null)
        {
            var profiles = String.Join(",", transcodingProfiles);
            return Start(profiles, uri, transferMethod, payload);
        }

        /// <summary>Starts transcoding job using specified transcoding profile or list of profiles </summary>
        /// <param name="transcodingProfile">One or several transcoding profile identifiers (as comma-separated string)</param>
        /// <param name="uri">a link to input video or TUS uri</param>
        /// <param name="transferMethod">Transfer method identifier</param>
        /// <param name="payload">Any string data of 1000 characters max length. E.g. you could pass id of your site user uploading the video or any json object.</param>
        public StartEncodeResponse Start(string transcodingProfile, string uri, string transferMethod = null, string payload = null)
        {
            var parameters = new Dictionary<string, string>() {
                { "task_token", taskToken },
                { "uri", uri },
                { "profiles", transcodingProfile }
            };
        
            if (transferMethod != null)
            {
                parameters.Add("transfer_method", transferMethod);
            }

            if (payload != null)
            {
                parameters.Add("payload", payload);
            }

            var numberFormat = new NumberFormatInfo();
            numberFormat.NumberDecimalSeparator = ".";
            if (StartTime > 0)
            { 
                parameters.Add("start_time", StartTime.ToString("0.####", numberFormat));
            }

            if (StartTime > 0)
            {
                parameters.Add("duration", Duration.ToString("0.####", numberFormat));
            }

            var response = api.Request<StartEncodeResponse>("start_encode", parameters) as StartEncodeResponse;
            this.statusUrl = response.status_url;

            return response;
        }

        //TODO: implement startCustom transcoding method
        /**

     * Starts transcoding job using custom params

     * @param CustomTranscodingParams $task_params

     * @param string $payload Any string data of 1000 characters max length. E.g. you could pass id of your site user uploading the video or any json object.

     * @return array start_encode API method response

     */

        public StartEncodeResponse StartCustom(CustomTranscodingParams taskParams, string payload = null)
        {
            var query = new Dictionary<string, CustomTranscodingParams>() { { "query", taskParams } };
            var query_json = JsonConvert.SerializeObject(query, 
                Formatting.None, 
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore});

            var parameters = new Dictionary<string, string>
            {
                {"task_token", taskToken },
                {"query", query_json }
            };
            if (payload != null)
            {
                parameters.Add("payload", payload);
            }

            return _do_request("start_encode2", parameters);
        }

        private StartEncodeResponse _do_request(string methodName, Dictionary<string, string> parameters)
        {
            var response = api.Request<StartEncodeResponse>(methodName, parameters) as StartEncodeResponse;
            this.statusUrl = response.status_url;
            return response;
        }

        /// <summary>
        /// Gets current task status from qencode service
        /// </summary>
        public TranscodingTaskStatus GetStatus()
        {
            var parameters = new Dictionary<string, string>() {
                { "task_tokens[]", this.taskToken }
            };
               
                //TODO: fallback to /v1/status

            var response = api.Request<StatusResponse>(statusUrl, parameters) as StatusResponse;
            lastStatus = response.statuses[this.taskToken];
            return lastStatus;
        }
    }
}
