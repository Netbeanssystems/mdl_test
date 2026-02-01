using Microsoft.AspNetCore.Http;
using System;
using System.Text;

namespace Application.Helpers
{
    public static class MessageBuilder
    {
        public static string BuildExceptionMessage(HttpContext context, Exception exception)
        {
            var msgBuilder = new StringBuilder();
            msgBuilder.AppendLine($"<h1>Message: {exception.Message}</h1>");
            msgBuilder.AppendLine(context.User.Identity?.IsAuthenticated == true
                ? $"Source: {exception.Source} User: {context.User.Identity.Name}<hr />"
                : $"Source: {exception.Source}<hr />");
            msgBuilder.AppendLine($"Request Path: {context.Request.Path}<hr />");
            msgBuilder.AppendLine($"QueryString: {context.Request.QueryString}<hr />");
            if (exception.StackTrace != null)
                msgBuilder.AppendLine(
                    $"Stack Trace: {exception.StackTrace.Replace(Environment.NewLine, "<br />")}<hr />");
            if (exception.InnerException != null)
                msgBuilder.AppendLine($"Inner Exception<hr />{exception.InnerException?.Message.Replace(Environment.NewLine, "<br />")}<hr />");
            if (!context.Request.HasFormContentType || context.Request.Form.Count <= 0)
                return msgBuilder.ToString();
            msgBuilder.AppendLine("<table border=\"1\"><tr><td colspan=\"2\">Form collection:</td></tr>");
            foreach (var (key, value) in context.Request.Form)
                msgBuilder.AppendLine($"<tr><td>{key}</td><td>{value}</td></tr>");
            msgBuilder.AppendLine("</table>");
            return msgBuilder.ToString();
        }
    }
}
