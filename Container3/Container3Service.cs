using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container3
{
    public class Container3Service : IContainer3
    {
        string InstanceId;
        IReliableDictionary<string, int> ContainerCounter;
        IReliableStateManager StateManager;
        public Container3Service()
        {

        }

        public Container3Service(IReliableStateManager stateManager, string id)
        {
            InstanceId = id;
            StateManager = stateManager;
        }
        public async Task<bool> ContainerCall()
        {
            ContainerCounter = await StateManager.GetOrAddAsync<IReliableDictionary<string, int>>("Container3Counter");

            using (var tx = this.StateManager.CreateTransaction())
            {
                int i = (await ContainerCounter.TryGetValueAsync(tx, "container3:" + InstanceId)).Value;
                await ContainerCounter.TryRemoveAsync(tx, "container3:" + InstanceId);
                i++;
                await ContainerCounter.TryAddAsync(tx, "container3:" + InstanceId, i);
                await tx.CommitAsync();
            }
            bool a = true;
            return a;
        }
    }
}
