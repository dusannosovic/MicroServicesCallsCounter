using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container1
{
    public class Container1Service : IContainer1
    {
        string InstanceId;
        IReliableDictionary<string, int> ContainerCounter;
        IReliableStateManager StateManager;
        public Container1Service()
        {

        }

        public Container1Service(IReliableStateManager stateManager, string id)
        {
            InstanceId = id;
            StateManager = stateManager;
        }
        public async Task<bool> ContainerCall()
        {
            ContainerCounter = await StateManager.GetOrAddAsync<IReliableDictionary<string, int>>("Container1Counter");

            using (var tx = this.StateManager.CreateTransaction())
            {
                int i = (await ContainerCounter.TryGetValueAsync(tx, "container1:" + InstanceId)).Value;
                await ContainerCounter.TryRemoveAsync(tx, "container1:" + InstanceId);
                i++;
                await ContainerCounter.TryAddAsync(tx, "container1:" + InstanceId, i);
                await tx.CommitAsync();
            }
            bool a = true;
            return a;
        }
    }
}
