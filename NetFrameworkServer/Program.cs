// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contracts;
using Grpc.Core;
using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Server;

namespace NetFrameworkServer
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Server server = new()
            {
                Ports = { new ServerPort("localhost", 5001, ServerCredentials.Insecure) }
            };
            server.Services.AddCodeFirst<ITestService>(new TestService());
            server.Start();
            Console.WriteLine("Server running...");
            Console.ReadKey();
            Environment.Exit(1);
            Console.ReadKey();
        }

        public class TestService : ITestService
        {
            /// <inheritdoc />
            public async Task TestClientStreaming(IAsyncEnumerable<TestMessage> messages, CallContext callContext)
            {
                await foreach (TestMessage message in messages)
                {
                    Console.WriteLine($"Message received");
                }
            }
        }
    }
}
