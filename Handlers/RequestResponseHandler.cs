using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SampleSnippets.Handlers
{
    public class RequestResponseHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var requestedMethod = request.Method;
            var userHostAddress = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : "0.0.0.0";
            var requestMessage = await request.Content.ReadAsByteArrayAsync();
            var uriAccessed = request.RequestUri.AbsoluteUri;

            var responseHeadersString = new StringBuilder();
            foreach (var header in request.Headers)
            {
                responseHeadersString.Append($"{header.Key}: {String.Join(", ", header.Value)}{Environment.NewLine}");
            }

            var messageLoggingHandler = new MessageLogging();

            var requestLog = new MessageApiLog()
            {
                Headers = responseHeadersString.ToString(),
                AbsoluteUri = uriAccessed,
                RequestBody = Encoding.UTF8.GetString(requestMessage),
                UserHostAddress = userHostAddress,
                RequestedMethod = requestedMethod.ToString(),
                StatusCode = string.Empty
            };

            messageLoggingHandler.IncomingMessageAsync(requestLog);

            var response = await base.SendAsync(request, cancellationToken);

            byte[] responseMessage;
            if (response.IsSuccessStatusCode)
                responseMessage = response.Content != null ? await response.Content.ReadAsByteArrayAsync() : new byte[10];
            else
                responseMessage = Encoding.UTF8.GetBytes(response.ReasonPhrase);

            var responseLog = new MessageApiLog()
            {
                Headers = responseHeadersString.ToString(),
                AbsoluteUri = uriAccessed,
                RequestBody = Encoding.UTF8.GetString(responseMessage),
                UserHostAddress = userHostAddress,
                RequestedMethod = requestedMethod.ToString(),
                StatusCode = string.Empty
            };

            messageLoggingHandler.OutgoingMessageAsync(responseLog);
            return response;
        }

    }

    public class MessageLogging
    {
        public void IncomingMessageAsync(MessageApiLog apiLog)
        {
            apiLog.RequestType = "Request";
            Trace.WriteLine(apiLog);
        }

        public void OutgoingMessageAsync(MessageApiLog apiLog)
        {
            apiLog.RequestType = "Response";
            Trace.WriteLine(apiLog);
        }
    }

    public class MessageApiLog
    {
        public string Headers { get; set; }
        public string StatusCode { get; set; }
        public string RequestBody { get; set; }
        public string RequestedMethod { get; set; }
        public string UserHostAddress { get; set; }
        public string AbsoluteUri { get; set; }
        public string RequestType { get; set; }

        public override string ToString()
        {
            return $"Headers: {Headers}, StatusCode: {StatusCode} \n " +
                $"RequestBody: {RequestBody}, RequestMethod: {RequestedMethod} \n" +
                $"UserHostAddress: {UserHostAddress} \n" +
                $"AbsoluteUri: {AbsoluteUri}, RequestType: {RequestType}";
        }
    }
}