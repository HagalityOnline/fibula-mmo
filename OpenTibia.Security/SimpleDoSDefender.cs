// <copyright file="SimpleDoSDefender.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Security
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class SimpleDoSDefender : IDoSDefender
    {
        // To prevent a memory attack... just blacklist a maximum of 1M addresses.
        private const int ListSizeLimit = 1000000;

        // Count to reach within 30 seconds (reduces count every 5 seconds);
        private const int BlockAtCount = 20;

        private readonly HashSet<string> blockedAddresses;

        private readonly ConcurrentDictionary<string, int> connectionCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleDoSDefender"/> class.
        /// </summary>
        public SimpleDoSDefender()
        {
            this.blockedAddresses = new HashSet<string>();
            this.connectionCount = new ConcurrentDictionary<string, int>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            await Task.Factory.StartNew(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    const int secondsToWait = 5;
                    Thread.Sleep(TimeSpan.FromSeconds(secondsToWait));
                    var cleaningList = this.connectionCount.ToList();

                    foreach (var kvp in cleaningList)
                    {
                        if (kvp.Value < secondsToWait)
                        {
                            this.connectionCount.TryRemove(kvp.Key, out int count);
                        }
                        else
                        {
                            this.connectionCount.TryUpdate(kvp.Key, kvp.Value - secondsToWait, kvp.Value);
                        }
                    }
                }
            });
        }

        public void BlockAddress(string addressStr)
        {
            if (this.blockedAddresses.Count >= ListSizeLimit || string.IsNullOrWhiteSpace(addressStr))
            {
                return;
            }

            this.AddInternal(addressStr);
        }

        public bool IsBlocked(string addressStr)
        {
            return this.blockedAddresses.Contains(addressStr);
        }

        public void LogConnectionAttempt(string addressStr)
        {
            this.connectionCount.AddOrUpdate(addressStr, 0, (key, prev) => { return prev + 1; });

            try
            {
                if (this.connectionCount[addressStr] == BlockAtCount)
                {
                    this.AddInternal(addressStr);

                    this.connectionCount.TryRemove(addressStr, out int count);
                }
            }
            catch
            {
                // happens if the key was removed exactly at the time we were querying. Just ignore.
            }
        }

        private void AddInternal(string addressStr)
        {
            try
            {
                this.blockedAddresses.Add(addressStr);
            }
            catch
            {
                // this will be thrown if there is already an element in there, so just ignore.
            }
        }
    }
}
