using System;
using System.Collections.Generic;
using System.Configuration;
using System.Fabric;
using System.Fabric.Description;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Wcf;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Container1
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class Container1 : StatefulService
    {
        Container1Service cont1S;
        public Container1(StatefulServiceContext context)
            : base(context)
        { cont1S = new Container1Service(this.StateManager, Context.PartitionId.ToString()); }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[] { new ServiceReplicaListener(context => this.CreateInternalListener(context)) };
        }
        private ICommunicationListener CreateInternalListener(ServiceContext context)
        {

            EndpointResourceDescription internalEndpoint = context.CodePackageActivationContext.GetEndpoint("ProcessingServiceEndpoint");
            string uriPrefix = String.Format(
                   "{0}://+:{1}/{2}/{3}-{4}/",
                   internalEndpoint.Protocol,
                   internalEndpoint.Port,
                   context.PartitionId,
                   context.ReplicaOrInstanceId,
                   Guid.NewGuid());

            string nodeIP = FabricRuntime.GetNodeContext().IPAddressOrFQDN;

            string uriPublished = uriPrefix.Replace("+", nodeIP);
            return new WcfCommunicationListener<IContainer1>(context, cont1S, WcfUtility.CreateTcpListenerBinding(), uriPrefix);
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");
            var containerCounter = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, int>>("Container1Counter");
            AddToDictionary();
            int count;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var tx = this.StateManager.CreateTransaction())
                {
                    var result = await myDictionary.TryGetValueAsync(tx, "Counter");
                    count = (await containerCounter.TryGetValueAsync(tx, "container1:" + Context.PartitionId.ToString())).Value;
                    ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
                        result.HasValue ? result.Value.ToString() : "Value does not exist.");

                    await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

                    // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
                    // discarded, and nothing is saved to the secondary replicas.
                    await tx.CommitAsync();
                }
                AddToTableStorage(count);
                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
            }
        }
        public async void AddToDictionary()
        {
            var containerCounter = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, int>>("Container1Counter");
            using (var tx = this.StateManager.CreateTransaction())
            {
                await containerCounter.TryAddAsync(tx, "container1:" + Context.PartitionId.ToString(), 0);
            }
        }
        public async void AddToTableStorage(int number)
        {
            try
            {
                CloudStorageAccount _storageAccount;
                CloudTable _table;
                string a = ConfigurationManager.AppSettings["DataConnectionString"];
                _storageAccount = CloudStorageAccount.Parse(a);
                CloudTableClient tableClient = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
                _table = tableClient.GetTableReference("CountTableStorage");
                TableOperation insertOperation = TableOperation.InsertOrReplace(new CountTableEntity("container1:" + Context.PartitionId.ToString(), number));
                _table.Execute(insertOperation);
            }
            catch
            {
                ServiceEventSource.Current.Message("Nije napravljen cloud");
            }
        }
    }
}
