// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using Contracts;
using Grpc.Core;
using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Client;
using Channel = Grpc.Core.Channel;

namespace NetFrameworkClient
{
    public class Program
    {

        private static async Task Main(string[] args)
        {
            Channel channel = new Channel("localhost", 5001, ChannelCredentials.Insecure);

            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            await TestClientStreaming(channel);

            Console.ReadLine();

            await channel.ShutdownAsync();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine($"AppDomain Unhandled Exception: {e.ExceptionObject}");
        }

        private static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Console.WriteLine($"Unobserved Task exception: {e.Exception.ToString()}");
        }

        private static async Task TestClientStreaming(ChannelBase grpcChannel)
        {
            ITestService roundtripService = grpcChannel.CreateGrpcService<ITestService>();
            Channel<TestMessage> channel = System.Threading.Channels.Channel.CreateUnbounded<TestMessage>();

            _ = Task.Factory.StartNew(async () =>
            {
                int i = 0;
                while (true)
                {
                    try
                    {
                        //channel.Writer.TryWrite(new TestMessage(i++));
                        await channel.Writer.WriteAsync(new TestMessage(i++));
                    }
                    catch (Exception ex)
                    {
                        // This is not hit
                    }
                    Console.WriteLine($"Sent message");
                    await Task.Delay(500).ConfigureAwait(false);
                }
            });

            try
            {
                await roundtripService.TestClientStreaming(channel.AsAsyncEnumerable(), new CallContext());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception thrown at call: {ex.ToString()}");

                await Task.Delay(1000);
                GC.Collect();
            }
        }
    }

}
