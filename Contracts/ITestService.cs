// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Configuration;

namespace Contracts
{
    [Service]
    public interface ITestService
    {
        [Operation]
        Task TestClientStreaming(IAsyncEnumerable<TestMessage> messages, CallContext context = default);
    }
    public record TestMessage(int Id);
}
