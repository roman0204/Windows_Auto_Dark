﻿using AutoDarkModeSvc.Communication;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoDarkModeComms
{
    public class PipeClient : IMessageClient
    {
        public string SendMessageAndGetReply(string message, int timeoutSeconds = 5)
        {
            string pipeId = $"C#_{Convert.ToBase64String(Guid.NewGuid().ToByteArray())}";
            using NamedPipeClientStream clientPipeRequest = new(".", Address.PipePrefix + Address.PipeRequest, PipeDirection.Out);
            try
            {
                clientPipeRequest.Connect(timeoutSeconds * 1000);
                StreamWriter sw = new(clientPipeRequest) { AutoFlush = true };
                using (sw)
                {
                    sw.WriteLine(message);
                    sw.WriteLine(pipeId);
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse()
                {
                    StatusCode = StatusCode.Timeout,
                    Message = "The service did not acknowledge the req in time",
                    Details = $"{ex.GetType()} {ex.Message}"
                }.ToString();
            }

            using NamedPipeClientStream clientPipeResponse = new(".", Address.PipePrefix + Address.PipeResponse + $"_{pipeId}", PipeDirection.In);
            try
            {
                clientPipeResponse.Connect(timeoutSeconds * 1000);
                if (clientPipeResponse.IsConnected && clientPipeResponse.CanRead)
                {
                    using StreamReader sr = new(clientPipeResponse);
                    //sr.BaseStream.ReadTimeout = timeoutSeconds * 1000;
                    string msg = sr.ReadToEnd();
                    if (msg == null)
                    {
                        return StatusCode.Timeout;
                    }
                    return msg;
                }
                else
                {
                    return new ApiResponse()
                    {
                        StatusCode = StatusCode.Err,
                        Message = "Pipe not connected or can't read"
                    }.ToString();
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse()
                {
                    StatusCode = StatusCode.Timeout,
                    Message = "The service did not respond in time",
                    Details = $"{ex.GetType()} {ex.Message}"
                }.ToString();
            }
        }

        public Task<string> SendMessageAndGetReplyAsync(string message, int timeoutSeconds = 5)
        {
            return Task.Run(() => SendMessageAndGetReply(message, timeoutSeconds));
        }

        public string SendMessageWithRetries(string message, int timeoutSeconds = 3, int retries = 3)
        {
            return SendMessageAndGetReply(message, timeoutSeconds * retries);
        }
    }
}
