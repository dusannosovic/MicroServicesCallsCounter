using Common;
using Container1;
using Container2;
using Container3;
using Microsoft.Azure;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compute
{
    class ComputeService : ICompute
    {
        public async Task<bool> ComputeAsync(int container1, int container2, int container3)
        {
            if (container1 > 0)
            {
                container1--;
                FabricClient fabricClient = new FabricClient();
                int partitionsNumber = (await fabricClient.QueryManager.GetPartitionListAsync(new Uri("fabric:/MicroServicesCallsCounter/Container1"))).Count;
                var binding = WcfUtility.CreateTcpClientBinding();
                int index = 0;
                //for (int i = 0; i < partitionsNumber; i++)
                //{
                ServicePartitionClient<WcfCommunicationClient<IContainer1>> servicePartitionClient = new ServicePartitionClient<WcfCommunicationClient<IContainer1>>(
                    new WcfCommunicationClientFactory<IContainer1>(clientBinding: binding),
                    new Uri("fabric:/MicroServicesCallsCounter/Container1"),
                    new ServicePartitionKey(container1));

                bool a = await servicePartitionClient.InvokeWithRetryAsync(client => client.Channel.ContainerCall());

            }
            if (container2>0)
            {
                container2--;
                FabricClient fabricClient = new FabricClient();
                int partitionsNumber = (await fabricClient.QueryManager.GetPartitionListAsync(new Uri("fabric:/MicroServicesCallsCounter/Container2"))).Count;
                var binding = WcfUtility.CreateTcpClientBinding();
                int index = 0;
                //for (int i = 0; i < partitionsNumber; i++)
                //{
                ServicePartitionClient<WcfCommunicationClient<IContainer2>> servicePartitionClient = new ServicePartitionClient<WcfCommunicationClient<IContainer2>>(
                    new WcfCommunicationClientFactory<IContainer2>(clientBinding: binding),
                    new Uri("fabric:/MicroServicesCallsCounter/Container2"),
                    new ServicePartitionKey(container2));

                bool a = await servicePartitionClient.InvokeWithRetryAsync(client => client.Channel.ContainerCall());
                
            }
            if (container3 > 0)
            {
                container3--;
                FabricClient fabricClient = new FabricClient();
                int partitionsNumber = (await fabricClient.QueryManager.GetPartitionListAsync(new Uri("fabric:/MicroServicesCallsCounter/Container3"))).Count;
                var binding = WcfUtility.CreateTcpClientBinding();
                int index = 0;
                //for (int i = 0; i < partitionsNumber; i++)
                //{
                ServicePartitionClient<WcfCommunicationClient<IContainer3>> servicePartitionClient = new ServicePartitionClient<WcfCommunicationClient<IContainer3>>(
                    new WcfCommunicationClientFactory<IContainer3>(clientBinding: binding),
                    new Uri("fabric:/MicroServicesCallsCounter/Container3"),
                    new ServicePartitionKey(container3));

                bool a = await servicePartitionClient.InvokeWithRetryAsync(client => client.Channel.ContainerCall());
            }
            return true;
        }
    }
}
