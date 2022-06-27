using System;
using System.Collections.Concurrent;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Extensions;
using InformationTree.Domain.Requests.Base;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using Newtonsoft.Json;
using NLog;

namespace InformationTree.Render.WinForms.Services
{
    public class ProfilingService : IProfilingService
    {
        private const string Separator = "#";
        
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ConcurrentDictionary<string, ProfilingInfo> _profilingInfo = new();

        public void StartProfiling(BaseRequest request)
        {
            var key = GetNewProfilingKeyForProfilingStart(string.Empty, request);

            _profilingInfo.TryAdd(key, new ProfilingInfo
            {
                Request = request,
                StartTime = DateTime.Now
            });
        }

        private string GetNewProfilingKeyForProfilingStart(string key, BaseRequest request)
        {
            key = key.IsEmpty() ? $"{request.GetType().Name}{Separator}1" : key.Trim();

            if (!_profilingInfo.ContainsKey(key))
            {
                // If the key doesn't exist already, it's a valid key
                return key;
            }

            var splitKey = key.Split(Separator);
            if (splitKey.Length != 2)
                throw new InvalidOperationException("Start profiling - Cannot increment current request number if the key has an invalid format.");

            // Increment existing request number (if it's a valid integer)
            var requestNumber = splitKey[1];

            if (!int.TryParse(requestNumber, out var requestNumberAsInt))
                throw new InvalidOperationException("Start profiling - Cannot increment current request numberif the request number is an invalid integer");

            var newKey = $"{request.GetType().Name}{Separator}{requestNumberAsInt + 1}";

            return GetNewProfilingKeyForProfilingStart(newKey, request);
        }

        public void EndProfiling(BaseRequest request, BaseResponse response)
        {
            var profilingInfo = GetUnfinishedProfilingInfoForProfilingEnd(string.Empty, request, string.Empty);

            profilingInfo.EndTime = DateTime.Now;
            profilingInfo.Response = response;
            profilingInfo.Duration = profilingInfo.EndTime.Value - profilingInfo.StartTime;

            LogProfilingInfo(profilingInfo);
        }

        private static void LogProfilingInfo(ProfilingInfo profilingInfo)
        {
            var serializedRequest = JsonConvert.SerializeObject(
                profilingInfo.Request,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                });
            
            var serializedResponse = JsonConvert.SerializeObject(
                profilingInfo.Response,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                });

            _logger.Debug(
                "{message}",
                $"Profiling info: Request {profilingInfo.Request.GetType().Name}: {serializedRequest} Response {profilingInfo?.Response?.GetType()?.Name}: {serializedResponse} Start time: {profilingInfo.StartTime} End time: {profilingInfo.EndTime} Duration: {profilingInfo.Duration}");
        }

        private ProfilingInfo GetUnfinishedProfilingInfoForProfilingEnd(string key, BaseRequest request, string oldKey)
        {
            key = key.IsEmpty() ? $"{request.GetType().Name}{Separator}1" : key.Trim();

            if (!_profilingInfo.ContainsKey(key))
            {
                if (oldKey.IsEmpty())
                    throw new InvalidOperationException("End profiling - No unfinished profiling info found");

                // If the next key doesn't exist, this is the last profiling key we're expecting
                return _profilingInfo[oldKey];
            }

            var profilingInfo = _profilingInfo[key];
            if (profilingInfo.EndTime == null)
            {
                // This profiling is not yet finished, so we can use it
                return profilingInfo;
            }

            var splitKey = key.Split(Separator);
            if (splitKey.Length != 2)
                throw new InvalidOperationException("End profiling - Cannot increment current request number if the key has an invalid format.");

            // Increment existing request number (if it's a valid integer)
            var requestNumber = splitKey[1];

            if (!int.TryParse(requestNumber, out var requestNumberAsInt))
                throw new InvalidOperationException("End profiling - Cannot increment current request numberif the request number is an invalid integer");

            var newKey = $"{request.GetType().Name}{Separator}{requestNumberAsInt + 1}";

            return GetUnfinishedProfilingInfoForProfilingEnd(newKey, request, key);
        }
    }
}